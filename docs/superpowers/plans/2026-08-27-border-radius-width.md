# Border Radius & Border Width Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement `rounded-*` (border-radius) and `border-*` (border-width) utilities on `Tw.Class`, closing two rows of the README Borders table.

**Architecture:** Both utilities follow the existing `Tw` partial-class pattern: a scale/descriptor table in `Tw.Descriptors.cs`, a `TryParse*` function in `Tw.Parsing.cs`, accumulation + apply wiring in `Tw.Apply.cs`, and (for border-radius only) new reflection-based property-access helpers in `Tw.PropertyAccess.cs` for the `CornerRadius` type. Border-width reuses the existing `Thickness`/`SpacingEdge`/`ApplyEdge` machinery built for margin/padding by adding a `BorderWidth` case to `SpacingTarget`.

**Tech Stack:** C# / .NET 10, Avalonia, xUnit.

**Spec:** [docs/superpowers/specs/2026-08-27-border-radius-width-design.md](../specs/2026-08-27-border-radius-width-design.md)

## Global Constraints

- No `hover:`/`pressed:`/`focus:` variant support for either utility (matches existing precedent for structural properties).
- No border-style, outline, or logical border-radius corner variants (`rounded-s-*`, `rounded-ss-*`, ...) in this pass — out of scope per spec.
- Border-radius supports only physical sides/corners (`rounded-t-*`, `rounded-tl-*`, ...), no logical (`rounded-s-*`).
- Border-width supports the full edge set already used by spacing: physical (`t`/`r`/`b`/`l`), axis (`x`/`y`), logical inline (`s`/`e`), logical block (`bs`/`be`), and bare all-sides (`border`).
- `CornerRadius` constructor/property order is `(TopLeft, TopRight, BottomRight, BottomLeft)`.
- `rounded-full` renders as `9999` px (finite stand-in for CSS's `calc(infinity*1px)`).
- Bare `rounded` = `4.0` px (the deprecated-but-live `--radius` theme key, same value as `rounded-sm` but a distinct default form). Bare `border`/`border-t`/etc. = `1.0` px (Tailwind's `--default-border-width`).
- No sample-docs-app pages — not part of this plan.

---

### Task 1: `CornerRadiusScale` lookup table

**Files:**
- Create: `src/Tailwind.Avalonia/Borders/CornerRadiusScale.cs`
- Test: `tests/Tailwind.Avalonia.Tests/CornerRadiusScaleTests.cs`

**Interfaces:**
- Produces: `internal static class CornerRadiusScale { public static bool TryGetPixels(string token, out double pixels); }` — consumed by Task 2's `TryParseCornerRadiusUtility`.

- [ ] **Step 1: Write the failing test**

```csharp
namespace Tailwind.Avalonia.Tests;

public class CornerRadiusScaleTests
{
    [Theory]
    [InlineData("none", 0.0)]
    [InlineData("xs", 2.0)]
    [InlineData("sm", 4.0)]
    [InlineData("md", 6.0)]
    [InlineData("lg", 8.0)]
    [InlineData("xl", 12.0)]
    [InlineData("2xl", 16.0)]
    [InlineData("3xl", 24.0)]
    [InlineData("4xl", 32.0)]
    [InlineData("full", 9999.0)]
    public void TryGetPixels_Returns_Expected_Value(string token, double expectedPixels)
    {
        var success = CornerRadiusScale.TryGetPixels(token, out var actualPixels);

        Assert.True(success);
        Assert.Equal(expectedPixels, actualPixels);
    }

    [Fact]
    public void TryGetPixels_Returns_False_For_Unknown_Token()
    {
        var success = CornerRadiusScale.TryGetPixels("999", out _);

        Assert.False(success);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj --filter "FullyQualifiedName~CornerRadiusScaleTests"`
Expected: FAIL to build — `CornerRadiusScale` does not exist.

- [ ] **Step 3: Write minimal implementation**

```csharp
namespace Tailwind.Avalonia;

internal static class CornerRadiusScale
{
    public static readonly (string Token, double Pixels)[] OrderedValues =
    {
        ("none", 0.0),
        ("xs", 2.0),
        ("sm", 4.0),
        ("md", 6.0),
        ("lg", 8.0),
        ("xl", 12.0),
        ("2xl", 16.0),
        ("3xl", 24.0),
        ("4xl", 32.0),
        ("full", 9999.0),
    };

    private static readonly Dictionary<string, double> TokenToPixels = CreateLookup();

    public static bool TryGetPixels(string token, out double pixels) => TokenToPixels.TryGetValue(token, out pixels);

    private static Dictionary<string, double> CreateLookup()
    {
        var lookup = new Dictionary<string, double>(StringComparer.Ordinal);

        foreach (var (token, pixels) in OrderedValues)
        {
            lookup[token] = pixels;
        }

        return lookup;
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj --filter "FullyQualifiedName~CornerRadiusScaleTests"`
Expected: PASS (11 tests)

- [ ] **Step 5: Commit**

```bash
git add src/Tailwind.Avalonia/Borders/CornerRadiusScale.cs tests/Tailwind.Avalonia.Tests/CornerRadiusScaleTests.cs
git commit -m "feat: add CornerRadiusScale lookup table"
```

---

### Task 2: `rounded-*` (border-radius) utility

**Files:**
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Descriptors.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Parsing.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.PropertyAccess.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.cs`
- Test: `tests/Tailwind.Avalonia.Tests/TwTests.cs`
- Test: `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs`

**Interfaces:**
- Consumes: `CornerRadiusScale.TryGetPixels(string, out double)` (Task 1).
- Produces: `Tw.PropertyAccess.cs` gains `TrySetCornerRadius(AvaloniaObject, string, CornerRadius)`, `ClearCornerRadius(AvaloniaObject, string)`, `FindCornerRadiusProperty(Type, string)` — not consumed by later tasks, but establishes the pattern Task 3 mirrors for its own type.

- [ ] **Step 1: Write the failing tests**

Add to `tests/Tailwind.Avalonia.Tests/TwTests.cs`:

```csharp
[Fact]
public void SetClass_Applies_Bare_Border_Radius_Utility()
{
    var border = new Border();

    Tw.SetClass(border, "rounded");

    Assert.Equal(new CornerRadius(4), border.CornerRadius);
}

[Fact]
public void SetClass_Applies_Named_Border_Radius_Utilities()
{
    var border = new Border();

    Tw.SetClass(border, "rounded-lg");

    Assert.Equal(new CornerRadius(8), border.CornerRadius);
}

[Fact]
public void SetClass_Applies_Border_Radius_Full()
{
    var border = new Border();

    Tw.SetClass(border, "rounded-full");

    Assert.Equal(new CornerRadius(9999), border.CornerRadius);
}

[Fact]
public void SetClass_Applies_Physical_Side_Border_Radius_Utilities()
{
    var border = new Border();

    Tw.SetClass(border, "rounded-t-lg rounded-b-sm");

    Assert.Equal(new CornerRadius(8, 8, 4, 4), border.CornerRadius);
}

[Fact]
public void SetClass_Applies_Physical_Corner_Border_Radius_Utilities()
{
    var border = new Border();

    Tw.SetClass(border, "rounded-tl-xl rounded-br-none");

    Assert.Equal(new CornerRadius(12, 0, 0, 0), border.CornerRadius);
}
```

Add to `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs`:

```csharp
[Fact]
public void SetClass_Applies_Arbitrary_Border_Radius()
{
    var border = new Border();

    Tw.SetClass(border, "rounded-[6px]");

    Assert.Equal(new CornerRadius(6), border.CornerRadius);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj --filter "FullyQualifiedName~Border_Radius"`
Expected: Builds fine (the test file only references pre-existing `Tw`, `Border`, and `CornerRadius` types), but FAILS at assertion time — `rounded`/`rounded-lg`/etc. are unrecognized tokens at this point, so `Tw.SetClass` logs a warning and leaves `border.CornerRadius` at its default `CornerRadius(0)`, which does not equal any of the non-zero expected values above.

- [ ] **Step 3: Add `CornerRadiusEdge`, descriptor struct, and descriptor table**

In `src/Tailwind.Avalonia/Tw/Tw.Descriptors.cs`, add (anywhere inside the `partial class Tw` body, alongside the other descriptor structs/enums):

```csharp
private readonly record struct CornerRadiusUtility(CornerRadiusEdge Edge, double Pixels);
private readonly record struct CornerRadiusUtilityDescriptor(string Prefix, CornerRadiusEdge Edge);

private enum CornerRadiusEdge
{
    All,
    Top,
    Right,
    Bottom,
    Left,
    TopLeft,
    TopRight,
    BottomRight,
    BottomLeft,
}

private static class CornerRadiusUtilityDescriptors
{
    public static readonly CornerRadiusUtilityDescriptor[] All =
    {
        new("rounded-tl-", CornerRadiusEdge.TopLeft),
        new("rounded-tr-", CornerRadiusEdge.TopRight),
        new("rounded-br-", CornerRadiusEdge.BottomRight),
        new("rounded-bl-", CornerRadiusEdge.BottomLeft),
        new("rounded-t-", CornerRadiusEdge.Top),
        new("rounded-r-", CornerRadiusEdge.Right),
        new("rounded-b-", CornerRadiusEdge.Bottom),
        new("rounded-l-", CornerRadiusEdge.Left),
        new("rounded-", CornerRadiusEdge.All),
    };
}
```

- [ ] **Step 4: Add `TryParseCornerRadiusUtility`**

In `src/Tailwind.Avalonia/Tw/Tw.Parsing.cs`, add:

```csharp
private static bool TryParseCornerRadiusUtility(string token, out CornerRadiusUtility utility)
{
    utility = default;

    if (token.Contains(':') || token.Contains('('))
    {
        return false;
    }

    if (token.Equals("rounded", StringComparison.Ordinal))
    {
        utility = new CornerRadiusUtility(CornerRadiusEdge.All, 4.0);
        return true;
    }

    foreach (var descriptor in CornerRadiusUtilityDescriptors.All)
    {
        if (!token.StartsWith(descriptor.Prefix, StringComparison.Ordinal))
        {
            continue;
        }

        var scaleToken = token[descriptor.Prefix.Length..];

        if (scaleToken.Length == 0)
        {
            return false;
        }

        // Try a scale-table token first (e.g. rounded-lg), then an arbitrary value (e.g. rounded-[6px]).
        if (TryParseScaleOrArbitraryPixels(scaleToken, CornerRadiusScale.TryGetPixels, static p => p >= 0, out var pixels))
        {
            utility = new CornerRadiusUtility(descriptor.Edge, pixels);
            return true;
        }
    }

    return false;
}
```

- [ ] **Step 5: Add `CornerRadius` property-access support**

In `src/Tailwind.Avalonia/Tw/Tw.PropertyAccess.cs`, add a field alongside the existing caches:

```csharp
private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> CornerRadiusPropertyCache = new();
```

And add these three members alongside the existing `TrySetThickness`/`ClearThickness`/`FindThicknessProperty` trio:

```csharp
private static bool TrySetCornerRadius(AvaloniaObject element, string propertyName, CornerRadius value)
{
    var property = FindCornerRadiusProperty(element.GetType(), propertyName);

    if (property is null)
    {
        Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
            element,
            "Tw.Class could not find a '{PropertyName}' CornerRadius property on {ElementType}; the utility was ignored.",
            propertyName,
            element.GetType());
        return false;
    }

    element.SetValue(property, value);
    return true;
}

private static void ClearCornerRadius(AvaloniaObject element, string propertyName)
{
    var property = FindCornerRadiusProperty(element.GetType(), propertyName);

    if (property is not null)
    {
        element.ClearValue(property);
    }
}

[UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
private static AvaloniaProperty? FindCornerRadiusProperty(Type type, string propertyName)
{
    return CornerRadiusPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
    {
        var property = FindPropertyField(key);
        return property?.PropertyType == typeof(CornerRadius) ? property : null;
    });
}
```

`CornerRadius` resolves via the file's existing `using Avalonia;` — no new `using` needed.

- [ ] **Step 6: Wire into `Tw.cs` and `Tw.Apply.cs`**

In `src/Tailwind.Avalonia/Tw/Tw.cs`, add a new mask constant after `OpacityMask`:

```csharp
private const int CornerRadiusMask = 65536;
```

In `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`:

Add locals alongside the other `has*`/value locals (near `hasFontSize`/`fontSize`):

```csharp
var hasCornerRadius = false;
var cornerRadius = default(CornerRadius);
```

Add a new branch in the token loop, after the `TryParseSizingUtility` block and before the `TryParseFontSizeUtility` block:

```csharp
if (TryParseCornerRadiusUtility(token, out var cornerRadiusUtility))
{
    if (!hasCornerRadius)
    {
        cornerRadius = default;
        hasCornerRadius = true;
    }

    cornerRadius = ApplyCornerRadiusEdge(cornerRadius, cornerRadiusUtility.Edge, cornerRadiusUtility.Pixels);
    continue;
}
```

Add a new helper method near `ApplyEdge`:

```csharp
private static CornerRadius ApplyCornerRadiusEdge(CornerRadius current, CornerRadiusEdge edge, double value) => edge switch
{
    CornerRadiusEdge.All => new CornerRadius(value),
    CornerRadiusEdge.Top => new CornerRadius(value, value, current.BottomRight, current.BottomLeft),
    CornerRadiusEdge.Right => new CornerRadius(current.TopLeft, value, value, current.BottomLeft),
    CornerRadiusEdge.Bottom => new CornerRadius(current.TopLeft, current.TopRight, value, value),
    CornerRadiusEdge.Left => new CornerRadius(value, current.TopRight, current.BottomRight, value),
    CornerRadiusEdge.TopLeft => new CornerRadius(value, current.TopRight, current.BottomRight, current.BottomLeft),
    CornerRadiusEdge.TopRight => new CornerRadius(current.TopLeft, value, current.BottomRight, current.BottomLeft),
    CornerRadiusEdge.BottomRight => new CornerRadius(current.TopLeft, current.TopRight, value, current.BottomLeft),
    CornerRadiusEdge.BottomLeft => new CornerRadius(current.TopLeft, current.TopRight, current.BottomRight, value),
    _ => current,
};
```

Add an entry to the `pendingUtilities` span, after the `OpacityMask` entry:

```csharp
new(CornerRadiusMask, hasCornerRadius, () => TrySetCornerRadius(element, "CornerRadius", cornerRadius), () => ClearCornerRadius(element, "CornerRadius")),
```

- [ ] **Step 7: Run tests to verify they pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj --filter "FullyQualifiedName~Border_Radius"`
Expected: PASS (6 tests)

- [ ] **Step 8: Run the full test suite to check for regressions**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`
Expected: PASS, no regressions in previously-passing tests.

- [ ] **Step 9: Commit**

```bash
git add src/Tailwind.Avalonia/Tw/Tw.Descriptors.cs src/Tailwind.Avalonia/Tw/Tw.Parsing.cs src/Tailwind.Avalonia/Tw/Tw.PropertyAccess.cs src/Tailwind.Avalonia/Tw/Tw.Apply.cs src/Tailwind.Avalonia/Tw/Tw.cs tests/Tailwind.Avalonia.Tests/TwTests.cs tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs
git commit -m "feat: add rounded-* border-radius utility"
```

---

### Task 3: `border-*` (border-width) utility

**Files:**
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Descriptors.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Parsing.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`
- Modify: `src/Tailwind.Avalonia/Tw/Tw.cs`
- Test: `tests/Tailwind.Avalonia.Tests/TwTests.cs`
- Test: `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs`

**Interfaces:**
- Consumes: `SpacingUtility`, `SpacingEdge`, `SpacingTarget`, `ApplyEdge`, `TrySetThickness`, `ClearThickness` (all pre-existing, from the margin/padding implementation).
- Produces: nothing consumed by later tasks — Task 4 only touches docs.

- [ ] **Step 1: Write the failing tests**

Add to `tests/Tailwind.Avalonia.Tests/TwTests.cs`:

```csharp
[Fact]
public void SetClass_Applies_Bare_Border_Width_Utility()
{
    var border = new Border();

    Tw.SetClass(border, "border");

    Assert.Equal(new Thickness(1), border.BorderThickness);
}

[Fact]
public void SetClass_Applies_Numeric_Border_Width_Utility()
{
    var border = new Border();

    Tw.SetClass(border, "border-2");

    Assert.Equal(new Thickness(2), border.BorderThickness);
}

[Fact]
public void SetClass_Applies_Physical_Side_Border_Width_Utilities()
{
    var border = new Border();

    Tw.SetClass(border, "border-t-4 border-b-2 border-x-1");

    Assert.Equal(new Thickness(1, 4, 1, 2), border.BorderThickness);
}

[Fact]
public void SetClass_Applies_Bare_Side_Border_Width_Utility()
{
    var border = new Border();

    Tw.SetClass(border, "border-t");

    Assert.Equal(new Thickness(0, 1, 0, 0), border.BorderThickness);
}

[Fact]
public void SetClass_Applies_Logical_Border_Width_For_RightToLeft()
{
    var border = new Border
    {
        FlowDirection = FlowDirection.RightToLeft,
    };

    Tw.SetClass(border, "border-s-6 border-e-2");

    Assert.Equal(new Thickness(2, 0, 6, 0), border.BorderThickness);
}

[Fact]
public void SetClass_Disambiguates_Border_Width_From_Border_Color()
{
    Assert.True(TailwindColorPalette.TryGetColor("red-500", out var red500));
    var border = new Border();

    Tw.SetClass(border, "border-2 border-red-500");

    border.ApplyStyling();

    Assert.Equal(new Thickness(2), border.BorderThickness);
    Assert.Equal(red500, Assert.IsType<SolidColorBrush>(border.BorderBrush).Color);
}
```

Add to `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs`:

```csharp
[Fact]
public void SetClass_Applies_Arbitrary_Border_Width()
{
    var border = new Border();

    Tw.SetClass(border, "border-[3px]");

    Assert.Equal(new Thickness(3), border.BorderThickness);
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj --filter "FullyQualifiedName~Border_Width"`
Expected: FAIL — `border.BorderThickness` stays `default` (all tokens ignored/logged as unrecognized, or in the disambiguation test, `border-2` is silently dropped while `border-red-500` still resolves as color).

- [ ] **Step 3: Add `BorderWidth` to `SpacingTarget` and the descriptor table**

In `src/Tailwind.Avalonia/Tw/Tw.Descriptors.cs`, change:

```csharp
private enum SpacingTarget
{
    Margin,
    Padding,
}
```

to:

```csharp
private enum SpacingTarget
{
    Margin,
    Padding,
    BorderWidth,
}
```

Add a new descriptor struct and table (note: these prefixes have **no trailing hyphen** — border-width has both a bare form (`border-t`) and a valued form (`border-t-2`), unlike every other descriptor table in this file):

```csharp
private readonly record struct BorderWidthUtilityDescriptor(string Prefix, SpacingEdge Edge);

private static class BorderWidthUtilityDescriptors
{
    public static readonly BorderWidthUtilityDescriptor[] All =
    {
        new("border-bs", SpacingEdge.BlockStart),
        new("border-be", SpacingEdge.BlockEnd),
        new("border-x", SpacingEdge.X),
        new("border-y", SpacingEdge.Y),
        new("border-s", SpacingEdge.Start),
        new("border-e", SpacingEdge.End),
        new("border-t", SpacingEdge.Top),
        new("border-r", SpacingEdge.Right),
        new("border-b", SpacingEdge.Bottom),
        new("border-l", SpacingEdge.Left),
        new("border", SpacingEdge.All),
    };
}
```

- [ ] **Step 4: Add `TryParseBorderWidthUtility`**

In `src/Tailwind.Avalonia/Tw/Tw.Parsing.cs`, add:

```csharp
private static bool TryParseBorderWidthUtility(string token, out SpacingUtility utility)
{
    utility = default;

    if (token.Contains(':') || token.Contains('('))
    {
        return false;
    }

    foreach (var descriptor in BorderWidthUtilityDescriptors.All)
    {
        if (token.Equals(descriptor.Prefix, StringComparison.Ordinal))
        {
            utility = new SpacingUtility(SpacingTarget.BorderWidth, descriptor.Edge, 1.0);
            return true;
        }

        var valuedPrefix = descriptor.Prefix + "-";

        if (!token.StartsWith(valuedPrefix, StringComparison.Ordinal))
        {
            continue;
        }

        var scaleToken = token[valuedPrefix.Length..];

        if (scaleToken.Length == 0)
        {
            continue;
        }

        // Try a bare non-negative integer first (e.g. border-2 = 2px, unlike the spacing scale's
        // 4px-per-step multiplier), then an arbitrary value (e.g. border-[3px]).
        if (TryParseScaleOrArbitraryPixels(scaleToken, TryParseBareBorderWidthPixels, static p => p >= 0, out var pixels))
        {
            utility = new SpacingUtility(SpacingTarget.BorderWidth, descriptor.Edge, pixels);
            return true;
        }
    }

    return false;
}

private static bool TryParseBareBorderWidthPixels(string token, out double pixels)
{
    pixels = default;

    if (token.Length == 0)
    {
        return false;
    }

    foreach (var ch in token)
    {
        if (!char.IsAsciiDigit(ch))
        {
            return false;
        }
    }

    pixels = double.Parse(token, NumberStyles.None, CultureInfo.InvariantCulture);
    return true;
}
```

- [ ] **Step 5: Wire into `Tw.cs` and `Tw.Apply.cs`**

In `src/Tailwind.Avalonia/Tw/Tw.cs`, add a mask constant after `CornerRadiusMask`:

```csharp
private const int BorderWidthMask = 131072;
```

In `src/Tailwind.Avalonia/Tw/Tw.Apply.cs`:

Add locals alongside `hasMargin`/`margin`:

```csharp
var hasBorderWidth = false;
var borderWidth = default(Thickness);
```

Change the existing spacing branch from:

```csharp
if (TryParseSpacingUtility(token, out var spacingUtility))
{
    switch (spacingUtility.Target)
    {
        case SpacingTarget.Margin:
            if (!hasMargin)
            {
                margin = default;
                hasMargin = true;
            }

            margin = ApplyEdge(margin, spacingUtility.Edge, spacingUtility.Pixels, element);
            break;

        case SpacingTarget.Padding:
            if (!hasPadding)
            {
                padding = default;
                hasPadding = true;
            }

            padding = ApplyEdge(padding, spacingUtility.Edge, spacingUtility.Pixels, element);
            break;
    }

    continue;
}
```

to:

```csharp
if (TryParseSpacingUtility(token, out var spacingUtility) || TryParseBorderWidthUtility(token, out spacingUtility))
{
    switch (spacingUtility.Target)
    {
        case SpacingTarget.Margin:
            if (!hasMargin)
            {
                margin = default;
                hasMargin = true;
            }

            margin = ApplyEdge(margin, spacingUtility.Edge, spacingUtility.Pixels, element);
            break;

        case SpacingTarget.Padding:
            if (!hasPadding)
            {
                padding = default;
                hasPadding = true;
            }

            padding = ApplyEdge(padding, spacingUtility.Edge, spacingUtility.Pixels, element);
            break;

        case SpacingTarget.BorderWidth:
            if (!hasBorderWidth)
            {
                borderWidth = default;
                hasBorderWidth = true;
            }

            borderWidth = ApplyEdge(borderWidth, spacingUtility.Edge, spacingUtility.Pixels, element);
            break;
    }

    continue;
}
```

Add an entry to the `pendingUtilities` span, after the `PaddingMask` entry:

```csharp
new(BorderWidthMask, hasBorderWidth, () => TrySetThickness(element, "BorderThickness", borderWidth), () => ClearThickness(element, "BorderThickness")),
```

- [ ] **Step 6: Run tests to verify they pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj --filter "FullyQualifiedName~Border_Width"`
Expected: PASS (7 tests)

- [ ] **Step 7: Run the full test suite to check for regressions**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`
Expected: PASS, no regressions.

- [ ] **Step 8: Commit**

```bash
git add src/Tailwind.Avalonia/Tw/Tw.Descriptors.cs src/Tailwind.Avalonia/Tw/Tw.Parsing.cs src/Tailwind.Avalonia/Tw/Tw.Apply.cs src/Tailwind.Avalonia/Tw/Tw.cs tests/Tailwind.Avalonia.Tests/TwTests.cs tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs
git commit -m "feat: add border-* border-width utility"
```

---

### Task 4: Docs — README and CHANGELOG

**Files:**
- Modify: `README.md`
- Modify: `CHANGELOG.md`

**Interfaces:**
- Consumes: nothing (docs-only).
- Produces: nothing (terminal task).

- [ ] **Step 1: Update the README Borders table**

In `README.md`, change:

```markdown
### Borders

| Utility        | Implemented |
| -------------- | :---------: |
| border-radius  |             |
| border-width   |             |
| border-color   |     ✅      |
| border-style   |             |
| outline-width  |             |
| outline-color  |             |
| outline-style  |             |
| outline-offset |             |
```

to:

```markdown
### Borders

| Utility        | Implemented |
| -------------- | :---------: |
| border-radius  |     ✅      |
| border-width   |     ✅      |
| border-color   |     ✅      |
| border-style   |             |
| outline-width  |             |
| outline-color  |             |
| outline-style  |             |
| outline-offset |             |
```

- [ ] **Step 2: Add a CHANGELOG entry**

At the top of `CHANGELOG.md`, above the `## 2.0.0` heading, add:

```markdown
## Unreleased

### Added

- `rounded-*` utility (border-radius) — bare `rounded` (0.25rem/4px, Tailwind's `--radius` default), named scale (`rounded-xs` through `rounded-4xl`, `rounded-none`, `rounded-full`), arbitrary values (`rounded-[6px]`), physical sides (`rounded-t-*`/`r-*`/`b-*`/`l-*`), and physical corners (`rounded-tl-*`/`tr-*`/`br-*`/`bl-*`). No logical corner variants (`rounded-s-*`, `rounded-ss-*`, ...) and no hover/pressed/focus variant support.
- `border-*` utility (border-width) — bare `border`/`border-t`/etc. (1px, Tailwind's `--default-border-width`), non-negative integer values (`border-2`), arbitrary values (`border-[3px]`), physical sides, axis (`border-x-*`/`y-*`), and logical inline/block edges (`border-s-*`/`e-*`/`bs-*`/`be-*`), sharing RTL-aware logical-edge behavior with the existing margin/padding utilities. No hover/pressed/focus variant support.

## 2.0.0 — 2026-08-20
```

- [ ] **Step 3: Commit**

```bash
git add README.md CHANGELOG.md
git commit -m "docs: mark border-radius and border-width implemented"
```
