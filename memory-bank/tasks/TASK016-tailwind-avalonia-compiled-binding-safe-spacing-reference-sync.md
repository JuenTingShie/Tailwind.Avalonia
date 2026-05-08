# [TASK016] - Tailwind Avalonia compiled-binding-safe spacing reference sync

**Status:** Completed  
**Added:** 2026-05-08  
**Updated:** 2026-05-08

## Original Request
1. remember `x:CompileBindings=False` is not allowed  
2. add an unsupported rows block that explicitly calls out `p-px`, arbitrary values, and custom-property syntax as unsupported  
3. keep things consistent by adding the same reference table pattern to the `Margin` page

## Thought Process
- The controlling issue was local to the sample docs pages, not the parser or generated resources.
- The previous padding reference table worked only because the whole page had been downgraded to reflection bindings, which violated the user's constraint and weakened the page unnecessarily.
- The smallest honest fix was to keep root compiled bindings intact, give the `DataGrid` row template an explicit row `DataType`, and continue driving the row list and toggle state from code-behind.
- Once that compiled-binding-safe path existed, the same reference-table shell could be reused for `Margin`, keeping the docs layout consistent without introducing a separate view model layer.

## Implementation Plan
- Remove the root `x:CompileBindings="False"` override from the padding page.
- Make the reference-table row bindings typed and compiled-binding-safe.
- Add explicit unsupported-row notes for the padding reference section.
- Add the same reference-table pattern to the margin page.
- Extract the shared table/toggle behavior into a helper and revalidate the sample build.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 16.1 | Remove padding compiled-binding fallback | Complete | 2026-05-08 | Dropped root `x:CompileBindings="False"` and switched row templates to typed compiled bindings |
| 16.2 | Add unsupported padding rows block | Complete | 2026-05-08 | Explicitly called out `p-px`, arbitrary value, and custom-property syntax |
| 16.3 | Sync the same reference-table pattern to Margin | Complete | 2026-05-08 | Added margin reference grid, toggle, and unsupported rows note |
| 16.4 | Share table behavior | Complete | 2026-05-08 | Added `SpacingUtilityReferenceTablePresenter` and shared row record |
| 16.5 | Validate sample build | Complete | 2026-05-08 | `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj` passed |

## Progress Log
### 2026-05-08
- Re-read `Padding.axaml`, `Padding.axaml.cs`, `Margin.axaml`, and `Margin.axaml.cs` to isolate the smallest compiled-binding-safe fix.
- Verified against the latest Avalonia compiled bindings docs that a typed `DataTemplate` is the right local fix, instead of disabling compiled bindings for the whole `UserControl`.
- Removed the padding root binding fallback, changed the row templates to typed compiled bindings, and kept the row list/toggle state in code-behind.
- Added explicit unsupported-row blocks so omitted Tailwind syntax stays visible instead of silently disappearing from the docs table.
- Added the same collapsible reference-table pattern to the margin page and extracted the shared table presenter helper.
- Revalidated with `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj`.

### 2026-05-08
- Follow-up fix: replaced the spacing reference tables' `DataGrid` body with a lightweight header + `ItemsControl` layout after the table still clipped rows when expanded.
- Simplified the shared presenter so it only swaps visible rows and button text, with no manual height calculation.
- Revalidated the sample build after the refactor.