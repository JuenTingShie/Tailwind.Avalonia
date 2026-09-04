# Sample page review fixes

## Context

A full review of the nine sample docs pages in `samples/Tailwind.Avalonia.Sample`
found factual errors (pages claiming support the parser does not have, and pages
denying support it does have), one library bug that makes a shipped feature
unreachable, one missing sample page for an implemented utility, and a set of
naming/structure inconsistencies. This plan fixes all of them.

Every claim below was verified empirically by driving the real parser
(`Tw.SetClass`) and reading the resulting Avalonia properties. Verified results:

```
p-px                  => Padding=1,1,1,1        (SUPPORTED)
w-px                  => Width=1                (SUPPORTED)
border-t              => BorderThickness=0,1,0,0 (SUPPORTED)
bg-[#f00]             => Red                    (SUPPORTED, 3-digit shorthand)
w-[50%]               => unset                  (NOT supported)
bg-[rgb(255,0,0)]     => null                   (NOT reachable — library bug)
bg-[hsl(0,100%,50%)]  => null                   (NOT reachable — library bug)
bg-[oklch(50%,0.1,20)] => null                  (NOT reachable — library bug)
```

## Global Constraints

- Do not change the public API surface of `Tw` beyond what Task 1 specifies.
- Sample pages must never claim support the parser does not have, and must never
  list a token as unimplemented when the parser implements it.
- Page prose is sentence case ("Border radius"), not Title Case.
- `tw:Tw.Class` replaces the whole token string when overridden by a style; any
  override must restate every token it still wants.
- The build must stay at 0 warnings / 0 errors.
- The xunit suite must stay green. Run it with
  `tests/Tailwind.Avalonia.Tests/bin/Debug/net10.0/Tailwind.Avalonia.Tests.exe`
  (until Task 2 lands, `dotnet test` reports zero tests).
- Do not reformat files wholesale. Keep diffs limited to the lines each task names.

---

## Task 1: Make CSS color functions reachable from `Tw.Class`

**Problem.** `src/Tailwind.Avalonia/Tw/Tw.ColorParsing.cs:124-139` parses
`rgb()`, `rgba()`, `hsl()`, `hsla()` and `oklch()` inside bracket arbitrary
values, and `TailwindCssColorParser` has full unit-test coverage. But the only
path into that code is `TryParseBrushUtility`, and
`src/Tailwind.Avalonia/Tw/Tw.Parsing.cs:62-67` rejects any token containing
`(` before it ever gets there:

```csharp
if (token.StartsWith("-", StringComparison.Ordinal) ||
    token.Contains(':') ||
    token.Contains('('))
{
    return false;
}
```

So `bg-[rgb(255,0,0)]` returns null. The feature is dead code from `Tw.Class`.

**Why the guard exists.** It rejects Tailwind's custom-property shorthand
(`bg-(<custom-property>)`), which this library deliberately does not support.
That rejection must be preserved.

**Required change.** In `TryParseBrushUtility` only, replace the blanket `(`
rejection with one that still rejects a `(` that is NOT inside a bracket
arbitrary value. A token qualifies as a bracket arbitrary value when, after the
utility prefix, it starts with `[`. Concretely: keep rejecting `bg-(...)`, start
accepting `bg-[rgb(...)]`.

Do not touch the `(` guards in `TryParseSpacingUtility`,
`TryParseSizingUtility`, `TryParseBorderWidthUtility`,
`TryParseCornerRadiusUtility`, `TryParseFontSizeUtility`, or
`TryParseOpacityUtility` — none of those families parse CSS functions.

**Whitespace caveat.** `ApplyUtilities` splits the class list on whitespace, so
only space-free function syntax can ever survive tokenization:
`bg-[rgb(255,0,0)]` works, `bg-[rgb(255, 0, 0)]` cannot. Do not try to fix that
in this task — the tokenizer contract is out of scope. Record the caveat in a
code comment next to the changed guard so the next reader does not re-derive it.

**Second required change — `oklch()` separators (controller ruling, pre-flight).**
Fixing the guard alone leaves `oklch()` still unreachable. Verified by probe:

