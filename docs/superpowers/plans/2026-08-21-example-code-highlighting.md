# Example Code Highlighting Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace `AxamlCodeBlock`'s light-theme-assuming XML highlighting with a TextMate-based dark theme, add token-level coloring for `tw:Tw.Class` utility strings and Avalonia markup extensions, and add line numbers + a copy button.

**Architecture:** `AxamlCodeBlock`'s `TextEditor` gets a TextMate installation (bundled `.xml` grammar + `DarkPlus` theme) for baseline coloring, via a new shared `AxamlHighlighting` helper (one `RegistryOptions` instance for the whole app, not per code block). A new `AxamlHighlightColorizer` (`DocumentColorizingTransformer`) layers on top, regex-matching `{...}` markup-extension spans and `tw:Tw.Class="..."` value tokens per visible line and recoloring them using colors pulled from the library's own `TailwindColorPalette`. Line numbers and a copy button are added directly to `AxamlCodeBlock.axaml`.

**Tech Stack:** Avalonia 12.1.1, `Avalonia.AvaloniaEdit` 12.0.0, `AvaloniaEdit.TextMate` 12.0.0, `TextMateSharp.Grammars` >= 2.0.3, .NET 10 (`net10.0` Desktop, `net10.0-browser` WASM).

**Spec:** `docs/superpowers/specs/2026-08-21-example-code-highlighting-design.md`

## Global Constraints

