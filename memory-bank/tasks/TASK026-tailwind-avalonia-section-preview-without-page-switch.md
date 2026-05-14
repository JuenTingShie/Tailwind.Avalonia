# [TASK026] - Tailwind Avalonia section preview without page switch

**Status:** Completed  
**Added:** 2026-05-15  
**Updated:** 2026-05-15

## Original Request
切換section的時候不要換頁

## Thought Process
- Request targeted shared shell behavior, so owning code path was `samples/Tailwind.Avalonia.Sample/SampleShell.axaml.cs`.
- Local root cause was direct: section selection flowed into logic that both refreshed page-list state and immediately called `ShowPage(...)`.
- Smallest root fix was to split section preview from page navigation, while preserving initial startup behavior and current lazy page cache.
- Desktop and browser builds were enough to validate the shared shell slice at compile level.

## Implementation Plan
- Split section-preview and page-navigation flows in `SampleShell.axaml.cs`.
- Track currently shown page separately from currently browsed section.
- Keep page highlight only when shown page belongs to browsed section; otherwise clear page selection without changing content.
- Preserve first-load behavior by explicitly showing initial page once on attach.
- Revalidate desktop and browser hosts.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 26.1 | Confirm forced page-switch root cause | Complete | 2026-05-15 | `SelectSection(...)` immediately surfaced a page through `ShowPage(...)`. |
| 26.2 | Separate section preview from page navigation | Complete | 2026-05-15 | Added separate preview flow and internal selection synchronization guard. |
| 26.3 | Preserve startup page load | Complete | 2026-05-15 | Shell still shows first page on attach, but later section clicks no longer navigate content. |
| 26.4 | Revalidate shared hosts | Complete | 2026-05-15 | Release builds passed for desktop and browser hosts. |

## Progress Log
### 2026-05-15
- Re-read memory-bank context and current `SampleShell` implementation before editing so the fix stayed on the controlling navigation code path.
- Confirmed the bug locally in code: section selection refreshed the page strip and immediately changed content through `ShowPage(...)`.
- Reworked `samples/Tailwind.Avalonia.Sample/SampleShell.axaml.cs` so section clicks now only preview the matching page list, while page clicks remain the only trigger that swaps hosted content.
- Added synchronization guards so internal `TabStrip` state updates do not recursively trigger unwanted page changes.
- Preserved initial shell startup behavior by previewing the first section and then showing its first page once during attach.
- Validated with `dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj -c Release` and `dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj -c Release`.