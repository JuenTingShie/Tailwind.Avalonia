# [TASK005] - Tailwind Avalonia color opacity and sample refresh

**Status:** Completed  
**Added:** 2026-05-07  
**Updated:** 2026-05-07

## Original Request
繼續補 border-t、border-x、border-s 這類 directional border color，順便決定要不要一起收 bg-transparent 和 opacity。
把 sample 改成直接示範 bg-/text-/border-*，讓新能力在 UI 上可見。

## Thought Process
Controlling code path remained in `Tw.cs`, so parser extension stayed local. Fastest discriminating check for directional border color feasibility was verifying Avalonia `Border` property surface. Official Avalonia docs and runtime reflection both showed only `BorderBrushProperty`, with no per-side border brush properties. That meant mapping `border-t-*`, `border-x-*`, or `border-s-*` onto the current generic parser would create false semantics. Instead, the work focused on honest additions that fit current architecture: `transparent` and `/opacity` for whole-property color utilities, plus a sample refresh that visibly uses `bg-*`, `text-*`, and `border-*` in XAML.

## Implementation Plan
- Add `transparent` lookup and `/opacity` parsing for whole-property color utilities.
- Add focused regression coverage for transparent and opacity behavior.
- Update sample XAML and sample tokens so `tw:Tw.Class` visibly drives background, border, and text color.
- Validate narrow tests, full tests, and sample build.
- Record why directional border color was deferred.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 5.1 | Add `transparent` color token support | Complete | 2026-05-07 | `TailwindColorPalette.TryGetColor(...)` now resolves `transparent`. |
| 5.2 | Add `/opacity` parser support for whole-property color utilities | Complete | 2026-05-07 | `Tw.cs` now parses and applies alpha-modified colors. |
| 5.3 | Add focused tests for transparent and opacity behavior | Complete | 2026-05-07 | `TwTests` gained transparent/opacity coverage. |
| 5.4 | Refresh sample to visibly use `bg-*`, `text-*`, and `border-*` | Complete | 2026-05-07 | Updated sample strings and XAML demo cards. |
| 5.5 | Decide directional border-color scope | Complete | 2026-05-07 | Deferred due missing native per-side brush properties on Avalonia `Border`. |

## Progress Log
### 2026-05-07
- Verified Tailwind semantics for `bg-transparent`, `text-transparent`, and opacity modifiers from official docs.
- Verified Avalonia `Border` property surface from official docs and runtime reflection; only `BorderBrushProperty` exists.
- Added `transparent` and `/opacity` support to whole-property `bg-*`, `text-*`, and `border-*` parsing.
- Added focused tests for transparency and opacity behavior, then validated `TwTests` and full test project successfully.
- Updated sample resources and `MainWindow.axaml` so utility-driven background, border, and text colors are visible in the demo UI.
- Deferred directional border-color utilities instead of faking support on unsupported host properties.