```
TailwindCssColorParser.Parse("oklch(0% 0 0)")  => Black
TailwindCssColorParser.Parse("oklch(0%,0,0)")  => THROWS FormatException
```

`ParseOklch` in `src/Tailwind.Avalonia/Colors/TailwindCssColorParser.cs` splits
its components on whitespace only, while `SplitFunctionComponents` (used by
`rgb()`/`hsl()`) accepts commas OR whitespace. Because the class-list tokenizer
destroys any token containing a space, the whitespace-only form can never reach
the parser, so `oklch()` would remain dead code after the guard fix.

Change `ParseOklch`'s component split to accept commas the same way
`SplitFunctionComponents` does — comma-split when a comma is present, otherwise
whitespace-split. Do not otherwise alter its validation, its alpha handling, or
its existing error messages; `TailwindCssColorParserTests.cs` covers those and
must stay green.

Note in a code comment that comma-separated `oklch()` is a deliberate deviation
from the CSS spec (which is space-separated only), forced by the class-list
tokenizer.

**Not in scope.** CSS-native inner alpha (`bg-[rgb(255,0,0/0.5)]`) does not work
and is not being fixed: `TryResolveUtilityColor` splits on the first `/` when it
precedes the closing bracket, which severs the token. The supported way to set
alpha stays the post-bracket modifier, `bg-[rgb(255,0,0)]/50`. Do not attempt a
fix; do not add a test asserting the inner form works.

**Tests.** Add to `tests/Tailwind.Avalonia.Tests/TwArbitraryValuesTests.cs`,
following the existing style in that file:

- `bg-[rgb(255,0,0)]` sets Background to opaque red
- `bg-[hsl(0,100%,50%)]` sets Background to opaque red
- `bg-[oklch(0%,0,0)]` sets Background to opaque black
- `text-[rgb(0,0,255)]` sets Foreground to opaque blue
- `border-[rgb(0,255,0)]` sets BorderBrush to opaque green
- `bg-[rgb(255,0,0)]/50` applies the opacity modifier on top of the function color
- `bg-(--my-color)` is still rejected (Background stays null)
- `bg-[rgb(255, 0, 0)]` (with spaces) does not apply — documents the tokenizer caveat

Add to `tests/Tailwind.Avalonia.Tests/TailwindCssColorParserTests.cs`:

- `oklch(0%,0,0)` parses to the same color as `oklch(0% 0 0)`
- the existing space-separated oklch cases still pass unchanged

**Verification.** Build clean, then run the test exe; all prior tests plus the
new ones pass.

---

## Task 2: Make `dotnet test` actually run the tests — VOID, DO NOT IMPLEMENT

> **This task was withdrawn during execution. There was no bug.**
>
> Verified from a clean `obj/`+`bin/` against the untouched csproj:
> `dotnet test Tailwind.Avalonia.slnx -c Debug` discovers and passes all 161
> tests and exits 0. The VSTest-era packages and the MTP runner coexist fine.
>
> The original "zero tests executed / exit code 5" observation was an artifact of
> passing `--nologo`, which MTP's `dotnet test` rejects as an invalid argument and
> reports in a way that reads like an empty test run. The correct invocation is
> `dotnet test Tailwind.Avalonia.slnx -c Debug` with no `--nologo`.
>
> An implementation of this task was committed (93137bd) and then reverted
> (7a1c30d) because it removed `coverlet.collector` and two other packages on the
> strength of the false premise. The section below is retained only so the
> reverted commit has a readable rationale; treat it as historical.

**Problem (as originally and incorrectly diagnosed).** `dotnet test` reports
"zero tests executed" with exit code 5, while running
`Tailwind.Avalonia.Tests.exe` directly runs 152 tests, all passing. CI using
`dotnet test` is silently testing nothing.

**Cause.** `global.json` selects the Microsoft.Testing.Platform runner:

```json
{ "test": { "runner": "Microsoft.Testing.Platform" } }
```

