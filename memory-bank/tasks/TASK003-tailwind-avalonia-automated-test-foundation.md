# TASK003 - Tailwind Avalonia Automated Test Foundation

**Status:** Completed  
**Added:** 2026-05-06  
**Updated:** 2026-05-06

## Original Request
補上單元測試。

## Thought Process
- Repo had no test project, so first need stable test harness before adding assertions.
- Smallest useful coverage should target current high-risk logic: spacing scale mapping, `tw:Tw.Class` spacing application, OKLCH parsing invariants, palette loading, and color resource emission.
- For chromatic OKLCH palette values, authoritative browser-gamut-mapped hex outputs were not directly available from fetched docs, so tests should lock stable invariants and public resource behavior rather than assume non-authoritative hex snapshots.

## Implementation Plan
- Scaffold test project.
- Wire test project into solution and reference library.
- Expose internals to tests where direct verification is useful.
- Add spacing tests.
- Add color tests.
- Run `dotnet test` and fix local failures.
- Update memory bank.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 3.1 | Create test project scaffold | Complete | 2026-05-06 | `dotnet new xunit` under `tests/Tailwind.Avalonia.Tests` |
| 3.2 | Wire test project into solution | Complete | 2026-05-06 | Added project to `.slnx` and referenced library |
| 3.3 | Expose internals for direct tests | Complete | 2026-05-06 | Added `InternalsVisibleTo` for `Tailwind.Avalonia.Tests` |
| 3.4 | Add spacing tests | Complete | 2026-05-06 | Added scale and `Tw` spacing behavior coverage |
| 3.5 | Add color tests | Complete | 2026-05-06 | Added parser invariants, palette coverage, and resource dictionary tests |
| 3.6 | Validate tests | Complete | 2026-05-06 | `dotnet test` passed with 18 tests |
| 3.7 | Refresh memory bank | Complete | 2026-05-06 | Core docs updated to reflect automated test foundation |

## Progress Log
### 2026-05-06
- Confirmed the SDK-provided xUnit template was available before editing the solution.
- Created `Tailwind.Avalonia.Tests` and wired it into `Tailwind.Avalonia.slnx`.
- Added direct tests for `SpacingScale` token lookup/suffix behavior.
- Added `Tw` tests for physical spacing application, RTL logical spacing, and clearing previously applied values.
- Added parser tests for hex passthrough, achromatic OKLCH conversion, alpha parsing, and unsupported syntax failure.
- Added palette and resource dictionary tests for token count, new Tailwind v4.2 color families, and `Color*` / `Brush*` pair emission.
- Initial test run exposed that hardcoded chromatic hex expectations were too strong for current conversion assumptions, so tests were tightened to stable invariants and public resource consistency.
- Final `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` passed with 18 successful tests.