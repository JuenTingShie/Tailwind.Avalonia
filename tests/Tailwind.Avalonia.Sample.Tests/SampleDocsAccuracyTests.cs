using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Tests;

/// <summary>
/// Keeps the prose on each page honest: a class listed under "Not implemented" must really be rejected by the
/// parser, and every class in a page's utility table must really be accepted.
/// </summary>
public partial class SampleDocsAccuracyTests
{
    // Placeholder → a concrete value that is valid wherever the placeholder is used on a docs page.
    private static readonly (string Placeholder, string Value)[] Fillers =
    [
        ("[rgb(r,g,b)]", "[rgb(14,165,233)]"), ("[hsl(h,s%,l%)]", "[hsl(199,89%,48%)]"), ("[oklch(l%,c,h)]", "[oklch(68%,0.15,237)]"),
        ("[<value>]", "[12px]"), ("[<px>]", "[3px]"), ("[#<hex>]", "[#ff0080]"), ("[<time>]", "[200ms]"),
        ("[<deg>deg]", "[12deg]"), ("<number>", "4"), ("<n>", "3"), ("<color>", "sky-500"), ("<deg>", "45"),
        ("<opacity>", "50"), ("<ms>", "150"), ("<0-100>", "50"), ("<0|1|2|4|8>", "2"),
    ];

    [Theory]
    [MemberData(nameof(SamplePageTests.Pages), MemberType = typeof(SamplePageTests))]
    public void Not_Implemented_Lists_Only_Name_Unsupported_Classes(string section, string page)
    {
        var supported = new List<string>();

        SampleHeadless.Run(() =>
        {
            var view = SamplePageTests.Open(section, page);

            foreach (var note in view.GetLogicalDescendants().OfType<DocsNote>().Where(n => n.Classes.Contains("gap")))
            {
                foreach (var text in note.GetLogicalDescendants().OfType<TextBlock>().Where(t => t.Classes.Contains("docs-tableClass")))
                {
                    foreach (var token in ConcreteTokens(text.Text ?? string.Empty))
                    {
                        if (ParserProbe.IsRecognized(token))
                        {
                            supported.Add(token);
                        }
                    }
                }
            }
        });

        Assert.True(supported.Count == 0, "Listed as not implemented but the parser accepts: " + string.Join(", ", supported));
    }

    [Theory]
    [MemberData(nameof(SamplePageTests.Pages), MemberType = typeof(SamplePageTests))]
    public void Utility_Table_Classes_Are_Supported(string section, string page)
    {
        var rejected = new List<string>();

        SampleHeadless.Run(() =>
        {
            var view = SamplePageTests.Open(section, page);

            foreach (var table in view.GetLogicalDescendants().OfType<DocsUtilityTable>())
            {
                foreach (var row in table.Rows ?? [])
                {
                    var token = Fill(row.ClassName);

                    if (token is not null && !ParserProbe.IsRecognized(token))
                    {
                        rejected.Add($"{row.ClassName} (probed as {token})");
                    }
                }
            }
        });

        Assert.True(rejected.Count == 0, "Utility table rows the parser rejects: " + string.Join(", ", rejected));
    }

    internal static string? Fill(string className)
    {
        var token = className;

        foreach (var (placeholder, value) in Fillers)
        {
            token = token.Replace(placeholder, value, StringComparison.Ordinal);
        }

        // Rows that still carry a placeholder or prose are documentation, not a single probe-able class.
        return token.Contains('<') || token.Contains(' ') || token.Contains("...") || token.Contains('*') ? null : token;
    }

    internal static IEnumerable<string> ConcreteTokens(string text)
    {
        foreach (Match match in TokenPattern().Matches(text))
        {
            var token = match.Value;

            if (token.Contains("...") || token.Contains('*') || token.Contains('<') || (token.StartsWith('-') && token.IndexOf('-', 1) < 0))
            {
                continue;
            }

            yield return token;
        }
    }

    // Anything that looks like one Tailwind class: optional variant prefixes, a dashed name, optional [arbitrary] part.
    [GeneratedRegex(@"(?<![\w\[-])-?(?:[a-z0-9-]+:)*[a-z][a-z0-9.%/-]*(?:\[[^\]\s]*\])?(?:/\d+)?")]
    private static partial Regex TokenPattern();
}
