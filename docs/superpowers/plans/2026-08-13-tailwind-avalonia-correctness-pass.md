# Tailwind.Avalonia Correctness Pass Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix three correctness/trust issues in the `Tailwind.Avalonia` library found during a whole-project audit: a palette that claims to be authentic Tailwind v4.2 but isn't, and two silent-failure paths in the `tw:Tw.Class` parser that currently drop unrecognized or unsupported utility tokens with zero diagnostics.

**Architecture:** No new subsystems. Two existing files change: `TailwindColorPalette.cs` (delete 4 fabricated color families) and `Tw.cs` (add `Avalonia.Logging`-based warning logs at the two points where a utility token or a reflected property lookup currently fails silently). Warnings use `Avalonia.Logging.Logger.TryGet`, the same opt-in idiom Avalonia's own framework uses for binding diagnostics — silent unless a consumer has wired up a log sink (e.g. `.LogToTrace()`), so this is non-breaking for every existing consumer.

**Tech Stack:** C# / .NET 10, Avalonia 12.0.2, xUnit 2.9.3. No new package references.

**Spec:** No separate spec document — this is a bounded-scope fix approved in chat during brainstorming (three items: remove fabricated colors, warn on unresolved `Tw.Class` tokens, warn on reflection-missed properties). This plan is the only written artifact.

## Global Constraints

