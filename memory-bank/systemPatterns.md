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
5. `FontSizeScale` stores the supported Tailwind font-size tokens from `xs` through `9xl` as the typography source of truth.
6. `FontSizeResourceDictionary` emits stable keys such as `FontSizeBase`, `FontSize2xl`, and `FontSize9xl`.
7. `tw:Tw.Class="p-4 mx-2 w-24 min-w-16 max-w-32 h-12 min-h-8 max-h-20 text-base bg-blue-700/80 text-white/90 border-green-400/65"` parses spacing, numeric sizing, font-size, and whole-property color tokens and applies them to Avalonia `Padding`, `Margin`, `Width`, `MinWidth`, `MaxWidth`, `Height`, `MinHeight`, `MaxHeight`, `FontSize`, `Background`, `Foreground`, and `BorderBrush` properties when those properties exist on the target control.
8. Arbitrary values (`*-[<value>]`) are supported for spacing, sizing, colors, and font size, with CSS unit conversion for absolute numeric values and hex color parsing (6 and 8 digit).
9. Package entry resource dictionary currently lives at `avares://Tailwind.Avalonia/Themes/Tailwind.axaml`.
10. Sample app currently loads the package entry with `ResourceInclude`; runtime loading works, while compile-time `MergeResourceInclude` did not resolve the project-reference asset during local build.

## Color Rules
- Use the official Tailwind docs palette as the package source of truth instead of copying approximate hex values.
- Convert OKLCH to Avalonia `Color` in code because Avalonia 12 parsing supports RGB/HSL/HSV but not OKLCH.
- Emit both `Color*` and `Brush*` resources so consumers can use tokens in XAML properties without wrapping values manually.

## Resolution Rules
- Later utility token wins when multiple tokens target same property.
- Padding utilities do not support negative values.
- Margin utilities support negative values.
- Logical start/end utilities are flow-direction aware inside the parser.
- The shared `text-*` namespace resolves known font-size tokens first; unclaimed `text-*` tokens continue to the existing text-color parser.

## Implemented Feature Boundary
Spacing MVP currently covers these utility families:
- Padding: `p`, `px`, `py`, `pt`, `pr`, `pb`, `pl`, `ps`, `psv`, `pe`, `pev`, `pbs`, `pbe` (plus arbitrary `p-[value]`, `px-[value]`, etc.)
- Margin: `m`, `mx`, `my`, `mt`, `mr`, `mb`, `ml`, `ms`, `me`, `mbs`, `mbe` (plus arbitrary with negative support)
- Typography: `text-xs`, `text-sm`, `text-base`, `text-lg`, `text-xl`, `text-2xl`, `text-3xl`, `text-4xl`, `text-5xl`, `text-6xl`, `text-7xl`, `text-8xl`, `text-9xl` (plus arbitrary `text-[value]` for absolute numeric font sizes)
- Colors: `bg-*`, `text-*`, `border-*` for whole-property color application, plus `transparent` and `/opacity` (plus arbitrary `bg-[#hex]`, `text-[#hex]`, etc.)

Static resources currently cover physical padding/margin directions plus negative physical margins, numeric sizing tokens, and font-size tokens. Logical directions are parser-only in this first pass.

Sizing currently covers these parser-only utility families:
- `w`, `min-w`, `max-w`, `h`, `min-h`, `max-h` with spacing-scale numeric suffixes (plus arbitrary `w-[value]`, `h-[value]`, etc.)

## Arbitrary Value Support
- Spacing and sizing support CSS units: `px`, `rem`, `em`, `%`, unitless
- Font size supports absolute CSS units: `px`, `rem`, `em`, unitless
- `rem` and `em` convert using 16px base
- Colors support 6-digit (`#rrggbb`) and 8-digit (`#rrggbbaa`) hex with opacity modifier support
- All arbitrary values are lowercase matched after scale token lookup fails (fallback approach)

## Deferred Items
- `text-sm/6`-style line-height modifiers
- `text-(length:<custom-property>)` shorthand
- `auto` margin semantics
- Custom-property syntax
- Fractions, viewport keywords, container sizes, `auto`, `full`, `none`, and `px` for sizing utilities (as predefined tokens, arbitrary values cover some use cases)
- Responsive variants
- State variants
- Directional border color utilities, blocked for generic controls by missing native per-side brush properties and the no-custom-component constraint
- `current` and `inherit` color utilities
- Other non-spacing utility families