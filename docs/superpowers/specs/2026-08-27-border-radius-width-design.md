# Border Radius & Border Width — Design

## Goal

Implement `rounded-*` (border-radius) and `border-*` (border-width) utilities, extending the Borders category beyond the existing `border-*` color utility. Closes two of the four Borders rows in the README coverage table.

## Background

The library currently implements `border-color` only (`border-red-500` → `BorderBrush`). The Borders category in the README also lists `border-radius`, `border-width`, `border-style`, and `outline-*` as unimplemented. This spec covers the first two; `border-style` and `outline-*` are out of scope (see "Out of scope").

Confirmed against the Tailwind CSS v4.3 reference (the version this repo tracks per the README):

- **Border-radius scale** (`--radius-*` theme keys): `xs`=0.125rem(2px), `sm`=0.25rem(4px), `md`=0.375rem(6px), `lg`=0.5rem(8px), `xl`=0.75rem(12px), `2xl`=1rem(16px), `3xl`=1.5rem(24px), `4xl`=2rem(32px), `none`=0, `full`=`calc(infinity*1px)`. Bare `rounded` (no suffix) resolves through the separate, deprecated-but-still-live `--radius` key, which defaults to `0.25rem` (4px) — same numeric value as `sm`, different theme key.
- Side/corner map: `rounded-t-*`/`r-*`/`b-*`/`l-*` (two adjacent corners each), `rounded-tl-*`/`tr-*`/`br-*`/`bl-*` (single corner each). Tailwind also defines logical variants (`rounded-s-*`, `rounded-ss-*`, etc.) — **excluded from this pass**, see "Out of scope".
- **Border-width** has no theme-driven scale — Tailwind generates it from bare non-negative integers directly (`border-2` → `2px`, arbitrary integer values all valid), plus a `--default-border-width` theme key (defaults to `1px`) used by the bare `border`/`border-t`/etc. forms.
- Border-width side map: `border-t`/`r`/`b`/`l`, `border-x`/`y` (axis), `border-s`/`e` (logical inline start/end), `border-bs`/`be` (logical block start/end), and bare `border` (all sides).

`Avalonia.CornerRadius` constructor/property order is `(TopLeft, TopRight, BottomRight, BottomLeft)` — confirmed against Avalonia source.

## Scope

**In scope:**
- `rounded-*`: bare (`rounded`), named scale (`rounded-lg`, `rounded-full`, `rounded-none`, ...), arbitrary (`rounded-[6px]`), physical sides (`rounded-t-*`/`r-*`/`b-*`/`l-*`), physical corners (`rounded-tl-*`/`tr-*`/`br-*`/`bl-*`).
- `border-*` (width): bare (`border` = 1px all sides), bare per-edge (`border-t` = 1px top, etc.), numeric (`border-2`), arbitrary (`border-[3px]`), physical sides (`border-t-*`/`r-*`/`b-*`/`l-*`), axis (`border-x-*`/`y-*`), logical inline (`border-s-*`/`e-*`), logical block (`border-bs-*`/`be-*`).
- Disambiguating `border-2` (width) from `border-red-500` (color) — both currently share the `border-` prefix, the latter already implemented via `BrushUtilityDescriptors`.
- Unit tests for both utilities (scale table + `Tw.SetClass` integration + arbitrary values + the disambiguation case).
- README checkbox updates and a CHANGELOG entry.

**Out of scope (explicitly deferred, not partially built):**
- `border-style` (`border-dashed`, `border-dotted`, ...) — no native Avalonia rendering primitive for dash patterns on `Border`; would need a custom control/render workaround. Separate design if pursued.
- `outline-*` — separate Borders row, not requested for this pass.
- Logical border-radius variants (`rounded-s-*`, `rounded-ss-*`, `rounded-se-*`, `rounded-ee-*`, `rounded-es-*`) — no precedent for logical *corner* addressing in this codebase yet (only logical *edges*, used by margin/padding). Physical corners cover the common case.
- `hover:`/`pressed:`/`focus:` variants for either utility — matches existing precedent that structural (non-brush, non-opacity) properties don't get variant support (see CHANGELOG 2.0.0).
- Sample docs app pages (`samples/Tailwind.Avalonia.Sample/Docs/...`) — every existing utility category has a matching sample page, but authoring one requires AXAML template work outside this spec's approved scope. Tracked as a follow-up, not built here.

## Architecture

### 1. `CornerRadiusScale` (new file: `src/Tailwind.Avalonia/Borders/CornerRadiusScale.cs`)

Static lookup table, same shape as `FontSizeScale`/`SpacingScale`: `OrderedValues` array of `(string Token, double Pixels)`, backed by a `Dictionary<string, double>`, exposing `TryGetPixels(string, out double)`. No `ToResourceSuffix` — that method exists on the older scales only to support the removed `*ResourceDictionary` generation (dead weight for a new scale; not reintroducing it).

