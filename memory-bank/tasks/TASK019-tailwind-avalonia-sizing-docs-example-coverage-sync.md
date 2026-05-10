# [TASK019] - Tailwind Avalonia sizing docs example coverage sync

**Status:** Completed  
**Added:** 2026-05-10  
**Updated:** 2026-05-10

## Original Request
you miss all examples, learn from mimic tailwind docs:
- https://tailwindcss.com/docs/width
- https://tailwindcss.com/docs/min-width
- https://tailwindcss.com/docs/max-width
- https://tailwindcss.com/docs/height
- https://tailwindcss.com/docs/min-height
- https://tailwindcss.com/docs/max-height

## Thought Process
- Current sizing pages covered only supported numeric examples, but they did not mirror the official Tailwind docs example headings closely enough.
- Full parser support for percentages, viewport units, container tokens, arbitrary values, `size-*`, and responsive variants is still out of scope, so docs should reflect those official examples as explicit unsupported sections instead of pretending they do not exist.
- This keeps the sample closer to the requested Tailwind-docs feel without overstating package capability.

## Implementation Plan
- Re-read the official Tailwind sizing docs example headings.
- Expand the width and height sample pages to include those headings.
- Keep supported numeric examples as live demos.
- Render unsupported official examples as explicit note sections.
- Rebuild the sample project to validate AXAML.

## Progress Tracking

**Overall Status:** Completed - 100%

### Subtasks
| ID | Description | Status | Updated | Notes |
|----|-------------|--------|---------|-------|
| 1.1 | Compare official headings to sample pages | Complete | 2026-05-10 | Re-read width/min/max-width and height/min/max-height docs. |
| 1.2 | Expand width sample coverage | Complete | 2026-05-10 | Added official example sections for percentage, container scale, viewport, auto reset, breakpoint container, size, custom value, and responsive topics. |
| 1.3 | Expand height sample coverage | Complete | 2026-05-10 | Added official example sections for percentage, viewport families, size, custom value, and responsive topics. |
| 1.4 | Validate sample build | Complete | 2026-05-10 | `dotnet build` for sample project succeeded. |

## Progress Log
### 2026-05-10
- Re-read the official Tailwind sizing docs pages and compared their example headings against the current sample.
- Updated `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml` so it now includes explicit sections for all major official width/min-width/max-width example categories.
- Updated `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml` so it now includes explicit sections for all major official height/min-height/max-height example categories.
- Kept supported numeric examples live and turned unsupported official examples into honest note sections.
- Rebuilt the sample project successfully after the AXAML updates.