# [TASK018] - Tailwind Avalonia sizing StaticResource parity

**Status:** Completed  
**Added:** 2026-05-10  
**Updated:** 2026-05-10

## Original Request
記得每一次新增功能都要包含一併新增生程式 StaticResource 版本

## Thought Process
- Treat this as a project rule, not a one-off comment, so the preference must be reflected both in code and in project memory.
- The existing numeric sizing feature already maps cleanly onto shared Avalonia `double` resources, so the honest fix is to generate resource keys instead of leaving the sample on a utility-only island.
- The most consistent implementation surface is `SpacingResourceDictionary`, because numeric sizing uses the same `SpacingScale` source of truth and the package theme already merges that dictionary.

## Implementation Plan
- Extend the existing resource dictionary to emit generated numeric sizing keys.
- Add focused tests that validate those keys and their values.
- Replace the sizing sample pages' `StaticResource` unsupported placeholders with real examples and readable AXAML snippets.
- Record the new-feature-equals-StaticResource-parity rule in memory.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 1.1 | Add generated sizing resource keys | Complete | 2026-05-10 | Added `Width*`, `MinWidth*`, `MaxWidth*`, `Height*`, `MinHeight*`, and `MaxHeight*` to `SpacingResourceDictionary`. |
| 1.2 | Add resource dictionary validation | Complete | 2026-05-10 | Added `SpacingResourceDictionaryTests` to assert key presence, values, and total dictionary count. |
| 1.3 | Convert sizing sample StaticResource tabs | Complete | 2026-05-10 | Replaced unsupported notes with real previews and multi-line AXAML snippets in `Width.axaml` and `Height.axaml`. |
| 1.4 | Persist the parity rule | Complete | 2026-05-10 | Updated user memory and repository memory-bank notes so future features keep StaticResource parity. |

## Progress Log
### 2026-05-10
- Extended `src/Tailwind.Avalonia/Spacing/SpacingResourceDictionary.cs` to emit generated numeric sizing resources from the existing spacing scale.
- Added `tests/Tailwind.Avalonia.Tests/SpacingResourceDictionaryTests.cs` and verified the new resource keys through focused test execution.
- Updated `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml` and `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml` so the `StaticResource` tabs now show real examples instead of unsupported placeholders.
- Rebuilt the sample project successfully and recorded the new parity preference in persistent memory.