Table: `none`=0, `xs`=2, `sm`=4, `md`=6, `lg`=8, `xl`=12, `2xl`=16, `3xl`=24, `4xl`=32, `full`=9999 (practical stand-in for `calc(infinity*1px)` — CSS's arbitrary-large-radius trick for pill shapes, translated to a concrete pixel ceiling since `CornerRadius` takes finite `double`s).

### 2. Border-radius utility (`Tw.Descriptors.cs`, `Tw.Parsing.cs`, `Tw.PropertyAccess.cs`, `Tw.Apply.cs`, `Tw.cs`)

- New enum `CornerRadiusEdge { All, Top, Right, Bottom, Left, TopLeft, TopRight, BottomRight, BottomLeft }`.
- New descriptor `CornerRadiusUtilityDescriptor(string Prefix, CornerRadiusEdge Edge)` and a `CornerRadiusUtilityDescriptors.All` array (prefixes: `rounded-t-`, `rounded-r-`, `rounded-b-`, `rounded-l-`, `rounded-tl-`, `rounded-tr-`, `rounded-br-`, `rounded-bl-`, `rounded-` last/bare-all).
- New value struct `CornerRadiusUtility(CornerRadiusEdge Edge, double Pixels)`.
- `TryParseCornerRadiusUtility(string token, out CornerRadiusUtility utility)` in `Tw.Parsing.cs`: handles the bare `rounded` exact-match case (4px, all corners) first, then the descriptor loop (scale-table-or-arbitrary lookup via the existing `TryParseScaleOrArbitraryPixels` helper against `CornerRadiusScale.TryGetPixels`).
- `Tw.PropertyAccess.cs`: new `TrySetCornerRadius`/`ClearCornerRadius`/`FindCornerRadiusProperty`, mirroring the existing `Thickness` trio, keyed on `AvaloniaProperty.PropertyType == typeof(CornerRadius)`.
- `Tw.Apply.cs`: new `CornerRadiusMask` bit (in `Tw.cs`), accumulator locals (`hasCornerRadius`, `cornerRadius`), a new `ApplyCornerRadiusEdge` helper (same shape as `ApplyEdge` but building a 4-corner `CornerRadius`, no RTL handling needed since only physical corners are supported), wired into the token loop and the `pendingUtilities` span.

### 3. Border-width utility (`Tw.Descriptors.cs`, `Tw.Parsing.cs`, `Tw.Apply.cs`, `Tw.cs`)

- Extends existing `SpacingTarget` enum with a `BorderWidth` case — reuses `SpacingUtility`, `SpacingEdge`, and `ApplyEdge` as-is (RTL-aware logical-edge handling comes for free).
- New descriptor `BorderWidthUtilityDescriptor(string Prefix, SpacingEdge Edge)` and `BorderWidthUtilityDescriptors.All` array: `border-bs`, `border-be`, `border-x`, `border-y`, `border-s`, `border-e`, `border-t`, `border-r`, `border-b`, `border-l`, `border` (bare-all, last). Note: **no trailing hyphen** in these prefixes (unlike every other descriptor table) — border-width has both a bare form (`border-t` = 1px) and a valued form (`border-t-2`), so the parser checks exact-match first, then `prefix + "-"` for the valued form.
- `TryParseBorderWidthUtility(string token, out SpacingUtility utility)` in `Tw.Parsing.cs`, producing `SpacingTarget.BorderWidth`. Numeric suffix parsing uses a new local non-negative-integer parser (not `SpacingScale` — border-width literally is the integer as pixels, unlike spacing's 4px-multiplier scale), composed with the existing arbitrary-value fallback via `TryParseScaleOrArbitraryPixels`.
- **Disambiguation with `border-*` color**: prefix-overlap between e.g. `border-` (width, bare-all) and `border-t-` (width, specific) is resolved the same way the existing spacing descriptors already resolve `m-` vs `mb-` overlap — matching is prefix + delimiter based, and a failed scale/arbitrary parse on an over-eager match just falls through to the next descriptor in the loop (no early return-false), so ordering doesn't affect correctness. Disambiguation with `BrushUtilityDescriptors`' `border-` (color) entry is resolved by parse order in `Tw.Apply.cs`: try `TryParseBorderWidthUtility` before falling through to `TryParseBrushUtility`. The two never produce false positives against each other because Tailwind color-scale tokens (`red-500`, `sky-300`, `black`, `[#fff]`) never parse as non-negative integers or `[<number><unit>]`, and vice versa.
- `Tw.Apply.cs`: new `BorderWidthMask` bit, accumulator locals (`hasBorderWidth`, `borderWidth`), reuses `TrySetThickness`/`ClearThickness` against the `"BorderThickness"` property name, wired into the token loop (as a second alternative alongside `TryParseSpacingUtility`, both funneling into the same `SpacingTarget` switch) and the `pendingUtilities` span.

### 4. Docs

- `README.md`: check `border-radius` and `border-width` rows in the Borders table.
- `CHANGELOG.md`: new entry under an `## Unreleased` (or next version) heading describing the two new utilities, following the format of the `2.0.0` entry.

## Testing

- `tests/Tailwind.Avalonia.Tests/CornerRadiusScaleTests.cs` — `TryGetPixels` for named tokens + unknown-token failure, mirroring `FontSizeScaleTests`.
- `tests/Tailwind.Avalonia.Tests/TwTests.cs` additions: bare `rounded`, named scale, physical side, physical corner, bare `border`, numeric `border-N`, physical/axis/logical side border-width, and the `border-2` + `border-red-500` disambiguation case (both applied together on one element, asserting `BorderThickness` and `BorderBrush` are both set correctly).
- `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs` additions: `rounded-[6px]`, `border-[3px]`.

## Risks / open questions resolved during design

- **`CornerRadius` field order** — verified against Avalonia source: `(TopLeft, TopRight, BottomRight, BottomLeft)`.
- **`rounded-full` value** — CSS uses `calc(infinity*1px)`, not representable as a finite `double` meaningfully; using `9999` as a large practical ceiling (any element smaller than ~9999px in each dimension renders as a full pill/circle, matching visual intent).
- **Bare `border`/`border-t`/etc. vs `Npx`-suffixed forms sharing a prefix without a scale table** — resolved by trying an exact string match before the `prefix + "-"` delimited match, per descriptor, in a single loop pass.
