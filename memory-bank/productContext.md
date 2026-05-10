# Product Context

## Why This Exists
Avalonia has strong styling and resource systems, but it does not offer a Tailwind-like utility workflow out of the box. This project aims to give Avalonia developers a constrained, predictable, low-friction styling vocabulary.

## Problem To Solve
- Reduce repeated hand-authored `Thickness`, brushes, and style boilerplate.
- Bring Tailwind-style utility thinking into XAML and Avalonia resources.
- Keep design tokens centralized instead of scattering magic numbers through views.

## User Experience Goals
- Consumer should add one package resource include and immediately access utility-style spacing.
- API should feel close to Tailwind semantics.
- API should still respect Avalonia patterns such as `ResourceDictionary`, `StaticResource`, styles, and attached properties.

## MVP Experience
- Consumer can use static keys like `Padding4` and `MarginX2`.
- Consumer can optionally use a Tailwind-like utility string API for composition.
- First supported utility areas are spacing, whole-property colors, and first-pass numeric sizing.

## Current MVP Delivery
- Sample app consumes package resources through `ResourceInclude` and uses `tw:Tw.Class` for composed spacing.
- Sample app now also acts as a docs-style browser with left-side tabs for supported spacing categories.
- Library currently exposes positive padding/margin resources plus negative margin resources.
- Logical spacing (`ps`, `pe`, `ms`, `me`, `pbs`, `pbe`, `mbs`, `mbe`) is supported through the parser surface.
- Library now also exposes Tailwind v4.2 palette tokens as `Color*` and `Brush*` resources.
- Whole-property color utilities now work on generic controls.
- Numeric sizing utilities now work through `tw:Tw.Class` for width/min-width/max-width/height/min-height/max-height.

## Next UX Priority After MVP
Under the no-custom-component constraint, directional border color remains deferred, so next UX priority is semantic aliases and dark/light theme composition on top of the concrete Tailwind palette.