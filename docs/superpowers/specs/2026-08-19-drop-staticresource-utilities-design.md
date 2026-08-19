# Drop StaticResource Utilities — Design Spec

**Date:** 2026-08-19
**Status:** Approved

## Problem

`Tailwind.Avalonia` currently ships two parallel ways to apply spacing/sizing/font-size/color styling: the `tw:Tw.Class="p-4 bg-slate-900"` attached-property parser, and three `ResourceDictionary` classes (`SpacingResourceDictionary`, `FontSizeResourceDictionary`, `ColorResourceDictionary`) that generate hundreds of named keys (`Margin8`, `Width24`, `FontSizeLg`, `BrushSky500`, ...) for `{StaticResource ...}` usage. The maintainer judges `tw:Tw.Class` to read better and wants the `StaticResource`-based utilities removed entirely, as a deliberate breaking change.

## Decisions

1. **Full removal, including colors.** All three dictionaries (`ColorResourceDictionary`, `SpacingResourceDictionary`, `FontSizeResourceDictionary`) and `Themes/Tailwind.axaml` (their merge point) are deleted. `TailwindColorPalette`/`TailwindCssColorParser` are untouched — `Tw.Class` calls them directly in C#, never through the resource dictionary.
2. **Setters convert to `tw:Tw.Class`, not literals.** Where `ColorResourceDictionary` brushes are used inside Style `Setter`s (including pseudo-class selectors like `:pointerover`, `:selected`), each `Setter Property="Background/BorderBrush/Foreground"` becomes `Setter Property="tw:Tw.Class"` carrying the **full** token string for that selector's state — because `Tw.Class` replaces properties wholesale from whatever token string is currently in effect, not as a delta merge. `Tw.Class` sets properties via `element.SetValue(property, value)` (LocalValue priority), which outranks Style Setters targeting the same property directly — so a plain `Setter Property="Background"` alongside a `tw:Tw.Class` on the same element would be invisible; retargeting every color-bearing Setter to `tw:Tw.Class` is the only way both coexist correctly. Avalonia's selector-specificity/precedence resolution is property-agnostic, so retargeting a Setter's target property does not change which selector wins when states combine (e.g. `:pointerover` + `:selected`).
3. **One unavoidable exception.** `SpacingDocsStyles.axaml`'s `SamplePaddingStripeBrush` is a `DrawingBrush` whose `GeometryDrawing.Brush="{StaticResource BrushViolet400}"`. `Tw.Class` only patches a fixed whitelist of properties (`Background`, `Foreground`, `BorderBrush`, sizing properties, `FontSize`) on the element it is directly attached to; a `GeometryDrawing.Brush` nested inside a resource is unreachable. This one reference becomes a literal hardcoded color instead of a resource lookup.
4. **Doc pages drop the comparison.** The 6 sample doc pages (Margin, Padding, Width, Height, FontSize, ColorUtilities) each show a "Utility" vs "StaticResource" tab comparison per example. The `StaticResource` `TabItem` (and its locally-scoped resources) is deleted outright; the "Utility" tab's content is promoted to be the page's only content for that example (no `TabControl` wrapper needed when there's one child).
5. **Versioning.** This repo has never declared a version (no `<Version>` in any csproj, no git tags, no CHANGELOG). This removal is treated as the first deliberately-versioned release: `<Version>1.0.0</Version>` is added to `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj`, and a new `CHANGELOG.md` at the repo root records the breaking change with a migration note (`StaticResource` → `tw:Tw.Class`).

## Conversion algorithm

This is the single mechanical rule applied everywhere a `{StaticResource ...}` reference from a removed dictionary is replaced:

- **Color** (`ColorResourceDictionary`): `Brush<ResourceSuffix>` on `Background` → merge Tailwind token `bg-<kebab-name>` into that element's `tw:Tw.Class`; on `Foreground` → `text-<kebab-name>`; on `BorderBrush` → `border-<kebab-name>`. `<kebab-name>` is `<ResourceSuffix>` lowercased with a hyphen inserted between the trailing letters and trailing digits (`Slate950` → `slate-950`, `Sky500` → `sky-500`, `White` → `white`). This is the exact inverse of `TailwindColorPalette.ToResourceSuffix`.
- **Sizing** (`SpacingResourceDictionary`'s width/height resources): `Width<N>` → `w-<N>`; `MinWidth<N>` → `min-w-<N>`; `MaxWidth<N>` → `max-w-<N>`; `Height<N>` → `h-<N>`; `MinHeight<N>` → `min-h-<N>`; `MaxHeight<N>` → `max-h-<N>`.
- **Spacing** (`SpacingResourceDictionary`'s margin/padding resources): `Margin<N>`/`Padding<N>` → `m-<N>`/`p-<N>`; `<Prefix>X<N>` → `mx-<N>`/`px-<N>`; `<Prefix>Y<N>` → `my-<N>`/`py-<N>`; `<Prefix>Top<N>` → `mt-<N>`/`pt-<N>`; `<Prefix>Right<N>` → `mr-<N>`/`pr-<N>`; `<Prefix>Bottom<N>` → `mb-<N>`/`pb-<N>`; `<Prefix>Left<N>` → `ml-<N>`/`pl-<N>`; `NegativeMarginTop<N>` → `-mt-<N>` (and analogous for the other negative-margin edges).
- **FontSize** (`FontSizeResourceDictionary`): `FontSize<Name>` → `text-<kebab-name>` (`FontSizeXs` → `text-xs`, `FontSize2xl` → `text-2xl`).
- If the target element already has a `tw:Tw.Class` attribute, append the new token into its existing space-separated value. Otherwise add `tw:Tw.Class="<token>"` as a new attribute, and delete the old resource-bound attribute (`Background="{StaticResource ...}"` etc.) entirely.
- Inline hex-literal Setter/attribute values (e.g. `Value="#DD1E293B"`) never used `StaticResource` and are out of scope — untouched.
- Locally-scoped resources defined inside a `StaticResource` comparison `TabItem` (e.g. `ArbitraryMargin8`, `ArbitraryWidth100`) are deleted along with that `TabItem` — they exist only to feed the removed comparison and are not referenced elsewhere.

## Out of scope

- `TailwindColorPalette`, `TailwindCssColorParser`, and their existing tests.
- The Avalonia package reference / target framework.
- Any inline hex-literal styling that never went through a `StaticResource`.

## Files touched

- **Delete:** `src/Tailwind.Avalonia/Colors/ColorResourceDictionary.cs`, `src/Tailwind.Avalonia/Spacing/SpacingResourceDictionary.cs`, `src/Tailwind.Avalonia/Typography/FontSizeResourceDictionary.cs`, `src/Tailwind.Avalonia/Themes/Tailwind.axaml`, and their 3 test files.
- **Edit:** `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj` (remove `Themes\**` resource include, add `<Version>1.0.0</Version>`), `samples/Tailwind.Avalonia.Sample/App.axaml`, `samples/Tailwind.Avalonia.Sample/MainWindow.axaml`, `samples/Tailwind.Avalonia.Sample/SampleShell.axaml`, `samples/Tailwind.Avalonia.Sample/Spacing/SpacingDocsStyles.axaml`, `samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml`, `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml`, `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml`, `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml`, `samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml` + `.axaml.cs`, `samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml` + `.axaml.cs`.
- **Create:** `CHANGELOG.md`.

## Verification

No Avalonia.Headless test infrastructure exists in this repo today, so the Setter-retargeting mechanism (point 2 above) is verified by manual pass in the running Desktop sample — hover, press, and select every interactive nav element and doc-page control — rather than a new automated UI test suite. Each file-level change is additionally verified by `dotnet build` succeeding and `grep StaticResource <file>` showing only resources this removal doesn't own (`DocsExampleTabMinWidth`, `SamplePaddingStripeBrush`, icon `StreamGeometry` keys).
