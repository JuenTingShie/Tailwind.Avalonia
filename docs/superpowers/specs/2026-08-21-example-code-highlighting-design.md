# Improve Example Code Highlighting — Design Spec

**Date:** 2026-08-21
**Status:** Approved

## Problem

`AxamlCodeBlock` (added in commit `c29b3b2`) renders docs AXAML snippets in a read-only `AvaloniaEdit.TextEditor`, using `HighlightingManager.Instance.GetDefinitionByExtension(".xml")` for syntax coloring. That built-in definition assumes a light background (blue tags, maroon attribute names, blue attribute values), but every docs page renders it inside a fixed-dark surface (`Border.docs-surface` = `bg-slate-900`/`border-slate-800`, white/slate text — the sample app has no light variant). The result is a color scheme mismatched to its container. Additionally, the highlighting is generic XML: it has no notion of Avalonia markup extensions (`{Binding ...}`) or of `tw:Tw.Class="..."` utility-class strings, which render as one plain attribute-value string. The block also lacks common docs-site affordances (line numbers, copy-to-clipboard).

## Decisions

1. **Replace stock `.xml` highlighting with `AvaloniaEdit.TextMate` + a bundled dark theme.** Add `AvaloniaEdit.TextMate` and `TextMateSharp.Grammars` (`>= 2.0.3` — `AvaloniaEdit.TextMate 12.0.0` requires it; a lower pin causes an `NU1605` downgrade error) package references to `Tailwind.Avalonia.Sample.csproj`. Use `RegistryOptions(ThemeName.DarkPlus)` and the bundled `.xml` grammar as the baseline tokenizer/theme.
   - **WASM feasibility confirmed by spike:** `onigwrap` (the Oniguruma regex engine backing `TextMateSharp`) ships a WASM-native static lib (`libonigwrap.a`) that links cleanly via emscripten. Both `Tailwind.Avalonia.Sample.Desktop` and `Tailwind.Avalonia.Sample.Browser` build with 0 errors/warnings with TextMate wired in. No native-dependency blocker for the Browser target.
