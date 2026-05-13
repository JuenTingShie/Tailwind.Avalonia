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
- Checked-in Tailwind font-size scale reference in C#.
- Runtime OKLCH-to-sRGB conversion for Avalonia color resources.
- `SpacingResourceDictionary` for generated spacing tokens.
- `ColorResourceDictionary` for generated `Color*` and `Brush*` tokens.
- `FontSizeResourceDictionary` for generated `FontSize*` typography tokens.
- `Tw.Class` attached property for spacing, numeric sizing, font-size, and whole-property color utility-string parsing.
- `Tailwind.Avalonia.Tests` xUnit test project for automated validation.
- Sample app for build and startup verification.

## Color Baseline
- Tailwind v4.2 docs expose the default palette in OKLCH, with `red`, `orange`, `amber`, `yellow`, `lime`, `green`, `emerald`, `teal`, `cyan`, `sky`, `blue`, `indigo`, `violet`, `purple`, `fuchsia`, `pink`, `rose`, `slate`, `gray`, `zinc`, `neutral`, `stone`, `taupe`, `mauve`, `mist`, `olive`, plus `black` and `white`.
- Avalonia 12 `Color.Parse` does not support `oklch(...)`, so package code must convert those values before emitting resources.

## Current Constraint
- Cross-assembly sample consumption currently uses `ResourceInclude` instead of compile-time merged include.
- Static resources currently cover physical directions only; logical spacing is parser-driven.
- Sizing utilities currently cover only spacing-scale numeric tokens and do not emit dedicated `StaticResource` keys.
- Font-size utilities currently cover predefined `text-xs` through `text-9xl` tokens plus absolute bracket arbitrary values, but they do not yet support slash line-height modifiers, responsive variants, percentages, or custom-property shorthand.
- `tw:Tw.Class` now applies whole-property `bg-*`, `text-*`, and `border-*` utilities, including `transparent` and `/opacity`, but it does not yet support directional border colors or variant prefixes.
- Avalonia `Border` exposes `BorderBrushProperty` but not per-side border brush properties, and user constraint disallows custom components.

## Test Coverage Baseline
- `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` currently passes with 78 tests.
- Coverage currently includes spacing scale lookup/suffix rules, spacing parser application/clear behavior, sizing utility application/overwrite/clear behavior, font-size scale/resource lookup, font-size parser application/disambiguation/clear behavior, arbitrary font-size unit handling, whole-property color utility application/clear behavior, transparent and opacity parsing behavior, achromatic OKLCH parsing, palette token count/family presence, and color/brush resource pair emission.