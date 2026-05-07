# [TASK009] - Tailwind Avalonia visual padding aliases

**Status:** Completed  
**Added:** 2026-05-08  
**Updated:** 2026-05-08

## Original Request
另外做一個 psv 來做為真正按照 v(visual) 畫面樣子的 padding start，並補上對應的 visual end alias。

## Thought Process
- The user no longer wanted to overload existing `ps-*` and `pe-*` semantics; they wanted separate parser surfaces for final rendered start/end.
- Under Avalonia's RTL mirror model, a visual-start padding alias should intentionally map to physical left padding, and a visual-end alias should intentionally map to physical right padding.
- The lowest-risk implementation path was to keep `ps-*` and `pe-*` unchanged, add `psv-*` and `pev-*` as padding-only parser aliases, and document the distinction directly in the sample page.

## Implementation Plan
- Add `psv-*` and `pev-*` to the spacing utility descriptor table.
- Add focused regression tests that prove `psv-*` and `pev-*` stay on the physical left/right side regardless of `FlowDirection`.
- Update the padding docs page with dedicated `psv-8` / `pev-8` previews and usage examples.
- Revalidate focused tests and the sample build.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 9.1 | Add `psv-*` / `pev-*` parser support | Complete | 2026-05-08 | Implemented as padding-only aliases mapped to physical left/right padding |
| 9.2 | Add focused regression coverage | Complete | 2026-05-08 | Added tests covering identical padding and arranged bounds in LTR and RTL |
| 9.3 | Update padding sample/docs | Complete | 2026-05-08 | Added dedicated visual alias demos and actual usage examples |
| 9.4 | Validate sample and tests | Complete | 2026-05-08 | Focused `TwTests` passed and sample build succeeded |

## Progress Log
### 2026-05-08
- Re-read the parser, RTL tests, and current sample docs before editing so the change stayed local and did not disturb existing logical spacing behavior.
- Confirmed the minimal implementation path: add a new parser alias instead of redefining `ps-*`.
- Added `psv-*` to `Tw`, keeping it padding-only and mapping it to physical left padding so final RTL rendering lands on visual start.
- Added focused regression coverage in `tests/Tailwind.Avalonia.Tests/TwTests.cs`.
- Updated `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml` with a dedicated `psv-8` demo and usage examples.
- Revalidated the change with focused tests and a sample build.
- Followed up by adding `pev-*` to `Tw` as the visual-end counterpart, mapped to physical right padding so final RTL rendering lands on visual end.
- Added matching focused test coverage and expanded the padding docs plus actual usage examples to cover both visual aliases.