but `tests/Tailwind.Avalonia.Tests/Tailwind.Avalonia.Tests.csproj` still carries
the VSTest-era package pair `Microsoft.NET.Test.Sdk` and
`xunit.runner.visualstudio` alongside `xunit.v3`. The two paths conflict and
discovery yields nothing.

**Required change.** Reconcile the test project with the MTP runner so
`dotnet test` discovers and runs the suite. xunit.v3 ships its own MTP entry
point, so the VSTest bridge packages are what has to go. Remove
`xunit.runner.visualstudio`, and remove or replace `Microsoft.NET.Test.Sdk`
according to what xunit.v3 4.0.0 + MTP actually requires. Add
`<UseMicrosoftTestingPlatformRunner>` / `<OutputType>` / `<IsTestProject>`
properties only if they are genuinely needed — verify, do not cargo-cult.

Keep `coverlet.collector` only if it still works under MTP; if it does not, drop
it and say so in the report.

**Verification.** `dotnet test Tailwind.Avalonia.slnx` must report 152 passing
tests (or 152 + whatever Task 1 added, if Task 1 landed first) and exit 0. Also
confirm `dotnet build` stays at 0 warnings / 0 errors.

---

## Task 3: Fix factually wrong claims across the sample pages

Six pages state things the parser does not do, and three deny things it does.
All of these are prose/table edits inside `samples/Tailwind.Avalonia.Sample`.

**3a. Remove the false `%` support claim (6 files).**
`Tw.Parsing.cs:340-345` maps only `px`, `""`, `rem`, `em`; every other unit
(including `%`) returns null and the whole token is dropped. `w-[50%]` sets
nothing — existing test `TwArbitraryValuesTests.cs:161` already asserts this.

Fix these lines so the unit list reads `px · rem · em · unitless` and the
explanatory line says percentages are unsupported. Use
`Typography/FontSize.axaml:99` and `:108` as the correct model to copy:

- `Sizing/Width.axaml:193` (unit list) and `:205` (explanatory line)
- `Sizing/Height.axaml:200` and `:212`
- `Spacing/Padding.axaml:836` and `:848`
- `Spacing/Margin.axaml:911` (keep `· negative prefix`) and `:923`
- `Borders/Radius.axaml:273` (unit list only; no explanatory line to fix)
- `Borders/Width.axaml:273` (unit list only)

**3b. Stop listing supported `*-px` tokens as unimplemented (3 files).**
`SpacingScale.cs:10` contains `("px", 1.0)`, so every `*-px` token below is
supported and verified working.

- `Spacing/Padding.axaml:27` — delete the whole `p-px · px-px · py-px · pt-px ·
  pr-px · pb-px · pl-px · ps-px · pe-px · pbs-px · pbe-px` line from the
  "Not implemented yet" note. (`Spacing/Margin.axaml` already gets this right —
  it does not list `m-px`. Match it.)
- `Sizing/Width.axaml:311` — remove `w-px` from the "keywords and resets" gap
  row, leaving the genuinely-unsupported entries.
- `Sizing/Height.axaml:321` — remove `h-px`, `min-h-px` and `max-h-px` from that
  gap row, leaving `h-auto`, `min-h-full`, `max-h-none`, `max-h-full`.

**3c. Fix the unrunnable example in the radius summary.**
`Borders/Radius.axaml:264` says "Bracket syntax like `rounded-[2vw]`", but `vw`
is not a supported unit, so that token is dropped. The demo underneath actually
uses `rounded-[6px]`. Change the summary to name `rounded-[6px]`.

**3d. Disambiguate the border-width variant gap note.**
`Borders/Width.axaml:29` lists `hover:border-*` and `focus:border-*` as not
implemented. That is true for border *width* but flatly contradicts
`Interactivity/PseudoClassVariants.axaml:82`, which demos a working
`focus:border-sky-400`. Reword to make the width scope explicit (e.g.
`hover:border-<number>` / `focus:border-<number>`) and note that the border
*color* utilities do accept variants.