- Package versions: `AvaloniaEdit.TextMate` `12.0.0`; `TextMateSharp.Grammars` must be `>= 2.0.3` — a lower pin (e.g. `1.0.66`) fails restore with `NU1605: package downgrade detected`.
- No automated test project covers `samples/`. `tests/Tailwind.Avalonia.Tests` only references `src/Tailwind.Avalonia`. Verification for every task in this plan is: `dotnet build` on the relevant `.csproj` (0 errors) + a manual visual pass in the running Desktop sample. This matches the existing repo convention (see the spec's Verification section).
- `src/Tailwind.Avalonia`'s public API surface must not change. The only change to that project is adding one `InternalsVisibleTo` entry.
- Colorizer colors must come from `TailwindColorPalette.TryGetColor("sky-400", out var color)` / `TryGetColor("violet-300", out var color)` — never hand-converted/approximated hex values.
- There is no solution (`.sln`) file in this repo. Build individual project files directly: `dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj` and `dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj`.

---

### Task 1: Swap stock XML highlighting for TextMate + DarkPlus baseline

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj`
- Create: `samples/Tailwind.Avalonia.Sample/AxamlHighlighting.cs`
- Modify: `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml.cs`

**Interfaces:**
- Produces: `internal static class AxamlHighlighting` with `public static void Install(AvaloniaEdit.TextEditor editor)`. Task 2 modifies this same method's body (adds colorizer attachment) — do not change its signature.

- [ ] **Step 1: Add TextMate package references**

Edit `samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj`:

```diff
     <PackageReference Include="Avalonia" Version="12.1.1" />
     <PackageReference Include="Avalonia.AvaloniaEdit" Version="12.0.0" />
+    <PackageReference Include="AvaloniaEdit.TextMate" Version="12.0.0" />
+    <PackageReference Include="TextMateSharp.Grammars" Version="2.0.3" />
     <PackageReference Include="Avalonia.Themes.Fluent" Version="12.1.1" />
```

- [ ] **Step 2: Create the shared TextMate installer**

Create `samples/Tailwind.Avalonia.Sample/AxamlHighlighting.cs`:

```csharp
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Shared TextMate registry so every AxamlCodeBlock reuses the same loaded grammar/theme
/// instead of re-parsing them per instance.
/// </summary>
internal static class AxamlHighlighting
{
    private static readonly RegistryOptions RegistryOptions = new(ThemeName.DarkPlus);
    private static readonly string XmlScope = RegistryOptions.GetScopeByExtension(".xml");

    public static void Install(TextEditor editor)
    {
        var installation = editor.InstallTextMate(RegistryOptions);
        installation.SetGrammar(XmlScope);
    }
}
```

- [ ] **Step 3: Wire it into AxamlCodeBlock**

Edit `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml.cs`:

```diff
 using Avalonia;
 using Avalonia.Controls;
 using Avalonia.Metadata;
-using AvaloniaEdit.Highlighting;

 namespace Tailwind.Avalonia.Sample;
```

```diff
     public AxamlCodeBlock()
     {
         InitializeComponent();
-        Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
+        AxamlHighlighting.Install(Editor);
     }
```

- [ ] **Step 4: Build both targets**

Run:
```bash
dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj
dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj
```
Expected: both succeed, 0 errors, 0 warnings.

- [ ] **Step 5: Manual visual check**

Run the Desktop sample, navigate to Spacing → Margin (or any page with a code block). Confirm the code block no longer looks like unstyled/mismatched light-theme text — XML tags, attribute names, and attribute values render in distinct colors against the `slate-900` background, consistent with a real dark editor theme.

- [ ] **Step 6: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj samples/Tailwind.Avalonia.Sample/AxamlHighlighting.cs samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml.cs
git commit -m "feat: swap AXAML code block highlighting to TextMate DarkPlus theme"
```

---

### Task 2: Layer AXAML/Tailwind-specific token colorizer

**Files:**
- Modify: `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`
- Create: `samples/Tailwind.Avalonia.Sample/AxamlHighlightColorizer.cs`
- Modify: `samples/Tailwind.Avalonia.Sample/AxamlHighlighting.cs`

**Interfaces:**
- Consumes: `Tailwind.Avalonia.TailwindColorPalette.TryGetColor(string tokenName, out Avalonia.Media.Color color)` (`internal`, requires `InternalsVisibleTo`).
- Produces: `internal sealed class AxamlHighlightColorizer : AvaloniaEdit.Rendering.DocumentColorizingTransformer` with a public parameterless constructor, attached in `AxamlHighlighting.Install`.

- [ ] **Step 1: Grant the sample project internal visibility into the library**

Edit `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`:

```diff
   <ItemGroup>
     <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
       <_Parameter1>Tailwind.Avalonia.Tests</_Parameter1>
     </AssemblyAttribute>
+    <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
+      <_Parameter1>Tailwind.Avalonia.Sample</_Parameter1>
+    </AssemblyAttribute>
   </ItemGroup>
```

- [ ] **Step 2: Create the colorizer**

Create `samples/Tailwind.Avalonia.Sample/AxamlHighlightColorizer.cs`:

```csharp
using System.Text.RegularExpressions;

using Avalonia.Media;

using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Overlays coloring for Avalonia markup extensions and tw:Tw.Class utility tokens on top
/// of the baseline TextMate XML highlighting installed by <see cref="AxamlHighlighting"/>.
/// </summary>
internal sealed class AxamlHighlightColorizer : DocumentColorizingTransformer
{
    private static readonly Regex MarkupExtensionPattern = new(@"\{[^{}]*\}", RegexOptions.Compiled);
    private static readonly Regex TwClassAttributePattern = new(@"tw:Tw\.Class=""([^""]*)""", RegexOptions.Compiled);
    private static readonly Regex TokenPattern = new(@"\S+", RegexOptions.Compiled);

    private static readonly IBrush MarkupExtensionBrush = CreateBrush("violet-300");
    private static readonly IBrush UtilityClassBrush = CreateBrush("sky-400");

    protected override void ColorizeLine(DocumentLine line)
    {
        var text = CurrentContext.Document.GetText(line);

        foreach (Match match in MarkupExtensionPattern.Matches(text))
        {
            var start = line.Offset + match.Index;
            var end = start + match.Length;
            ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(MarkupExtensionBrush));
        }

        foreach (Match attribute in TwClassAttributePattern.Matches(text))
        {
            var value = attribute.Groups[1];

            foreach (Match token in TokenPattern.Matches(value.Value))
            {
                var start = line.Offset + value.Index + token.Index;
                var end = start + token.Length;
                ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(UtilityClassBrush));
            }
        }
    }

    private static IBrush CreateBrush(string tailwindToken)
    {
        return TailwindColorPalette.TryGetColor(tailwindToken, out var color)
            ? new SolidColorBrush(color)
            : Brushes.White;
    }
}
```

- [ ] **Step 3: Attach the colorizer in AxamlHighlighting**

Edit `samples/Tailwind.Avalonia.Sample/AxamlHighlighting.cs`:

```diff
     private static readonly RegistryOptions RegistryOptions = new(ThemeName.DarkPlus);
     private static readonly string XmlScope = RegistryOptions.GetScopeByExtension(".xml");
+    private static readonly AxamlHighlightColorizer Colorizer = new();

     public static void Install(TextEditor editor)
     {
         var installation = editor.InstallTextMate(RegistryOptions);
         installation.SetGrammar(XmlScope);
+        editor.TextArea.TextView.LineTransformers.Add(Colorizer);
     }
```

- [ ] **Step 4: Build both targets**

Run:
```bash
dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj
dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj
```
Expected: both succeed, 0 errors, 0 warnings. (Confirms `InternalsVisibleTo` resolved correctly and the colorizer compiles against `TailwindColorPalette`.)

- [ ] **Step 5: Manual visual check — utility-class tokens**

Run the Desktop sample, navigate to Spacing → Margin. Its first code block shows `<Border tw:Tw.Class="m-8" />`. Confirm `m-8` renders in a distinct light-blue (`sky-400`) color, different from the surrounding tag/attribute-name colors.

- [ ] **Step 6: Manual visual check — markup extensions (throwaway edit)**

No existing docs snippet currently contains a markup extension (`{Binding ...}` / `{StaticResource ...}`) — this was confirmed by searching all `samples/Tailwind.Avalonia.Sample/**/*.axaml` files during planning; only the app's own chrome (e.g. `AxamlCodeBlock.axaml` itself) uses them, never a snippet shown inside a code block. To verify this path visually:

1. Temporarily edit one `<docs:AxamlCodeBlock>` snippet in `samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml` (e.g. the one at line 165) to read `&lt;Border tw:Tw.Class=&quot;m-8&quot; Tag=&quot;{Binding Test}&quot; /&gt;`.
2. Rebuild and run the Desktop sample, navigate to the same page, confirm the `{Binding Test}` span renders in violet-300, distinct from both the baseline tag color and the sky-400 utility-token color.
3. Revert the temporary edit: `git checkout -- samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml`.

- [ ] **Step 7: Commit**

```bash
git add src/Tailwind.Avalonia/Tailwind.Avalonia.csproj samples/Tailwind.Avalonia.Sample/AxamlHighlightColorizer.cs samples/Tailwind.Avalonia.Sample/AxamlHighlighting.cs
git commit -m "feat: highlight tw:Tw.Class tokens and markup extensions in AXAML code blocks"
```

---

### Task 3: Add line numbers and a copy button

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml.cs`

**Interfaces:**
- Produces: `private async void OnCopyButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)` — wired as the `Click` handler of the new `CopyButton` element in the `.axaml`.

- [ ] **Step 1: Add line numbers and the copy button to the layout**

Replace the full contents of `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml`:

```xml
<UserControl
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ae="using:AvaloniaEdit"
    xmlns:tw="using:Tailwind.Avalonia"
    x:Class="Tailwind.Avalonia.Sample.AxamlCodeBlock">
    <UserControl.Content>
        <Grid>
            <ae:TextEditor
                x:Name="Editor"
                Background="Transparent"
                FontFamily="{Binding $parent[UserControl].FontFamily}"
                FontSize="{Binding $parent[UserControl].FontSize}"
                Foreground="{Binding $parent[UserControl].Foreground}"
                HorizontalScrollBarVisibility="Disabled"
                IsReadOnly="True"
                ShowLineNumbers="True"
                VerticalScrollBarVisibility="Disabled"
                WordWrap="True" />
            <Button
                x:Name="CopyButton"
                Click="OnCopyButtonClick"
                Content="Copy"
                HorizontalAlignment="Right"
                VerticalAlignment="Top"
                Margin="0,4,4,0"
                Padding="10,4"
                FontSize="11"
                BorderThickness="1"
                CornerRadius="4"
                tw:Tw.Class="bg-slate-800 border-slate-700 text-slate-300" />
        </Grid>
    </UserControl.Content>
