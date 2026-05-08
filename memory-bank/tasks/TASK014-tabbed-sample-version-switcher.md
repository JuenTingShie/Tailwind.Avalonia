# [TASK014] - Tailwind Avalonia tabbed sample version switcher

**Status:** Completed  
**Added:** 2026-05-08  
**Updated:** 2026-05-08

## Original Request
using tab under docs-code to switch code and demo between utility and staticresource, if staticresource version is not supported, keep current description in the new tab

## Thought Process
- The request targeted sample-doc interaction only, not library parsing or generated resources.
- The sample pages already showed both utility and StaticResource variants in sequence, so the smallest correct change was to reorganize those sections into nested tabs rather than inventing a new docs surface.
- Unsupported logical-spacing or transparent cases still needed to stay honest, so the new StaticResource tabs should keep the current unsupported note instead of faking a demo.

## Implementation Plan
- Add shared styles for nested example tabs in the sample docs style dictionary.
- Update each sample section on the `Padding`, `Margin`, and `Colors` pages so the tab selection switches both preview and code.
- Keep unsupported StaticResource explanations in the StaticResource tab for parser-only or non-generated cases.
- Revalidate the sample build and test project.
- Refresh the memory bank.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 14.1 | Add shared nested-tab styling | Complete | 2026-05-08 | Added reusable `docs-exampleTabs` / `docs-exampleTab` styles |
| 14.2 | Tab the spacing sample sections | Complete | 2026-05-08 | Updated `Padding.axaml` and `Margin.axaml` to switch preview and code together |
| 14.3 | Tab the color sample sections | Complete | 2026-05-08 | Updated `ColorUtilities.axaml` with utility/static-resource tabs |
| 14.4 | Preserve unsupported docs messaging | Complete | 2026-05-08 | Parser-only logical spacing and missing `BrushTransparent` remain explicit in StaticResource tabs |
| 14.5 | Validate sample build and tests | Complete | 2026-05-08 | Sample build and test project both passed after a transient file-lock retry |
| 14.6 | Refresh memory bank | Complete | 2026-05-08 | Active context, progress, and task index updated |

## Progress Log
### 2026-05-08
- Re-read the current sample pages and shared docs styles to confirm the change should stay local to sample AXAML.
- Added shared nested-tab styling so the new example switcher does not inherit the main left-side page-tab look.
- Reworked the `Padding`, `Margin`, and `Colors` sample sections so each example now switches both the live preview and AXAML snippet between utility and StaticResource variants.
- Kept existing unsupported explanations inside the new StaticResource tabs for parser-only logical spacing and missing `bg-transparent` resource coverage.
- Revalidated with `dotnet test tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` and `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj`, retrying once after a transient file-lock failure.
