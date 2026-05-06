# Progress

## Current Status
Spacing foundation is implemented and locally validated. Planning-only state is finished.

## What Works
- Solution and project scaffold exists.
- `Tailwind.Avalonia` library builds on .NET 10 with Avalonia 12.0.2.
- Spacing resources are generated through `SpacingResourceDictionary`.
- Color resources are generated through `ColorResourceDictionary` using the official Tailwind v4.2 palette reference.
- `tw:Tw.Class` applies spacing utilities for padding and margin, including logical parser support.
- Sample app demonstrates both `StaticResource` and utility-string consumption.
- `dotnet build` succeeds for the full solution.
- Sample app startup was exercised with no immediate runtime exception output.
- Sample app now consumes package `Brush*` color tokens instead of local hardcoded brush values.

## What Is Left
- Add automated tests around spacing resources, color resources, conversion behavior, and parser behavior.
- Validate package/publish consumption beyond the local project-reference sample.
- Decide whether compile-time merged include support is needed after packaging.
- Implement color utility parsing and semantic/dark-light theme layering.

## Known Risks
- Avalonia selector syntax differs from Tailwind token syntax.
- Some utility semantics may not map 1:1 across all control types.
- Parser application currently relies on reflected `PaddingProperty` / `MarginProperty` discovery.
- Logical spacing resources are not yet exposed as dedicated static keys.
- Runtime `ResourceInclude` works locally, but packaged-consumer behavior still needs explicit verification.
- Color tokens currently depend on runtime OKLCH conversion and do not yet have automated regression coverage.

## Estimated Stage
78%: spacing and color token foundations are implemented and locally validated, with utilities/tests/theme layering still remaining.