**3e. Add the missing bare-prefix rows to the border-width table.**
`Tw.Parsing.cs:137` gives every descriptor a bare 1px form, verified:
`border-t` produces `BorderThickness=0,1,0,0`. But
`Borders/Width.axaml.cs` only lists bare `border`. Add rows for the bare
`border-t`, `border-r`, `border-b`, `border-l`, `border-x`, `border-y`,
`border-s`, `border-e`, `border-bs`, `border-be` forms, each mapping to a 1px
thickness on its edge, placed next to the matching `-<number>` row. Follow the
existing `UtilityReferenceRow` string style in that file exactly.

**3f. Document shorthand hex on the colors page.**
`Tw.ColorParsing.cs:156-165` expands `#rgb` and `#rgba`, verified: `bg-[#f00]`
produces red, and tests at `TwArbitraryValuesTests.cs:492` and `:537` cover it.
`Typography/ColorUtilities.axaml:166` only lists `#rrggbb · #rrggbbaa`. Add the
3- and 4-digit shorthand forms to that list.

**Do not** touch `SampleShell.axaml.cs`, page `Title=` attributes, or add any
overview banner in this task — Tasks 4 and 5 own those.

**Verification.** Build clean. Cross-check each edited claim against the source
file cited beside it.

---

## Task 4: Add the missing Opacity sample page

**Problem.** `README.md:155` marks `opacity` as implemented, and
`Tw.Parsing.cs:258` implements `opacity-<0-100>`, but no sample page documents
it. It appears only incidentally on the pseudo-class variants page. It is the
only implemented utility family with no page of its own.

**Required change.** Add a new section `Effects` with a single page `Opacity`,
built the same way every other page is built.

Create `samples/Tailwind.Avalonia.Sample/Effects/Opacity.axaml` and
`Opacity.axaml.cs`, namespace `Tailwind.Avalonia.Sample.Effects`, class
`Opacity`. Model the structure directly on
`samples/Tailwind.Avalonia.Sample/Typography/FontSize.axaml` and its code-behind:

- `docs:DocsPage` with `Section="Effects"`, `Title="Opacity"`, and an `Intro`
  that says opacity maps to Avalonia's `Opacity` property.
- An overview `StackPanel Classes="docs-overviewStack"` containing a
  `docs:DocsNote Classes="supported"` titled "Implemented in this sample", plus
  a `docs:DocsUtilityTable x:Name="UtilityTable"` seeded from code-behind.
- A `docs:DocsNote Classes="gap"` naming what is not supported.
- An `Examples` block using `docs:DocsSection` + `docs:DocsExample` with live
  previews carrying real `tw:Tw.Class` tokens.

**Exact parser behaviour to document — verify each before writing it down.**
`TryParseOpacityUtility` delegates to `TryParseOpacity`
(`Tw.ColorParsing.cs:71-87`), which requires an integer 0-100 inclusive. So:

- `opacity-0` through `opacity-100` work; the value is `percent / 100`.
- Non-integers are rejected: `opacity-52.5` does not apply.
- Out-of-range values are rejected: `opacity-150` does not apply.
- There is no bracket arbitrary form — `opacity-[0.5]` does not parse.
- `hover:`, `pressed:` and `focus:` variants DO work on `opacity-*`
  (`Tw.Apply.cs:80`), unlike every non-color family.

Confirm each of these against the source before documenting it; do not assume.

**Sections to include:** a basic example stepping through several opacity
values on colored `Border`s; a section showing the variant prefixes working
(cross-reference the Interactivity page); and a gap note covering the
unsupported forms listed above.

**Register the page.** In `samples/Tailwind.Avalonia.Sample/SampleShell.axaml.cs`,
add to the `CreateSections()` array (`:63-89`):

```csharp
new(
    "Effects",
    new SampleShellPageDescriptor("Opacity", static () => new Effects.Opacity())),
```

Place it after the `Interactivity` section. Section headers there are sentence
case — match that.

**Verification.** Build clean; the page must compile and the new section must
appear in the nav array.

---

## Task 5: Consistency pass across all pages

Purely cosmetic/structural alignment. No factual claims change here.

