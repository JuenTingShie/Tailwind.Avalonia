# [TASK017] - Tailwind Avalonia sizing utility sample

**Status:** Completed  
**Added:** 2026-05-10  
**Updated:** 2026-05-10

## Original Request
add Sizing function and sample to "Sizing" folder

    width
    min-width
    max-width
    height
    min-height
    max-height

    mimic tailwind doc page (https://tailwindcss.com/docs/width) and styling like Spacing/Padding and Spacing/Margin

## Thought Process
- Extend the existing `tw:Tw.Class` parser instead of creating a separate sizing pipeline, so sizing stays consistent with the current hybrid utility approach.
- Reuse `SpacingScale` for first-pass numeric sizing because Tailwind `w-<number>` and related numeric utilities are also spacing-scale driven.
- Keep the sample consistent with existing docs pages: reference table, unsupported rows note, utility-first live previews, and an explicit `StaticResource` unsupported note when no generated key exists.
- Group the sample under `Width` and `Height` pages, with min/max sections inside each page, because that matches the existing category-level sample structure used by `Padding` and `Margin`.

## Implementation Plan
- Add parser support for `w-*`, `min-w-*`, `max-w-*`, `h-*`, `min-h-*`, and `max-h-*` using spacing-scale numeric suffixes.
- Add focused tests for sizing utility application, overwrite order, and clearing behavior.
- Add docs-style sample pages under `samples/Tailwind.Avalonia.Sample/Sizing/` for width/min/max width and height/min/max height.
- Wire the new sample pages into `MainWindow.axaml` and validate the sample build.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 1.1 | Add sizing utility parsing in `Tw` | Complete | 2026-05-10 | Added numeric width/min/max width and height/min/max height property application through reflected double-property discovery. |
| 1.2 | Add focused sizing tests | Complete | 2026-05-10 | Covered apply, last-token-wins, and clear-on-remove behavior in `TwTests`. |
| 1.3 | Create sizing docs sample pages | Complete | 2026-05-10 | Added `Width` and `Height` sample pages with min/max sections, reference tables, and unsupported-row notes. |
| 1.4 | Wire sample navigation and validate | Complete | 2026-05-10 | Connected the pages to the `SIZING` shell and validated the sample build; only existing file-lock copy warnings remained. |

## Progress Log
### 2026-05-10
- Added numeric sizing utility parsing in `src/Tailwind.Avalonia/Tw.cs` for width/min-width/max-width/height/min-height/max-height.
- Added focused sizing coverage in `tests/Tailwind.Avalonia.Tests/TwTests.cs` and verified the file-level test run passed.
- Added `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml` and `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml` plus code-behind for their docs reference tables.
- Wired the new sizing pages into the `SIZING` tab in `samples/Tailwind.Avalonia.Sample/MainWindow.axaml`.
- Built the sample project successfully; the only warnings were transient copy retries caused by an already running sample process locking the output DLL.