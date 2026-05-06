# [TASK007] - Tailwind Avalonia no-custom-component rollback

**Status:** Completed  
**Added:** 2026-05-07  
**Updated:** 2026-05-07

## Original Request
不可以自製元件

## Thought Process
Local hypothesis: without custom components, directional border colors have no honest implementation path because generic Avalonia `Border` exposes only `BorderBrushProperty`, and no supported attached-property rendering mechanism was confirmed for patching side-specific border drawing onto existing controls. Cheap disconfirming check was nearby and external: inspect current `TwBorder` usage locally, then search Avalonia docs for adorner or overlay rendering options on existing controls. No concrete supported path appeared, so the smallest correct change was rollback.

## Implementation Plan
- Remove `TwBorder` and all repository references to it.
- Revert `tw:Tw.Class` directional border parsing to whole-border semantics only.
- Remove directional border tests.
- Update sample back to whole-border color utilities.
- Validate focused tests, full tests, and sample build.
- Sync memory bank to record the no-custom-component constraint.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 7.1 | Remove custom component path | Complete | 2026-05-07 | Deleted `TwBorder` and all active references. |
| 7.2 | Revert directional border parser | Complete | 2026-05-07 | `tw:Tw.Class` back to whole-border semantics only. |
| 7.3 | Align tests and sample | Complete | 2026-05-07 | Removed directional tests; sample now demonstrates whole-border only. |
| 7.4 | Validate repository state | Complete | 2026-05-07 | Focused tests, full tests, and sample build all passed. |
| 7.5 | Sync memory and task history | Complete | 2026-05-07 | TASK006 marked abandoned; current constraint documented. |

## Progress Log
### 2026-05-07
- Verified that current user constraint forbids the previously added custom-component path.
- Removed `TwBorder` from source, tests, sample, and current architecture description.
- Restored `tw:Tw.Class` to whole-border color behavior while keeping `transparent` and `/opacity` support intact.
- Revalidated with focused `TwTests`, full `dotnet test`, and sample `dotnet build`.
- Updated task history so repository state and memory bank match the current accepted direction.