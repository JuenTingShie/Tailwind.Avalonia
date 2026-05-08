# [TASK011] - Tailwind Avalonia StaticResource sample code

**Status:** Completed  
**Added:** 2026-05-08  
**Updated:** 2026-05-08

## Original Request
Every sample page should also have StaticResource version sample code.

Update existed sample, if cannot achieve that, still need to create a "unsupport" to revealed that.

## Thought Process
- The request targeted sample docs coverage, not library behavior, so the safest scope was to update only the sample XAML pages.
- Existing resource generation already determined what could be shown honestly: physical spacing keys, negative physical margin keys, and whole-property color resources were available; logical spacing aliases and `bg-transparent` had no direct generated StaticResource equivalents.
- The sample should therefore pair each utility example with a StaticResource version when possible and use an explicit unsupported note where the current generated resource surface cannot represent the same behavior.

## Implementation Plan
- Review current sample pages and generated resource boundaries.
- Add StaticResource code snippets beside existing utility examples on each sample page.
- Add explicit unsupported notes for parser-only or non-generated cases.
- Revalidate the sample build and existing tests.
- Sync the memory bank with the new docs convention.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 11.1 | Review sample pages and resource support limits | Complete | 2026-05-08 | Confirmed physical spacing, negative physical margin, and whole-property color resources are available |
| 11.2 | Add StaticResource snippets to padding and margin docs | Complete | 2026-05-08 | Added matching StaticResource examples plus unsupported notes for logical spacing |
| 11.3 | Add StaticResource snippets to color docs | Complete | 2026-05-08 | Added brush/color resource examples and unsupported note for `bg-transparent` |
| 11.4 | Validate sample build and test project | Complete | 2026-05-08 | `dotnet build` on sample and `dotnet test` on test project both passed |
| 11.5 | Refresh memory bank | Complete | 2026-05-08 | Active context, progress, and task index updated |

## Progress Log
### 2026-05-08
- Re-read the sample pages and the generated spacing/color resource dictionaries before editing so the docs would stay honest about current StaticResource coverage.
- Updated `Padding.axaml`, `Margin.axaml`, and `ColorUtilities.axaml` so every example section now includes a StaticResource code variant when one exists.
- Added explicit unsupported notes for logical spacing aliases and `bg-transparent`, because those cases still do not have generated StaticResource equivalents.
- Revalidated the sample page changes with `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj` and `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`.
