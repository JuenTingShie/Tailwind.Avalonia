# Active Context

## Current Focus
With the sample docs refresh complete, build on the spacing and colors foundation with semantic theme composition and package validation, under the no-custom-component constraint.

## Recent Changes
- Scaffolded the solution, library project, and sample app.
- Added `SpacingScale` as the spacing source of truth.
- Added `SpacingResourceDictionary` and package theme entry `Themes/Tailwind.axaml`.
- Implemented `tw:Tw.Class` for spacing utilities, including logical start/end handling with RTL awareness.
- Validated the solution with `dotnet build` and a sample app startup run.
- Added a colors source-of-truth based on the official Tailwind v4.2 palette reference.
- Added `ColorResourceDictionary` and an internal OKLCH-to-Avalonia conversion path because Avalonia 12 does not parse `oklch(...)` values directly.
- Updated the sample app to consume package brush tokens instead of local hardcoded brushes.
- Added `Tailwind.Avalonia.Tests` as an xUnit test project and wired it into the solution.
- Added focused unit coverage for `SpacingScale`, `tw:Tw.Class`, `TailwindCssColorParser`, `TailwindColorPalette`, and `ColorResourceDictionary`.
- Extended `tw:Tw.Class` with first-pass color utilities for `bg-*`, `text-*`, and `border-*` by mapping Tailwind palette tokens onto Avalonia brush properties.
- Extended `tw:Tw.Class` color utilities with `transparent` and `/opacity` support for `bg-*`, `text-*`, and `border-*` tokens.
- Added focused regression tests for color utility application and clearing behavior.
- Updated sample XAML to visibly demonstrate `bg-*`, `text-*`, and `border-*` utility usage instead of only package brush resources.
- Removed the `TwBorder` experiment because user constraint now disallows custom components.
- Reverted directional border-color parsing and sample usage back to honest whole-property border color behavior.
- Validated the sample build and the test project with 21 passing tests.
- Rebuilt the sample app into a Tailwind-docs-inspired layout using a standard Avalonia `TabControl` with left-side `Padding` and `Margin` tabs.
- Added sectioned sample content for basic, one-side, axis, logical, and negative-margin spacing examples.
- Kept whole-property `bg-*`, `text-*`, and `border-*` utilities visible in the support cards so the docs sample still advertises current color capability.
- Revalidated the sample project build after the docs-page redesign.
- Split the `Padding` and `Margin` tab bodies out of `MainWindow.axaml` into `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml` and `samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml` as standalone `UserControl` views.
- Moved the docs page styles plus the striped padding brush out of `MainWindow.axaml` into `samples/Tailwind.Avalonia.Sample/Spacing/SpacingDocsStyles.axaml`, and each spacing `UserControl` now merges its own resources/styles so the views can be opened and compared standalone in the designer.
- Removed `samples/Tailwind.Avalonia.Sample/Resources/SampleTokens.axaml`; sample-only copy, dimensions, and tab labels are now inlined directly in `MainWindow.axaml`, `Padding.axaml`, and `Margin.axaml`, while shared visual styles stay in `SpacingDocsStyles.axaml`.
- Investigated the remaining RTL `ps-8` / `pe-8` docs mismatch and confirmed through focused `Tw` tests that logical thickness/layout was already correct in the library layer.
- Updated `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml` so the RTL logical padding preview keeps `FlowDirection="RightToLeft"` on the utility target but cancels Avalonia's visual mirror transform with a local `ScaleTransform`, and revalidated both the sample build and focused `Tw` tests.
- Reworked the logical padding docs section so it now separates honest `FlowDirection`-only RTL rendering from a second, explicitly labeled teaching visualization that uses `ScaleTransform` only to cancel Avalonia mirroring for side-mapping explanation.
- Added `psv-<number>` and `pev-<number>` as padding-only parser aliases for visual start/end, mapping to physical left/right padding so Avalonia's RTL mirror places the spacing on the final rendered side.
- Updated the padding docs page with dedicated `psv-8` / `pev-8` demos and actual-usage examples, then revalidated the focused `TwTests` file and the sample build.

## Active Decisions
- Hybrid API remains the chosen direction: stable resource keys plus utility parsing.
- Package resource entry is kept, but sample consumption currently uses `ResourceInclude` instead of `MergeResourceInclude` because compile-time flattening did not resolve the project-reference resource during build.
- Official Tailwind docs values remain the source of truth for colors; the package converts those OKLCH values to Avalonia `Color` and `SolidColorBrush` instances at runtime.
- Current color utilities are property-wide: `bg-*` targets `Background`, `text-*` targets `Foreground`, and `border-*` targets `BorderBrush` when the target control exposes those Avalonia properties.
- `transparent` and `/opacity` are in scope for whole-property color utilities.
- Custom components are off-limits for this project direction.
- Generic `Border` stays whole-property only because Avalonia `Border` exposes only `BorderBrushProperty` and sealed rendering leaves no honest no-custom-component path for directional border colors.
- `MainWindow.axaml` now owns shared docs resources and styles, while the spacing page bodies live in dedicated child `UserControl` files under `samples/Tailwind.Avalonia.Sample/Spacing/`.
- `MainWindow.axaml` now owns only the tab shell styles; spacing-page presentation styles are loaded locally by the child `UserControl` files.
- The sample app no longer carries a separate sample token dictionary; only package theme resources remain merged, and demo-local text/metrics live next to the sample markup that uses them.
- Docs-style RTL logical spacing previews may need visual mirror cancellation on the preview target when the goal is to show the physical side chosen by the utility rather than Avalonia's mirrored overall control.
- When docs need both truthful behavior and easier pedagogy, the sample should present them as separate demos instead of hiding transform-based teaching aids inside the only example.
- `ps-*` and `pe-*` remain logical spacing utilities; `psv-*` and `pev-*` are now the explicit parser surface for final visual start/end padding under Avalonia's mirror model.

## Immediate Next Steps
1. Start semantic alias and dark/light theme layering on top of the concrete Tailwind palette.
2. Decide whether the sample should gain additional docs tabs for `Background`, `Text`, and `Border Color` utilities.
3. Validate package/publish consumption beyond the local project-reference sample.

## Open Questions
- Whether logical start/end spacing should gain dedicated `StaticResource` keys or remain parser-only.
- Whether spacing generation should remain checked-in C# or move to a generator pipeline.
- Whether color conversion should stay as runtime code or move to generated, precomputed sRGB values later.
- Whether a C# helper API is worth exposing in addition to the XAML-facing surface.
- Whether directional border colors should remain unsupported indefinitely under the no-custom-component constraint or be documented as intentionally out of scope.