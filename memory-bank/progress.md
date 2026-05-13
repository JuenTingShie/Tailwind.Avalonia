# Progress

## Current Status
Spacing, color, font-size, and first-pass sizing foundations are implemented, locally validated, and now documented through a docs-style sample surface.

## What Works
- `FontSizeScale` and `FontSizeResourceDictionary` now expose Tailwind font-size tokens as generated `FontSizeXs` through `FontSize9xl` `StaticResource` keys.
- `tw:Tw.Class` now applies `text-xs` through `text-9xl` plus bracket arbitrary absolute values like `text-[14px]` onto Avalonia `FontSize`, while keeping existing `text-sky-300` / `text-[#hex]` foreground color parsing intact.
- Sample app now exposes a dedicated `Typography/Font size` docs page with Tailwind-docs-inspired basic/custom-value demos plus explicit unsupported notes for line-height modifiers and responsive variants.
- `tw:Tw.Class` now applies numeric sizing utilities for `w-*`, `min-w-*`, `max-w-*`, `h-*`, `min-h-*`, and `max-h-*` on controls that expose the corresponding Avalonia layout properties.
- Focused `TwTests` now cover sizing utility application, last-token-wins behavior, and clearing previously applied width/height constraints.
- Sample app now exposes a dedicated `SIZING` area with `Width` and `Height` docs pages under `samples/Tailwind.Avalonia.Sample/Sizing/`, each showing width/min/max width and height/min/max height examples with the same docs-style surface used by spacing pages.
- `SpacingResourceDictionary` now also emits generated sizing resource keys for numeric scale tokens: `Width*`, `MinWidth*`, `MaxWidth*`, `Height*`, `MinHeight*`, and `MaxHeight*`.
- Those sizing sample pages now show real `StaticResource` previews and AXAML snippets for width/min/max width and height/min/max height instead of fallback unsupported notes.
- Those sizing sample pages now also mirror official Tailwind example coverage more closely by keeping supported numeric examples live and surfacing unsupported official examples as explicit sections rather than omitting them.
- The spacing sample reference tables now use lightweight `ItemsControl` rows instead of `DataGrid`, so expanding the list shows every supported row without any inner scrollbar behavior.
- The spacing sample pages now keep compiled bindings enabled while still supporting the docs reference `DataGrid` sections through typed row `DataTemplate` bindings plus code-behind-managed row state.
- The `Padding` sample page now includes a top-of-page reference `DataGrid` that maps supported numeric padding utility families to AXAML `Padding="..."` equivalents.
- That reference table now mimics the Tailwind docs interaction pattern with a compact initial row set plus a working `SHOW MORE` / `CLOSE` toggle for the extra logical and visual alias rows.
- The `Padding` and `Margin` sample pages now both include explicit unsupported-row notes alongside their reference tables so omitted Tailwind syntax stays visible and honest.
- The `Margin` sample page now also includes a top-of-page reference `DataGrid` for positive, negative, and logical margin utility families mapped to AXAML `Margin="..."` equivalents.
- Solution and project scaffold exists.
- `Tailwind.Avalonia` library builds on .NET 10 with Avalonia 12.0.2.
- Spacing resources are generated through `SpacingResourceDictionary`.
- Color resources are generated through `ColorResourceDictionary` using the official Tailwind v4.2 palette reference.
- `tw:Tw.Class` applies spacing utilities for padding and margin, including logical parser support.
- `tw:Tw.Class` now also applies whole-property color utilities for `bg-*`, `text-*`, and `border-*`, including `transparent` and `/opacity`, on controls that expose the matching Avalonia brush properties.
- Sample app now presents a docs-style left-tab browser for `Padding` and `Margin`, with detailed example sections mapped to the currently supported spacing subset.
- The `Padding` and `Margin` tab content is now split into dedicated `UserControl` files under `samples/Tailwind.Avalonia.Sample/Spacing/`, keeping `MainWindow.axaml` focused on shell resources and tab wiring.
- The spacing `UserControl` files now merge sample/package resources and include a shared `SpacingDocsStyles.axaml` file locally, so they render correctly even when developed outside the `MainWindow` shell.
- Sample app now also exposes a dedicated `Colors` tab backed by `samples/Tailwind.Avalonia.Sample/Typography/ColorUtilities.axaml`, demonstrating `bg-*`, `text-*`, `border-*`, `transparent`, and `/opacity` on real controls instead of only mentioning color support inside spacing notes.
- Sample docs pages now also include `StaticResource` code variants for implemented physical spacing, negative margin, and whole-property color examples, while explicitly calling out unsupported StaticResource equivalents for logical spacing aliases and `bg-transparent`.
- Those StaticResource docs-code samples are now reformatted as clearer multi-line AXAML blocks across the spacing and color pages.
- Those multi-line docs-code samples now preserve indentation with `xml:space="preserve"` on a single `TextBlock` per snippet.
- The sample docs now use nested tabs inside each example surface so readers can switch both preview and AXAML between utility and StaticResource variants, with unsupported StaticResource cases staying explicit in their tab.
- The sample no longer uses `SampleTokens.axaml`; demo-only copy, layout values, and tab labels are inlined where they are used, while shared presentation rules stay in `SpacingDocsStyles.axaml`.
- The RTL logical padding docs preview now cancels Avalonia's visual RTL mirror transform on the showcased target, so `ps-8` and `pe-8` display the physical side selected by the real utility while still using live `tw:Tw.Class` behavior.
- The logical padding docs now distinguish between real RTL rendering and a separate visualized side-mapping preview, with an explicit teaching note that the transform-based version is for explanation only.
- `tw:Tw.Class` now also supports `psv-*` and `pev-*` as padding-only visual start/end aliases, letting sample and app code target final rendered sides without changing existing logical `ps-*` and `pe-*` semantics.
- The padding docs page now includes dedicated `psv-8` / `pev-8` previews plus actual-usage examples for both LTR and RTL.
- Sample app still visibly demonstrates `bg-*`, `text-*`, and `border-*` utility strings in XAML through the support cards embedded in the docs layout.
- `dotnet build` succeeds for the full solution.
- Sample app startup was exercised with no immediate runtime exception output.
- Sample app now consumes package `Brush*` color tokens instead of local hardcoded brush values.
- Automated tests now exist and pass for spacing scale behavior, spacing parser behavior, color parser invariants, palette coverage, and color resource emission.
- Automated tests now also cover color utility application and clearing behavior.
- Automated tests now also cover transparent and opacity parsing behavior.

