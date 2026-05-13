# [TASK022] - Tailwind Avalonia browser tab switch performance

**Status:** Completed  
**Added:** 2026-05-14  
**Updated:** 2026-05-14

## Original Request
fix switching tab is slow on browser

## Thought Process
- The request targeted the shared sample shell because the browser host reuses `SampleShell` directly; changing individual docs pages first would have been a wider and less falsifiable path.
- The controlling evidence was local: `SampleShell.axaml` embedded six heavy docs pages directly inside nested `TabControl` content, while the page constructors were effectively just `InitializeComponent()` plus small reference-table setup.
- Avalonia's current docs explicitly note that `TabControl` creates tab content eagerly on first load, and recommend `TabStrip` when custom lazy loading or view caching is needed.
- The smallest high-confidence fix was therefore to replace the eager shell navigation with a two-level `TabStrip` that lazy-creates each page the first time it is selected and keeps the page alive afterward, so revisiting a tab only changes visibility.

## Implementation Plan
- Replace the nested shell `TabControl` structure with top-level and second-level `TabStrip` selectors.
- Add local page/section descriptors plus lazy page factory logic in `SampleShell.axaml.cs`.
- Cache created page controls in the shell so repeat visits do not rebuild the page.
- Validate the browser and desktop sample hosts still build.
- Sync the memory bank with the verified pattern.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 22.1 | Confirm the local tab-switch bottleneck | Complete | 2026-05-14 | `SampleShell.axaml` was eagerly embedding all heavy docs pages inside nested `TabControl` content. |
| 22.2 | Verify an Avalonia-supported lazy-loading pattern | Complete | 2026-05-14 | Rechecked the current Avalonia docs for `TabControl` and `TabStrip`; `TabStrip` is the recommended route for custom lazy loading and caching. |
| 22.3 | Rework the shell navigation | Complete | 2026-05-14 | Replaced nested eager `TabControl` content with a two-level `TabStrip` and a shared page host grid. |
| 22.4 | Add lazy page caching | Complete | 2026-05-14 | `SampleShell.axaml.cs` now creates a page only on first navigation and keeps it attached for future visibility toggles. |
| 22.5 | Revalidate sample hosts | Complete | 2026-05-14 | Release builds passed for both browser and desktop hosts; a later designer-only follow-up moved `TabStrip` event wiring to code-behind and swapped the simple item templates to reflection binding for previewer compatibility. |

## Progress Log
### 2026-05-14
- Re-read the memory-bank context and the current `SampleShell`/page code-behind before editing so the fix stayed on the owning UI surface.
- Confirmed the heavy docs pages were not doing meaningful work in constructors beyond `InitializeComponent()`, which made eager XAML construction the strongest local hypothesis.
- Rechecked the current Avalonia docs and confirmed that `TabControl` eagerly creates tab content, while `TabStrip` is intended for custom lazy loading and caching scenarios.
- Reworked `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` to use two vertical `TabStrip`s for section/page navigation and a shared `Grid` content host.
- Added lazy page descriptor and cache logic in `samples/Tailwind.Avalonia.Sample/SampleShell.axaml.cs` so pages are created on first selection and then only toggled visible on later visits.
- Fixed a follow-up XAML compiler issue by exposing explicit descriptor types to the item templates.
- Validated the slice with `dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj -c Release` and `dotnet build samples/Tailwind.Avalonia.Sample.Desktop/Tailwind.Avalonia.Sample.Desktop.csproj -c Release`.
- Followed up on a browser-only blank-screen regression and captured the real startup exception through Playwright `pageerror`: Avalonia was raising `SelectionChanged` during `SampleShell` XAML `EndInit`, before the named `TabStrip` fields were safe to dereference.
- Hardened `SampleShell` so both tab-selection handlers use the event sender plus initialization guards instead of assuming `SectionTabStrip`, `PageTabStrip`, and `PageHost` are already assigned during the first routed event.
- Replaced the browser splash readiness observer with frame polling against the Avalonia canvas/native host so the splash drops promptly once the browser surface is attached.
- Revalidated the final browser path with `dotnet build samples/Tailwind.Avalonia.Sample.Browser/Tailwind.Avalonia.Sample.Browser.csproj -c Release` plus an integrated-browser reload check showing zero `pageerror` events, zero captured error console messages, `hasSplash: false`, and both `canvas` and `.avalonia-native-host` present within 5 seconds.
- Added temporary query-driven browser marks in both the fixed shell and a detached `HEAD` worktree copy of the old eager shell, then ran three integrated-browser reload measurements per version for the same six navigation scenarios.
- The real browser numbers showed the intended tradeoff clearly: first uncached lazy hops were slightly slower than the old eager shell (`section-first-sizing` ~599.8ms vs 555.0ms, `page-first-height` ~539.7ms vs 503.5ms), but every cached revisit dropped to about 49-51ms instead of roughly 428-545ms (`section-return-spacing` ~49.2ms vs 544.5ms, `section-cached-sizing` ~50.4ms vs 453.2ms, `page-return-width` ~49.5ms vs 434.5ms, `page-cached-height` ~51.0ms vs 428.1ms).
- Removed the temporary measurement code and detached worktree after collecting the data, then revalidated the current browser host build.
- Followed up on a desktop designer regression where Avalonia's runtime preview compiler failed on `SampleShell` string event handlers and compiled item templates over local descriptor types; moved the `TabStrip` event hookups into code-behind and changed the header templates to reflection binding, then revalidated the desktop host with a Release build after the open previewer process blocked Debug output files.