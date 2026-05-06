# System Patterns

## Chosen Direction
Use a hybrid architecture instead of StaticResource-only or Avalonia-style-class-only.

## Key Decision
Primary public API should be an attached property parser plus generated static resources.

## Why Hybrid
- Avalonia style classes are not a safe primary Tailwind surface because Tailwind tokens like `p-0.5` and variants like `hover:bg-red-500` conflict with Avalonia selector syntax.
- Static resources are stable and Avalonia-native, but by themselves they do not preserve Tailwind composition well.
- Hybrid gives stable token storage plus Tailwind-like authoring.

## Current Layers
1. `SpacingScale` is the current source of truth for the spacing scale.
2. `SpacingResourceDictionary` emits stable keys such as `Padding4`, `Padding0_5`, `MarginX2`, and `NegativeMarginX2`.
3. `tw:Tw.Class="p-4 mx-2"` parses spacing tokens and applies them to Avalonia `Padding` and `Margin` properties.
4. Package entry resource dictionary currently lives at `avares://Tailwind.Avalonia/Themes/Tailwind.axaml`.
5. Sample app currently loads the package entry with `ResourceInclude`; runtime loading works, while compile-time `MergeResourceInclude` did not resolve the project-reference asset during local build.

## Resolution Rules
- Later utility token wins when multiple tokens target same property.
- Padding utilities do not support negative values.
- Margin utilities support negative values.
- Logical start/end utilities are flow-direction aware inside the parser.

## Implemented Feature Boundary
Spacing MVP currently covers these utility families:
- Padding: `p`, `px`, `py`, `pt`, `pr`, `pb`, `pl`, `ps`, `pe`, `pbs`, `pbe`
- Margin: `m`, `mx`, `my`, `mt`, `mr`, `mb`, `ml`, `ms`, `me`, `mbs`, `mbe`

Static resources currently cover physical padding/margin directions plus negative physical margins. Logical directions are parser-only in this first pass.

## Deferred Items
- `auto` margin semantics
- Arbitrary values
- Custom-property syntax
- Responsive variants
- State variants
- Non-spacing utility families