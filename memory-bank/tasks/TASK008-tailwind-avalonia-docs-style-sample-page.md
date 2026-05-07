# [TASK008] - Tailwind Avalonia docs-style sample page

**Status:** Completed  
**Added:** 2026-05-06  
**Updated:** 2026-05-06

## Original Request
重作sample頁面，左邊有tab，一個tab一個分類，如 Padding / Margin，每一個tab會有該功能的詳細使用方法，模仿 tailwind 的說明文件頁面。

## Thought Process
- The sample redesign should stay inside sample XAML and resource files only; core parser work was already good enough for the requested spacing pages.
- The cheapest honest implementation path was a standard Avalonia `TabControl` with `TabStripPlacement="Left"`, because the user wanted left-side tabs but the project cannot introduce custom components.
- Tailwind docs section order was useful as a content blueprint, but the sample must only document the subset that Tailwind.Avalonia actually supports today.
- The previous sample refresh had made `bg-*`, `text-*`, and `border-*` visible, so the redesign needed to preserve that signal somewhere instead of silently regressing the sample surface.

## Implementation Plan
- Replace the old three-card showcase with a docs-style layout built around a left-side `TabControl`.
- Add `Padding` tab content covering basic, one-side, axis, and logical spacing usage.
- Add `Margin` tab content covering basic, one-side, axis, negative, and logical spacing usage.
- Move the new page copy and layout tokens into sample resources.
- Rebuild the sample project and sync the memory bank.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 8.1 | Replace showcase shell with left-tab docs layout | Complete | 2026-05-06 | Used standard Avalonia `TabControl` with `TabStripPlacement="Left"` |
| 8.2 | Add `Padding` docs content | Complete | 2026-05-06 | Added basic, one-side, axis, and logical sections |
| 8.3 | Add `Margin` docs content | Complete | 2026-05-06 | Added basic, one-side, axis, negative, and logical sections |
| 8.4 | Preserve visible color utility coverage in the new sample | Complete | 2026-05-06 | Support cards now use `bg-*`, `text-*`, and `border-*` utilities |
| 8.5 | Validate sample build and sync memory | Complete | 2026-05-06 | `dotnet build` passed after the redesign and follow-up tweak |

## Progress Log
### 2026-05-06
- Re-read the sample XAML, sample resources, and memory-bank state before editing so the redesign stayed local.
- Modeled the section structure on Tailwind docs, but limited content to the spacing features already supported by `tw:Tw.Class`.
- Replaced the old sample with a left-tab docs shell and moved the new page copy plus layout tokens into `SampleTokens.axaml`.
- Added structured `Padding` and `Margin` walkthroughs with preview cards and code strings.
- Restored explicit `bg-*`, `text-*`, and `border-*` visibility inside the support cards so the new layout did not regress earlier sample coverage.
- Validated the sample by rebuilding `samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj` successfully.