## What Is Left
- Validate package/publish consumption beyond the local project-reference sample.
- Decide whether compile-time merged include support is needed after packaging.
- Expand semantic/dark-light theme layering and remaining non-border utility coverage.
- Decide whether typography should add Tailwind-style slash line-height modifiers or keep font size as a standalone first-pass utility family.
- Decide which non-numeric sizing families should be added next, while keeping the new-feature-equals-StaticResource-parity rule.
- Decide whether the combined color-utility docs page should remain a single tab or split by utility family.
- Decide whether directional border colors should be explicitly documented as unsupported under the no-custom-component constraint.

## Known Risks
- Avalonia selector syntax differs from Tailwind token syntax.
- Some utility semantics may not map 1:1 across all control types.
- Parser application currently relies on reflected `PaddingProperty` / `MarginProperty` discovery.
- Sizing utility application now also relies on reflected `WidthProperty` / `MinWidthProperty` / `MaxWidthProperty` / `HeightProperty` / `MinHeightProperty` / `MaxHeightProperty` discovery.
- Font-size utility application now also relies on reflected `FontSizeProperty` discovery, and future typography tokens sharing the `text-*` namespace will need to preserve the current size-before-color disambiguation rule.
- Color utility application currently relies on reflected `BackgroundProperty` / `ForegroundProperty` / `BorderBrushProperty` discovery.
- Tailwind directional border-color semantics still cannot be mapped onto generic Avalonia `Border` under the no-custom-component constraint.
- Logical spacing resources are not yet exposed as dedicated static keys.
- Sizing now exposes generated static keys for numeric scale tokens, but still does not cover fractions, viewport/container keywords, or arbitrary/custom values.
- Runtime `ResourceInclude` works locally, but packaged-consumer behavior still needs explicit verification.
- Color tokens currently depend on runtime OKLCH conversion; current tests cover stable invariants but do not yet assert browser-gamut-mapped hex outputs for chromatic palette colors.
- Other RTL logical docs previews, especially margin, may need the same mirror-cancel presentation pattern if the goal is to show physical side effects instead of Avalonia's mirrored chrome.

## Estimated Stage
98%: spacing, color, font-size, and first-pass sizing token foundations plus whole-property color utilities, docs-style sample coverage now including typography sizing and compiled-binding-safe reference tables, and core automated tests are in place, with semantic theme layering, package validation, and deeper typography/theme scope decisions still remaining.
