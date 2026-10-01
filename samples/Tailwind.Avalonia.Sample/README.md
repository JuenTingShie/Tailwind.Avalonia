# Tailwind.Avalonia sample

The sample is a docs site: one page per utility family, grouped into sections. It runs on desktop
(`Tailwind.Avalonia.Sample.Desktop`) and in the browser (`Tailwind.Avalonia.Sample.Browser`).

The side pane lists every page under its section heading. Type in the search box (or press
Ctrl+K / Cmd+K) to filter by page or section name; Enter opens the first match and Esc clears the
search. Each page ends with previous / next links in catalog order. The browser build bundles the
Inter font so text looks the same as on desktop.

## Adding a page

1. Create `<Section>/<Name>.axaml` (+ a code-behind that only calls `InitializeComponent()`).
2. Add one line to `SampleCatalog.cs`: `Page("Section", "Title", static () => new Section.Name()),`
   Keep a section's pages together; the list order is the navigation order.

A page declares data and examples; `DocsPage` builds the shared layout around them:

```xml
<docs:DocsPage
    Section="Layout"
    Title="Z-index"
    Intro="One paragraph on what the family does."
    Supported="What this library implements, and how it maps to Avalonia.">
    <docs:DocsPage.Utilities>
        <docs:UtilityReferenceRow ClassName="z-&lt;number&gt;" AxamlStyle="ZIndex: &lt;number&gt;" />
    </docs:DocsPage.Utilities>
    <docs:DocsPage.Gaps>
        <docs:DocsGap Text="-z-10 · hover:z-10" />
    </docs:DocsPage.Gaps>

    <docs:DocsSection Heading="Stacking" Summary="What the example shows.">
        <docs:DocsExample>
            <Border docs:DocsCode.Show="True" tw:Tw.Class="z-30 bg-red-500 w-24 h-16" />
        </docs:DocsExample>
    </docs:DocsSection>
</docs:DocsPage>
```

- **Code is generated.** Mark each preview element the reader should copy with `docs:DocsCode.Show="True"`;
  its snippet is built from its real `tw:Tw.Class` (add `docs:DocsCode.Attributes="Width,FlowDirection"` to
  print plain properties too). Hand-written `<docs:DocsExample.Snippets>` are only for code that has no
  preview element, such as an RTL "actual usage" note.
- **Gaps** list Tailwind forms the parser rejects, separated by ` · `; short prose goes in parentheses.
  Use `Title` on a `DocsGap` to group lines.
- **Diagram helpers** in `Docs/DocsSpecimens.cs`: `DocsChip` (centered label), `PaddingSpecimen`,
  `MarginSpecimen`, `DirectionRow` (start/end labels) and `DirectionCompare` (LTR next to RTL).

## Responsive layout

`SampleLayout` holds the breakpoints. Below 960 px wide (or 640 px high) pages get the `docs-mobile` class,
which `Docs/DocsStyles.axaml` uses for compact spacing. Example previews scroll sideways when they are wider
than the column.

## Tests

`tests/Tailwind.Avalonia.Sample.Tests` opens every page headlessly and fails when:

- a class on a page is not understood by the parser;
- an example has no code, or its code names a class the preview does not use;
- a `Gaps` entry is actually supported, or a utility-table class is rejected;
- a page is missing from `SampleCatalog` or registered twice;
- at 420 px wide, content overflows the column without a horizontal scroller.

```sh
dotnet test --project tests/Tailwind.Avalonia.Sample.Tests -c Release
```
