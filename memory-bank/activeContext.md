# Active Context

## Current Focus
Build on the new spacing and colors foundation with broader utility coverage, package validation, and semantic theme composition.

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
- Confirmed Avalonia `Border` exposes only `BorderBrushProperty`, so directional border-color utilities such as `border-t-*`, `border-x-*`, and `border-s-*` were deferred instead of mapped to misleading whole-border behavior.
- Validated the new test project with `dotnet test` and 21 passing tests.

## Active Decisions
- Hybrid API remains the chosen direction: stable resource keys plus utility parsing.
- Package resource entry is kept, but sample consumption currently uses `ResourceInclude` instead of `MergeResourceInclude` because compile-time flattening did not resolve the project-reference resource during build.
- Official Tailwind docs values remain the source of truth for colors; the package converts those OKLCH values to Avalonia `Color` and `SolidColorBrush` instances at runtime.
- Current color utilities are property-wide: `bg-*` targets `Background`, `text-*` targets `Foreground`, and `border-*` targets `BorderBrush` when the target control exposes those Avalonia properties.
- `transparent` and `/opacity` are in scope for whole-property color utilities.
- Directional border-color utilities are deferred until there is either a custom rendering strategy or a control-specific property surface that can represent side-specific border colors honestly.

## Immediate Next Steps
1. Decide whether directional border-color support deserves a custom attached-property/rendering layer or should stay unsupported for generic controls.
2. Add more regression coverage around palette conversion edge cases if the color conversion strategy changes.
3. Start semantic alias and dark/light theme layering on top of the concrete Tailwind palette.

## Open Questions
- Whether logical start/end spacing should gain dedicated `StaticResource` keys or remain parser-only.
- Whether spacing generation should remain checked-in C# or move to a generator pipeline.
- Whether color conversion should stay as runtime code or move to generated, precomputed sRGB values later.
- Whether a C# helper API is worth exposing in addition to the XAML-facing surface.
- Whether directional border colors should be supported only for specific controls with custom rendering instead of through the current generic reflection path.