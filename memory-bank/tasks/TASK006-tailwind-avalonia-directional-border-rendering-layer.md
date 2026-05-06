# [TASK006] - Tailwind Avalonia directional border rendering layer

**Status:** Abandoned  
**Added:** 2026-05-07  
**Updated:** 2026-05-07

## Original Request
border-t, border-x, border-s next, build custom attached-property or rendering layer for Border.
Else next clean step is semantic alias layer plus dark/light theme composition.

## Thought Process
Fastest disconfirming check showed subclassing `Border` would not work because Avalonia `Border.Render(...)` is sealed, even though `Border` itself is not sealed. That ruled out a simple subclass override. Honest implementation path was a new control layer: `TwBorder`, built on `Decorator`, reusing core `Border`-style properties and adding side-specific brush properties. `tw:Tw.Class` could then keep generic whole-property behavior for normal controls while mapping directional border-color utilities onto `TwBorder` side brushes only.

## Implementation Plan
- Verify Avalonia `Border` extensibility surface and rendering constraints.
- Add `TwBorder` custom control with side-specific border brush properties and custom rendering.
- Extend `Tw.Class` parser for directional border color tokens targeting `TwBorder`.
- Add focused tests for directional border application, logical remapping, and clearing.
- Update sample to demonstrate `TwBorder` and directional border utilities.
- Validate narrow tests, full tests, and sample build.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 6.1 | Verify Avalonia `Border` rendering extension path | Complete | 2026-05-07 | Confirmed `Border.Render(...)` is sealed and generic `Border` still has only `BorderBrushProperty`. |
| 6.2 | Add `TwBorder` custom rendering control | Complete | 2026-05-07 | `TwBorder` added with side-specific brush properties and custom rendering. |
| 6.3 | Extend `Tw.Class` for directional border color tokens | Complete | 2026-05-07 | Added physical, axis, and logical directional border color parsing. |
| 6.4 | Add focused regression tests | Complete | 2026-05-07 | Tests cover directional property application, logical remap, and clearing. |
| 6.5 | Update sample demo | Complete | 2026-05-07 | Sample now uses `TwBorder` for directional border color examples. |
| 6.6 | Validate build and tests | Complete | 2026-05-07 | Narrow tests, full tests, and sample build all passed. |

## Progress Log
### 2026-05-07
- Verified via reflection and Avalonia source/docs that `Border.Render(...)` is sealed, so a simple `Border` subclass override was not viable.
- Added `TwBorder`, a `Decorator`-based control reusing core border properties and adding `BorderTopBrush`, `BorderRightBrush`, `BorderBottomBrush`, and `BorderLeftBrush`.
- Extended `tw:Tw.Class` to parse `border-t-*`, `border-r-*`, `border-b-*`, `border-l-*`, `border-x-*`, `border-y-*`, `border-s-*`, `border-e-*`, `border-bs-*`, and `border-be-*`.
- Kept generic controls on honest whole-property semantics; directional border colors are now an opt-in `TwBorder` feature.
- Added tests for directional border utilities, `FlowDirection` logical remapping, and cleanup behavior.
- Updated sample XAML and tokens to visually demonstrate directional border color support through `TwBorder`.
- Validated with `dotnet test` and `dotnet build` successfully.

### 2026-05-07
- Reverted this path after user constraint disallowed custom components.
- Current repository state no longer includes `TwBorder`; directional border colors are again deferred.