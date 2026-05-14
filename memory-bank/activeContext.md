# Active Context

## Current Focus
With the desktop/browser sample split now in place, finish browser runtime validation on a machine that can install the WebAssembly workload, then return to semantic theme composition and package-validation work under the no-custom-component constraint.

## Recent Changes
- Changed `samples/Tailwind.Avalonia.Sample/SampleShell.axaml.cs` so switching sections no longer forces a content page jump; section clicks now only preview that group's page list, while actual content only changes after a page item is selected.
- Reworked `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` again toward a GitHub-style drawer: the sample shell now opens its navigation as a default-closed overlay pane across widths, uses a dedicated close icon inside the pane, collapses again after page navigation, and trims the header into a flatter top bar instead of a floating card.
- Compacted the shared sample shell page header into a single-row section/page line with reduced vertical padding so the chrome now reads closer to the hamburger-button height instead of a large hero header.
- Simplified the shared sample shell chrome again by removing the sidebar's descriptive copy and switching both pane-toggle controls to hamburger icon buttons, keeping the visual polish while making the navigation read as a cleaner docs sidebar.
- Refined `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` so the responsive shell now visually matches the richer docs sample language: the navigation pane is card-based, section/page groups are visually separated, header chrome carries badges and stronger hierarchy, and both pane/content areas use subtle accent layers instead of a flat all-slate frame.
- Reworked `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` into a responsive `SplitView` shell with one stacked vertical navigation pane for sections and pages, explicit show/hide buttons, and width-based `Inline`/`Overlay` switching so the shared sample is more mobile-friendly without giving up the existing lazy page cache.
- Followed up on a desktop previewer regression in `SampleShell` by moving the `TabStrip` `SelectionChanged` wiring out of XAML and into code-behind, and by replacing the item-template compiled bindings with reflection bindings because Avalonia's runtime preview compiler could not resolve the shared shell's string event handlers or non-public local descriptor types.
- Fixed the browser sample's post-canvas blank screen by making `SampleShell` tab-selection event handlers initialization-safe; Avalonia was firing `SelectionChanged` during XAML `EndInit` before the named `TabStrip` fields were reliably available.
- Tightened the browser splash bootstrap so it now polls per frame for the Avalonia canvas/native host and drops the splash promptly once the browser surface is ready, instead of waiting on the slower observer timeout path.
- Reworked `samples/Tailwind.Avalonia.Sample/SampleShell.axaml` from nested eager `TabControl` content into a two-level `TabStrip` plus lazy cached page host so the browser sample no longer instantiates every heavy docs page up front and revisiting tabs only toggles visibility.
- Extracted the existing sample UI out of `MainWindow` into a reusable `SampleShell` so the same docs-style surface can be hosted both as a desktop window and as a browser single-view app without duplicating sample pages.
- Converted `samples/Tailwind.Avalonia.Sample` into the shared Avalonia app assembly, then added thin `samples/Tailwind.Avalonia.Sample.Desktop` and `samples/Tailwind.Avalonia.Sample.Browser` host projects following Avalonia's official cross-platform hosting pattern.
- Added browser-specific `wwwroot` assets with relative paths plus a checked-in `.nojekyll` marker so the published WebAssembly output is compatible with GitHub Pages project-site hosting.
- Added `.github/workflows/sample-browser-pages.yml` with pinned, Node24-compatible GitHub Actions to restore the browser workload, publish the browser sample, upload `publish/wwwroot`, and deploy it to GitHub Pages.
- Added `samples/Tailwind.Avalonia.Sample.Browser/README.md` covering local `wasm-tools` prerequisites, local run/publish commands, Pages output path, and the checked-in `.nojekyll` behavior.
- Replaced the temporary `AvaloniaPropertyRegistry` property lookup attempt in `Tw.cs` with trim-aware reflected field lookup keyed by a custom annotated cache struct, because the registry path triggered a duplicate-key runtime failure in `TwTests` while the browser publish motivation was only to fix trimming analysis on the cached tuple path.
- Revalidated the desktop host build and the full test project; browser publish remains blocked only by the current Windows machine failing `dotnet workload install wasm-tools` while a pending reboot/MSI cancellation state is active.
- Added `FontSizeScale` and `FontSizeResourceDictionary`, then merged those typography sizing tokens into `Themes/Tailwind.axaml` so package consumers now get generated `FontSizeXs` through `FontSize9xl` `StaticResource` keys alongside the existing spacing, sizing, and color surfaces.
- Extended `tw:Tw.Class` so recognized `text-*` size tokens now set Avalonia `FontSize` for `text-xs` through `text-9xl` plus bracket arbitrary values like `text-[14px]`, while palette and arbitrary color tokens such as `text-sky-300` and `text-[#ff6b6b]` still fall through to the existing `Foreground` parser.
- Added focused font-size coverage in the test project for token lookup, resource generation, parser application, arbitrary units, clear behavior, and the shared `text-*` namespace disambiguation between font size and text color.
- Added `samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml` as a standalone docs-style `UserControl`, wired it into the `TYPOGRAPHY` tab, and mirrored the official Tailwind font-size docs structure with live basic/custom-value demos plus explicit unsupported notes for line-height modifiers and responsive variants.
- Expanded `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml` and `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml` so they now mirror the official Tailwind docs example headings more closely, keeping live demos for supported numeric sizing and explicit note sections for unsupported percentage, viewport, container, custom-value, size, and responsive examples.
- Extended `SpacingResourceDictionary` so numeric sizing tokens now also emit generated `Width*`, `MinWidth*`, `MaxWidth*`, `Height*`, `MinHeight*`, and `MaxHeight*` `StaticResource` keys.
- Reworked the `Width` and `Height` sample pages so their `StaticResource` tabs now show real sizing examples instead of unsupported placeholders.
- Added focused resource-dictionary coverage for the new sizing `StaticResource` keys and revalidated both the sizing tests and the sample build.
- Extended `tw:Tw.Class` with first-pass numeric sizing utilities for `w-*`, `min-w-*`, `max-w-*`, `h-*`, `min-h-*`, and `max-h-*`, all driven by the existing spacing scale.
- Added focused `TwTests` coverage for sizing utility application, last-token-wins behavior, and clearing previously applied width/height constraints.
- Added `samples/Tailwind.Avalonia.Sample/Sizing/Width.axaml` and `samples/Tailwind.Avalonia.Sample/Sizing/Height.axaml` as standalone docs-style `UserControl` pages covering width/min/max width and height/min/max height.
- Wired those new sizing pages into the sample shell under a dedicated `SIZING` tab with nested `Width` and `Height` tabs.
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
- `SampleShell` section clicks and page clicks are now separate interactions: choosing a section updates drawer context only, and only choosing a page may replace the hosted sample content.
- `SampleShell` navigation should now behave like a transient drawer, not a persistent left rail: default closed, overlay content instead of reserving width, and close again after navigation so the content canvas stays full-width by default.
- The shared `SampleShell` content header should stay compact: keep the section/page context on a single row with restrained vertical padding so the header chrome tracks the hamburger toggle height instead of presenting as a tall hero block.
- The shared `SampleShell` sidebar should stay function-first: keep structural labels like `SECTIONS` and `PAGES`, but avoid marketing/explanatory copy inside the pane and prefer compact icon-only hamburger toggles for opening and closing navigation.
- `SampleShell` should reuse the sample docs visual language instead of staying a plain transport shell: prefer grouped rounded surfaces, stronger selected-state contrast, and restrained accent color layers so navigation looks intentional on both desktop and mobile widths without inventing a second design system.
- `SampleShell` navigation should stay as a single vertical `SplitView` pane: keep the docs navigation inline on wider widths, switch to overlay on narrow widths, and auto-close the pane after page selection in narrow mode so the content area stays readable on phones and small browser viewports.
- Browser hosting uses a shared app assembly plus thin desktop/browser host projects, matching Avalonia's official pattern rather than duplicating the sample UI across two app projects.
- GitHub Pages deployment should publish the browser host's generated `publish/wwwroot` folder directly; relative asset paths plus `.nojekyll` are sufficient for project-site hosting under `/<repository-name>/`.
- Browser-oriented trim cleanup in `Tw.cs` should keep the existing reflected Avalonia `*Property` field discovery model rather than rely on `AvaloniaPropertyRegistry`, because the registry path is not stable for the current control/test surface.
- Local browser validation now depends on a successful `wasm-tools` workload install for the active .NET 10 SDK band; if the machine has a pending reboot after SDK changes, finish the reboot before retrying the workload install.
- `text-*` is now a split namespace: known font-size tokens and numeric arbitrary values claim `FontSize` first, and any remaining `text-*` token still flows to the existing text-color parser.
- Generated typography sizing keys follow the established property-prefix naming pattern: `FontSizeXs`, `FontSizeBase`, `FontSize2xl`, and so on.
- New utility families should gain generated `StaticResource` parity at the same time whenever the behavior maps honestly onto shared Avalonia resources; do not defer that parity to a later cleanup pass.
- Numeric sizing utilities now have generated `StaticResource` parity through `Width*`, `MinWidth*`, `MaxWidth*`, `Height*`, `MinHeight*`, and `MaxHeight*` keys, so sample pages should demonstrate both utility and StaticResource variants just like spacing pages do.
- When sample pages mimic official Tailwind docs, include the official example headings even when current package support is partial; supported sections should stay live, unsupported sections should be explicit note blocks instead of being silently omitted.
- First-pass sizing support is intentionally limited to spacing-scale numeric tokens for width/min-width/max-width/height/min-height/max-height; fractions, viewport keywords, container widths, `auto`, `full`, `none`, `px`, arbitrary values, and `size-*` remain out of scope for now.
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
1. Reboot the current Windows machine, install `wasm-tools`, and rerun browser publish plus an actual browser smoke test to close the remaining environment-only validation gap.
2. Decide whether `samples/Tailwind.Avalonia.Sample` should stay as the shared app assembly or whether the repo should introduce a separate shared project and restore the original sample project name to the desktop executable.
3. Enable GitHub Pages in the repository settings and verify the first `sample-browser-pages` workflow deployment end to end.
4. Return to semantic alias and dark/light theme layering once browser-hosting validation is closed.

## Open Questions
- Whether the shared-app-project architecture should remain as-is, or whether the repo should preserve `Tailwind.Avalonia.Sample` as the desktop executable and move shared app code into a new project.
- Whether browser-targeted publish checks should become part of regular CI beyond the new Pages workflow.
- Whether Tailwind's default line-height behavior for `text-*` should eventually map onto Avalonia `LineHeight`, or remain intentionally out of scope.
- Whether logical start/end spacing should gain dedicated `StaticResource` keys or remain parser-only.
- Whether spacing generation should remain checked-in C# or move to a generator pipeline.
- Whether color conversion should stay as runtime code or move to generated, precomputed sRGB values later.
- Whether a C# helper API is worth exposing in addition to the XAML-facing surface.
- Whether directional border colors should remain unsupported indefinitely under the no-custom-component constraint or be documented as intentionally out of scope.
