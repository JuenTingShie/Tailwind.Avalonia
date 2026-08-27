# Changelog

All notable changes to this project are documented in this file.

## Unreleased

### Added

- `rounded-*` utility (border-radius) — bare `rounded` (0.25rem/4px, Tailwind's `--radius` default), named scale (`rounded-xs` through `rounded-4xl`, `rounded-none`, `rounded-full`), arbitrary values (`rounded-[6px]`), physical sides (`rounded-t-*`/`r-*`/`b-*`/`l-*`), and physical corners (`rounded-tl-*`/`tr-*`/`br-*`/`bl-*`). No logical corner variants (`rounded-s-*`, `rounded-ss-*`, ...) and no hover/pressed/focus variant support.
- `border-*` utility (border-width) — bare `border`/`border-t`/etc. (1px, Tailwind's `--default-border-width`), non-negative integer values (`border-2`), arbitrary values (`border-[3px]`), physical sides, axis (`border-x-*`/`y-*`), and logical inline/block edges (`border-s-*`/`e-*`/`bs-*`/`be-*`), sharing RTL-aware logical-edge behavior with the existing margin/padding utilities. No hover/pressed/focus variant support.

## 2.0.0 — 2026-08-20

### Added

- `opacity-*` utility (e.g. `opacity-50`) — sets the element's `Opacity` property from a `0`-`100` percent value, same syntax as the existing color-utility alpha modifier (`bg-black/50`).
- `hover:`, `pressed:`, and `focus:` variants for color utilities (`bg-`, `text-`, `border-`) and `opacity-*`, backed by Avalonia's `:pointerover`/`:pressed`/`:focus` selectors (e.g. `bg-blue-500 hover:bg-blue-700`). Not supported for spacing, sizing, or font-size utilities in this release.
- When multiple variant pseudo-classes are active at once (e.g. hovering while pressed), the variant declared later — order is `hover` < `pressed` < `focus` — wins.

### Breaking

- `bg-*`, `text-*`, `border-*` (and the new `opacity-*`) utilities now resolve through per-element Avalonia `Style` objects instead of `SetValue`, so hover/pressed/focus variants can out-rank the base value (Avalonia always ranks a local value set via `SetValue` above any style trigger, regardless of selector, so this was structurally required). **Consequence:** these properties only reflect their resolved value once Avalonia's style engine has run for that element (e.g. `element.ApplyStyling()`, or a normal layout pass once attached to a visual tree), not immediately after `Tw.Class` changes. Any code that reads `Background`/`Foreground`/`BorderBrush`/`Opacity` right after setting `Tw.Class`, before styles have been applied, will now see the previous (or default) value instead of the new one. Elements attached to a running visual tree are unaffected — layout always runs before paint.
- Spacing, sizing, and font-size utilities are unaffected — they still resolve synchronously via `SetValue`.

## 1.0.0 — 2026-08-19

### Breaking

- Removed `ColorResourceDictionary`, `SpacingResourceDictionary`, and `FontSizeResourceDictionary`, and the `Themes/Tailwind.axaml` resource dictionary that merged them. These generated hundreds of named `{StaticResource ...}` keys (`Margin8`, `Width24`, `FontSizeLg`, `BrushSky500`, etc.) as an alternative to `tw:Tw.Class`.
- **Migration:** replace `Margin="{StaticResource Margin8}"` with `tw:Tw.Class="m-8"` (and the equivalent for padding, width, height, font-size, and color resources — see the README/sample docs for the full utility token reference). Remove any `<ResourceInclude Source="avares://Tailwind.Avalonia/Themes/Tailwind.axaml" />` from your app's resources; it no longer exists.
- This is the first version this package has explicitly declared.