**5a/5b. Align page titles with nav labels (single combined change).**

> Controller ruling, pre-flight: the plan originally had 5a retitle to
> "Border radius"/"Border width" and 5b then supersede it with "Radius"/"Width".
> That contradiction is resolved here in favour of 5b. Make ONE edit per file to
> the final value below; do not apply 5a's intermediate wording first.

In `SampleShell.axaml.cs` `CreateSections()`, the breadcrumb shows the nav label
while the page shows its own title, and three pairs disagree:
- Typography nav `"Colors"` vs page title `"Color utilities"`
- Borders nav `"Radius"` vs page title `"Border Radius"`
- Borders nav `"Width"` vs page title `"Border Width"`

The nav rows already sit under a section header, so repeating the section word in
the page title is redundant, and Title Case violates the sentence-case convention
documented at `SampleShell.axaml.cs:67-68`. Keep the nav labels exactly as they
are and change the page titles to match them:

- `Borders/Radius.axaml:9` — `Title="Border Radius"` → `Title="Radius"`
- `Borders/Width.axaml:9` — `Title="Border Width"` → `Title="Width"`
- `Typography/ColorUtilities.axaml:9` — `Title="Color utilities"` → `Title="Colors"`

The `Section=` attribute on each page already carries the family name
("Borders", "Typography"), so no information is lost. Update each page's `Intro`
only if it reads awkwardly after the retitle. Do not change any nav label.

**5c. Add the missing "Implemented in this sample" overview banner.**
Only `FontSize`, `ColorUtilities` and `PseudoClassVariants` carry the
`docs:DocsNote Classes="supported"` capability banner at the top. Add an
equivalent banner as the first child of the `docs-overviewStack` on:
- `Sizing/Width.axaml`
- `Sizing/Height.axaml`
- `Borders/Radius.axaml`
- `Borders/Width.axaml`
- `Spacing/Padding.axaml`
- `Spacing/Margin.axaml`

Each banner's body must accurately summarise what that page's family supports.
Derive the wording from the page's own utility table and the parser source; do
not invent capabilities. Copy the markup shape from
`Typography/FontSize.axaml:15-18`.

**5d. Move the Sizing gap notes to match every other page.**
`Sizing/Width.axaml` and `Sizing/Height.axaml` put their "Not implemented yet"
content at the very end as a trailing `docs:DocsSection`, while Spacing,
Borders and Typography put the gap note inside the top `docs-overviewStack`.
Move the Sizing gap content into the overview stack as a
`docs:DocsNote Classes="gap"`, preserving all of its grouped sub-lists verbatim,
and delete the now-empty trailing section.

**Verification.** Build clean. Launch is not required, but confirm no page's
`x:Class`, `x:Name` or style class references were disturbed.

---

## Task 6: Document CSS color functions on the colors page

**Depends on Task 1.** Only do this once Task 1 has landed and its tests pass —
before Task 1, the feature genuinely does not work and documenting it would
introduce exactly the class of error this plan exists to remove.

**Required change.** In `Typography/ColorUtilities.axaml`, extend the
"Arbitrary color values" section:

- Add the supported CSS function forms to the resolution note near `:166`.
- Fix the line at `:172` that currently says "Only bracket syntax is parsed" so
  it stays true: bracket syntax is still the only wrapper, but the bracket may
  now contain a CSS color function as well as a hex literal. The
  `bg-(<custom-property>)` forms remain unsupported and must stay listed as such.
- State the whitespace caveat from Task 1 explicitly: the class list is split on
  whitespace, so `bg-[rgb(255,0,0)]` works and `bg-[rgb(255, 0, 0)]` does not.
- Add a live `docs:DocsExample` with real working tokens, plus matching snippet
  strings, in the same style as the existing arbitrary-hex example.

Also add the corresponding rows to the `UtilityRows` array in
`Typography/ColorUtilities.axaml.cs`.

**Verification.** Build clean. Every token shown on the page must actually
render — verify by driving `Tw.SetClass` in a scratch test, not by eye.
