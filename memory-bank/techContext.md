# Tech Context

## Target Stack
- Avalonia 12+
- Tailwind CSS 4.2 reference model
- .NET 10

## Relevant Framework Facts
- Avalonia styles can target controls by type, class, pseudoclass, and property matches.
- Avalonia `ResourceDictionary` supports merge/include patterns suitable for package distribution.
- `StaticResource` is fast and stable for token consumption.
- `MergeResourceInclude` is compile-time flattened, while `ResourceInclude` defers external dictionary loading to runtime.
- Tailwind spacing utilities are driven by a single `--spacing` theme variable.

## Spacing Baseline
Tailwind default spacing baseline maps to `0.25rem`. In this project, initial planning assumes a 4 DIP base step so Tailwind numeric spacing can map cleanly into Avalonia `Thickness` values.

## Planned Initial Scale
`0`, `0.5`, `1`, `1.5`, `2`, `2.5`, `3`, `3.5`, `4`, `5`, `6`, `7`, `8`, `9`, `10`, `11`, `12`, `14`, `16`, `20`, `24`, `28`, `32`, `36`, `40`, `44`, `48`, `52`, `56`, `60`, `64`, `72`, `80`, `96`

## Delivery Mechanics
- Checked-in spacing source-of-truth in C#.
- Checked-in Tailwind v4.2 color palette reference in C#.
- Runtime OKLCH-to-sRGB conversion for Avalonia color resources.
- `SpacingResourceDictionary` for generated spacing tokens.
- `ColorResourceDictionary` for generated `Color*` and `Brush*` tokens.
- `Tw.Class` attached property for utility-string parsing.
- Sample app for build and startup verification.

## Color Baseline
- Tailwind v4.2 docs expose the default palette in OKLCH, with `red`, `orange`, `amber`, `yellow`, `lime`, `green`, `emerald`, `teal`, `cyan`, `sky`, `blue`, `indigo`, `violet`, `purple`, `fuchsia`, `pink`, `rose`, `slate`, `gray`, `zinc`, `neutral`, `stone`, `taupe`, `mauve`, `mist`, `olive`, plus `black` and `white`.
- Avalonia 12 `Color.Parse` does not support `oklch(...)`, so package code must convert those values before emitting resources.

## Current Constraint
- No automated tests exist yet for spacing parsing, color conversion, or resource coverage.
- Cross-assembly sample consumption currently uses `ResourceInclude` instead of compile-time merged include.
- Static resources currently cover physical directions only; logical spacing is parser-driven.
- Color tokens exist as resources only; `tw:Tw.Class` does not yet apply color utilities.