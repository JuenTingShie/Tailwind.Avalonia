# [TASK023] - Tailwind Avalonia sample shell responsive navigation

**Status:** Completed  
**Added:** 2026-05-14  
**Updated:** 2026-05-14

## Original Request
make sample tab vertial and toggle show and hide, made it mobile viewing friendly

## Thought Process
- The request pointed at the shared sample shell because both the desktop and browser/mobile surfaces host `SampleShell` directly.
- The falsifiable local hypothesis was that the fixed three-column shell in `SampleShell.axaml` was the real cause of the poor narrow-screen experience: two always-visible navigation columns permanently reduced content width.
- The smallest high-confidence fix was to keep the existing lazy `TabStrip` navigation model, but move both navigation levels into one collapsible `SplitView` pane and let code-behind switch between `Inline` and `Overlay` modes based on the available width.
- Avalonia's current `SplitView` docs explicitly support collapsible navigation-sidebar patterns, which made it the most direct framework-native control for this slice.

## Implementation Plan
- Replace the fixed three-column shell layout with a single responsive `SplitView`.
- Keep section and page navigation vertical, but stack them inside one navigation pane.
- Add code-behind toggle handlers plus narrow-width layout switching.
- Auto-collapse the pane after page selection in narrow mode so content regains width immediately.
- Revalidate both desktop and browser hosts because the shell is shared.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 23.1 | Confirm the owning narrow-screen layout surface | Complete | 2026-05-14 | `SampleShell.axaml` owned the fixed two-column navigation layout and was the narrowest reliable edit point. |
| 23.2 | Rework the shell into a collapsible vertical navigation pane | Complete | 2026-05-14 | Replaced the fixed grid shell with a `SplitView` and stacked both `TabStrip`s into one vertical sidebar. |
| 23.3 | Add responsive and toggle behavior | Complete | 2026-05-14 | `SampleShell.axaml.cs` now flips between `Inline` and `Overlay`, updates shell padding, and closes the pane after page selection on narrow widths. |
| 23.4 | Revalidate shared sample hosts | Complete | 2026-05-14 | Release builds passed for both the desktop host and the browser host after the shell refactor. |

## Progress Log
### 2026-05-14
- Re-read the memory-bank context and the shared `SampleShell` implementation before editing so the change stayed on the owning navigation surface.
- Confirmed the fixed `ColumnDefinitions="Auto,Auto,*"` shell was the real narrow-screen bottleneck because the section and page strips always consumed width even when the reader only needed content.
- Rechecked current Avalonia documentation for `SplitView` and used its documented navigation-sidebar pattern as the framework-native basis for a collapsible mobile-friendly shell.
- Reworked `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` so the sample shell now uses one vertical `SplitView` pane that stacks `SectionTabStrip` and `PageTabStrip`, keeps the navigation vertical, and adds explicit menu hide/show buttons.
- Updated `samples/Tailwind.Avalonia.Sample/SampleShell.axaml.cs` with responsive width switching, shared pane-state helpers, and narrow-layout auto-collapse after page selection while preserving the existing lazy page-cache behavior.
- Validated the slice with `dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj -c Release` and `dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj -c Release`.