# [TASK015] - Tailwind Avalonia padding class reference table

**Status:** Completed  
**Added:** 2026-05-08  
**Updated:** 2026-05-08

## Original Request
add Class list to DataGrid in `Padding.axaml`, mimic Tailwind padding docs, write the `Styles` column in AXAML format like `p-<number>` -> `Padding="<number>"`, and add a `SHOW MORE` button with expand and close behavior

## Thought Process
- The request targeted the sample docs surface only, not the parser or generated spacing resources.
- The page already had an empty `DataGrid` placeholder near the top, so the smallest correct change was to turn that slot into a Tailwind-style reference table rather than inventing a second docs section elsewhere.
- The table needed to stay honest to the current library surface, so it should list only supported numeric padding families and spell logical `ps-*` / `pe-*` mappings out in AXAML terms instead of implying unsupported arbitrary-value syntax.
- Because the sample project enables compiled bindings globally but this one table only needed a few simple row bindings, a local `x:CompileBindings="False"` override on the page kept the implementation small and low-risk.

## Implementation Plan
- Replace the empty `DataGrid` placeholder in `Padding.axaml` with a styled reference table and toggle footer.
- Add the row data and show-more state in `Padding.axaml.cs`.
- Extend `SpacingDocsStyles.axaml` with table and button styling that fits the existing docs shell.
- Revalidate the sample build.
- Refresh the memory bank.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 15.1 | Replace the empty placeholder with a reference table | Complete | 2026-05-08 | Added a `DataGrid` with `Class` and `Styles` columns to `Padding.axaml` |
| 15.2 | Add expand and close behavior | Complete | 2026-05-08 | `SHOW MORE` / `CLOSE` now swaps the compact and full row sets |
| 15.3 | Style the table to fit the docs shell | Complete | 2026-05-08 | Added shared grid, text, and button styles in `SpacingDocsStyles.axaml` |
| 15.4 | Validate sample build | Complete | 2026-05-08 | `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj` passed |
| 15.5 | Refresh memory bank | Complete | 2026-05-08 | Updated active context, progress, and task index |

## Progress Log
### 2026-05-08
- Re-read the top of `Padding.axaml`, the sample style dictionary, and the `Tw` spacing mappings to confirm the change should stay on the sample UI layer.
- Added a Tailwind-docs-inspired reference `DataGrid` at the top of the page and mapped the supported padding families to AXAML `Padding="..."` equivalents.
- Kept the table honest to current support by omitting unsupported `p-px`, arbitrary-value, and custom-property rows.
- Added `SHOW MORE` / `CLOSE` behavior in `Padding.axaml.cs` and used a local `x:CompileBindings="False"` override so the page could use simple binding expressions inside the `DataGridTemplateColumn` cells.
- Revalidated with `dotnet build samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj`.