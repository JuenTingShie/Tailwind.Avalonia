# Drop StaticResource Utilities Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Remove the `StaticResource`-based utility surface (`ColorResourceDictionary`, `SpacingResourceDictionary`, `FontSizeResourceDictionary`, `Themes/Tailwind.axaml`) from Tailwind.Avalonia, converting every consumer (library, sample app chrome, sample doc pages) to `tw:Tw.Class`, and ship this as the repo's first explicitly versioned release (`1.0.0`) with a documented breaking change.

**Architecture:** Delete the three resource-dictionary classes and their merge point outright (they have no remaining callers once XAML stops referencing them). Every place that consumed their output converts via one deterministic algorithm: direct element attributes become `tw:Tw.Class` tokens 1:1; Style `Setter`s targeting `Background`/`Foreground`/`BorderBrush` retarget to `Setter Property="tw:Tw.Class"` carrying the full token string for that selector's state (required because `Tw.Class` applies via `SetValue`, which is LocalValue priority and outranks a same-property Setter). One reference (`SamplePaddingStripeBrush`'s nested `GeometryDrawing.Brush`) cannot go through `Tw.Class` at all and becomes a literal hardcoded color.

**Tech Stack:** .NET 10 / Avalonia 12.0.2, AXAML, xUnit.

**Spec:** `docs/superpowers/specs/2026-08-19-drop-staticresource-utilities-design.md`

## Global Constraints

