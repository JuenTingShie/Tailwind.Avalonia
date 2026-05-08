# [TASK013] - Tailwind Avalonia xml:space docs-code formatting

**Status:** Completed  
**Added:** 2026-05-08  
**Updated:** 2026-05-08

## Original Request
using xml:space="preserve" to preserve space without creating multiple TextBlocks

## Thought Process
- The follow-up still targeted sample-doc presentation only, not library behavior.
- After the previous formatting pass, the sample pages still represented multi-line AXAML by splitting one snippet into many `TextBlock` rows.
- The smallest correct improvement was to keep the same code examples and unsupported notes, but collapse each multi-line StaticResource snippet into a single `TextBlock` using `xml:space="preserve"`.

## Implementation Plan
- Review current multi-line StaticResource snippet sections on the sample pages.
- Replace line-by-line docs-code `TextBlock` groups with single `xml:space="preserve"` `TextBlock` elements.
- Keep sample semantics and unsupported notes unchanged.
- Revalidate the sample build and test project.
- Sync the memory bank.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 13.1 | Review remaining line-by-line docs-code snippets | Complete | 2026-05-08 | Confirmed issue was limited to StaticResource sample snippets on sample pages |
| 13.2 | Convert spacing-page snippets to `xml:space` format | Complete | 2026-05-08 | Updated `Padding.axaml` and `Margin.axaml` |
| 13.3 | Convert color-page snippets to `xml:space` format | Complete | 2026-05-08 | Updated `ColorUtilities.axaml` |
| 13.4 | Validate sample build and tests | Complete | 2026-05-08 | Sample build and test project both passed |
| 13.5 | Refresh memory bank | Complete | 2026-05-08 | Active context, progress, and task index updated |

## Progress Log
### 2026-05-08
- Re-read the current sample pages and confirmed the remaining formatting issue was the use of one `TextBlock` per code line in multi-line StaticResource snippets.
- Replaced those line-by-line groups with single `TextBlock` elements using `xml:space="preserve"` so indentation stays visible without extra controls.
- Left existing utility examples and unsupported notes untouched.
- Revalidated with `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj` and `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj`.
