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
3. `TailwindColorPalette` stores the official Tailwind v4.2 palette reference and `TailwindCssColorParser` converts OKLCH values into Avalonia `Color` instances.
4. `ColorResourceDictionary` emits stable keys such as `ColorBlue500`, `BrushBlue500`, `ColorWhite`, and `BrushWhite`.
5. `tw:Tw.Class="p-4 mx-2 w-24 min-w-16 max-w-32 h-12 min-h-8 max-h-20 bg-blue-700/80 text-white/90 border-green-400/65"` parses spacing, first-pass numeric sizing, and whole-property color tokens and applies them to Avalonia `Padding`, `Margin`, `Width`, `MinWidth`, `MaxWidth`, `Height`, `MinHeight`, `MaxHeight`, `Background`, `Foreground`, and `BorderBrush` properties when those properties exist on the target control.
6. Package entry resource dictionary currently lives at `avares://Tailwind.Avalonia/Themes/Tailwind.axaml`.
7. Sample app currently loads the package entry with `ResourceInclude`; runtime loading works, while compile-time `MergeResourceInclude` did not resolve the project-reference asset during local build.

## Color Rules
- Use the official Tailwind docs palette as the package source of truth instead of copying approximate hex values.
- Convert OKLCH to Avalonia `Color` in code because Avalonia 12 parsing supports RGB/HSL/HSV but not OKLCH.
- Emit both `Color*` and `Brush*` resources so consumers can use tokens in XAML properties without wrapping values manually.

## Resolution Rules
- Later utility token wins when multiple tokens target same property.
- Padding utilities do not support negative values.
- Margin utilities support negative values.
- Logical start/end utilities are flow-direction aware inside the parser.

## Implemented Feature Boundary
Spacing MVP currently covers these utility families:
- Padding: `p`, `px`, `py`, `pt`, `pr`, `pb`, `pl`, `ps`, `psv`, `pe`, `pev`, `pbs`, `pbe`
- Margin: `m`, `mx`, `my`, `mt`, `mr`, `mb`, `ml`, `ms`, `me`, `mbs`, `mbe`
- Colors: `bg-*`, `text-*`, `border-*` for whole-property color application, plus `transparent` and `/opacity`

Static resources currently cover physical padding/margin directions plus negative physical margins. Logical directions are parser-only in this first pass.

Sizing currently covers these parser-only utility families:
- `w`, `min-w`, `max-w`, `h`, `min-h`, `max-h` with spacing-scale numeric suffixes

## Deferred Items
- `auto` margin semantics
- Arbitrary values
- Custom-property syntax
- Fractions, viewport keywords, container sizes, `auto`, `full`, `none`, and `px` for sizing utilities
- Generated sizing `StaticResource` keys
- Responsive variants
- State variants
- Directional border color utilities, blocked for generic controls by missing native per-side brush properties and the no-custom-component constraint
- `current` and `inherit` color utilities
- Other non-spacing utility families