- Target framework `net10.0`, Avalonia `12.0.2` — unchanged.
- Package version: add `<Version>1.0.0</Version>` to `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj` (first version this repo has ever declared).
- **This sandbox cannot restore NuGet packages** — `dotnet build`/`test`/`run` all fail here with `NU1301` because the configured feed (`git.f62y.com`, an internal/corporate mirror) is unreachable from this environment. Every task below therefore separates verification into (a) static checks executable here (XML well-formedness by re-reading the file, `grep` for leftover `StaticResource` references) and (b) a `dotnet build` / manual run that **must be executed by the user in their own local environment** — call this out explicitly in each task rather than silently claiming a build passed.
- No Avalonia.Headless test infrastructure exists in this repo. Do not add one for this change. The Setter-retargeting mechanism is verified by the user manually exercising the running Desktop sample (hover/press/select every interactive control), not by a new automated UI test.
- **Conversion algorithm** (the single rule applied throughout this plan):
  - *Color* (`ColorResourceDictionary`): `Brush<ResourceSuffix>` used on `Background` → merge Tailwind token `bg-<kebab-name>` into that element's `tw:Tw.Class`; on `Foreground` → `text-<kebab-name>`; on `BorderBrush` → `border-<kebab-name>`. `<kebab-name>` is `<ResourceSuffix>` lowercased with a hyphen inserted between the trailing letters and trailing digits: `Slate950`→`slate-950`, `Sky500`→`sky-500`, `White`→`white` (no digits, no hyphen).
  - *Sizing* (`SpacingResourceDictionary`): `Width<N>`→`w-<N>`; `MinWidth<N>`→`min-w-<N>`; `MaxWidth<N>`→`max-w-<N>`; `Height<N>`→`h-<N>`; `MinHeight<N>`→`min-h-<N>`; `MaxHeight<N>`→`max-h-<N>`.
  - *Spacing* (`SpacingResourceDictionary`): `Margin<N>`/`Padding<N>`→`m-<N>`/`p-<N>`; `<Prefix>X<N>`→`mx-<N>`/`px-<N>`; `<Prefix>Y<N>`→`my-<N>`/`py-<N>`; `<Prefix>Top<N>`→`mt-<N>`/`pt-<N>`; `<Prefix>Right<N>`→`mr-<N>`/`pr-<N>`; `<Prefix>Bottom<N>`→`mb-<N>`/`pb-<N>`; `<Prefix>Left<N>`→`ml-<N>`/`pl-<N>`; `NegativeMarginTop<N>`→`-mt-<N>`.
  - *FontSize* (`FontSizeResourceDictionary`): `FontSize<Name>`→`text-<kebab-name>` (`FontSizeXs`→`text-xs`, `FontSize2xl`→`text-2xl`).
  - If the target element already carries `tw:Tw.Class="..."`, append the new token into the existing space-separated value. Otherwise add `tw:Tw.Class="<token>"` as a new attribute and delete the old resource-bound attribute line entirely.
  - Every `<TabItem Classes="docs-exampleTab" Header="StaticResource">...</TabItem>` block (and any `StackPanel.Resources`/locally-scoped resources defined only inside it) is deleted outright, along with the `TabControl` wrapper if that leaves only one `TabItem` (in which case the surviving `TabItem`'s child content is promoted to replace the whole `TabControl`, keeping the surrounding `<Border Classes="docs-surface">`).
  - Inline hex-literal values (e.g. `Value="#DD1E293B"`) never used `StaticResource` — leave untouched.

---

## Task 1: Remove the three ResourceDictionary classes and their tests

**Files:**
- Delete: `src/Tailwind.Avalonia/Colors/ColorResourceDictionary.cs`
- Delete: `src/Tailwind.Avalonia/Spacing/SpacingResourceDictionary.cs`
- Delete: `src/Tailwind.Avalonia/Typography/FontSizeResourceDictionary.cs`
- Delete: `tests/Tailwind.Avalonia.Tests/ColorResourceDictionaryTests.cs`
- Delete: `tests/Tailwind.Avalonia.Tests/SpacingResourceDictionaryTests.cs`
- Delete: `tests/Tailwind.Avalonia.Tests/FontSizeResourceDictionaryTests.cs`

**Interfaces:**
- Consumes: nothing from other tasks.
- Produces: nothing later tasks call — `TailwindColorPalette`, `TailwindCssColorParser`, `SpacingScale`, `FontSizeScale`, and `Tw`/`Tw.Class` are all untouched and keep working exactly as before, since none of them reference these three classes.

- [ ] **Step 1: Confirm nothing else in `src/` references these three classes**

Run: `grep -rn "ColorResourceDictionary\|SpacingResourceDictionary\|FontSizeResourceDictionary" src/Tailwind.Avalonia --include=*.cs`
Expected: only the 3 files' own class declarations match (no other `.cs` file under `src/Tailwind.Avalonia` references them). If anything else matches, stop and investigate before deleting.

- [ ] **Step 2: Delete the 3 library files and their 3 test files**

```bash
git rm src/Tailwind.Avalonia/Colors/ColorResourceDictionary.cs
git rm src/Tailwind.Avalonia/Spacing/SpacingResourceDictionary.cs
git rm src/Tailwind.Avalonia/Typography/FontSizeResourceDictionary.cs
git rm tests/Tailwind.Avalonia.Tests/ColorResourceDictionaryTests.cs
git rm tests/Tailwind.Avalonia.Tests/SpacingResourceDictionaryTests.cs
git rm tests/Tailwind.Avalonia.Tests/FontSizeResourceDictionaryTests.cs
```

- [ ] **Step 3: Verify no dangling references remain in the library or test project**

Run: `grep -rln "ColorResourceDictionary\|SpacingResourceDictionary\|FontSizeResourceDictionary" src/Tailwind.Avalonia tests/Tailwind.Avalonia.Tests --include=*.cs`
Expected: no output (empty). `dotnet build src/Tailwind.Avalonia/Tailwind.Avalonia.csproj` cannot run in this sandbox (see Global Constraints) — note in the task result that a local build confirmation is still needed from the user.

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "feat!: remove StaticResource utility resource dictionaries"
```

---

## Task 2: Delete Themes/Tailwind.axaml and its 7 references

**Files:**
- Delete: `src/Tailwind.Avalonia/Themes/Tailwind.axaml`
- Modify: `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`
- Modify: `samples/Tailwind.Avalonia.Sample/App.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml`

**Interfaces:**
- Consumes: nothing from Task 1.
- Produces: nothing later tasks call — this only removes a now-empty include point. `MainWindow.axaml` and `SampleShell.axaml` never referenced `Themes/Tailwind.axaml` directly (only `App.axaml` and the 6 doc-page `UserControl.Resources` blocks did), so they are untouched by this task.

- [ ] **Step 1: Delete the merge point file**

```bash
git rm src/Tailwind.Avalonia/Themes/Tailwind.axaml
```

- [ ] **Step 2: Remove the `AvaloniaResource` include for the now-deleted folder**

In `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`, remove this block:

```xml
  <ItemGroup>
    <AvaloniaResource
       Include="Themes\**"/>
  </ItemGroup>
```

- [ ] **Step 3: Remove the resource include from `App.axaml`**

Old:
```xml
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceInclude
                    Source="avares://Tailwind.Avalonia/Themes/Tailwind.axaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
```
New: delete the whole `<Application.Resources>` block (nothing else was in it).

- [ ] **Step 4: Remove the same resource-include block from each of the 6 doc pages**

In each of `Margin.axaml`, `Padding.axaml`, `Width.axaml`, `Height.axaml`, `FontSize.axaml`, `ColorUtilities.axaml`, delete this identical block (it appears once near the top of each file, right after the `x:Class` attribute):

```xml
    <UserControl.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceInclude
                    Source="avares://Tailwind.Avalonia/Themes/Tailwind.axaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </UserControl.Resources>
```

Leave the `<UserControl.Styles><StyleInclude Source="/Spacing/SpacingDocsStyles.axaml" /></UserControl.Styles>` block that follows it untouched — that's a different include (the shared page styles, handled in Task 5) and stays.

- [ ] **Step 5: Verify no file still points at the deleted theme**

Run: `grep -rln "Themes/Tailwind.axaml" samples src --include=*.axaml`
Expected: no output.

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "feat!: remove Themes/Tailwind.axaml resource-dictionary merge point"
```

---

## Task 3: Version bump and CHANGELOG

**Files:**
- Modify: `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`
- Create: `CHANGELOG.md`

**Interfaces:**
- Consumes: nothing.
- Produces: nothing later tasks call. This task can run any time relative to the others; it's grouped here because it's the natural place to record what Tasks 1-2 (and 4-13) do as a single breaking release.

- [ ] **Step 1: Add the explicit package version**

In `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`, inside the existing `<PropertyGroup>` (add it right after `<PackageId>Tailwind.Avalonia</PackageId>`):

```xml
    <Version>1.0.0</Version>
```

- [ ] **Step 2: Create `CHANGELOG.md` at the repo root**

```markdown
# Changelog

All notable changes to this project are documented in this file.

## 1.0.0 — 2026-08-19

### Breaking

- Removed `ColorResourceDictionary`, `SpacingResourceDictionary`, and `FontSizeResourceDictionary`, and the `Themes/Tailwind.axaml` resource dictionary that merged them. These generated hundreds of named `{StaticResource ...}` keys (`Margin8`, `Width24`, `FontSizeLg`, `BrushSky500`, etc.) as an alternative to `tw:Tw.Class`.
- **Migration:** replace `Margin="{StaticResource Margin8}"` with `tw:Tw.Class="m-8"` (and the equivalent for padding, width, height, font-size, and color resources — see the README/sample docs for the full utility token reference). Remove any `<ResourceInclude Source="avares://Tailwind.Avalonia/Themes/Tailwind.axaml" />` from your app's resources; it no longer exists.
- This is the first version this package has explicitly declared.
```

- [ ] **Step 3: Verify the csproj is well-formed**

Re-read `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj` and confirm `<Version>1.0.0</Version>` sits inside the single `<PropertyGroup>` alongside `<PackageId>`, and that the file still has exactly one closing `</Project>` tag with no unbalanced elements.

- [ ] **Step 4: Commit**

```bash
git add CHANGELOG.md src/Tailwind.Avalonia/Tailwind.Avalonia.csproj
git commit -m "chore: declare v1.0.0 and record the StaticResource-removal breaking change"
```

---

## Task 4: Rewrite SampleShell.axaml (nav shell Setters)

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/SampleShell.axaml`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm.
- Produces: nothing later tasks call directly, but this is the first file that exercises the Setter-retargeting mechanism — if the visual result here is wrong, the same pattern will be wrong in Task 5 too, so review this one carefully before moving on.

**Complete list of every `Setter` in this file that targets `Background`/`BorderBrush`/`Foreground` with a `{StaticResource Brush...}` value, grouped by `Style Selector`:**

| Selector | Property | Old value | New `tw:Tw.Class` token |
|---|---|---|---|
| `Border.shell-paneSurface` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `Border.shell-navGroup` | Background | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `Border.shell-navGroup` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `Border.shell-headerSurface` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `Border.shell-contentFrame` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `Button.shell-toggle` | Background | `{StaticResource BrushSlate900}` | `bg-slate-900` |
| `Button.shell-toggle` | BorderBrush | `{StaticResource BrushSlate700}` | `border-slate-700` |
| `Button.shell-toggle` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TextBlock.shell-navLabel` | Foreground | `{StaticResource BrushSlate500}` | `text-slate-500` |
| `TextBlock.shell-currentSection` | Foreground | `{StaticResource BrushSky300}` | `text-sky-300` |
| `TextBlock.shell-currentPage` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TabStrip.navigation-strip TabStripItem` | Foreground | `{StaticResource BrushSlate400}` | `text-slate-400` |
| `TabStrip.navigation-strip TabStripItem` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `TabStrip.navigation-strip TabStripItem:pointerover` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TabStrip.navigation-strip TabStripItem:pointerover` | BorderBrush | `{StaticResource BrushSlate700}` | `border-slate-700` |
| `TabStrip.navigation-strip TabStripItem:selected` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TabStrip.navigation-strip TabStripItem:selected` | BorderBrush | `{StaticResource BrushSky500}` | `border-sky-500` |
| `TabStrip.navigation-strip.section-strip TabStripItem:selected` | BorderBrush | `{StaticResource BrushSky400}` | `border-sky-400` |
| `TabStrip.navigation-strip.page-strip TabStripItem:selected` | BorderBrush | `{StaticResource BrushViolet400}` | `border-violet-400` |

Note: `Button.shell-toggle.accent`, `Button.shell-toggle:pointerover`, `Button.shell-toggle:pressed`, and the `TabStrip...:pointerover`/`:selected` blocks that only set `Opacity`/`Background` hex literals (`#DD1E293B`, `#CC172554`, etc.) never used `StaticResource` — leave those `Setter`s untouched.

**Two direct (non-Setter) attribute usages, on the `SplitView` and the `/` separator `TextBlock`:**

| Element (unique context) | Old | New |
|---|---|---|
| `<SplitView x:Name="NavigationSplitView" ...>` root `Background` | `{StaticResource BrushSlate950}` | `tw:Tw.Class="bg-slate-950"` |
| `<Grid Background="..." ClipToBounds="True" RowDefinitions="Auto,*">` inside `SplitView.Content` | `{StaticResource BrushSlate950}` | `tw:Tw.Class="bg-slate-950"` |
| `<TextBlock Grid.Column="1" Foreground="..." Text="/" .../>` (the section/page separator) | `{StaticResource BrushSlate600}` | `tw:Tw.Class="text-slate-600"` |

- [ ] **Step 1: Add the `tw` namespace to the root element**

This file currently has no `xmlns:tw`. Add it to the `<UserControl ...>` opening tag, alongside the existing `xmlns:mc`:

Old:
```xml
<UserControl
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
```
New:
```xml
<UserControl
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:tw="using:Tailwind.Avalonia"
    mc:Ignorable="d"
```

- [ ] **Step 2: Worked example — convert `Border.shell-paneSurface` (single-property Setter, establishes the pattern)**

Old:
```xml
        <Style
            Selector="Border.shell-paneSurface">
            <Setter
                Property="Background"
                Value="#F80B1120" />
            <Setter
                Property="BorderBrush"
                Value="{StaticResource BrushSlate800}" />
            <Setter
                Property="BorderThickness"
                Value="0,0,1,0" />
            <Setter
                Property="CornerRadius"
                Value="0" />
            <Setter
                Property="ClipToBounds"
                Value="True" />
        </Style>
```
New (the hex-literal `Background` Setter is untouched; only the `BorderBrush` Setter retargets):
```xml
        <Style
            Selector="Border.shell-paneSurface">
            <Setter
                Property="Background"
                Value="#F80B1120" />
            <Setter
                Property="tw:Tw.Class"
                Value="border-slate-800" />
            <Setter
                Property="BorderThickness"
                Value="0,0,1,0" />
            <Setter
                Property="CornerRadius"
                Value="0" />
            <Setter
                Property="ClipToBounds"
                Value="True" />
        </Style>
```

- [ ] **Step 3: Worked example — convert `Button.shell-toggle` (multiple properties collapse into one `tw:Tw.Class` Setter)**

Old:
```xml
        <Style
            Selector="Button.shell-toggle">
            <Setter
                Property="Background"
                Value="{StaticResource BrushSlate900}" />
            <Setter
                Property="BorderBrush"
                Value="{StaticResource BrushSlate700}" />
            <Setter
                Property="BorderThickness"
                Value="1" />
            <Setter
                Property="Foreground"
                Value="{StaticResource BrushWhite}" />
```
New (three separate Setters for Background/BorderBrush/Foreground collapse into a single `tw:Tw.Class` Setter carrying all three tokens; `BorderThickness` and everything after it is untouched):
```xml
        <Style
            Selector="Button.shell-toggle">
            <Setter
                Property="tw:Tw.Class"
                Value="bg-slate-900 border-slate-700 text-white" />
            <Setter
                Property="BorderThickness"
                Value="1" />
```

- [ ] **Step 4: Apply the same pattern to the remaining 12 Setters from the table in Step 0**

For each remaining row in the table above: if it is the only `StaticResource`-valued color Setter in its `Style` block, replace `Property="Background"/"BorderBrush"/"Foreground"` with `Property="tw:Tw.Class"` and the `Value` with the token from the table (same single-property pattern as Step 2). Where a `Style` block has more than one color Setter (e.g. `Border.shell-navGroup` has both `Background` and `BorderBrush`; `TabStrip.navigation-strip TabStripItem` has both `Foreground` and `BorderBrush`; the two `:pointerover`/`:selected` `TabStripItem` blocks each have `Foreground` + `BorderBrush`), collapse them into one `tw:Tw.Class` Setter carrying both tokens space-separated (same multi-property pattern as Step 3), and leave every other property in that block (`BorderThickness`, `CornerRadius`, `Padding`, `Width`, `Height`, `FontWeight`, alignment properties, hex-literal `Background`/`Opacity` Setters) untouched.

- [ ] **Step 5: Convert the two direct `Background` attributes and the separator `TextBlock`'s `Foreground`**

Old (on `SplitView`):
```xml
    <SplitView
        x:Name="NavigationSplitView"
        Background="{StaticResource BrushSlate950}"
        CompactPaneLength="0"
```
New:
```xml
    <SplitView
        x:Name="NavigationSplitView"
        tw:Tw.Class="bg-slate-950"
        CompactPaneLength="0"
```

Old (on the content `Grid`):
```xml
            <Grid
                Background="{StaticResource BrushSlate950}"
                ClipToBounds="True"
                RowDefinitions="Auto,*">
```
New:
```xml
            <Grid
                tw:Tw.Class="bg-slate-950"
                ClipToBounds="True"
                RowDefinitions="Auto,*">
```

Old (the `/` separator):
```xml
                            <TextBlock
                                Grid.Column="1"
                                Foreground="{StaticResource BrushSlate600}"
                                Text="/"
                                VerticalAlignment="Center" />
```
New:
```xml
                            <TextBlock
                                Grid.Column="1"
                                tw:Tw.Class="text-slate-600"
                                Text="/"
                                VerticalAlignment="Center" />
```

- [ ] **Step 6: Verify no `StaticResource` reference remains in this file**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/SampleShell.axaml`
Expected: no output (this file has zero legitimate leftover resources — `HamburgerIconGeometry`/`CloseIconGeometry` are referenced via `{StaticResource ...}` too since they're `UserControl.Resources`-scoped `StreamGeometry` keys defined in this same file, not from a removed dictionary, so they will still show up — confirm any remaining matches are exactly `HamburgerIconGeometry` or `CloseIconGeometry` and nothing else).

- [ ] **Step 7: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/SampleShell.axaml
git commit -m "feat!: convert SampleShell.axaml color Setters from StaticResource to tw:Tw.Class"
```

---

## Task 5: Rewrite SpacingDocsStyles.axaml (shared doc-page Setters + the DrawingBrush exception)

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Spacing/SpacingDocsStyles.axaml`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm; the Setter-retargeting pattern established in Task 4.
- Produces: this file is `<StyleInclude>`d by all 6 doc pages (Tasks 7-12), so it must be fully converted before those pages are visually verified.

This file has no `xmlns:tw` declared yet (it's a `<Styles>` root, not `<UserControl>`) and no pseudo-class-driven color Setters — every color Setter here is a plain base-state Setter, so each one converts 1:1 with no multi-selector precedence concerns.

**Complete list of color Setters to convert, by `Style Selector`:**

| Selector | Property | Old value | New token |
|---|---|---|---|
| `TextBlock.docs-library` | Foreground | `{StaticResource BrushSlate500}` | `text-slate-500` |
| `TextBlock.docs-pageTitle` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TextBlock.docs-eyebrow` | Foreground | `{StaticResource BrushSlate400}` | `text-slate-400` |
| `TextBlock.docs-body` | Foreground | `{StaticResource BrushSlate300}` | `text-slate-300` |
| `TextBlock.docs-sectionTitle` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TextBlock.docs-noteTitle` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TextBlock.docs-noteBody` | Foreground | `{StaticResource BrushSlate300}` | `text-slate-300` |
| `TextBlock.docs-miniTitle` | Foreground | `{StaticResource BrushSlate200}` | `text-slate-200` |
| `TextBlock.docs-code` | Foreground | `{StaticResource BrushSlate200}` | `text-slate-200` |
| `TextBlock.docs-chip` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TabItem.docs-exampleTab` | Foreground | `{StaticResource BrushSlate300}` | `text-slate-300` |
| `TabItem.docs-exampleTab` | Background | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `TabItem.docs-exampleTab` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `TabItem.docs-exampleTab:selected` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TabItem.docs-exampleTab:selected` | Background | `{StaticResource BrushSlate800}` | `bg-slate-800` |
| `TabItem.docs-exampleTab:selected` | BorderBrush | `{StaticResource BrushSlate700}` | `border-slate-700` |
| `Border.docs-surface` | Background | `{StaticResource BrushSlate900}` | `bg-slate-900` |
| `Border.docs-surface` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `Border.docs-note` | Background | `{StaticResource BrushSlate900}` | `bg-slate-900` |
| `Border.docs-note` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `Border.docs-inset` | Background | `{StaticResource BrushSlate800}` | `bg-slate-800` |
| `Border.docs-inset` | BorderBrush | `{StaticResource BrushSlate700}` | `border-slate-700` |
| `Border.docs-paddingCore` | Background | `{StaticResource BrushViolet600}` | `bg-violet-600` |
| `Border.docs-marginShell` | BorderBrush | `{StaticResource BrushSlate700}` | `border-slate-700` |
| `Border.docs-referenceHeader` | Background | `{StaticResource BrushSlate900}` | `bg-slate-900` |
| `Border.docs-referenceHeader` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `Border.docs-referenceRow` | Background | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `Border.docs-referenceRow` | BorderBrush | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `TextBlock.docs-referenceHeading` | Foreground | `{StaticResource BrushWhite}` | `text-white` |
| `TextBlock.docs-tableClass` | Foreground | `{StaticResource BrushSky400}` | `text-sky-400` |
| `TextBlock.docs-tableStyle` | Foreground | `{StaticResource BrushViolet300}` | `text-violet-300` |
| `Button.docs-showMoreButton` | BorderBrush | `{StaticResource BrushSlate600}` | `border-slate-600` |
| `Button.docs-showMoreButton` | Background | `{StaticResource BrushSlate700}` | `bg-slate-700` |
| `Button.docs-showMoreButton` | Foreground | `{StaticResource BrushWhite}` | `text-white` |

Not in scope: `Border.docs-paddingShell`'s and `Border.docs-marginShell`'s `Background="{StaticResource SamplePaddingStripeBrush}"` Setters — `SamplePaddingStripeBrush` is a **local** `DrawingBrush` resource (defined in this same file's `<Styles.Resources>`), not a `Brush*` key from the removed `ColorResourceDictionary`. It is a valid `StaticResource` reference to a resource that still exists after this change, so leave `Background="{StaticResource SamplePaddingStripeBrush}"` on both selectors exactly as-is. Only the `GeometryDrawing.Brush` *inside* the `SamplePaddingStripeBrush` definition itself needs to change (Step 3 below), because that one color came from the removed `ColorResourceDictionary`.

- [ ] **Step 1: Add the `tw` namespace to the root element**

Old:
```xml
<Styles
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:sample="using:Tailwind.Avalonia.Sample">
```
New:
```xml
<Styles
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:sample="using:Tailwind.Avalonia.Sample"
    xmlns:tw="using:Tailwind.Avalonia">
```

- [ ] **Step 2: Worked example — convert `TextBlock.docs-library` (single-property) and `TabItem.docs-exampleTab` (multi-property, base + `:selected` states stay as separate Setters since they're different selectors)**

Old:
```xml
    <Style
        Selector="TextBlock.docs-library">
        <Setter
            Property="Foreground"
            Value="{StaticResource BrushSlate500}" />
        <Setter
            Property="FontSize"
            Value="12" />
        <Setter
            Property="FontWeight"
            Value="SemiBold" />
    </Style>
```
New:
```xml
    <Style
        Selector="TextBlock.docs-library">
        <Setter
            Property="tw:Tw.Class"
            Value="text-slate-500" />
        <Setter
            Property="FontSize"
            Value="12" />
        <Setter
            Property="FontWeight"
            Value="SemiBold" />
    </Style>
```

Old:
```xml
    <Style
        Selector="TabItem.docs-exampleTab">
        <Setter
            Property="MinWidth"
            Value="{StaticResource DocsExampleTabMinWidth}" />
        <Setter
            Property="Margin"
            Value="0,0,8,0" />
        <Setter
            Property="Padding"
            Value="14,10" />
        <Setter
            Property="FontSize"
            Value="13" />
        <Setter
            Property="FontWeight"
            Value="SemiBold" />
        <Setter
            Property="Foreground"
            Value="{StaticResource BrushSlate300}" />
        <Setter
            Property="Background"
            Value="{StaticResource BrushSlate950}" />
        <Setter
            Property="BorderBrush"
            Value="{StaticResource BrushSlate800}" />
        <Setter
            Property="BorderThickness"
            Value="1" />
        <Setter
            Property="HorizontalContentAlignment"
            Value="Center" />
    </Style>
```
New (note `MinWidth="{StaticResource DocsExampleTabMinWidth}"` is untouched — it's a local `x:Double` resource defined in this same file's `<Styles.Resources>`, not from the removed dictionaries):
```xml
    <Style
        Selector="TabItem.docs-exampleTab">
        <Setter
            Property="MinWidth"
            Value="{StaticResource DocsExampleTabMinWidth}" />
        <Setter
            Property="Margin"
            Value="0,0,8,0" />
        <Setter
            Property="Padding"
            Value="14,10" />
        <Setter
            Property="FontSize"
            Value="13" />
        <Setter
            Property="FontWeight"
            Value="SemiBold" />
        <Setter
            Property="tw:Tw.Class"
            Value="text-slate-300 bg-slate-950 border-slate-800" />
        <Setter
            Property="BorderThickness"
            Value="1" />
        <Setter
            Property="HorizontalContentAlignment"
            Value="Center" />
    </Style>
```

- [ ] **Step 3: The one unavoidable exception — `SamplePaddingStripeBrush`'s nested `GeometryDrawing.Brush`**

`Tw.Class` cannot reach a `Brush` property nested inside a `GeometryDrawing` resource — it only patches properties directly on the element it's attached to. This one reference must become a literal color instead. Because this sandbox cannot restore NuGet packages (see Global Constraints), do not guess the hex value. Instead:

1. In a local environment with working NuGet access, temporarily add this test to `tests/Tailwind.Avalonia.Tests/`:
   ```csharp
   using Avalonia.Media;
   namespace Tailwind.Avalonia.Tests;
   public class _TempColorProbe
   {
       [Fact]
       public void Probe()
       {
           TailwindColorPalette.TryGetColor("violet-400", out var color);
           Assert.Fail(color.ToString());
       }
   }
   ```
2. Run `dotnet test tests/Tailwind.Avalonia.Tests --filter Probe` and read the exact hex string out of the assertion failure message (Avalonia's `Color.ToString()` prints `#AARRGGBB`).
3. Delete the temporary test file (`git rm` it, or just delete and don't commit it).
4. Use that exact hex string as a literal `Color` on the `GeometryDrawing`:

Old:
```xml
                    <GeometryDrawing
                        Brush="{StaticResource BrushViolet400}"
                        Geometry="M0,0 H24 V24 H0 Z" />
```
New (replace `<HEX>` with the exact string captured in step 2, e.g. `#FFA78BFA` — do not invent this value):
```xml
                    <GeometryDrawing
                        Brush="<HEX>"
                        Geometry="M0,0 H24 V24 H0 Z" />
```

- [ ] **Step 4: Apply the same pattern to the remaining 30 Setters from the table in Step 0**

For each remaining row: single-color-property `Style` blocks (`TextBlock.docs-pageTitle`, `.docs-eyebrow`, `.docs-body`, `.docs-sectionTitle`, `.docs-noteTitle`, `.docs-noteBody`, `.docs-miniTitle`, `.docs-code`, `.docs-chip`, `.docs-referenceHeading`, `.docs-tableClass`, `.docs-tableStyle`, `Border.docs-paddingCore`) follow the Step 2 single-property pattern. Multi-color-property blocks (`TabItem.docs-exampleTab:selected` [Foreground+Background+BorderBrush], `Border.docs-surface` [Background+BorderBrush], `Border.docs-note` [Background+BorderBrush], `Border.docs-inset` [Background+BorderBrush], `Border.docs-referenceHeader` [Background+BorderBrush], `Border.docs-referenceRow` [Background+BorderBrush], `Button.docs-showMoreButton` [BorderBrush+Background+Foreground]) follow the Step 2 multi-property pattern, collapsing into one `tw:Tw.Class` Setter with space-separated tokens. `Border.docs-marginShell`'s lone `BorderBrush="{StaticResource BrushSlate700}"` Setter converts to `tw:Tw.Class="border-slate-700"` on its own (its `Background="{StaticResource SamplePaddingStripeBrush}"` sibling Setter stays untouched, per the note above — so this selector ends up with both a `Background` Setter and a `tw:Tw.Class` Setter side by side, which is fine since they target different properties). Leave every non-color property (`FontSize`, `FontWeight`, `TextWrapping`, `MaxWidth`, `CornerRadius`, `Padding`, `ClipToBounds`, `HorizontalContentAlignment`, `MinWidth`, `Margin`, the `sample:DocsCodeTextBehavior.NormalizeIndent` Setter, and all `UserControl.docs-mobile ...` responsive-override `Style` blocks that only touch `FontSize`/`Padding`/`Spacing`/`Margin`/`MinWidth`/`ColumnSpacing`) untouched.

- [ ] **Step 5: Verify no removed-dictionary `StaticResource` reference remains**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/Spacing/SpacingDocsStyles.axaml`
Expected: only `DocsExampleTabMinWidth` (an `x:Double`) and `SamplePaddingStripeBrush` (the local `DrawingBrush`, referenced by `docs-paddingShell` and `docs-marginShell`) remain. No `Brush*` matches.

- [ ] **Step 6: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Spacing/SpacingDocsStyles.axaml
git commit -m "feat!: convert SpacingDocsStyles.axaml color Setters from StaticResource to tw:Tw.Class"
```

---

## Task 6: Rewrite MainWindow.axaml

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/MainWindow.axaml`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm.
- Produces: nothing later tasks call.

- [ ] **Step 1: Add the `tw` namespace and convert the one `Background` attribute**

Old (full file):
```xml
<Window
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:local="using:Tailwind.Avalonia.Sample"
    mc:Ignorable="d"
    d:DesignWidth="1440"
    d:DesignHeight="920"
    x:Class="Tailwind.Avalonia.Sample.MainWindow"
    Title="Tailwind.Avalonia Docs Sample"
    Background="{StaticResource BrushSlate950}">
    <local:SampleShell />
</Window>
```
New:
```xml
<Window
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:local="using:Tailwind.Avalonia.Sample"
    xmlns:tw="using:Tailwind.Avalonia"
    mc:Ignorable="d"
    d:DesignWidth="1440"
    d:DesignHeight="920"
    x:Class="Tailwind.Avalonia.Sample.MainWindow"
    Title="Tailwind.Avalonia Docs Sample"
    tw:Tw.Class="bg-slate-950">
    <local:SampleShell />
</Window>
```

- [ ] **Step 2: Verify**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/MainWindow.axaml`
Expected: no output.

- [ ] **Step 3: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/MainWindow.axaml
git commit -m "feat!: convert MainWindow.axaml background from StaticResource to tw:Tw.Class"
```

---

## Task 7: Rewrite Margin.axaml

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm; assumes Task 2 already removed this file's `UserControl.Resources` block.
- Produces: nothing later tasks call.

**Part A — delete 6 `StaticResource` comparison `TabItem`s wholesale (with their `TabControl` collapsing to the surviving single `TabItem`'s content):**

| Section (`docs-sectionTitle` Text) | Identifying content inside the `StaticResource` `TabItem` to delete |
|---|---|
| "Basic example" | `Margin="{StaticResource Margin8}"`, chip text `"Margin8"` |
| "Adding margin to one side" | `Margin="{StaticResource MarginTop6}"` / `MarginRight4` / `MarginBottom8` / `MarginLeft2` |
| "Adding horizontal and vertical margin" | `Margin="{StaticResource MarginX8}"` / `MarginY8` |
| "Using negative values" | `Margin="{StaticResource NegativeMarginTop8}"` |
| "Using logical properties" | The note-only `TabItem` whose sole content is a `TextBlock` reading "Unsupported: generated StaticResource margin keys only cover physical directions..." |
| "Arbitrary margin values" | The `StackPanel.Resources` block declaring `ArbitraryMargin8`/`ArbitraryMarginAxis`/`ArbitraryNegativeMargin4` |

**Part B — convert every remaining direct color attribute in the surviving content** (the page's root `Border`, every `showMoreShell`/AXAML-code-block divider `BorderBrush`, and every demo chip `Background` inside the kept "Utility" tabs):

| Unique context | Old | New token |
|---|---|---|
| Page root `<Border Background="..." >` (wraps the whole page) | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `showMoreShell` Border | `Background="{StaticResource BrushSlate900}"` | `bg-slate-900` |
| `showMoreShell` Border | `BorderBrush="{StaticResource BrushSlate800}"` | `border-slate-800` (merge with the token above into one `tw:Tw.Class`) |
| Every `<Border BorderBrush="{StaticResource BrushSlate800}" BorderThickness="0,1,0,0" Padding="20,16">` AXAML-code divider (appears once per kept example block — 6 occurrences: Basic example, one-side margin, horizontal/vertical margin, negative values, logical-properties "Actual usage", arbitrary margin) | `{StaticResource BrushSlate800}` | `border-slate-800` |
| Every `Background="{StaticResource BrushSlate950}"` on the demo-preview `Border` wrapping each `TabItem Header="Utility"` (6 occurrences, one per kept example) | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `m-8` demo chip | `Background="{StaticResource BrushSky500}"` | `bg-sky-500` |
| `ml-2` demo chip | `Background="{StaticResource BrushViolet500}"` (×4: `mt-6`, `ml-2`, `mr-4`, `mb-8` chips all use `BrushViolet500`) | `bg-violet-500` |
| `mx-8` demo chip | `Background="{StaticResource BrushAmber500}"` | `bg-amber-500` |
| `my-8` demo chip | `Background="{StaticResource BrushRose500}"` | `bg-rose-500` |
| "base block" chip (negative-values section) | `Background="{StaticResource BrushSky400}"` | `bg-sky-400` |
| `-mt-8` chip | `Background="{StaticResource BrushBlue500}"` | `bg-blue-500` |
| `ms-8`/`msv-8` chips (lime, ×3 occurrences: LTR real-RTL demo, RTL real-RTL demo, visualized-side-mapping demo) | `Background="{StaticResource BrushLime500}"` | `bg-lime-500` |
| `me-8`/`mev-8` chips (cyan, ×3 occurrences, same 3 demos) | `Background="{StaticResource BrushCyan500}"` | `bg-cyan-500` |
| `mbs-8` chip (×2: LTR and RTL block-start demos) | `Background="{StaticResource BrushFuchsia500}"` | `bg-fuchsia-500` |
| `mbe-8` chip (×2: LTR and RTL block-end demos) | `Background="{StaticResource BrushSky500}"` | `bg-sky-500` |
| `m-[8px]` chip (arbitrary values) | `Background="{StaticResource BrushSky500}"` | `bg-sky-500` |
| `mx/my` chip (arbitrary values) | `Background="{StaticResource BrushEmerald500}"` | `bg-emerald-500` |
| "base block" chip (arbitrary values) | `Background="{StaticResource BrushSky400}"` | `bg-sky-400` |
| `-m-[4px]` chip (arbitrary values) | `Background="{StaticResource BrushOrange500}"` | `bg-orange-500` |

- [ ] **Step 1: Worked example — delete the "Basic example" `StaticResource` `TabItem` and collapse its `TabControl`**

Old:
```xml
                            <TabControl
                                Classes="docs-exampleTabs">
                                <TabItem
                                    Classes="docs-exampleTab"
                                    Header="Utility">
                                    <StackPanel
                                        Spacing="0">
                                        <Border
                                            Background="{StaticResource BrushSlate950}"
                                            Height="208"
                                            Padding="28">
                                            <Grid>
                                                <Border
                                                    Classes="docs-marginShell"
                                                    HorizontalAlignment="Center"
                                                    VerticalAlignment="Center">
                                                    <Grid
                                                        Width="280"
                                                        Height="120">
                                                        <Border
                                                            Background="{StaticResource BrushSky500}"
                                                            CornerRadius="12"
                                                            HorizontalAlignment="Stretch"
                                                            VerticalAlignment="Stretch"
                                                            tw:Tw.Class="m-8">
                                                            <TextBlock
                                                                Classes="docs-chip"
                                                                HorizontalAlignment="Center"
                                                                VerticalAlignment="Center"
                                                                Text="m-8" />
                                                        </Border>
                                                    </Grid>
                                                </Border>
                                            </Grid>
                                        </Border>
                                        <Border
                                            BorderBrush="{StaticResource BrushSlate800}"
                                            BorderThickness="0,1,0,0"
                                            Padding="20,16">
                                            <StackPanel
                                                Spacing="6">
                                                <TextBlock
                                                    Classes="docs-miniTitle"
                                                    Text="AXAML" />
                                                <TextBlock
                                                    Classes="docs-code">
                                                    &lt;Border tw:Tw.Class=&quot;m-8&quot; /&gt;
                                                </TextBlock>
                                            </StackPanel>
                                        </Border>
                                    </StackPanel>
                                </TabItem>
                                <TabItem
                                    Classes="docs-exampleTab"
                                    Header="StaticResource">
                                    <StackPanel
                                        Spacing="0">
                                        <Border
                                            Background="{StaticResource BrushSlate950}"
                                            Height="208"
                                            Padding="28">
                                            <Grid>
                                                <Border
                                                    Classes="docs-marginShell"
                                                    HorizontalAlignment="Center"
                                                    VerticalAlignment="Center">
                                                    <Grid
                                                        Width="280"
                                                        Height="120">
                                                        <Border
                                                            Background="{StaticResource BrushSky500}"
                                                            CornerRadius="12"
                                                            HorizontalAlignment="Stretch"
                                                            VerticalAlignment="Stretch"
                                                            Margin="{StaticResource Margin8}">
                                                            <TextBlock
                                                                Classes="docs-chip"
                                                                HorizontalAlignment="Center"
                                                                VerticalAlignment="Center"
                                                                Text="Margin8" />
                                                        </Border>
                                                    </Grid>
                                                </Border>
                                            </Grid>
                                        </Border>
                                        <Border
                                            BorderBrush="{StaticResource BrushSlate800}"
                                            BorderThickness="0,1,0,0"
                                            Padding="20,16">
                                            <StackPanel
                                                Spacing="6">
                                                <TextBlock
                                                    Classes="docs-miniTitle"
                                                    Text="AXAML" />
                                                <TextBlock
                                                    Classes="docs-code">
                                                    &lt;Border Margin=&quot;{StaticResource Margin8}&quot; /&gt;
                                                </TextBlock>
                                            </StackPanel>
                                        </Border>
                                    </StackPanel>
                                </TabItem>
                            </TabControl>
```
New (the `TabControl`/`TabItem Header="Utility"` wrapper is removed too, since it now holds only one child — the `Border Classes="docs-surface"` from the parent scope goes directly to the former Utility `StackPanel`, with all its `{StaticResource Brush...}` occurrences converted per the table above):
```xml
                            <StackPanel
                                Spacing="0">
                                <Border
                                    tw:Tw.Class="bg-slate-950"
                                    Height="208"
                                    Padding="28">
                                    <Grid>
                                        <Border
                                            Classes="docs-marginShell"
                                            HorizontalAlignment="Center"
                                            VerticalAlignment="Center">
                                            <Grid
                                                Width="280"
                                                Height="120">
                                                <Border
                                                    tw:Tw.Class="bg-sky-500 m-8"
                                                    CornerRadius="12"
                                                    HorizontalAlignment="Stretch"
                                                    VerticalAlignment="Stretch">
                                                    <TextBlock
                                                        Classes="docs-chip"
                                                        HorizontalAlignment="Center"
                                                        VerticalAlignment="Center"
                                                        Text="m-8" />
                                                </Border>
                                            </Grid>
                                        </Border>
                                    </Grid>
                                </Border>
                                <Border
                                    tw:Tw.Class="border-slate-800"
                                    BorderThickness="0,1,0,0"
                                    Padding="20,16">
                                    <StackPanel
                                        Spacing="6">
                                        <TextBlock
                                            Classes="docs-miniTitle"
                                            Text="AXAML" />
                                        <TextBlock
                                            Classes="docs-code">
                                            &lt;Border tw:Tw.Class=&quot;m-8&quot; /&gt;
                                        </TextBlock>
                                    </StackPanel>
                                </Border>
                            </StackPanel>
```

Note the element that already had `tw:Tw.Class="m-8"` merges the new `bg-sky-500` token into the same attribute (`tw:Tw.Class="bg-sky-500 m-8"`) rather than getting a second `tw:Tw.Class` attribute — an XAML element cannot repeat the same attribute name twice.

- [ ] **Step 2: Apply the same collapse to the other 5 `StaticResource` `TabItem`s listed in Part A**, converting every `{StaticResource Brush...}` in the surviving "Utility" content per the Part B table as you go (each surviving Utility `TabItem`'s own `Background="{StaticResource BrushSlate950}"` preview wrapper and the trailing AXAML-code divider's `BorderBrush="{StaticResource BrushSlate800}"` both need the same treatment shown in Step 1, once per kept example).

- [ ] **Step 3: Convert the page root `Border`**

Old:
```xml
    <Border
        Background="{StaticResource BrushSlate950}">
```
New:
```xml
    <Border
        tw:Tw.Class="bg-slate-950">
```

- [ ] **Step 4: Convert the `showMoreShell` Border**

Old:
```xml
                            <Border
                                Background="{StaticResource BrushSlate900}"
                                BorderBrush="{StaticResource BrushSlate800}"
                                Padding="0,12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```
New:
```xml
                            <Border
                                tw:Tw.Class="bg-slate-900 border-slate-800"
                                Padding="0,12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```

- [ ] **Step 5: Verify no removed-dictionary `StaticResource` reference remains**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml`
Expected: no output at all (this file has no locally-scoped resources left after Part A removes the `StackPanel.Resources` block that held `ArbitraryMargin8` etc.).

- [ ] **Step 6: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml
git commit -m "feat!: convert Margin.axaml from StaticResource comparisons to tw:Tw.Class only"
```

---

## Task 8: Rewrite Padding.axaml

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm; the exact collapse mechanics demonstrated in Task 7 Step 1.
- Produces: nothing later tasks call.

**Part A — delete 5 `StaticResource` comparison `TabItem`s wholesale:**

| Section | Identifying content inside the `StaticResource` `TabItem` to delete |
|---|---|
| "Basic example" | `Padding="{StaticResource Padding8}"`, chip text `"Padding8"` |
| "Adding padding to one side" | `Padding="{StaticResource PaddingTop6}"` / `PaddingRight4` / `PaddingBottom8` / `PaddingLeft2` |
| "Adding horizontal and vertical padding" | `Padding="{StaticResource PaddingX8}"` / `PaddingY8` |
| "Using logical properties" | The note-only `TabItem` reading "Unsupported: generated StaticResource padding keys only cover physical directions..." |
| "Arbitrary padding values" | The `StackPanel.Resources` block declaring `ArbitraryPadding12`/`ArbitraryPaddingAxis`/`ArbitraryPaddingEdges` |

**Part B — convert remaining direct color attributes in surviving content:**

| Unique context | Old | New token |
|---|---|---|
| Page root `Border` | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `showMoreShell` Border | `Background="{StaticResource BrushSlate900}"` + `BorderBrush="{StaticResource BrushSlate800}"` | `bg-slate-900 border-slate-800` |
| Every kept-example preview `Border Background="{StaticResource BrushSlate950}"` (5 occurrences: basic, one-side, horiz/vert, logical-properties, arbitrary) | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| Every kept-example AXAML-divider `Border BorderBrush="{StaticResource BrushSlate800}"` (5 occurrences, same sections) | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `docs-paddingCore` note: `Border.docs-paddingCore`'s `Background` comes from `SpacingDocsStyles.axaml` (Task 5), not from this file — no per-instance chip color attributes exist in Padding.axaml's demo chips (they rely entirely on the shared `docs-paddingShell`/`docs-paddingCore` style classes) | n/a | n/a |

- [ ] **Step 1: Apply the exact TabItem-collapse + attribute-conversion pattern from Task 7 Step 1** to each of the 5 `StaticResource` `TabItem`s listed in Part A, converting `Background="{StaticResource BrushSlate950}"` and `BorderBrush="{StaticResource BrushSlate800}"` in each surviving "Utility" `TabItem`'s content per Part B as you go.

- [ ] **Step 2: Convert the page root `Border`**

Old: `Background="{StaticResource BrushSlate950}">` (on the outermost `<Border>`)
New: `tw:Tw.Class="bg-slate-950">`

- [ ] **Step 3: Convert the `showMoreShell` Border**

Old:
```xml
                            <Border
                                Background="{StaticResource BrushSlate900}"
                                BorderBrush="{StaticResource BrushSlate800}"
                                Padding="0, 12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```
New:
```xml
                            <Border
                                tw:Tw.Class="bg-slate-900 border-slate-800"
                                Padding="0, 12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```

- [ ] **Step 4: Verify**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml`
Expected: no output.

- [ ] **Step 5: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml
git commit -m "feat!: convert Padding.axaml from StaticResource comparisons to tw:Tw.Class only"
```

---

## Task 9: Rewrite Width.axaml

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm; the collapse mechanics from Task 7 Step 1.
- Produces: nothing later tasks call.

**Part A — delete 3 `StaticResource` comparison `TabItem`s wholesale:**

| Section | Identifying content to delete |
|---|---|
| "Basic example" | `Width="{StaticResource Width24}"` / `Width40` / `Width64`, chip texts `"Width24"`/`"Width40"`/`"Width64"` |
| "Setting a minimum width" | `MinWidth="{StaticResource MinWidth24}"` / `MinWidth40` |
| "Setting a maximum width" | `MaxWidth="{StaticResource MaxWidth24}"` / `MaxWidth40` |
| "Using a custom value" | The `StackPanel.Resources` block declaring `ArbitraryWidth100`/`ArbitraryMinWidth200`/`ArbitraryMaxWidth80` |

**Part B — convert remaining direct color attributes:**

| Unique context | Old | New token |
|---|---|---|
| Page root `Border` | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `showMoreShell` Border | `Background="{StaticResource BrushSlate900}"` + `BorderBrush="{StaticResource BrushSlate800}"` | `bg-slate-900 border-slate-800` |
| Every kept-example preview `Border Background="{StaticResource BrushSlate950}"` (4 occurrences: basic, min-width, max-width, custom-value) | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| Every kept-example AXAML-divider `BorderBrush="{StaticResource BrushSlate800}"` (4 occurrences, same sections) | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `w-24` chip | `Background="{StaticResource BrushSky500}"` | `bg-sky-500` |
| `w-40` chip | `Background="{StaticResource BrushViolet500}"` | `bg-violet-500` |
| `w-64` chip | `Background="{StaticResource BrushEmerald500}"` | `bg-emerald-500` |
| `min-w-24` chip | `Background="{StaticResource BrushAmber500}"` | `bg-amber-500` |
| `min-w-40` chip | `Background="{StaticResource BrushOrange500}"` | `bg-orange-500` |
| `max-w-24` chip | `Background="{StaticResource BrushRose500}"` | `bg-rose-500` |
| `max-w-40` chip | `Background="{StaticResource BrushPink500}"` | `bg-pink-500` |
| `w-[100px]` chip | `Background="{StaticResource BrushSky500}"` | `bg-sky-500` |
| `min-w-[12.5rem]` chip | `Background="{StaticResource BrushViolet500}"` | `bg-violet-500` |
| `max-w-[5em]` chip | `Background="{StaticResource BrushEmerald500}"` | `bg-emerald-500` |

- [ ] **Step 1: Apply the TabItem-collapse pattern from Task 7 Step 1** to each of the 3 `StaticResource` `TabItem`s in Part A, converting the surviving "Utility" content's `Background`/chip colors per Part B as you go. In the "Basic example" and "Setting a minimum/maximum width" sections, each demo `Border` already has `tw:Tw.Class="w-24"` (etc.) — merge the new `bg-...` token into that same attribute (e.g. `tw:Tw.Class="bg-sky-500 w-24"`), same as Task 7 Step 1's note.

- [ ] **Step 2: Convert the page root `Border`**

Old: `Background="{StaticResource BrushSlate950}">`
New: `tw:Tw.Class="bg-slate-950">`

- [ ] **Step 3: Convert the `showMoreShell` Border**

Old:
```xml
                            <Border
                                Background="{StaticResource BrushSlate900}"
                                BorderBrush="{StaticResource BrushSlate800}"
                                Padding="0,12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```
New:
```xml
                            <Border
                                tw:Tw.Class="bg-slate-900 border-slate-800"
                                Padding="0,12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```

- [ ] **Step 4: Verify**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml`
Expected: no output.

- [ ] **Step 5: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml
git commit -m "feat!: convert Width.axaml from StaticResource comparisons to tw:Tw.Class only"
```

---

## Task 10: Rewrite Height.axaml

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm; the collapse mechanics from Task 7 Step 1.
- Produces: nothing later tasks call.

**Part A — delete 3 `StaticResource` comparison `TabItem`s wholesale:**

| Section | Identifying content to delete |
|---|---|
| "Basic example" | `Height="{StaticResource Height24}"` / `Height40` / `Height64` |
| "Setting a minimum height" | `MinHeight="{StaticResource MinHeight24}"` / `MinHeight40` |
| "Setting a maximum height" | `MaxHeight="{StaticResource MaxHeight24}"` / `MaxHeight40` |
| "Using a custom value" | The `StackPanel.Resources` block declaring `ArbitraryHeight60`/`ArbitraryMinHeight128`/`ArbitraryMaxHeight80` |

**Part B — convert remaining direct color attributes:**

| Unique context | Old | New token |
|---|---|---|
| Page root `Border` | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `showMoreShell` Border | `Background="{StaticResource BrushSlate900}"` + `BorderBrush="{StaticResource BrushSlate800}"` | `bg-slate-900 border-slate-800` |
| Every kept-example preview `Border Background="{StaticResource BrushSlate950}"` (4 occurrences: basic, min-height, max-height, custom-value) | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| Every kept-example AXAML-divider `BorderBrush="{StaticResource BrushSlate800}"` (4 occurrences) | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `h-24` chip | `Background="{StaticResource BrushSky500}"` | `bg-sky-500` |
| `h-40` chip | `Background="{StaticResource BrushViolet500}"` | `bg-violet-500` |
| `h-64` chip | `Background="{StaticResource BrushEmerald500}"` | `bg-emerald-500` |
| `min-h-24` chip | `Background="{StaticResource BrushAmber500}"` | `bg-amber-500` |
| `min-h-40` chip | `Background="{StaticResource BrushOrange500}"` | `bg-orange-500` |
| `max-h-24` chip | `Background="{StaticResource BrushRose500}"` | `bg-rose-500` |
| `max-h-40` chip | `Background="{StaticResource BrushPink500}"` | `bg-pink-500` |
| `h-[60px]` chip | `Background="{StaticResource BrushSky500}"` | `bg-sky-500` |
| `min-h-[8rem]` chip | `Background="{StaticResource BrushViolet500}"` | `bg-violet-500` |
| `max-h-[5em]` chip | `Background="{StaticResource BrushEmerald500}"` | `bg-emerald-500` |

- [ ] **Step 1: Apply the TabItem-collapse pattern from Task 7 Step 1** to each of the 3 `StaticResource` `TabItem`s in Part A, converting the surviving "Utility" content per Part B as you go (merge new `bg-...` tokens into each demo `Border`'s existing `tw:Tw.Class`, same merge note as Task 9 Step 1).

- [ ] **Step 2: Convert the page root `Border`**

Old: `Background="{StaticResource BrushSlate950}">`
New: `tw:Tw.Class="bg-slate-950">`

- [ ] **Step 3: Convert the `showMoreShell` Border**

Old:
```xml
                            <Border
                                Background="{StaticResource BrushSlate900}"
                                BorderBrush="{StaticResource BrushSlate800}"
                                Padding="0,12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```
New:
```xml
                            <Border
                                tw:Tw.Class="bg-slate-900 border-slate-800"
                                Padding="0,12"
                                Classes="docs-showMoreShell"
                                HorizontalAlignment="Center">
```

- [ ] **Step 4: Verify**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml`
Expected: no output.

- [ ] **Step 5: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml
git commit -m "feat!: convert Height.axaml from StaticResource comparisons to tw:Tw.Class only"
```

---

## Task 11: Rewrite FontSize.axaml and FontSize.axaml.cs

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml.cs`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm; the collapse mechanics from Task 7 Step 1.
- Produces: nothing later tasks call.

**Part A — delete 2 `StaticResource` comparison `TabItem`s wholesale:**

| Section | Identifying content to delete |
|---|---|
| "Basic example" | `FontSize="{StaticResource FontSizeXs}"` / `FontSizeBase` / `FontSize2xl` / `FontSize5xl` |
| "Using a custom value" | The `StackPanel.Resources` block declaring `ArbitraryFontSize14`/`ArbitraryFontSize24`/`ArbitraryFontSize32` |

**Part B — convert remaining direct color attributes:**

| Unique context | Old | New token |
|---|---|---|
| Page root `Border` | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `showMoreShell` Border | `Background="{StaticResource BrushSlate900}"` + `BorderBrush="{StaticResource BrushSlate800}"` | `bg-slate-900 border-slate-800` |
| Basic-example preview `Border Background` | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| Basic-example AXAML-divider `BorderBrush` | `{StaticResource BrushSlate800}` | `border-slate-800` |
| Custom-value preview `Border Background` | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| Custom-value AXAML-divider `BorderBrush` | `{StaticResource BrushSlate800}` | `border-slate-800` |
| `text-xs`/`text-base` demo `TextBlock`s | `Foreground="{StaticResource BrushSlate100}"` (×2) | `text-slate-100` (merge into existing `tw:Tw.Class="text-xs"` etc.) |
| `text-2xl` demo `TextBlock` | `Foreground="{StaticResource BrushSlate100}"` | `text-slate-100` |
| `text-5xl` demo `TextBlock` | `Foreground="{StaticResource BrushSlate50}"` | `text-slate-50` |
| `text-[14px]` demo `TextBlock` | `Foreground="{StaticResource BrushSlate100}"` | `text-slate-100` |
| `text-[1.5rem]` demo `TextBlock` | `Foreground="{StaticResource BrushSlate100}"` | `text-slate-100` |
| `text-[2em]` demo `TextBlock` | `Foreground="{StaticResource BrushSlate50}"` | `text-slate-50` |

- [ ] **Step 1: Apply the TabItem-collapse pattern from Task 7 Step 1** to the "Basic example" and "Using a custom value" `StaticResource` `TabItem`s, converting the surviving "Utility" content's `Foreground`/`Background`/`BorderBrush` per Part B (merge `text-slate-100` etc. into each `TextBlock`'s existing `tw:Tw.Class`, e.g. `tw:Tw.Class="text-slate-100 text-xs"`).

- [ ] **Step 2: Convert the page root `Border`, `showMoreShell`, and reference-table AXAML text**

Same pattern as prior tasks for the root `Border` and `showMoreShell`. Additionally, this page's reference-table rows (in the `.cs` file, handled in Step 3) are the only place besides `ColorUtilities.axaml.cs` where the doc-string `AxamlStyle` text itself says `StaticResource` — the `.axaml` file's own markup has no other `StaticResource` usage beyond what's listed above.

- [ ] **Step 3: Rewrite the reference-table `AllUtilityRows` in `FontSize.axaml.cs`**

Old:
```csharp
    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("text-xs", "<TextBlock FontSize=\"{StaticResource FontSizeXs}\" />"),
        new("text-sm", "<TextBlock FontSize=\"{StaticResource FontSizeSm}\" />"),
        new("text-base", "<TextBlock FontSize=\"{StaticResource FontSizeBase}\" />"),
        new("text-lg", "<TextBlock FontSize=\"{StaticResource FontSizeLg}\" />"),
        new("text-xl", "<TextBlock FontSize=\"{StaticResource FontSizeXl}\" />"),
        new("text-2xl", "<TextBlock FontSize=\"{StaticResource FontSize2xl}\" />"),
        new("text-3xl", "<TextBlock FontSize=\"{StaticResource FontSize3xl}\" />"),
        new("text-4xl", "<TextBlock FontSize=\"{StaticResource FontSize4xl}\" />"),
        new("text-5xl", "<TextBlock FontSize=\"{StaticResource FontSize5xl}\" />"),
        new("text-6xl", "<TextBlock FontSize=\"{StaticResource FontSize6xl}\" />"),
        new("text-7xl", "<TextBlock FontSize=\"{StaticResource FontSize7xl}\" />"),
        new("text-8xl", "<TextBlock FontSize=\"{StaticResource FontSize8xl}\" />"),
        new("text-9xl", "<TextBlock FontSize=\"{StaticResource FontSize9xl}\" />"),
        new("text-[<value>]", "<TextBlock FontSize=\"<parsed absolute value>\" />"),
    ];
```
New (the "Styles" column now shows the actual `tw:Tw.Class` usage instead of the removed `StaticResource` syntax — consistent with how Margin/Padding/Width/Height's reference tables already show real physical output, this shows the real markup a consumer would write):
```csharp
    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("text-xs", "<TextBlock tw:Tw.Class=\"text-xs\" />"),
        new("text-sm", "<TextBlock tw:Tw.Class=\"text-sm\" />"),
        new("text-base", "<TextBlock tw:Tw.Class=\"text-base\" />"),
        new("text-lg", "<TextBlock tw:Tw.Class=\"text-lg\" />"),
        new("text-xl", "<TextBlock tw:Tw.Class=\"text-xl\" />"),
        new("text-2xl", "<TextBlock tw:Tw.Class=\"text-2xl\" />"),
        new("text-3xl", "<TextBlock tw:Tw.Class=\"text-3xl\" />"),
        new("text-4xl", "<TextBlock tw:Tw.Class=\"text-4xl\" />"),
        new("text-5xl", "<TextBlock tw:Tw.Class=\"text-5xl\" />"),
        new("text-6xl", "<TextBlock tw:Tw.Class=\"text-6xl\" />"),
        new("text-7xl", "<TextBlock tw:Tw.Class=\"text-7xl\" />"),
        new("text-8xl", "<TextBlock tw:Tw.Class=\"text-8xl\" />"),
        new("text-9xl", "<TextBlock tw:Tw.Class=\"text-9xl\" />"),
        new("text-[<value>]", "<TextBlock tw:Tw.Class=\"text-[<value>]\" />"),
    ];
```

- [ ] **Step 4: Verify**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml.cs`
Expected: no output from either file.

- [ ] **Step 5: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml.cs
git commit -m "feat!: convert FontSize.axaml from StaticResource comparisons to tw:Tw.Class only"
```

---

## Task 12: Rewrite ColorUtilities.axaml and ColorUtilities.axaml.cs

**Files:**
- Modify: `samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml`
- Modify: `samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml.cs`

**Interfaces:**
- Consumes: the Global Constraints conversion algorithm; the collapse mechanics from Task 7 Step 1.
- Produces: nothing later tasks call. This is the last doc-page file — Task 13 does the final full-repo sweep after this.

**Part A — delete 3 `StaticResource` comparison `TabItem`s wholesale:**

| Section | Identifying content to delete |
|---|---|
| "Text color" | `Foreground="{StaticResource BrushSky300}"`, the `SolidColorBrush Color="{StaticResource ColorRose300}"` block, `Foreground="{StaticResource BrushSlate200}"` |
| "Background and border color" | `Background="{StaticResource BrushEmerald500}"`, the `ColorAmber400`/`ColorAmber300` `SolidColorBrush` blocks, the "Unsupported StaticResource equivalent: bg-transparent..." note `Border`, the `ColorViolet500`/`ColorViolet300` `SolidColorBrush` blocks |
| "Whole-property border scope" | `BorderBrush="{StaticResource BrushCyan400}"`, the `SolidColorBrush Color="{StaticResource ColorRose400}"` block |
| "Arbitrary color values" | The `StackPanel.Resources` block declaring `ArbitraryTextCoralBrush`/`ArbitraryTextTeal70Brush`/`ArbitraryBackgroundMintBrush`/`ArbitraryTextGreenBrush`/`ArbitraryBorderTealBrush` |

Note "Arbitrary color values" only has the resources block above listed because its `StaticResource` `TabItem`'s preview content itself uses those local brushes (not `Brush*`/`Color*` palette keys) — the whole `TabItem` still gets deleted per the standard rule, this just documents what's inside it for identification.

**Part B — convert remaining direct color attributes:**

| Unique context | Old | New token |
|---|---|---|
| Page root `Border` | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| `showMoreShell` Border | `Background="{StaticResource BrushSlate900}"` + `BorderBrush="{StaticResource BrushSlate800}"` | `bg-slate-900 border-slate-800` |
| Every kept-example preview `Border Background="{StaticResource BrushSlate950}"` (4 occurrences: text color, bg/border color, border scope, arbitrary values) | `{StaticResource BrushSlate950}` | `bg-slate-950` |
| Every kept-example AXAML-divider `BorderBrush="{StaticResource BrushSlate800}"` (4 occurrences) | `{StaticResource BrushSlate800}` | `border-slate-800` |

No other direct color attributes exist in the surviving "Utility" tab content on this page — every demo element there already uses `tw:Tw.Class` exclusively (that's the whole point of this page: it demonstrates `bg-*`/`text-*`/`border-*` directly).

- [ ] **Step 1: Apply the TabItem-collapse pattern from Task 7 Step 1** to each of the 3 `StaticResource` `TabItem`s in Part A.

- [ ] **Step 2: Convert the page root `Border`, `showMoreShell`, and the 4 kept-example wrapper Borders**

Same pattern as prior tasks.

- [ ] **Step 3: Rewrite the reference-table `AllUtilityRows` in `ColorUtilities.axaml.cs`**

Old:
```csharp
    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("bg-&lt;color&gt;", "<Control Background=\"{StaticResource BrushBlue500}\" />"),
        new("text-&lt;color&gt;", "<Control Foreground=\"{StaticResource BrushBlue500}\" />"),
        new("border-&lt;color&gt;", "<Border BorderBrush=\"{StaticResource BrushBlue500}\" />"),
        new("*&lt;color&gt;/&lt;opacity&gt;", "<SolidColorBrush Color=\"{StaticResource ColorBlue500}\" Opacity=\"0.5\" />"),
        new("bg-[#&lt;hex&gt;]", "<Control Background=\"arbitrary hex brush\" />"),
        new("text-[#&lt;hex&gt;]", "<Control Foreground=\"arbitrary hex brush\" />"),
        new("border-[#&lt;hex&gt;]", "<Border BorderBrush=\"arbitrary hex brush\" />"),
        new("text-[#&lt;hex&gt;]/&lt;opacity&gt;", "<Control Foreground=\"arbitrary hex brush with opacity\" />"),
    ];
```
New (the "Styles" column now shows the real `tw:Tw.Class` markup instead of the removed `StaticResource`/raw-`SolidColorBrush` syntax):
```csharp
    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("bg-&lt;color&gt;", "<Control tw:Tw.Class=\"bg-blue-500\" />"),
        new("text-&lt;color&gt;", "<Control tw:Tw.Class=\"text-blue-500\" />"),
        new("border-&lt;color&gt;", "<Border tw:Tw.Class=\"border-blue-500\" />"),
        new("*&lt;color&gt;/&lt;opacity&gt;", "<Control tw:Tw.Class=\"bg-blue-500/50\" />"),
        new("bg-[#&lt;hex&gt;]", "<Control tw:Tw.Class=\"bg-[#3b82f6]\" />"),
        new("text-[#&lt;hex&gt;]", "<Control tw:Tw.Class=\"text-[#3b82f6]\" />"),
        new("border-[#&lt;hex&gt;]", "<Border tw:Tw.Class=\"border-[#3b82f6]\" />"),
        new("text-[#&lt;hex&gt;]/&lt;opacity&gt;", "<Control tw:Tw.Class=\"text-[#3b82f6]/50\" />"),
    ];
```

- [ ] **Step 4: Verify**

Run: `grep -n StaticResource samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml.cs`
Expected: no output from either file.

- [ ] **Step 5: Commit**

```bash
git add samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml.cs
git commit -m "feat!: convert ColorUtilities.axaml from StaticResource comparisons to tw:Tw.Class only"
```

---

## Task 13: Full-repo sweep and manual verification

**Files:**
- None modified directly — this task only verifies Tasks 1-12.

**Interfaces:**
- Consumes: the completed state of every prior task.
- Produces: the final go/no-go signal for this change.

- [ ] **Step 1: Repo-wide grep for any remaining reference to the removed API surface**

Run:
```bash
grep -rn "ColorResourceDictionary\|SpacingResourceDictionary\|FontSizeResourceDictionary\|Themes/Tailwind.axaml" --include=*.cs --include=*.axaml .
```
Expected: no output anywhere in `src/`, `samples/`, or `tests/`.

- [ ] **Step 2: Repo-wide grep for leftover `StaticResource` usages of the removed keys**

Run:
```bash
grep -rn "StaticResource" samples/Tailwind.Avalonia.Sample --include=*.axaml
```
Expected: matches only `DocsExampleTabMinWidth`, `SamplePaddingStripeBrush`, `HamburgerIconGeometry`, `CloseIconGeometry` — every `Brush*`, `Color*`, `Margin*`, `Padding*`, `Width*`, `Height*`, `MinWidth*`, `MaxWidth*`, `MinHeight*`, `MaxHeight*`, `NegativeMargin*`, `FontSize*` key must be gone. If anything else matches, go back to the corresponding file's task and finish the conversion.

- [ ] **Step 3: Ask the user to build and run locally**

This sandbox cannot restore NuGet packages (Global Constraints). Report to the user that the mechanical conversion and the two greps above are complete, and ask them to run, in their own environment:
```bash
dotnet build
dotnet run --project samples/Tailwind.Avalonia.Sample.Desktop
```
and confirm: (a) the build succeeds with no XAML compilation errors, (b) the nav shell renders with the same colors as before this change, (c) hovering/pressing/selecting every nav `TabStripItem` and toggle `Button` shows the same visual states as before, (d) each of the 6 doc pages (Margin, Padding, Width, Height, FontSize, ColorUtilities) renders correctly with only the "Utility" example content (no leftover "StaticResource" tab), and (e) the "SHOW MORE" reference tables on FontSize and ColorUtilities show the updated `tw:Tw.Class` syntax in their "Styles" column.

- [ ] **Step 4: Record the outcome**

If the user reports build/visual issues, fix them in the relevant task's file before considering this plan complete. If everything checks out, this plan is done — no further commit needed for this task (it's verification-only).