2. **Shared, not per-instance, grammar/theme.** A new static `AxamlHighlighting` helper in `samples/Tailwind.Avalonia.Sample/` owns one shared `RegistryOptions` instance and one shared `AxamlHighlightColorizer` instance, built once on first use (not once per `AxamlCodeBlock`, since docs pages host many code blocks). It exposes an `Install(TextEditor editor)` method that installs TextMate on the given editor and attaches the shared colorizer to `editor.TextArea.TextView.LineTransformers`.
3. **Custom colorizer overlay for AXAML/Tailwind-specific tokens, not a custom grammar.** A new `AxamlHighlightColorizer : DocumentColorizingTransformer` (pure C#, regex-based) runs on top of the TextMate baseline coloring, per visible line, and:
   - Recolors `{...}` markup-extension delimiters/contents (e.g. `{Binding Foo}`, `{StaticResource Bar}`) in `violet-300` — matching the existing `TextBlock.docs-tableStyle` semantic (property/expression-like values) in `SpacingDocsStyles.axaml`.
   - Inside `tw:Tw.Class="..."` attribute values specifically, recolors each space-separated utility-class token individually in `sky-400` — matching the existing `TextBlock.docs-tableClass` semantic (class-name-like tokens) in the same file.
   - This was chosen over hand-authoring a custom `.tmLanguage.json` AXAML grammar + patched theme JSON (rejected: more code — balanced-brace grammar rules, scope-name/theme-token plumbing — for the same visible result, and an ongoing grammar-maintenance surface).
4. **Colorizer colors come from the library's own palette, not hand-converted hex.** `TailwindColorPalette` (in `src/Tailwind.Avalonia/Colors/TailwindColorPalette.cs`) stores its source colors as OKLCH values and is `internal`. Add `Tailwind.Avalonia.Sample` to the existing `InternalsVisibleTo` list in `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj` (which currently only lists `Tailwind.Avalonia.Tests`), so the colorizer can call `TailwindColorPalette.TryGetColor("sky-400", out var color)` / `TryGetColor("violet-300", out var color)` directly — the exact same resolved color already used elsewhere in the docs UI (e.g. `TextBlock.docs-tableClass` / `TextBlock.docs-tableStyle` in `SpacingDocsStyles.axaml`), rather than a hand-converted approximation. No public API of `Tailwind.Avalonia` changes.
5. **Editor affordances.** In `AxamlCodeBlock.axaml`: set `ShowLineNumbers="True"` (currently `False`). Add a small copy button overlaid top-right on the editor; its `Click` handler calls `TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(Editor.Text)`, guarded with a null check (no-op, no throw, if `Clipboard` is unavailable on a given platform), with a brief label swap (e.g. "Copy" → "Copied") as feedback — no timer/animation library, just a simple state flip reverted after a short delay.

## Data flow

`Code` (dependency property) is set → `AxamlSnippetFormatter.Format` (unchanged, existing XML reindent logic) produces the display string → `Editor.Text` is set → AvaloniaEdit's TextMate installation tokenizes on render, applying baseline DarkPlus XML coloring → `AxamlHighlightColorizer` runs per visible line during the same rendering pass, overlaying markup-extension and `tw:Tw.Class` token spans → line numbers render natively via AvaloniaEdit → the copy button reads `Editor.Text` directly on click.

## Error handling

- Clipboard: null-guarded, silent no-op on failure/unavailability — not a condition worth surfacing to the user in a docs sample.
- Colorizer regexes run over arbitrary (already XML-well-formed, since `AxamlSnippetFormatter` guarantees that) formatted AXAML text; unmatched/malformed spans (e.g. an unclosed brace, which shouldn't occur post-formatting) simply fall through to the baseline TextMate color — no exception path.
- Grammar/theme asset load failure (e.g. missing embedded resource in `TextMateSharp.Grammars`) is a startup-time/build-configuration issue, not a runtime user-data issue — allowed to throw and surface immediately rather than being swallowed.

## Out of scope

- A custom `.tmLanguage.json` AXAML grammar (rejected approach, see Decision 3).
- Light-theme support for the docs sample (the app has no light variant today; not introduced by this change).
- Automated test coverage for the sample app (see Verification below — matches existing repo convention).

## Files touched

- **Edit:** `samples/Tailwind.Avalonia.Sample/Tailwind.Avalonia.Sample.csproj` (add `AvaloniaEdit.TextMate`, `TextMateSharp.Grammars >= 2.0.3` package references), `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml.cs` (install TextMate + colorizer via `AxamlHighlighting.Install`), `samples/Tailwind.Avalonia.Sample/AxamlCodeBlock.axaml` (`ShowLineNumbers="True"`, copy button), `src/Tailwind.Avalonia/Tailwind.Avalonia.csproj` (add `Tailwind.Avalonia.Sample` to `InternalsVisibleTo`).
- **Create:** `samples/Tailwind.Avalonia.Sample/AxamlHighlighting.cs` (shared `RegistryOptions`/install helper), `samples/Tailwind.Avalonia.Sample/AxamlHighlightColorizer.cs` (the `DocumentColorizingTransformer`).

## Verification

No automated test project covers `samples/` today — `tests/Tailwind.Avalonia.Tests` only references `src/Tailwind.Avalonia` (the library), and the sample app is verified by running it, per existing repo convention (see `2026-08-19-drop-staticresource-utilities-design.md`'s Verification section for precedent). This change follows the same pattern:

- `dotnet build` on both `Tailwind.Avalonia.Sample.Desktop.csproj` and `Tailwind.Avalonia.Sample.Browser.csproj` must succeed with 0 errors (already confirmed feasible for the TextMate package combination by the pre-design spike).
- Manual visual pass in the running Desktop sample across every docs page that hosts an `AxamlCodeBlock` (Spacing/Margin, Spacing/Padding, Sizing/Width, Sizing/Height, Interactivity/PseudoClassVariants, Typography/ColorUtilities, Typography/FontSize): confirm tag/attribute/comment coloring reads correctly against the dark surface, `tw:Tw.Class` values and `{Binding ...}`-style markup extensions are visibly distinguished from plain attribute text, line numbers are legible, and the copy button places the exact formatted snippet text on the clipboard.
