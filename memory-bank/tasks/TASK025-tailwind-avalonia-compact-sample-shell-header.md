# [TASK025] - Tailwind Avalonia compact sample shell header

**Status:** Completed  
**Added:** 2026-05-14  
**Updated:** 2026-05-14

## Original Request
Page header is too large

Make it compact hamburger button height

## Thought Process
- The request targeted the shared sample shell presentation, so the relevant surface stayed `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` plus its responsive padding rule in `SampleShell.axaml.cs`.
- The current header looked oversized because it stacked section and page labels vertically and also added generous vertical padding around a 46px hamburger button, turning the header into a hero block.
- The smallest high-confidence fix was to keep the same information and behavior but compress the chrome into a single row, trim the typography, and remove the extra vertical padding so the header height tracks the existing toggle button more closely.
- The browser workload is still unavailable in this environment, so the safest validation set for this slice remained the desktop sample build plus the existing automated test project.

## Implementation Plan
- Confirm the header sizing issue in the shared sample shell markup and responsive padding code.
- Reduce the header to a single-row layout while keeping the existing section/page context visible.
- Tighten the header padding so the outer chrome aligns with the hamburger toggle height.
- Revalidate the desktop sample build and the existing tests.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 25.1 | Identify the header sizing drivers | Complete | 2026-05-14 | The oversized look came from stacked text plus large vertical header padding around a 46px toggle button. |
| 25.2 | Compact the header layout | Complete | 2026-05-14 | The header now renders section and page context on a single row with smaller typography and a separator. |
| 25.3 | Tighten responsive header padding | Complete | 2026-05-14 | Wide and narrow layout padding now remove extra vertical space while preserving horizontal breathing room. |
| 25.4 | Revalidate the affected sample and tests | Complete | 2026-05-14 | `dotnet build` passed for the desktop sample host and `dotnet test` stayed green with 78 passing tests. |

## Progress Log
### 2026-05-14
- Re-read the memory-bank context and `SampleShell` implementation so the compacting pass stayed limited to the shared shell header.
- Verified the local build baseline: the full solution still hits the known browser `wasm-tools` workload requirement, while the focused test project already passed.
- Checked current compact-toolbar sizing guidance and used the existing 46px hamburger toggle as the local target for the header chrome.
- Updated `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` so the header now uses one row for section/page context, reduced title typography, and zero vertical chrome padding.
- Updated `samples/Tailwind.Avalonia.Sample/SampleShell.axaml.cs` so both narrow and wide responsive states keep the compact header padding.
- Revalidated the change with `dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj` and `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`.
