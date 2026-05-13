# [TASK020] - Tailwind Avalonia font size utility and sample

**Status:** Completed  
**Added:** 2026-05-13  
**Updated:** 2026-05-13

## Original Request
add "font-size" function, parser from text-xs -> text-base -> text-9xl, and text-* and text-[<value>]. remomber to add sample to typography. sample learn from tailwind docs(https://tailwindcss.com/docs/font-size)

## Thought Process
- Tailwind font-size utilities reuse the same `text-*` prefix as the existing text-color parser, so the main implementation risk was namespace collision rather than raw value mapping.
- The cleanest local fix was to add a dedicated font-size token scale plus a `FontSize` parser branch ahead of the brush parser, while letting unclaimed `text-*` tokens continue to the existing color path.
- Because Avalonia `FontSize` is an absolute numeric property, predefined tokens and bracket arbitrary absolute values map honestly to generated `StaticResource` keys and parser behavior, but slash line-height modifiers, percentages, responsive variants, and custom-property shorthand should remain explicit non-support for now.
- The sample page should mirror the official Tailwind docs structure the same way the sizing pages do: keep supported sections live and render unsupported official examples as honest note blocks.

## Implementation Plan
- Re-read the official Tailwind font-size docs and confirm the default size scale plus arbitrary-value syntax.
- Add a font-size source-of-truth class and generated `FontSize*` `StaticResource` keys.
- Extend `tw:Tw.Class` with a font-size parser branch that disambiguates `text-*` size tokens from existing text-color tokens.
- Add focused tests for scale lookup, resource generation, parser behavior, arbitrary font-size units, and clear behavior.
- Add a `Typography/FontSize` sample page and wire it into the main sample shell.
- Re-run the test project and rebuild the sample app.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 1.1 | Re-read official Tailwind font-size docs | Complete | 2026-05-13 | Confirmed default tokens `text-xs` through `text-9xl`, arbitrary `text-[value]`, unsupported-for-now slash line-height modifiers, and theme namespace details. |
| 1.2 | Add font-size token scale and resource dictionary | Complete | 2026-05-13 | Added `FontSizeScale`, `FontSizeResourceDictionary`, and merged generated `FontSize*` keys into the package theme. |
| 1.3 | Extend parser with text-size disambiguation | Complete | 2026-05-13 | `tw:Tw.Class` now routes recognized font-size `text-*` tokens to `FontSize` first, then falls back to existing text-color parsing for palette and arbitrary color tokens. |
| 1.4 | Add focused tests | Complete | 2026-05-13 | Added scale/resource tests plus parser and arbitrary-value coverage; `dotnet test` passed with 78 green tests. |
| 1.5 | Add Typography font-size sample page | Complete | 2026-05-13 | Added docs-style sample page with live basic/custom-value demos and explicit unsupported note sections. |
| 1.6 | Validate sample build | Complete | 2026-05-13 | `dotnet build` for the sample project succeeded after wiring the new Typography tab. |

## Progress Log
### 2026-05-13
- Re-read the official Tailwind font-size docs plus the related theme-variable reference to confirm the supported token set and arbitrary value syntax.
- Added `src/Tailwind.Avalonia/Typography/FontSizeScale.cs` and `src/Tailwind.Avalonia/Typography/FontSizeResourceDictionary.cs`, then merged the new dictionary into `Themes/Tailwind.axaml`.
- Updated `src/Tailwind.Avalonia/Tw.cs` so known font-size `text-*` tokens now set Avalonia `FontSize`, with arbitrary absolute bracket values supported and remaining `text-*` tokens still delegated to the existing text-color parser.
- Added `tests/Tailwind.Avalonia.Tests/FontSizeScaleTests.cs`, `tests/Tailwind.Avalonia.Tests/FontSizeResourceDictionaryTests.cs`, and expanded `TwTests` / `TwArbitraryValuesTests` for font-size behavior.
- Added `samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml` and its code-behind, then wired the page into `samples/Tailwind.Avalonia.Sample/MainWindow.axaml` under `TYPOGRAPHY`.
- Validated the feature with `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` and `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj`.