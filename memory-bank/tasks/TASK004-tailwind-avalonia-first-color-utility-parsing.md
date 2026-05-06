# [TASK004] - Tailwind Avalonia first color utility parsing

**Status:** Completed  
**Added:** 2026-05-06  
**Updated:** 2026-05-06

## Original Request
把 color utilities 接到 Tw.Class，先做 bg-、text-、border-*。

## Thought Process
The narrowest safe implementation path was to extend the existing `Tw.Class` parser rather than adding a second color-only surface. The controlling code path was already in `Tw.cs`, so the change stayed local: parse known color utility prefixes, resolve them against the existing Tailwind palette source of truth, and set reflected Avalonia brush properties when available. Focused tests on `Border` and `TextBlock` provided the cheapest disconfirming check for both parser semantics and property-application behavior.

## Implementation Plan
- Extend `Tw.Class` parsing to recognize `bg-*`, `text-*`, and `border-*` tokens.
- Reuse the current Tailwind palette as the token lookup source.
- Add focused tests for color utility application and clearing behavior.
- Run the relevant test slice, then the full test project.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 4.1 | Extend `Tw.Class` with first-pass color token parsing | Complete | 2026-05-06 | Added `bg-*`, `text-*`, and `border-*` support in `Tw.cs`. |
| 4.2 | Reuse palette tokens for parser color resolution | Complete | 2026-05-06 | Added `TailwindColorPalette.TryGetColor(...)` for parser-side lookup. |
| 4.3 | Add focused regression tests | Complete | 2026-05-06 | `TwTests` now covers color application and clearing. |
| 4.4 | Validate full test project | Complete | 2026-05-06 | `dotnet test` passed with 20 tests. |

## Progress Log
### 2026-05-06
- Extended `Tw.Class` so color utilities share the existing attached-property parsing path with spacing utilities.
- Added brush-property reflection for `Background`, `Foreground`, and `BorderBrush` to keep the implementation generic across controls.
- Reused the existing Tailwind palette token source instead of introducing a second color registry.
- Added focused tests for `bg-*`, `text-*`, and `border-*` application and property clearing.
- Validated the narrow `TwTests` slice first, then reran the full test project successfully with 20 passing tests.