- No new NuGet package references — `Avalonia.Logging` ships inside the already-referenced `Avalonia` package (`Avalonia.Base.dll`).
- Logging must be silent by default (zero behavior/perf change for consumers who haven't attached a log sink) — always call through `Logger.TryGet(...)?.Log(...)`, never call a sink directly.
- `border-2` (Tailwind border-*width*) gets no dedicated parsing logic — it is only made *visible* as an unrecognized token, per the approved design's YAGNI note.
- Every new test follows the existing `TwTests.cs` style: plain xUnit `[Fact]`/`[Theory]`, no headless Avalonia app bootstrap (the project doesn't reference `Avalonia.Headless` and existing tests already construct/measure/arrange controls directly).

---

## File Structure

- **Modify:** `src/Tailwind.Avalonia/Colors/TailwindColorPalette.cs` — delete the 4 non-Tailwind color families (`mauve`, `olive`, `mist`, `taupe`).
- **Create:** `tests/Tailwind.Avalonia.Tests/TailwindColorPaletteTests.cs` — regression coverage proving the fake families are gone and real families still resolve.
- **Modify:** `src/Tailwind.Avalonia/Tw.cs` — add `Avalonia.Logging` warning calls at the two silent-failure points.
- **Modify:** `tests/Tailwind.Avalonia.Tests/TwTests.cs` — add a private `CapturingLogSink` test helper and 4 new `[Fact]`s covering both warning call sites.

---

### Task 1: Remove fabricated color families from the palette

**Files:**
- Modify: `src/Tailwind.Avalonia/Colors/TailwindColorPalette.cs:332-375`
- Create: `tests/Tailwind.Avalonia.Tests/TailwindColorPaletteTests.cs`

**Interfaces:**
- Consumes: `TailwindColorPalette.TryGetColor(string tokenName, out Color color)` — already `internal static`, already reachable from the test project via the existing `InternalsVisibleTo("Tailwind.Avalonia.Tests")` attribute in `Tailwind.Avalonia.csproj:18-23`. No signature changes.
- Produces: nothing new — this task only shrinks the data the existing method searches.

- [ ] **Step 1: Write the failing tests**

Create `tests/Tailwind.Avalonia.Tests/TailwindColorPaletteTests.cs`:

```csharp
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TailwindColorPaletteTests
{
    [Theory]
    [InlineData("mauve-500")]
    [InlineData("olive-500")]
    [InlineData("mist-500")]
    [InlineData("taupe-500")]
    public void TryGetColor_Returns_False_For_Non_Tailwind_Color_Families(string tokenName)
    {
        var resolved = TailwindColorPalette.TryGetColor(tokenName, out _);

        Assert.False(resolved);
    }

    [Fact]
    public void TryGetColor_Still_Resolves_Real_Tailwind_Families()
    {
        Assert.True(TailwindColorPalette.TryGetColor("red-500", out _));
        Assert.True(TailwindColorPalette.TryGetColor("black", out var black));
        Assert.Equal(Colors.Black, black);
    }
}
```

- [ ] **Step 2: Run the tests and confirm the expected failures**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter TailwindColorPaletteTests`

Expected: the 4 `TryGetColor_Returns_False_For_Non_Tailwind_Color_Families` cases FAIL (`Assert.False` receives `true`, because `mauve-500` etc. currently resolve). `TryGetColor_Still_Resolves_Real_Tailwind_Families` PASSES already — that's fine, it's a guard against a regression in the next step, not a new-behavior test.

- [ ] **Step 3: Delete the fabricated color blocks**

In `src/Tailwind.Avalonia/Colors/TailwindColorPalette.cs`, inside the `PaletteCss` triple-quoted string literal, delete every line from `--color-mauve-50: ...` through `--color-taupe-950: ...` inclusive (currently lines 332-375, immediately before `--color-black: #000;`). After the edit, the line right before `--color-black: #000;` must be `--color-stone-950: oklch(14.7% 0.004 49.25);`.

- [ ] **Step 4: Run the tests and confirm they pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter TailwindColorPaletteTests`

Expected: PASS (5 tests: 4 `[Theory]` cases from `TryGetColor_Returns_False_For_Non_Tailwind_Color_Families` + 1 `[Fact]` from `TryGetColor_Still_Resolves_Real_Tailwind_Families`).

- [ ] **Step 5: Run the full test suite**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` (NOT bare `dotnet test` — this repo's `.slnx` includes `samples/Tailwind.Avalonia.Sample.Browser`, which fails with `NETSDK1147` unless the `wasm-tools` workload is installed; that failure is a pre-existing environment gap unrelated to this plan, confirmed by running bare `dotnet test` against a clean worktree before any task started)

Expected: PASS, same total test count as before minus zero (no existing test references `mauve`/`olive`/`mist`/`taupe`).

- [ ] **Step 6: Commit**

```bash
git add src/Tailwind.Avalonia/Colors/TailwindColorPalette.cs tests/Tailwind.Avalonia.Tests/TailwindColorPaletteTests.cs
git commit -m "fix: remove non-Tailwind color families from the palette"
```

---

### Task 2: Warn on unrecognized `Tw.Class` tokens

**Files:**
- Modify: `src/Tailwind.Avalonia/Tw.cs:1-9` (usings), `Tw.cs:11-18` (constants), `Tw.cs:139-142` (unrecognized-token path)
- Test: `tests/Tailwind.Avalonia.Tests/TwTests.cs`

**Interfaces:**
- Consumes: `Avalonia.Logging.Logger.TryGet(LogEventLevel level, string area) : ParametrizedLogger?` and `ParametrizedLogger.Log<T0>(object? source, string messageTemplate, T0 value)` — both confirmed present in `Avalonia.Base.dll` 12.0.2 via reflection; no other package needed.
- Produces: `Tw.LogArea` (`private const string`, value `"Tailwind.Avalonia"`) — Task 3 reuses this same constant, do not introduce a second one.
- This task also introduces the `CapturingLogSink` test helper in `TwTests.cs`, which Task 3's tests reuse — implement it once here.

- [ ] **Step 1: Write the failing tests**

Add to `tests/Tailwind.Avalonia.Tests/TwTests.cs`. First add these usings at the top of the file (alongside the existing `using Avalonia;`, `using Avalonia.Controls;`, `using Avalonia.Media;`):

```csharp
using System;
using System.Collections.Generic;
using Avalonia.Logging;
```

Then add the test helper and two facts anywhere inside the `TwTests` class body (e.g. at the end, before the closing brace):

```csharp
    [Fact]
    public void SetClass_Logs_Warning_For_Unrecognized_Token()
    {
        var sink = new CapturingLogSink();
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            var border = new Border();

            Tw.SetClass(border, "not-a-real-utility");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("not-a-real-utility", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    [Fact]
    public void SetClass_Logs_Warning_For_Unsupported_Border_Width_Token()
    {
        var sink = new CapturingLogSink();
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            var border = new Border();

            Tw.SetClass(border, "border-2");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("border-2", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    private sealed class CapturingLogSink : ILogSink
    {
        public List<(LogEventLevel Level, string Area, string MessageTemplate, object?[] PropertyValues)> Entries { get; } = new();

        public bool IsEnabled(LogEventLevel level, string area) => true;

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
            => Entries.Add((level, area, messageTemplate, Array.Empty<object?>()));

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate, object?[] propertyValues)
            => Entries.Add((level, area, messageTemplate, propertyValues));
    }
```

`border-2` is included here (not deferred to a separate task) because today it fails for the same reason as any other typo: `TryParseBrushUtility` matches the `border-` prefix, then `TryResolveUtilityColor("2", ...)` fails because `"2"` isn't a color token, so the whole call returns `false` and the token is silently dropped — no special-case code needed.

- [ ] **Step 2: Run the tests and confirm they fail**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "SetClass_Logs_Warning_For_Unrecognized_Token|SetClass_Logs_Warning_For_Unsupported_Border_Width_Token"`

Expected: both FAIL with `Assert.Single` throwing because `sink.Entries` is empty (nothing logs yet).

- [ ] **Step 3: Add the logging using and constant to `Tw.cs`**

In `src/Tailwind.Avalonia/Tw.cs`, change the top of the file from:

```csharp
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
```

to:

```csharp
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Data;
using Avalonia.Logging;
using Avalonia.Media;
```

Then change the constants block (currently `Tw.cs:13-17`) from:

```csharp
    private const int MarginMask = 1;
    private const int PaddingMask = 2;
    private const int BackgroundMask = 4;
    private const int ForegroundMask = 8;
    private const int BorderBrushMask = 16;
```

to:

```csharp
    private const int MarginMask = 1;
    private const int PaddingMask = 2;
    private const int BackgroundMask = 4;
    private const int ForegroundMask = 8;
    private const int BorderBrushMask = 16;
    private const string LogArea = "Tailwind.Avalonia";
```

- [ ] **Step 4: Log a warning for unrecognized tokens**

In `src/Tailwind.Avalonia/Tw.cs`, inside `ApplyUtilities`, change (currently `Tw.cs:139-142`):

```csharp
                if (!TryParseBrushUtility(token, out var brushUtility))
                {
                    continue;
                }
```

to:

```csharp
                if (!TryParseBrushUtility(token, out var brushUtility))
                {
                    Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                        element,
                        "Tw.Class ignored unrecognized utility token '{Token}'.",
                        token);
                    continue;
                }
```

- [ ] **Step 5: Run the tests and confirm they pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "SetClass_Logs_Warning_For_Unrecognized_Token|SetClass_Logs_Warning_For_Unsupported_Border_Width_Token"`

Expected: PASS.

- [ ] **Step 6: Run the full test suite**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` (NOT bare `dotnet test` — this repo's `.slnx` includes `samples/Tailwind.Avalonia.Sample.Browser`, which fails with `NETSDK1147` unless the `wasm-tools` workload is installed; that failure is a pre-existing environment gap unrelated to this plan, confirmed by running bare `dotnet test` against a clean worktree before any task started)

Expected: PASS. Watch specifically for any pre-existing test that applies a token the parser doesn't recognize — none currently do, based on a read of `TwTests.cs`, but the full run confirms no `Logger.Sink` state leaked between tests (the `try`/`finally` in the new tests restores `Logger.Sink` to `null` afterward).

- [ ] **Step 7: Commit**

```bash
git add src/Tailwind.Avalonia/Tw.cs tests/Tailwind.Avalonia.Tests/TwTests.cs
git commit -m "feat: warn on unrecognized Tw.Class utility tokens"
```

---

### Task 3: Warn when a resolved utility has no matching property on the element

**Files:**
- Modify: `src/Tailwind.Avalonia/Tw.cs:381-415` (`TrySetThickness`, `TrySetBrush`)
- Test: `tests/Tailwind.Avalonia.Tests/TwTests.cs`

**Interfaces:**
- Consumes: `Tw.LogArea` constant from Task 2. `Avalonia.Controls.Shapes.Rectangle` as the test target — confirmed via reflection to declare none of `Padding`/`Background`/`BorderBrush`/`Foreground` anywhere in its type hierarchy, unlike `Canvas` (has `Background` via `Panel`) or `TextBlock` (has all three of `Background`/`Padding`/`Foreground`).
- Produces: nothing new for later tasks — this is the last task in this plan.

- [ ] **Step 1: Write the failing tests**

Add this using to `tests/Tailwind.Avalonia.Tests/TwTests.cs` (alongside the others):

```csharp
using Avalonia.Controls.Shapes;
```

Add these two facts to the `TwTests` class:

```csharp
    [Fact]
    public void SetClass_Logs_Warning_When_Padding_Property_Is_Missing()
    {
        var sink = new CapturingLogSink();
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            var rectangle = new Rectangle();

            Tw.SetClass(rectangle, "p-4");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("Padding", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    [Fact]
    public void SetClass_Logs_Warning_When_Background_Property_Is_Missing()
    {
        var sink = new CapturingLogSink();
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            var rectangle = new Rectangle();

            Tw.SetClass(rectangle, "bg-red-500");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("Background", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }
```

`Rectangle` is used (not `Canvas`) because `Canvas` inherits `Background` from `Panel`, which would make `SetClass_Logs_Warning_When_Background_Property_Is_Missing` fail to trigger the miss path. `p-4` and `bg-red-500` are both valid, recognized tokens — Task 2's "unrecognized token" warning must NOT fire here; only the property-lookup warning should, which is why each test asserts exactly one (`Assert.Single`) log entry.

- [ ] **Step 2: Run the tests and confirm they fail**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "SetClass_Logs_Warning_When_Padding_Property_Is_Missing|SetClass_Logs_Warning_When_Background_Property_Is_Missing"`

Expected: both FAIL with `Assert.Single` throwing because `sink.Entries` is empty.

- [ ] **Step 3: Log a warning when the reflected Thickness property isn't found**

In `src/Tailwind.Avalonia/Tw.cs`, change `TrySetThickness` (currently `Tw.cs:381-392`) from:

```csharp
    private static bool TrySetThickness(AvaloniaObject element, string propertyName, Thickness value)
    {
        var property = FindThicknessProperty(element.GetType(), propertyName);

        if (property is null)
        {
            return false;
        }

        element.SetValue(property, value);
        return true;
    }
```

to:

```csharp
    private static bool TrySetThickness(AvaloniaObject element, string propertyName, Thickness value)
    {
        var property = FindThicknessProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' Thickness property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }
```

- [ ] **Step 4: Log a warning when the reflected brush property isn't found**

In `src/Tailwind.Avalonia/Tw.cs`, change `TrySetBrush` (currently `Tw.cs:404-415`) from:

```csharp
    private static bool TrySetBrush(AvaloniaObject element, string propertyName, IBrush? value)
    {
        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is null)
        {
            return false;
        }

        element.SetValue(property, value);
        return true;
    }
```

to:

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
```

- [ ] **Step 5: Run the tests and confirm they pass**

Run: `dotnet test tests/Tailwind.Avalonia.Tests --filter "SetClass_Logs_Warning_When_Padding_Property_Is_Missing|SetClass_Logs_Warning_When_Background_Property_Is_Missing"`

Expected: PASS.

- [ ] **Step 6: Run the full test suite**

Run: `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` (NOT bare `dotnet test` — this repo's `.slnx` includes `samples/Tailwind.Avalonia.Sample.Browser`, which fails with `NETSDK1147` unless the `wasm-tools` workload is installed; that failure is a pre-existing environment gap unrelated to this plan, confirmed by running bare `dotnet test` against a clean worktree before any task started)

Expected: PASS, full suite green (all three tasks' tests plus every pre-existing test).

- [ ] **Step 7: Commit**

```bash
git add src/Tailwind.Avalonia/Tw.cs tests/Tailwind.Avalonia.Tests/TwTests.cs
git commit -m "feat: warn when a Tw.Class utility has no matching property on the element"
```