</UserControl>
```

- [ ] **Step 2: Add the click handler**

Edit `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml.cs`:

```diff
+using System;
+using System.Threading.Tasks;
 using Avalonia;
 using Avalonia.Controls;
+using Avalonia.Interactivity;
 using Avalonia.Metadata;

 namespace Tailwind.Avalonia.Sample;
```

```diff
     private void ApplyCode()
     {
         Editor.Text = AxamlSnippetFormatter.Format(Code ?? string.Empty);
     }
+
+    private async void OnCopyButtonClick(object? sender, RoutedEventArgs e)
+    {
+        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
+
+        if (clipboard is null)
+        {
+            return;
+        }
+
+        await clipboard.SetTextAsync(Editor.Text);
+
+        CopyButton.Content = "Copied";
+        await Task.Delay(TimeSpan.FromSeconds(1.5));
+        CopyButton.Content = "Copy";
+    }
 }
```

- [ ] **Step 3: Build Desktop target**

Run:
```bash
dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj
```
Expected: succeeds, 0 errors.

- [ ] **Step 4: Manual visual check**

Run the Desktop sample, navigate to any docs page with a code block. Confirm:
- Line numbers appear in the left gutter.
- A "Copy" button is visible in the top-right corner of the block.
- Clicking it changes the label to "Copied", and after ~1.5s it reverts to "Copy".
- Pasting the clipboard contents afterward (e.g. into another app or a text field) matches the exact formatted snippet text shown in the block.

- [ ] **Step 5: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml.cs
git commit -m "feat: add line numbers and copy button to AXAML code blocks"
```

---

### Task 4: End-to-end verification across all docs pages

**Files:** None (verification-only task; only produces a commit if a regression fix is needed).

**Interfaces:** None — this task consumes the combined output of Tasks 1–3 as a whole.

- [ ] **Step 1: Build both targets from a clean state**

Run:
```bash
dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj
dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj
```
Expected: both succeed, 0 errors, 0 warnings.

- [ ] **Step 2: Manual pass across every docs page hosting AxamlCodeBlock**

Run the Desktop sample and visit each of the following pages, confirming on each: baseline TextMate coloring renders correctly against the dark surface, `tw:Tw.Class` values are highlighted sky-400, line numbers show, and the copy button works.

- Spacing → Margin
- Spacing → Padding
- Sizing → Width
- Sizing → Height
- Interactivity → PseudoClassVariants
- Typography → ColorUtilities
- Typography → FontSize

- [ ] **Step 3: Fix any regression found, or confirm none**

If a page renders incorrectly, fix it in the relevant file from Tasks 1–3 and re-run Steps 1–2. If everything checks out, no code changes are needed — proceed to Step 4.

- [ ] **Step 4: Commit any fixups (skip if none were needed)**

```bash
git add -u
git commit -m "fix: address regressions found in cross-page highlighting verification"
```
