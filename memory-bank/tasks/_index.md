# Tasks Index

## In Progress
- None yet.

## Pending
- None yet.

## Completed
- [TASK026] Tailwind Avalonia section preview without page switch - Updated `SampleShell` so section changes only preview page groups and no longer force content navigation, then revalidated desktop/browser builds.
- [TASK025] Tailwind Avalonia compact sample shell header - Reduced the shared shell header to a single-row, button-height-oriented layout and revalidated the desktop sample build plus tests.
- [TASK024] Tailwind Avalonia sample shell visual polish - Applied card-based shell styling, stronger selected states, and richer header/pane chrome to the shared responsive sample shell, then revalidated desktop/browser host builds.
- [TASK023] Tailwind Avalonia sample shell responsive navigation - Reworked `SampleShell` into a collapsible vertical `SplitView` navigation shell with mobile-friendly narrow-width behavior and revalidated desktop/browser host builds.
- [TASK001] Tailwind Avalonia spacing foundation - Solution scaffolded, spacing resources/parser implemented, sample build and startup validated.
- [TASK002] Tailwind Avalonia colors token foundation - Official Tailwind v4.2 palette resources added, sample build validated, and runtime startup smoke-tested.
- [TASK003] Tailwind Avalonia automated test foundation - xUnit project added, spacing and color tests implemented, `dotnet test` passing.
- [TASK004] Tailwind Avalonia first color utility parsing - `tw:Tw.Class` now applies `bg-*`, `text-*`, and `border-*`, with 20 passing tests.
- [TASK005] Tailwind Avalonia color opacity and sample refresh - `transparent` and `/opacity` support added, sample updated, directional border-color deferred with documented rationale.
- [TASK007] Tailwind Avalonia no-custom-component rollback - Removed `TwBorder`, restored whole-border semantics, tests and sample updated.
- [TASK008] Tailwind Avalonia docs-style sample page - Sample rebuilt into left-tab `Padding` / `Margin` docs layout and sample build revalidated.
- [TASK009] Tailwind Avalonia visual padding aliases - Added `psv-*` and `pev-*`, updated padding docs, and revalidated focused tests plus sample build.
- [TASK010] Tailwind Avalonia color utility sample user control - Added standalone `Typography/ColorUtilities` docs page, wired a `Colors` tab, and revalidated the sample build.
- [TASK011] Tailwind Avalonia StaticResource sample code - Added StaticResource code variants or explicit unsupported notes to all sample pages and revalidated sample build plus tests.
- [TASK012] Tailwind Avalonia StaticResource docs-code formatting - Reformatted StaticResource snippets into clearer multi-line AXAML blocks and revalidated sample build plus tests.
- [TASK013] Tailwind Avalonia xml:space docs-code formatting - Collapsed multi-line StaticResource snippets into single `xml:space="preserve"` TextBlocks and revalidated sample build plus tests.
- [TASK014] Tailwind Avalonia tabbed sample version switcher - Added nested example tabs that switch preview and AXAML between utility and StaticResource variants, while keeping unsupported notes honest.
- [TASK015] Tailwind Avalonia padding class reference table - Added a Tailwind-docs-style padding reference DataGrid with AXAML mappings and a show-more toggle.
- [TASK016] Tailwind Avalonia compiled-binding-safe spacing reference sync - Removed the padding root compiled-binding fallback, added unsupported-row notes, and brought the same reference-table pattern to Margin.
- [TASK017] Tailwind Avalonia sizing utility sample - Added numeric sizing parsing for width/min/max width and height/min/max height, docs-style sample pages, and focused sizing tests.
- [TASK018] Tailwind Avalonia sizing StaticResource parity - Added generated sizing resource keys, converted sizing sample tabs to real StaticResource examples, and recorded the new-feature parity rule.
- [TASK019] Tailwind Avalonia sizing docs example coverage sync - Expanded sizing sample pages to mirror official Tailwind example headings while keeping unsupported examples explicit.
- [TASK020] Tailwind Avalonia font size utility and sample - Added `FontSize*` resources, `text-*` font-size parsing with text-color disambiguation, and a Typography font-size docs page.
- [TASK021] Tailwind Avalonia browser sample and Pages hosting - Split the sample into shared/desktop/browser hosts, added Pages-ready browser assets and workflow, and documented the remaining local `wasm-tools` prerequisite.
- [TASK022] Tailwind Avalonia browser tab switch performance - Reworked the shared sample shell to lazy-load and cache docs pages so browser tab switching no longer pays the eager nested-tab construction cost.

## Abandoned
- [TASK006] Tailwind Avalonia directional border rendering layer - Reverted after user constraint disallowed custom components.
