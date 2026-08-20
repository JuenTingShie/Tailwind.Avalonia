# Pseudo-Class Variants for tw:Tw.Class Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add Tailwind-style pseudo-class variants (`hover:`, `pressed:`, `focus:`) to `tw:Tw.Class`, scoped to color utilities (`bg-`/`text-`/`border-`) and a new `opacity-*` utility, backed by Avalonia's own `:pointerover`/`:pressed`/`:focus` selectors.

**Architecture:** Background/Foreground/BorderBrush/Opacity move from direct `element.SetValue(...)` (local value) to per-element `Avalonia.Styling.Style` objects added to `element.Styles`. This is required, not optional: Avalonia's value precedence puts local values above style triggers unconditionally, so a `hover:` style can never out-rank a base value set via `SetValue`, no matter the selector. Making both base and variant go through `Style` objects lets normal Avalonia cascade/precedence resolve which one wins. Confirmed empirically (throwaway spikes, deleted, not part of this plan) that this works with zero `Application`/headless bootstrap — a bare `new Border()` resolves styles fine — but resolution is no longer synchronous: a property set via `Style` only reflects after `StyledElement.ApplyStyling()` runs, whereas the old `SetValue` path was immediate. (An earlier iteration of this plan used `Measure`+`Arrange` to force that resolution; that broke on `TextBlock`, whose `MeasureOverride` throws `Unable to locate 'Avalonia.Platform.IFontManagerImpl'` with no platform/font backend registered — unrelated to styling, but fatal all the same. `ApplyStyling()` is the public, parameterless, no-layout method Avalonia itself uses to force this resolution, confirmed via reflection and a throwaway spike to behave identically for base-value resolution, pseudo-class toggling, and style removal/reversion — and it never touches text layout, so it works on `TextBlock` too.) This is a real, user-visible breaking change (see Task 2) and gets a major version bump (Task 4).

Spacing, sizing, and font-size utilities are untouched — they keep using `SetValue`/mask-based clearing exactly as today; only color and opacity utilities move to the new mechanism.

**Tech Stack:** C# / .NET 10, Avalonia 12.0.2, xUnit. No new package references. New Avalonia APIs used: `Avalonia.Styling.Style`, `Avalonia.Styling.Setter`, the `Selectors.Is(Type)` / `Selectors.Class(string)` selector-builder extensions, and `Avalonia.Controls.IPseudoClasses` (test-only, to simulate pseudo-class state without pointer/focus input).

**Spec:** No separate spec document — this is a bounded-scope addition approved in chat during brainstorming (feature: hover/pressed/focus variants on colors+opacity). The behavior-change consequence (deferred resolution, existing tests need layout calls) was surfaced and explicitly re-approved in chat before this plan was written. This plan is the only written artifact.

## Global Constraints

