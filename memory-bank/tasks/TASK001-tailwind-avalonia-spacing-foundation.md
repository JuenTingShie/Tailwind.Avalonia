# TASK001 - Tailwind Avalonia Spacing Foundation

**Status:** Completed  
**Added:** 2026-05-06  
**Updated:** 2026-05-06

## Original Request
建立執行計畫，目標是作為一個 AvaloniaUI 的套件庫，產生 Tailwind CSS 哲學的樣式庫。先做出 Spacing（Padding、Margin），並列出下一個實作目標。目標版本為 Avalonia 12+、Tailwind 4.2、.NET 10。

## Thought Process
- Tailwind mental model should survive in public API.
- Avalonia-native resource usage should also remain available.
- StaticResource-only is stable but loses Tailwind-style composition.
- Avalonia `Classes` should not be primary surface because Tailwind token syntax includes characters that collide with Avalonia selector syntax.
- Best current direction is hybrid: generated resources plus attached-property parser.
- `MergeResourceInclude` looked attractive for app-level consumption, but the current project-reference setup resolved more reliably with runtime `ResourceInclude` while preserving the package entry URI.

## Implementation Plan
- Define package architecture and public API direction.
- Freeze Spacing MVP surface.
- Define naming adaptation rules for Tailwind tokens to Avalonia-safe resource keys.
- Scaffold project and resource entry point.
- Implement spacing manifest, generation, parser, and samples.
- Choose next feature family after spacing.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 1.1 | Capture product goals and version targets | Complete | 2026-05-06 | Avalonia 12+, Tailwind 4.2, .NET 10 recorded |
| 1.2 | Decide spacing MVP boundary | Complete | 2026-05-06 | Padding and Margin families only |
| 1.3 | Choose initial architecture | Complete | 2026-05-06 | Hybrid resources + attached-property parser |
| 1.4 | Record plan in memory bank | Complete | 2026-05-06 | Core memory-bank files initialized |
| 1.5 | Scaffold implementation project | Complete | 2026-05-06 | Solution, library, and sample app created |
| 1.6 | Identify next implementation target | Complete | 2026-05-06 | Colors selected as next family |
| 1.7 | Implement spacing resources and parser | Complete | 2026-05-06 | `SpacingResourceDictionary`, `Themes/Tailwind.axaml`, and `Tw.Class` added |
| 1.8 | Validate build and sample startup | Complete | 2026-05-06 | `dotnet build` passed and sample app launched without immediate startup errors |
| 1.9 | Refresh memory bank with implementation state | Complete | 2026-05-06 | Core docs updated to reflect implemented spacing foundation |

## Progress Log
### 2026-05-06
- Captured project brief, product context, system patterns, active context, and progress summary.
- Recorded decision to avoid a classes-only API because Tailwind token syntax does not map cleanly to Avalonia selector syntax.
- Locked first milestone to spacing utilities only.
- Recorded Colors as next feature family after spacing because theme expression and dark/light support depend on it.

### 2026-05-06
- Scaffolded the solution, library, and sample app on .NET 10 with Avalonia 12.0.2.
- Implemented the spacing scale source-of-truth, generated spacing resources, and the `tw:Tw.Class` spacing parser.
- Added a package-level theme entry and wired the sample app to consume it through `ResourceInclude`.
- Validated the solution with `dotnet build` and a sample application startup run.
- Updated the memory bank to move TASK001 from planning state to completed spacing foundation.