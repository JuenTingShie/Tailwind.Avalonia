# Progress

## Current Status
Spacing and color foundations are implemented, locally validated, and now documented through a docs-style sample surface.

## What Works
- Solution and project scaffold exists.
- `Tailwind.Avalonia` library builds on .NET 10 with Avalonia 12.0.2.
- Spacing resources are generated through `SpacingResourceDictionary`.
- Color resources are generated through `ColorResourceDictionary` using the official Tailwind v4.2 palette reference.
- `tw:Tw.Class` applies spacing utilities for padding and margin, including logical parser support.
- `tw:Tw.Class` now also applies whole-property color utilities for `bg-*`, `text-*`, and `border-*`, including `transparent` and `/opacity`, on controls that expose the matching Avalonia brush properties.
- Sample app now presents a docs-style left-tab browser for `Padding` and `Margin`, with detailed example sections mapped to the currently supported spacing subset.
- The `Padding` and `Margin` tab content is now split into dedicated `UserControl` files under `samples/Tailwind.Avalonia.Sample/Spacing/`, keeping `MainWindow.axaml` focused on shell resources and tab wiring.
- The spacing `UserControl` files now merge sample/package resources and include a shared `SpacingDocsStyles.axaml` file locally, so they render correctly even when developed outside the `MainWindow` shell.
- The sample no longer uses `SampleTokens.axaml`; demo-only copy, layout values, and tab labels are inlined where they are used, while shared presentation rules stay in `SpacingDocsStyles.axaml`.
- Sample app still visibly demonstrates `bg-*`, `text-*`, and `border-*` utility strings in XAML through the support cards embedded in the docs layout.
- `dotnet build` succeeds for the full solution.
- Sample app startup was exercised with no immediate runtime exception output.
- Sample app now consumes package `Brush*` color tokens instead of local hardcoded brush values.
- Automated tests now exist and pass for spacing scale behavior, spacing parser behavior, color parser invariants, palette coverage, and color resource emission.
- Automated tests now also cover color utility application and clearing behavior.
- Automated tests now also cover transparent and opacity parsing behavior.

## What Is Left
- Validate package/publish consumption beyond the local project-reference sample.
- Decide whether compile-time merged include support is needed after packaging.
- Expand semantic/dark-light theme layering and remaining non-border utility coverage.
- Decide whether the docs-style sample should expand beyond spacing into dedicated color utility tabs.
- Decide whether directional border colors should be explicitly documented as unsupported under the no-custom-component constraint.

## Known Risks
- Avalonia selector syntax differs from Tailwind token syntax.
- Some utility semantics may not map 1:1 across all control types.
- Parser application currently relies on reflected `PaddingProperty` / `MarginProperty` discovery.
- Color utility application currently relies on reflected `BackgroundProperty` / `ForegroundProperty` / `BorderBrushProperty` discovery.
- Tailwind directional border-color semantics still cannot be mapped onto generic Avalonia `Border` under the no-custom-component constraint.
- Logical spacing resources are not yet exposed as dedicated static keys.
- Runtime `ResourceInclude` works locally, but packaged-consumer behavior still needs explicit verification.
- Color tokens currently depend on runtime OKLCH conversion; current tests cover stable invariants but do not yet assert browser-gamut-mapped hex outputs for chromatic palette colors.

## Estimated Stage
92%: spacing and color token foundations plus whole-property color utilities, docs-style sample coverage for spacing, and core automated tests are in place, with semantic theme layering, packaging validation, and longer-term docs/theme scope decisions still remaining.