- No new NuGet package references.
- Variant scope is v1-limited to colors (`bg-`, `text-`, `border-`) and the new `opacity-*` utility. `hover:p-4`, `hover:w-24`, etc. are NOT supported — a variant-prefixed token whose remainder isn't a brush or opacity utility logs the existing "unrecognized utility token" warning citing the *raw* token (e.g. `hover:p-4`), it does not silently fall through to plain (non-variant) parsing.
- Every new test follows the existing `TwTests.cs` style: plain xUnit `[Fact]`/`[Theory]`, no headless Avalonia app bootstrap. Tests that read a resolved `Background`/`Foreground`/`BorderBrush`/`Opacity` value MUST call `element.ApplyStyling()` first (new requirement introduced by this plan) — NOT `Measure`+`Arrange`, which breaks on `TextBlock` (see Architecture note above). Tests that only read a `Thickness`/sizing/`FontSize` value, or only inspect the log sink, do NOT need it — that part of the pipeline is unchanged.
- Simulate pseudo-class state in tests via `((IPseudoClasses)element.Classes).Add(":pointerover")` / `.Remove(...)` (from `Avalonia.Controls`, already `using`'d in `TwTests.cs`) — never via real pointer/input events.
- When multiple variant pseudo-classes are simultaneously active (e.g. hovering AND pressed at once), the variant declared later in `VariantKind` (`Hover` < `Pressed` < `Focus`) wins — its `Style` is added to `element.Styles` after the earlier one, and Avalonia resolves same-specificity conflicts by insertion order. No extra conflict-resolution logic. This is a documented v1 behavior, not a bug.
- Full test suite command is `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` (NOT bare `dotnet test` — the `.slnx` also includes `samples/Tailwind.Avalonia.Sample.Browser`, which fails with `NETSDK1147` unless the `wasm-tools` workload is installed; pre-existing environment gap, unrelated to this plan).

---

## File Structure

- **Create:** `src/Tailwind.Avalonia/Tw/Tw.Variants.cs` — `VariantKind` enum, pseudo-class mapping, variant-token parsing, and the new `Style`-based apply/track/clear mechanism for colors + opacity (built incrementally across Tasks 1–3).
- **Modify:** `src/Tailwind.Avalonia/Tw/Tw.Parsing.cs` — add `TryParseOpacityUtility`.
- **Modify:** `src/Tailwind.Avalonia/Tw/Tw.Apply.cs` — wire opacity parsing, variant-token parsing, and the new `ApplyVariantStyles` call into `ApplyUtilities`; remove the old `SetValue`-based Background/Foreground/BorderBrush entries.
- **Modify:** `src/Tailwind.Avalonia/Tw/Tw.cs` — delete the now-unused `BackgroundMask`/`ForegroundMask`/`BorderBrushMask` constants.
- **Modify:** `src/Tailwind.Avalonia/Tw/Tw.PropertyAccess.cs` — delete `TrySetBrush`/`ClearBrush`, now dead code once Background/Foreground/BorderBrush no longer go through the mask-based `SetValue` path.
- **Modify:** `tests/Tailwind.Avalonia.Tests/TwTests.cs` — add `ApplyStyling()` to existing color-utility tests that read resolved values; add new opacity tests and new variant tests.
- **Modify:** `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs` — add `ApplyStyling()` to its 11 existing tests that read a resolved `Background`/`Foreground`/`BorderBrush` value (discovered during Task 2 execution — this file wasn't in the original plan's inventory; it exercises the same color-utility code path with arbitrary/hex values, so it needs the identical fix).
- **Modify:** `CHANGELOG.md`, `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj` — version bump to 2.0.0.

---

### Task 1: Add a Style-based `opacity-*` base utility (no variants yet)

**Files:**
- Create: `src/Tailwind.Avalonia/Tw/Tw.Variants.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Parsing.cs` (add method after `TryParseFontSizeUtility`, currently ending at line 145)
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`
- Test: `tests/Tailwind.Avalonia.Tests/TwTests.cs`

**Interfaces:**
- Consumes: `TryParseOpacity(string token, out double opacity)` — already exists in `Tw.ColorParsing.cs:46`, parses a `0`-`100` percent string to a `0.0`-`1.0` fraction. `FindDoubleProperty(Type, string)` — already exists in `Tw.PropertyAccess.cs:121`.
- Produces: `TryParseOpacityUtility(string token, out double opacity)`, `Tw.OpacityCategoryState` (record struct: `bool HasBase, double Base`), `Tw.AppliedVariantStylesProperty`, `Tw.ApplyVariantStyles(AvaloniaObject element, OpacityCategoryState opacity)`. Tasks 2 and 3 extend all four of these — the exact shapes matter for later tasks, don't rename.

- [ ] **Step 1: Write the failing tests**

Add to `tests/Tailwind.Avalonia.Tests/TwTests.cs` (anywhere among the other `SetClass_Applies_*` tests, e.g. right after `SetClass_Applies_Sizing_Utilities`):

```csharp
[Fact]
public void SetClass_Applies_Opacity_Utility()
{
    var border = new Border();

    Tw.SetClass(border, "opacity-50");
    border.ApplyStyling();

    Assert.Equal(0.5d, border.Opacity);
}

[Fact]
public void SetClass_Uses_Last_Opacity_Utility()
{
    var border = new Border();

    Tw.SetClass(border, "opacity-80 opacity-20");
    border.ApplyStyling();

    Assert.Equal(0.2d, border.Opacity);
}

[Fact]
public void SetClass_Clears_Previously_Applied_Opacity_When_Class_Removed()
{
    var border = new Border();

    Tw.SetClass(border, "opacity-50");
    Tw.SetClass(border, null);
    border.ApplyStyling();

    Assert.Equal(1d, border.Opacity);
}
```

- [ ] **Step 2: Run the tests and confirm they fail**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "FullyQualifiedName~SetClass_Applies_Opacity_Utility|FullyQualifiedName~SetClass_Uses_Last_Opacity_Utility|FullyQualifiedName~SetClass_Clears_Previously_Applied_Opacity_When_Class_Removed"`

Expected: FAIL — `opacity-50` is an unrecognized token today (`border.Opacity` stays at its default `1`, so `SetClass_Applies_Opacity_Utility` and `SetClass_Uses_Last_Opacity_Utility` fail their `Assert.Equal`; `SetClass_Clears_Previously_Applied_Opacity_When_Class_Removed` happens to pass already since nothing ever changes `Opacity` — that's fine, it becomes a real regression guard once Step 4 lands).

- [ ] **Step 3: Add `TryParseOpacityUtility`**

In `src/Tailwind.Avalonia/Tw/Tw.Parsing.cs`, add this method immediately after `TryParseFontSizeUtility` (which currently ends at line 145, right before the `private delegate bool ScalePixelLookup` line):

```csharp

    private static bool TryParseOpacityUtility(string token, out double opacity)
    {
        opacity = default;

        if (!token.StartsWith("opacity-", StringComparison.Ordinal) ||
            token.Contains(':') ||
            token.Contains('('))
        {
            return false;
        }

        var valueToken = token["opacity-".Length..];

        return TryParseOpacity(valueToken, out opacity);
    }
```

- [ ] **Step 4: Create `Tw.Variants.cs` with the Style-based opacity mechanism**

Create `src/Tailwind.Avalonia/Tw/Tw.Variants.cs`:

```csharp
using Avalonia;
using Avalonia.Logging;
using Avalonia.Styling;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private readonly record struct OpacityCategoryState(bool HasBase, double Base);

    private static readonly AttachedProperty<List<Style>?> AppliedVariantStylesProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, List<Style>?>("AppliedVariantStyles");

    private static void ApplyVariantStyles(AvaloniaObject element, OpacityCategoryState opacity)
    {
        if (element is not StyledElement styled)
        {
            return;
        }

        var previous = element.GetValue(AppliedVariantStylesProperty);

        if (previous is not null)
        {
            foreach (var style in previous)
            {
                styled.Styles.Remove(style);
            }
        }

        var newStyles = new List<Style>();

        AddOpacityStyles(styled, opacity, newStyles);

        element.SetValue(AppliedVariantStylesProperty, newStyles.Count > 0 ? newStyles : null);
    }

    private static void AddOpacityStyles(StyledElement element, OpacityCategoryState state, List<Style> target)
    {
        if (!state.HasBase)
        {
            return;
        }

        var property = FindDoubleProperty(element.GetType(), "Opacity");

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' numeric property on {ElementType}; the utility was ignored.",
                "Opacity",
                element.GetType());
            return;
        }

        var elementType = element.GetType();
        var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
        element.Styles.Add(style);
        target.Add(style);
    }
}
```

- [ ] **Step 5: Wire opacity parsing and application into `ApplyUtilities`**

In `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`:

Add two locals alongside the other `has*`/value locals (after `var hasFontSize = false;` / `var fontSize = default(double);`):

```csharp
        var hasOpacity = false;
        var opacity = default(double);
```

Insert an opacity check in the token loop, right after the `TryParseFontSizeUtility` block and before `if (!TryParseBrushUtility(token, out var brushUtility))`:

```csharp
            if (TryParseOpacityUtility(token, out var opacityUtility))
            {
                opacity = opacityUtility;
                hasOpacity = true;
                continue;
            }

```

At the very end of the method, right after `element.SetValue(AppliedMaskProperty, newMask);`, add:

```csharp

        ApplyVariantStyles(element, new OpacityCategoryState(hasOpacity, opacity));
```

- [ ] **Step 6: Run the tests and confirm they pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "FullyQualifiedName~SetClass_Applies_Opacity_Utility|FullyQualifiedName~SetClass_Uses_Last_Opacity_Utility|FullyQualifiedName~SetClass_Clears_Previously_Applied_Opacity_When_Class_Removed"`

Expected: PASS (all 3).

- [ ] **Step 7: Run the full test suite**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`

Expected: PASS, no regressions (this task adds a new code path, doesn't touch any existing one).

- [ ] **Step 8: Commit**

```bash
git add src/Tailwind.Avalonia/Tw/Tw.Variants.cs src/Tailwind.Avalonia/Tw/Tw.Parsing.cs src/Tailwind.Avalonia/Tw/Tw.Apply.cs tests/Tailwind.Avalonia.Tests/TwTests.cs
git commit -m "feat: add opacity-* utility backed by Avalonia styles"
```

---

### Task 2: Migrate Background/Foreground/BorderBrush to the Style-based mechanism

**Files:**
- Modify: `src/Tailwind.Avalonia/Tw/Tw.cs:10-12` (delete 3 mask constants)
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Variants.cs` (extend from Task 1)
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Apply.cs` (remove old pendingUtilities entries, update final call)
- Modify: `src/Tailwind.Avalonia/Tw/Tw.PropertyAccess.cs:44-70` (delete dead `TrySetBrush`/`ClearBrush`)
- Modify: `tests/Tailwind.Avalonia.Tests/TwTests.cs` (add `ApplyStyling()` to 4 existing tests)
- Modify: `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs` (add `ApplyStyling()` to 11 existing tests — discovered mid-task, not in the original file inventory; see note below)

**Interfaces:**
- Consumes: `FindBrushProperty(Type, string)` — already exists in `Tw.PropertyAccess.cs:111`, unchanged.
- Produces: `Tw.BrushCategoryState` (record struct: `bool HasBase, IBrush? Base`), extends `ApplyVariantStyles` to `(AvaloniaObject element, BrushCategoryState background, BrushCategoryState foreground, BrushCategoryState borderBrush, OpacityCategoryState opacity)`. Task 3 extends this signature further (adds variant arrays to both record types) — do not change the parameter order established here.

This task is a pure mechanism swap for *base* values only — no new user-visible utility syntax. The existing color-utility tests are the regression guard; four of them in `TwTests.cs` need `ApplyStyling()` added because they now read a value that resolves through Avalonia's style engine instead of a plain local value. (Tests that only check the log sink, or that assert an *unset* value that was never touched, don't need it — confirmed by inspection: nothing changes when nothing is ever applied.) **Discovered during execution:** `TwArbitraryValuesTests.cs` exercises the same `bg-`/`text-`/`border-` code path with arbitrary/hex color values and was missed from this plan's original file inventory — 11 of its tests hit the identical synchronous-read problem and need the identical fix. Any future task touching this code path should grep the whole test project for `.Background)`/`.Foreground)`/`.BorderBrush)`/`.Opacity` reads, not just `TwTests.cs`.

- [ ] **Step 1: Update the 4 existing tests that read a Style-resolved value**

In `tests/Tailwind.Avalonia.Tests/TwTests.cs`, replace these four test bodies exactly as shown (only the bodies change — signatures/attributes stay the same):

```csharp
    [Fact]
    public void SetClass_Applies_Color_Utilities()
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));
        Assert.True(TailwindColorPalette.TryGetColor("green-800", out var green800));
        Assert.True(TailwindColorPalette.TryGetColor("orange-800", out var orange800));

        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(textBlock, "text-orange-800");

        border.ApplyStyling();
        textBlock.ApplyStyling();

        Assert.Equal(blue700, Assert.IsType<SolidColorBrush>(border.Background).Color);
        Assert.Equal(green800, Assert.IsType<SolidColorBrush>(border.BorderBrush).Color);
        Assert.Equal(orange800, Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }
```

```csharp
    [Fact]
    public void SetClass_Clears_Previously_Applied_Color_Utilities_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(border, null);

        border.ApplyStyling();

        Assert.Null(border.Background);
        Assert.Null(border.BorderBrush);
    }
```

```csharp
    [Fact]
    public void SetClass_Applies_Transparent_And_Opacity_Color_Utilities()
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));
        Assert.True(TailwindColorPalette.TryGetColor("orange-800", out var orange800));

        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700/50 border-transparent");
        Tw.SetClass(textBlock, "text-orange-800/25");

        border.ApplyStyling();
        textBlock.ApplyStyling();

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal((byte)128, background.A);
        Assert.Equal(blue700.R, background.R);
        Assert.Equal(blue700.G, background.G);
        Assert.Equal(blue700.B, background.B);
        Assert.Equal(Colors.Transparent, borderBrush);
        Assert.Equal((byte)64, foreground.A);
        Assert.Equal(orange800.R, foreground.R);
        Assert.Equal(orange800.G, foreground.G);
        Assert.Equal(orange800.B, foreground.B);
    }
```

```csharp
    [Fact]
    public void SetClass_Applies_Font_Size_Utilities_And_Preserves_Text_Color_Parsing()
    {
        Assert.True(TailwindColorPalette.TryGetColor("sky-300", out var sky300));
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-base text-sky-300");

        textBlock.ApplyStyling();

        Assert.Equal(16d, textBlock.FontSize);
        Assert.Equal(sky300, Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }
```

- [ ] **Step 2: Run the full suite and confirm the 4 updated tests still pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`

Expected: PASS, including the 4 tests just touched — `SetValue` resolves synchronously, so adding `ApplyStyling()` doesn't change what they observe yet. This is a baseline checkpoint, not a red bar; the real red state comes next, from deleting the mechanism these tests depend on.

- [ ] **Step 3: Delete the mask constants and the dead `SetValue`-based brush helpers**

In `src/Tailwind.Avalonia/Tw/Tw.cs`, delete these 3 lines:

```csharp
    private const int BackgroundMask = 4;
    private const int ForegroundMask = 8;
    private const int BorderBrushMask = 16;
```

In `src/Tailwind.Avalonia/Tw/Tw.PropertyAccess.cs`, delete `TrySetBrush` and `ClearBrush` in full (currently lines 44-70):

```csharp
    private static bool TrySetBrush(AvaloniaObject element, string propertyName, IBrush? value)
    {
        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' brush property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearBrush(AvaloniaObject element, string propertyName)
    {
        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

```

(`FindBrushProperty` and `BrushPropertyCache` stay — Step 4 below still calls `FindBrushProperty` directly.)

- [ ] **Step 4: Extend `Tw.Variants.cs` with `BrushCategoryState` and `AddBrushStyles`, and wire in the 3 brush categories**

In `src/Tailwind.Avalonia/Tw/Tw.Variants.cs`, add `using Avalonia.Media;` to the usings, add the new record struct next to `OpacityCategoryState`:

```csharp
    private readonly record struct BrushCategoryState(bool HasBase, IBrush? Base);
```

Replace the `ApplyVariantStyles` method with:

```csharp
    private static void ApplyVariantStyles(
        AvaloniaObject element,
        BrushCategoryState background,
        BrushCategoryState foreground,
        BrushCategoryState borderBrush,
        OpacityCategoryState opacity)
    {
        if (element is not StyledElement styled)
        {
            return;
        }

        var previous = element.GetValue(AppliedVariantStylesProperty);

        if (previous is not null)
        {
            foreach (var style in previous)
            {
                styled.Styles.Remove(style);
            }
        }

        var newStyles = new List<Style>();

        AddBrushStyles(styled, "Background", background, newStyles);
        AddBrushStyles(styled, "Foreground", foreground, newStyles);
        AddBrushStyles(styled, "BorderBrush", borderBrush, newStyles);
        AddOpacityStyles(styled, opacity, newStyles);

        element.SetValue(AppliedVariantStylesProperty, newStyles.Count > 0 ? newStyles : null);
    }

    private static void AddBrushStyles(StyledElement element, string propertyName, BrushCategoryState state, List<Style> target)
    {
        if (!state.HasBase)
        {
            return;
        }

        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' brush property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return;
        }

        var elementType = element.GetType();
        var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
        element.Styles.Add(style);
        target.Add(style);
    }
```

- [ ] **Step 5: Remove the old `SetValue`-based Background/Foreground/BorderBrush handling from `ApplyUtilities`**

In `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`, remove these 3 lines from the `pendingUtilities` `Span<PendingUtility>` initializer:

```csharp
            new(BackgroundMask, hasBackground, () => TrySetBrush(element, "Background", background), () => ClearBrush(element, "Background")),
            new(ForegroundMask, hasForeground, () => TrySetBrush(element, "Foreground", foreground), () => ClearBrush(element, "Foreground")),
            new(BorderBrushMask, hasBorderBrush, () => TrySetBrush(element, "BorderBrush", borderBrush), () => ClearBrush(element, "BorderBrush")),
```

(`hasBackground`/`background`/`hasForeground`/`foreground`/`hasBorderBrush`/`borderBrush` stay as locals — the token-loop `switch` on `brushUtility.Target` that populates them is unchanged, only where they get *applied* changes.)

Update the final call (added in Task 1, Step 5) from:

```csharp
        ApplyVariantStyles(element, new OpacityCategoryState(hasOpacity, opacity));
```

to:

```csharp
        ApplyVariantStyles(
            element,
            new BrushCategoryState(hasBackground, background),
            new BrushCategoryState(hasForeground, foreground),
            new BrushCategoryState(hasBorderBrush, borderBrush),
            new OpacityCategoryState(hasOpacity, opacity));
```

- [ ] **Step 6: Run the full test suite and confirm everything passes**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`

Expected: PASS — the 4 tests updated in Step 1 now exercise the new Style-based path for real (and would fail without their `ApplyStyling()` calls, proving the behavior change is real); every other test is unaffected.

- [ ] **Step 7: Commit**

```bash
git add src/Tailwind.Avalonia/Tw/Tw.cs src/Tailwind.Avalonia/Tw/Tw.Variants.cs src/Tailwind.Avalonia/Tw/Tw.Apply.cs src/Tailwind.Avalonia/Tw/Tw.PropertyAccess.cs tests/Tailwind.Avalonia.Tests/TwTests.cs
git commit -m "feat!: resolve bg-/text-/border- utilities via Avalonia styles instead of local values

Background/Foreground/BorderBrush are now applied through per-element
Style objects instead of SetValue, so they only resolve once Avalonia's
style engine has run (e.g. after ApplyStyling(), or a normal layout pass
once attached to a visual tree) instead of immediately. This is required
to let the upcoming hover:/pressed:/focus: variants outrank the base
value -- Avalonia's local-value precedence otherwise always beats style
triggers, regardless of selector. Reading Background/Foreground/BorderBrush
right after SetClass, before styles have been applied, now returns
null/unset instead of the resolved brush."
```

---

### Task 3: Add `hover:`/`pressed:`/`focus:` variants for colors and opacity

**Files:**
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Variants.cs` (add `VariantKind`, prefix table, parsing; extend both category records and both `Add*Styles` methods)
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Apply.cs` (variant-slot arrays, variant branch in the token loop, updated final call)
- Test: `tests/Tailwind.Avalonia.Tests/TwTests.cs`

**Interfaces:**
- Consumes: `TryParseBrushUtility`, `TryParseOpacityUtility` (both already reject any token containing `:`, so passing them an already-variant-stripped remainder — which itself has no more `:` — is safe and requires no changes to either).
- Produces: `Tw.VariantKind` enum (`Hover = 0, Pressed = 1, Focus = 2`), `Tw.TryParseVariantToken(string token, out VariantKind kind, out string remainder)`. Nothing outside `Tw.Variants.cs`/`Tw.Apply.cs` depends on these.

- [ ] **Step 1: Write the failing tests**

Add to `tests/Tailwind.Avalonia.Tests/TwTests.cs`:

```csharp
[Fact]
public void SetClass_Applies_Hover_Variant_For_Background()
{
    Assert.True(TailwindColorPalette.TryGetColor("blue-500", out var blue500));
    Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));

    var border = new Border();

    Tw.SetClass(border, "bg-blue-500 hover:bg-blue-700");

    border.ApplyStyling();
    Assert.Equal(blue500, Assert.IsType<SolidColorBrush>(border.Background).Color);

    ((IPseudoClasses)border.Classes).Add(":pointerover");
    border.ApplyStyling();
    Assert.Equal(blue700, Assert.IsType<SolidColorBrush>(border.Background).Color);

    ((IPseudoClasses)border.Classes).Remove(":pointerover");
    border.ApplyStyling();
    Assert.Equal(blue500, Assert.IsType<SolidColorBrush>(border.Background).Color);
}

[Fact]
public void SetClass_Applies_Pressed_Variant_For_Opacity()
{
    var border = new Border();

    Tw.SetClass(border, "opacity-100 pressed:opacity-50");

    border.ApplyStyling();
    Assert.Equal(1d, border.Opacity);

    ((IPseudoClasses)border.Classes).Add(":pressed");
    border.ApplyStyling();
    Assert.Equal(0.5d, border.Opacity);
}

[Fact]
public void SetClass_Applies_Focus_Variant_For_Foreground()
{
    Assert.True(TailwindColorPalette.TryGetColor("sky-500", out var sky500));

    var textBlock = new TextBlock();

    Tw.SetClass(textBlock, "text-gray-500 focus:text-sky-500");

    ((IPseudoClasses)textBlock.Classes).Add(":focus");
    textBlock.ApplyStyling();

    Assert.Equal(sky500, Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
}

[Fact]
public void SetClass_Prefers_Later_Declared_Variant_When_Multiple_PseudoClasses_Are_Active()
{
    Assert.True(TailwindColorPalette.TryGetColor("green-500", out var green500));

    var border = new Border();

    Tw.SetClass(border, "bg-blue-500 hover:bg-red-500 pressed:bg-green-500");

    ((IPseudoClasses)border.Classes).Add(":pointerover");
    ((IPseudoClasses)border.Classes).Add(":pressed");
    border.ApplyStyling();

    // Pressed is declared after Hover in VariantKind, so its Style is added
    // later and wins while both pseudo-classes are simultaneously active.
    Assert.Equal(green500, Assert.IsType<SolidColorBrush>(border.Background).Color);
}

[Fact]
public void SetClass_Logs_Warning_For_Variant_Token_With_Unsupported_Utility()
{
    var border = new Border();
    var sink = new CapturingLogSink(border);
    var originalSink = Logger.Sink;
    Logger.Sink = sink;

    try
    {
        Tw.SetClass(border, "hover:p-4");

        var entry = Assert.Single(sink.Entries);
        Assert.Equal(LogEventLevel.Warning, entry.Level);
        Assert.Equal("hover:p-4", entry.PropertyValues[0]);
    }
    finally
    {
        Logger.Sink = originalSink;
    }
}
```

- [ ] **Step 2: Run the tests and confirm they fail**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "FullyQualifiedName~SetClass_Applies_Hover_Variant_For_Background|FullyQualifiedName~SetClass_Applies_Pressed_Variant_For_Opacity|FullyQualifiedName~SetClass_Applies_Focus_Variant_For_Foreground|FullyQualifiedName~SetClass_Prefers_Later_Declared_Variant_When_Multiple_PseudoClasses_Are_Active|FullyQualifiedName~SetClass_Logs_Warning_For_Variant_Token_With_Unsupported_Utility"`

Expected: FAIL — `hover:`/`pressed:`/`focus:`-prefixed tokens are unrecognized today, so no variant `Style` is ever added and the pseudo-class toggles have no effect; the last test currently fails because `hover:p-4` isn't yet special-cased (it already logs a warning today via the generic unrecognized-token path with the *same* raw-token text, so this specific assertion may already pass — that's fine, Step 4 must not regress it).

- [ ] **Step 3: Add `VariantKind`, the prefix table, and `TryParseVariantToken` to `Tw.Variants.cs`**

At the top of the `partial class Tw` body in `src/Tailwind.Avalonia/Tw/Tw.Variants.cs` (before `OpacityCategoryState`), add:

```csharp
    private enum VariantKind
    {
        Hover,
        Pressed,
        Focus,
    }

    private const int VariantCount = 3;

    private static readonly (string Prefix, VariantKind Kind)[] VariantPrefixes =
    {
        ("hover:", VariantKind.Hover),
        ("pressed:", VariantKind.Pressed),
        ("focus:", VariantKind.Focus),
    };

    private static string PseudoClassFor(VariantKind kind) => kind switch
    {
        VariantKind.Hover => ":pointerover",
        VariantKind.Pressed => ":pressed",
        VariantKind.Focus => ":focus",
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    private static bool TryParseVariantToken(string token, out VariantKind kind, out string remainder)
    {
        foreach (var (prefix, variantKind) in VariantPrefixes)
        {
            if (token.StartsWith(prefix, StringComparison.Ordinal))
            {
                kind = variantKind;
                remainder = token[prefix.Length..];
                return remainder.Length > 0;
            }
        }

        kind = default;
        remainder = string.Empty;
        return false;
    }
```

- [ ] **Step 4: Extend both category records with a `Variants` array and update both `Add*Styles` methods**

In `src/Tailwind.Avalonia/Tw/Tw.Variants.cs`, change:

```csharp
    private readonly record struct BrushCategoryState(bool HasBase, IBrush? Base);
    private readonly record struct OpacityCategoryState(bool HasBase, double Base);
```

to:

```csharp
    private readonly record struct BrushCategoryState(bool HasBase, IBrush? Base, IBrush?[] Variants);
    private readonly record struct OpacityCategoryState(bool HasBase, double Base, double?[] Variants);
```

Update `AddBrushStyles`'s early-return guard and add the variant loop at the end of the method:

```csharp
    private static void AddBrushStyles(StyledElement element, string propertyName, BrushCategoryState state, List<Style> target)
    {
        if (!state.HasBase && Array.TrueForAll(state.Variants, v => v is null))
        {
            return;
        }

        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' brush property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return;
        }

        var elementType = element.GetType();

        if (state.HasBase)
        {
            var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
            element.Styles.Add(style);
            target.Add(style);
        }

        for (var i = 0; i < state.Variants.Length; i++)
        {
            if (state.Variants[i] is not { } variantBrush)
            {
                continue;
            }

            var pseudoClass = PseudoClassFor((VariantKind)i);
            var variantStyle = new Style(x => x.Is(elementType).Class(pseudoClass)) { Setters = { new Setter(property, variantBrush) } };
            element.Styles.Add(variantStyle);
            target.Add(variantStyle);
        }
    }
```

Apply the same shape change to `AddOpacityStyles`:

```csharp
    private static void AddOpacityStyles(StyledElement element, OpacityCategoryState state, List<Style> target)
    {
        if (!state.HasBase && Array.TrueForAll(state.Variants, v => v is null))
        {
            return;
        }

        var property = FindDoubleProperty(element.GetType(), "Opacity");

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' numeric property on {ElementType}; the utility was ignored.",
                "Opacity",
                element.GetType());
            return;
        }

        var elementType = element.GetType();

        if (state.HasBase)
        {
            var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
            element.Styles.Add(style);
            target.Add(style);
        }

        for (var i = 0; i < state.Variants.Length; i++)
        {
            if (state.Variants[i] is not { } variantOpacity)
            {
                continue;
            }

            var pseudoClass = PseudoClassFor((VariantKind)i);
            var variantStyle = new Style(x => x.Is(elementType).Class(pseudoClass)) { Setters = { new Setter(property, variantOpacity) } };
            element.Styles.Add(variantStyle);
            target.Add(variantStyle);
        }
    }
```

- [ ] **Step 5: Wire variant parsing into `ApplyUtilities`**

In `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`, add 4 array locals alongside the other locals (after the `hasOpacity`/`opacity` locals added in Task 1):

```csharp
        var backgroundVariants = new IBrush?[VariantCount];
        var foregroundVariants = new IBrush?[VariantCount];
        var borderBrushVariants = new IBrush?[VariantCount];
        var opacityVariants = new double?[VariantCount];
```

At the very start of the `foreach (var token in tokens)` loop body (rename the loop variable from `token` to `rawToken` and derive `token` inside, since the variant branch needs the original raw token for the warning message), replace:

```csharp
        foreach (var token in tokens)
        {
            if (TryParseSpacingUtility(token, out var spacingUtility))
```

with:

```csharp
        foreach (var rawToken in tokens)
        {
            if (TryParseVariantToken(rawToken, out var variantKind, out var variantRemainder))
            {
                if (TryParseBrushUtility(variantRemainder, out var variantBrush))
                {
                    switch (variantBrush.Target)
                    {
                        case BrushTarget.Background:
                            backgroundVariants[(int)variantKind] = variantBrush.Brush;
                            break;

                        case BrushTarget.Foreground:
                            foregroundVariants[(int)variantKind] = variantBrush.Brush;
                            break;

                        case BrushTarget.BorderBrush:
                            borderBrushVariants[(int)variantKind] = variantBrush.Brush;
                            break;
                    }

                    continue;
                }

                if (TryParseOpacityUtility(variantRemainder, out var variantOpacity))
                {
                    opacityVariants[(int)variantKind] = variantOpacity;
                    continue;
                }

                Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                    element,
                    "Tw.Class ignored unrecognized utility token '{Token}'.",
                    rawToken);
                continue;
            }

            var token = rawToken;

            if (TryParseSpacingUtility(token, out var spacingUtility))
```

The remaining two `Logger.TryGet(...)"Tw.Class ignored unrecognized utility token..."` call sites further down in the same loop (the non-variant "unrecognized token" branch) already reference `token` — since `token` is now assigned from `rawToken` at the top of the non-variant branch, those are unaffected and need no edit.

Finally, update the closing `ApplyVariantStyles` call (from Task 2) from:

```csharp
        ApplyVariantStyles(
            element,
            new BrushCategoryState(hasBackground, background),
            new BrushCategoryState(hasForeground, foreground),
            new BrushCategoryState(hasBorderBrush, borderBrush),
            new OpacityCategoryState(hasOpacity, opacity));
```

to:

```csharp
        ApplyVariantStyles(
            element,
            new BrushCategoryState(hasBackground, background, backgroundVariants),
            new BrushCategoryState(hasForeground, foreground, foregroundVariants),
            new BrushCategoryState(hasBorderBrush, borderBrush, borderBrushVariants),
            new OpacityCategoryState(hasOpacity, opacity, opacityVariants));
```

- [ ] **Step 6: Run the tests and confirm they pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "FullyQualifiedName~SetClass_Applies_Hover_Variant_For_Background|FullyQualifiedName~SetClass_Applies_Pressed_Variant_For_Opacity|FullyQualifiedName~SetClass_Applies_Focus_Variant_For_Foreground|FullyQualifiedName~SetClass_Prefers_Later_Declared_Variant_When_Multiple_PseudoClasses_Are_Active|FullyQualifiedName~SetClass_Logs_Warning_For_Variant_Token_With_Unsupported_Utility"`

Expected: PASS (all 5).

- [ ] **Step 7: Run the full test suite**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`

Expected: PASS, no regressions.

- [ ] **Step 8: Commit**

```bash
git add src/Tailwind.Avalonia/Tw/Tw.Variants.cs src/Tailwind.Avalonia/Tw/Tw.Apply.cs tests/Tailwind.Avalonia.Tests/TwTests.cs
git commit -m "feat: add hover:/pressed:/focus: variants for color and opacity utilities"
```

---

### Task 4: Version bump and CHANGELOG

**Files:**
- Modify: `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`
- Modify: `CHANGELOG.md`

**Interfaces:**
- Consumes: nothing.
- Produces: nothing later tasks call — this is the last task.

- [ ] **Step 1: Bump the package version**

In `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`, change:

```xml
    <Version>1.0.0</Version>
```

to:

```xml
    <Version>2.0.0</Version>
```

- [ ] **Step 2: Add a CHANGELOG entry**

In `CHANGELOG.md`, insert a new section above the existing `## 1.0.0 — 2026-08-19` entry:

```markdown
## 2.0.0 — 2026-08-20

### Added

- `opacity-*` utility (e.g. `opacity-50`) — sets the element's `Opacity` property from a `0`-`100` percent value, same syntax as the existing color-utility alpha modifier (`bg-black/50`).
- `hover:`, `pressed:`, and `focus:` variants for color utilities (`bg-`, `text-`, `border-`) and `opacity-*`, backed by Avalonia's `:pointerover`/`:pressed`/`:focus` selectors (e.g. `bg-blue-500 hover:bg-blue-700`). Not supported for spacing, sizing, or font-size utilities in this release.
- When multiple variant pseudo-classes are active at once (e.g. hovering while pressed), the variant declared later — order is `hover` < `pressed` < `focus` — wins.

### Breaking

- `bg-*`, `text-*`, `border-*` (and the new `opacity-*`) utilities now resolve through per-element Avalonia `Style` objects instead of `SetValue`, so hover/pressed/focus variants can out-rank the base value (Avalonia always ranks a local value set via `SetValue` above any style trigger, regardless of selector, so this was structurally required). **Consequence:** these properties only reflect their resolved value once Avalonia's style engine has run for that element (e.g. `element.ApplyStyling()`, or a normal layout pass once attached to a visual tree), not immediately after `Tw.Class` changes. Any code that reads `Background`/`Foreground`/`BorderBrush`/`Opacity` right after setting `Tw.Class`, before styles have been applied, will now see the previous (or default) value instead of the new one. Elements attached to a running visual tree are unaffected — layout always runs before paint.
- Spacing, sizing, and font-size utilities are unaffected — they still resolve synchronously via `SetValue`.
```

- [ ] **Step 3: Run the full test suite one more time**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`

Expected: PASS.

- [ ] **Step 4: Commit**

```bash
git add CHANGELOG.md src/Tailwind.Avalonia/Tailwind.Avalonia.csproj
git commit -m "chore: declare v2.0.0 and record the Style-based color/opacity breaking change"
```
