# Active Context

## Current Focus
Build on the new spacing and colors foundation with utilities, tests, and semantic theme composition.

## Recent Changes
- Scaffolded the solution, library project, and sample app.
- Added `SpacingScale` as the spacing source of truth.
- Added `SpacingResourceDictionary` and package theme entry `Themes/Tailwind.axaml`.
- Implemented `tw:Tw.Class` for spacing utilities, including logical start/end handling with RTL awareness.
- Validated the solution with `dotnet build` and a sample app startup run.
- Added a colors source-of-truth based on the official Tailwind v4.2 palette reference.
- Added `ColorResourceDictionary` and an internal OKLCH-to-Avalonia conversion path because Avalonia 12 does not parse `oklch(...)` values directly.
- Updated the sample app to consume package brush tokens instead of local hardcoded brushes.

## Active Decisions
- Hybrid API remains the chosen direction: stable resource keys plus utility parsing.
- Package resource entry is kept, but sample consumption currently uses `ResourceInclude` instead of `MergeResourceInclude` because compile-time flattening did not resolve the project-reference resource during build.
- Official Tailwind docs values remain the source of truth for colors; the package converts those OKLCH values to Avalonia `Color` and `SolidColorBrush` instances at runtime.

## Immediate Next Steps
1. Add automated tests around spacing coverage, colors coverage, and `tw:Tw.Class` parsing behavior.
2. Extend `tw:Tw.Class` with the first color utility families such as `bg-*`, `text-*`, and `border-*`.
3. Start semantic alias and dark/light theme layering on top of the concrete Tailwind palette.

## Open Questions
- Whether logical start/end spacing should gain dedicated `StaticResource` keys or remain parser-only.
- Whether spacing generation should remain checked-in C# or move to a generator pipeline.
- Whether color conversion should stay as runtime code or move to generated, precomputed sRGB values later.
- Whether a C# helper API is worth exposing in addition to the XAML-facing surface.