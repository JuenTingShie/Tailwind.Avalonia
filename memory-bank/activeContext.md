# Active Context

## Current Focus
With the sample docs refresh complete, build on the spacing and colors foundation with semantic theme composition and package validation, under the no-custom-component constraint.

## Recent Changes
- Replaced the sample spacing reference tables' `DataGrid` shell with a lightweight header-plus-`ItemsControl` layout so `SHOW MORE` expands to the full row set without relying on manual height math or internal grid scroll behavior.
- Removed the temporary `x:CompileBindings="False"` escape hatch from `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml`; the padding reference `DataGrid` now stays compatible with compiled bindings by giving each `DataTemplate` an explicit row `DataType` and keeping the row list/toggle state in code-behind.
- Added explicit unsupported-row note blocks for the padding reference section so omitted Tailwind rows like `p-px`, arbitrary values, and custom-property syntax are called out instead of silently disappearing.
- Added the same Tailwind-docs-style collapsible reference table pattern to `samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml`, including negative and logical margin mappings plus a matching unsupported-row note.
- Extracted the shared `SpacingUtilityReferenceRow` and `SpacingUtilityReferenceTablePresenter` helper into the sample spacing folder so `Padding` and `Margin` keep one consistent expand/collapse implementation.
- Filled the empty top-of-page placeholder in `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml` with a Tailwind-docs-inspired `DataGrid` reference table for supported numeric padding utility families, expressed as AXAML `Padding="..."` equivalents.
- Added `SHOW MORE` / `CLOSE` behavior for that padding reference table, driven by lightweight `Padding.axaml.cs` state and a local `x:CompileBindings="False"` override so the sample can use simple runtime bindings inside `DataGridTemplateColumn` cells without introducing a separate view model.
- Scaffolded the solution, library project, and sample app.
- Added `SpacingScale` as the spacing source of truth.
- Added `SpacingResourceDictionary` and package theme entry `Themes/Tailwind.axaml`.
- Implemented `tw:Tw.Class` for spacing utilities, including logical start/end handling with RTL awareness.
- Validated the solution with `dotnet build` and a sample app startup run.
- Added a colors source-of-truth based on the official Tailwind v4.2 palette reference.
- Added `ColorResourceDictionary` and an internal OKLCH-to-Avalonia conversion path because Avalonia 12 does not parse `oklch(...)` values directly.
- Updated the sample app to consume package brush tokens instead of local hardcoded brushes.
- Added `Tailwind.Avalonia.Tests` as an xUnit test project and wired it into the solution.
- Added focused unit coverage for `SpacingScale`, `tw:Tw.Class`, `TailwindCssColorParser`, `TailwindColorPalette`, and `ColorResourceDictionary`.
- Extended `tw:Tw.Class` with first-pass color utilities for `bg-*`, `text-*`, and `border-*` by mapping Tailwind palette tokens onto Avalonia brush properties.
- Extended `tw:Tw.Class` color utilities with `transparent` and `/opacity` support for `bg-*`, `text-*`, and `border-*` tokens.
- Added focused regression tests for color utility application and clearing behavior.
- Updated sample XAML to visibly demonstrate `bg-*`, `text-*`, and `border-*` utility usage instead of only package brush resources.
- Removed the `TwBorder` experiment because user constraint now disallows custom components.
- Reverted directional border-color parsing and sample usage back to honest whole-property border color behavior.
- Validated the sample build and the test project with 21 passing tests.
- Rebuilt the sample app into a Tailwind-docs-inspired layout using a standard Avalonia `TabControl` with left-side `Padding` and `Margin` tabs.
- Added sectioned sample content for basic, one-side, axis, logical, and negative-margin spacing examples.
- Kept whole-property `bg-*`, `text-*`, and `border-*` utilities visible in the support cards so the docs sample still advertises current color capability.
- Revalidated the sample project build after the docs-page redesign.
- Split the `Padding` and `Margin` tab bodies out of `MainWindow.axaml` into `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml` and `samples/Tailwind.Avalonia.Sample/Spacing/Margin.axaml` as standalone `UserControl` views.
- Moved the docs page styles plus the striped padding brush out of `MainWindow.axaml` into `samples/Tailwind.Avalonia.Sample/Spacing/SpacingDocsStyles.axaml`, and each spacing `UserControl` now merges its own resources/styles so the views can be opened and compared standalone in the designer.
- Removed `samples/Tailwind.Avalonia.Sample/Resources/SampleTokens.axaml`; sample-only copy, dimensions, and tab labels are now inlined directly in `MainWindow.axaml`, `Padding.axaml`, and `Margin.axaml`, while shared visual styles stay in `SpacingDocsStyles.axaml`.
- Investigated the remaining RTL `ps-8` / `pe-8` docs mismatch and confirmed through focused `Tw` tests that logical thickness/layout was already correct in the library layer.
- Updated `samples/Tailwind.Avalonia.Sample/Spacing/Padding.axaml` so the RTL logical padding preview keeps `FlowDirection="RightToLeft"` on the utility target but cancels Avalonia's visual mirror transform with a local `ScaleTransform`, and revalidated both the sample build and focused `Tw` tests.
- Reworked the logical padding docs section so it now separates honest `FlowDirection`-only RTL rendering from a second, explicitly labeled teaching visualization that uses `ScaleTransform` only to cancel Avalonia mirroring for side-mapping explanation.
- Added `psv-<number>` and `pev-<number>` as padding-only parser aliases for visual start/end, mapping to physical left/right padding so Avalonia's RTL mirror places the spacing on the final rendered side.
- Updated the padding docs page with dedicated `psv-8` / `pev-8` demos and actual-usage examples, then revalidated the focused `TwTests` file and the sample build.
- Added `samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml` as a standalone `UserControl`, wired it into a new `Colors` tab, and moved color-utility docs into a dedicated sample page instead of leaving them only as spacing-page support notes.
- Updated the `Padding`, `Margin`, and `Colors` sample pages so every example section now shows a `StaticResource` version when possible, with explicit unsupported notes for parser-only logical spacing and for `bg-transparent`, which still has no generated brush resource.
- Reformatted the StaticResource sample snippets across `Padding`, `Margin`, and `Colors` so the docs-code examples now read as clearer multi-line AXAML blocks instead of compact one-line fragments.
- Reworked those multi-line StaticResource snippets to use single `TextBlock` elements with `xml:space="preserve"` so indentation is preserved without stacking one control per line.
- Reworked the sample example sections again so nested tabs now switch both the live preview and the AXAML snippet between `Utility` and `StaticResource` variants, while unsupported cases keep their honest explanatory note in the `StaticResource` tab.

