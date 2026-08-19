# Changelog

All notable changes to this project are documented in this file.

## 1.0.0 — 2026-08-19

### Breaking

- Removed `ColorResourceDictionary`, `SpacingResourceDictionary`, and `FontSizeResourceDictionary`, and the `Themes/Tailwind.axaml` resource dictionary that merged them. These generated hundreds of named `{StaticResource ...}` keys (`Margin8`, `Width24`, `FontSizeLg`, `BrushSky500`, etc.) as an alternative to `tw:Tw.Class`.
- **Migration:** replace `Margin="{StaticResource Margin8}"` with `tw:Tw.Class="m-8"` (and the equivalent for padding, width, height, font-size, and color resources — see the README/sample docs for the full utility token reference). Remove any `<ResourceInclude Source="avares://Tailwind.Avalonia/Themes/Tailwind.axaml" />` from your app's resources; it no longer exists.
- This is the first version this package has explicitly declared.
