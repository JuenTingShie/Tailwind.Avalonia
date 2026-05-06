# Progress

## Current Status
Spacing foundation is implemented and locally validated. Planning-only state is finished.

## What Works
- Solution and project scaffold exists.
- `Tailwind.Avalonia` library builds on .NET 10 with Avalonia 12.0.2.
- Spacing resources are generated through `SpacingResourceDictionary`.
- Color resources are generated through `ColorResourceDictionary` using the official Tailwind v4.2 palette reference.
- `tw:Tw.Class` applies spacing utilities for padding and margin, including logical parser support.
- `tw:Tw.Class` now also applies whole-property color utilities for `bg-*`, `text-*`, and `border-*`, including `transparent` and `/opacity`, on controls that expose the matching Avalonia brush properties.
- Sample app demonstrates both `StaticResource` and utility-string consumption.
- Sample app now visibly demonstrates `bg-*`, `text-*`, and `border-*` utility strings in XAML.
- `dotnet build` succeeds for the full solution.
- Sample app startup was exercised with no immediate runtime exception output.
- Sample app now consumes package `Brush*` color tokens instead of local hardcoded brush values.
- Automated tests now exist and pass for spacing scale behavior, spacing parser behavior, color parser invariants, palette coverage, and color resource emission.
- Automated tests now also cover color utility application and clearing behavior.
- Automated tests now also cover transparent and opacity parsing behavior.

## What Is Left
- Validate package/publish consumption beyond the local project-reference sample.
- Decide whether compile-time merged include support is needed after packaging.
- Decide whether directional border-color support needs custom rendering, then expand remaining color utility coverage and semantic/dark-light theme layering.

## Known Risks
- Avalonia selector syntax differs from Tailwind token syntax.
- Some utility semantics may not map 1:1 across all control types.
- Parser application currently relies on reflected `PaddingProperty` / `MarginProperty` discovery.
- Color utility application currently relies on reflected `BackgroundProperty` / `ForegroundProperty` / `BorderBrushProperty` discovery.
- Tailwind directional border-color semantics cannot be mapped honestly on generic Avalonia `Border` with current native property surface.
- Logical spacing resources are not yet exposed as dedicated static keys.
- Runtime `ResourceInclude` works locally, but packaged-consumer behavior still needs explicit verification.
- Color tokens currently depend on runtime OKLCH conversion; current tests cover stable invariants but do not yet assert browser-gamut-mapped hex outputs for chromatic palette colors.

## Estimated Stage
90%: spacing and color token foundations plus whole-property color utilities, transparent/opacity support, sample demo refresh, and core automated tests are in place, with directional-border strategy and semantic theme layering still remaining.