## Active Decisions
- For short docs reference tables on the sample pages, prefer a custom header row plus `ItemsControl` over `DataGrid`; it is lighter, expands naturally, and avoids internal scrollbars/clipping when the row count changes.
- Sample docs pages should not disable compiled bindings at the root just to support a small `DataGrid` reference table; keep compiled bindings on, give the row `DataTemplate` an explicit `DataType`, and drive table state from code-behind.
- Reference sections should include an explicit unsupported-row block when the official Tailwind table contains parser features the sample does not implement yet, so omissions stay honest.
- The padding reference table on the sample page should list only currently supported numeric utility families instead of echoing unsupported Tailwind rows like `p-px`, arbitrary values, or custom-property syntax.
- The table's `Styles` column should use AXAML-facing `Padding="..."` examples, and logical `ps-*` / `pe-*` rows should spell out both LTR and RTL outcomes explicitly.
- Hybrid API remains the chosen direction: stable resource keys plus utility parsing.
- Package resource entry is kept, but sample consumption currently uses `ResourceInclude` instead of `MergeResourceInclude` because compile-time flattening did not resolve the project-reference resource during build.
- Official Tailwind docs values remain the source of truth for colors; the package converts those OKLCH values to Avalonia `Color` and `SolidColorBrush` instances at runtime.
- Current color utilities are property-wide: `bg-*` targets `Background`, `text-*` targets `Foreground`, and `border-*` targets `BorderBrush` when the target control exposes those Avalonia properties.
- `transparent` and `/opacity` are in scope for whole-property color utilities.
- Custom components are off-limits for this project direction.
- Generic `Border` stays whole-property only because Avalonia `Border` exposes only `BorderBrushProperty` and sealed rendering leaves no honest no-custom-component path for directional border colors.
- `MainWindow.axaml` now owns shared docs resources and styles, while the spacing page bodies live in dedicated child `UserControl` files under `samples/Tailwind.Avalonia.Sample/Spacing/`.
- `MainWindow.axaml` now owns only the tab shell styles; spacing-page presentation styles are loaded locally by the child `UserControl` files.
- The sample docs shell now hosts a dedicated color-utility page from `samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml`, while still keeping each page locally responsible for including package resources and shared docs styles.
- The sample app no longer carries a separate sample token dictionary; only package theme resources remain merged, and demo-local text/metrics live next to the sample markup that uses them.
- Sample docs pages should pair `tw:Tw.Class` examples with `StaticResource` equivalents whenever the package emits a matching resource key; when the current resource surface cannot express the same behavior, the page should say so explicitly instead of implying support.
- When a sample page shows a `StaticResource` version, the docs-code snippet should be formatted like readable multi-line AXAML rather than compressed into a single-line fragment.
- For multi-line docs-code snippets, prefer a single `TextBlock` with `xml:space="preserve"` over separate `TextBlock` elements per line.
- Sample docs example sections should use nested tabs to switch preview and code between `Utility` and `StaticResource`; unsupported StaticResource behavior should live in the `StaticResource` tab as an explicit note.
- Docs-style RTL logical spacing previews may need visual mirror cancellation on the preview target when the goal is to show the physical side chosen by the utility rather than Avalonia's mirrored overall control.
- When docs need both truthful behavior and easier pedagogy, the sample should present them as separate demos instead of hiding transform-based teaching aids inside the only example.
- `ps-*` and `pe-*` remain logical spacing utilities; `psv-*` and `pev-*` are now the explicit parser surface for final visual start/end padding under Avalonia's mirror model.

## Immediate Next Steps
1. Start semantic alias and dark/light theme layering on top of the concrete Tailwind palette.
2. Decide whether the new combined `Colors` page should stay unified or split further into dedicated `Background`, `Text`, and `Border Color` tabs.
3. Validate package/publish consumption beyond the local project-reference sample.

## Open Questions
- Whether logical start/end spacing should gain dedicated `StaticResource` keys or remain parser-only.
- Whether spacing generation should remain checked-in C# or move to a generator pipeline.
- Whether color conversion should stay as runtime code or move to generated, precomputed sRGB values later.
- Whether a C# helper API is worth exposing in addition to the XAML-facing surface.
- Whether directional border colors should remain unsupported indefinitely under the no-custom-component constraint or be documented as intentionally out of scope.
