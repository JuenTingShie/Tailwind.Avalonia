# Active Context

## Current Focus
Stabilize the implemented spacing MVP and prepare the next feature family.

## Recent Changes
- Scaffolded the solution, library project, and sample app.
- Added `SpacingScale` as the spacing source of truth.
- Added `SpacingResourceDictionary` and package theme entry `Themes/Tailwind.axaml`.
- Implemented `tw:Tw.Class` for spacing utilities, including logical start/end handling with RTL awareness.
- Validated the solution with `dotnet build` and a sample app startup run.

## Active Decisions
- Hybrid API remains the chosen direction: stable resource keys plus utility parsing.
- Package resource entry is kept, but sample consumption currently uses `ResourceInclude` instead of `MergeResourceInclude` because compile-time flattening did not resolve the project-reference resource during build.
- Colors remain the next major target after spacing.

## Immediate Next Steps
1. Add automated tests around spacing scale coverage and `tw:Tw.Class` parsing behavior.
2. Decide whether packaged consumers should stay on `ResourceInclude` or whether compile-time merged include support should be revisited after pack/publish validation.
3. Start the Colors utility family.

## Open Questions
- Whether logical start/end spacing should gain dedicated `StaticResource` keys or remain parser-only.
- Whether spacing generation should remain checked-in C# or move to a generator pipeline.
- Whether a C# helper API is worth exposing in addition to the XAML-facing surface.