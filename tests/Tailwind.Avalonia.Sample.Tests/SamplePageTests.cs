using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Tests;

/// <summary>
/// Opens every page the sample shell registers and checks the invariants that used to drift by hand:
/// every class the page uses is understood by the parser, every example shows code, and every class a code
/// snippet shows is actually used by that example's preview.
/// </summary>
public partial class SamplePageTests
{
    public static TheoryData<string, string> Pages()
    {
        var data = new TheoryData<string, string>();

        foreach (var section in SampleShell.CreateSections())
        {
            foreach (var page in section.Pages)
            {
                data.Add(section.Header, page.Header);
            }
        }

        return data;
    }

    internal static Control Open(string section, string page)
    {
        var descriptor = SampleShell.CreateSections().Single(s => s.Header == section).Pages.Single(p => p.Header == page);
        var view = descriptor.CreateView();
        var window = new Window { Width = 1280, Height = 900, Content = view };
        window.Show();
        window.UpdateLayout();
        return view;
    }

    [Theory]
    [MemberData(nameof(Pages))]
    public void Page_Uses_Only_Recognized_Utilities(string section, string page)
    {
        IReadOnlyList<string> warnings = [];

        SampleHeadless.Run(() =>
        {
            SampleHeadless.Warnings.Clear();
            Open(section, page);
            warnings = SampleHeadless.Warnings.Messages.ToArray();
        });

        Assert.True(warnings.Count == 0, string.Join(Environment.NewLine, warnings));
    }

    [Theory]
    [MemberData(nameof(Pages))]
    public void Every_Example_Shows_Code(string section, string page)
    {
        var missing = new List<string>();

        SampleHeadless.Run(() =>
        {
            var view = Open(section, page);
            var index = 0;

            foreach (var example in view.GetLogicalDescendants().OfType<DocsExample>())
            {
                index++;

                if (!example.HasCode)
                {
                    missing.Add($"example #{index}");
                }
            }
        });

        Assert.True(missing.Count == 0, string.Join(Environment.NewLine, missing));
    }

    [Theory]
    [MemberData(nameof(Pages))]
    public void Snippet_Classes_Appear_In_The_Preview(string section, string page)
    {
        var problems = new List<string>();

        SampleHeadless.Run(() =>
        {
            var view = Open(section, page);
            var index = 0;

            foreach (var example in view.GetLogicalDescendants().OfType<DocsExample>())
            {
                index++;
                var used = PreviewTokens(example);

                foreach (var snippet in example.Snippets)
                {
                    foreach (Match match in ClassAttribute().Matches(snippet))
                    {
                        foreach (var token in match.Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!used.Contains(token))
                            {
                                problems.Add($"example #{index}: '{token}' is in the code but not in the preview");
                            }
                        }
                    }
                }
            }
        });

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }

    internal static HashSet<string> PreviewTokens(DocsExample example)
    {
        var tokens = new HashSet<string>(StringComparer.Ordinal);

        if (example.Content is not ILogical root)
        {
            return tokens;
        }

        foreach (var node in root.GetSelfAndLogicalDescendants())
        {
            if (node is global::Avalonia.AvaloniaObject element && Tw.GetClass(element) is { } classes)
            {
                tokens.UnionWith(classes.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        return tokens;
    }

    [GeneratedRegex("tw:Tw\\.Class=\"([^\"]*)\"")]
    private static partial Regex ClassAttribute();
}

public class SamplePageQualityTests
{
    // When one element in a row of demos prints its code, its siblings that carry classes are demos too.
    [Theory]
    [MemberData(nameof(SamplePageTests.Pages), MemberType = typeof(SamplePageTests))]
    public void Siblings_Of_A_Shown_Element_Are_Shown_Too(string section, string page)
    {
        var problems = new List<string>();

        SampleHeadless.Run(() =>
        {
            var view = SamplePageTests.Open(section, page);

            foreach (var example in view.GetLogicalDescendants().OfType<DocsExample>())
            {
                if (example.Content is not ILogical content)
                {
                    continue;
                }

                var shown = content.GetSelfAndLogicalDescendants().OfType<global::Avalonia.AvaloniaObject>().Where(DocsCode.GetShow).ToList();

                foreach (var parent in shown.Select(e => ((ILogical)e).LogicalParent).Distinct())
                {
                    if (parent is null)
                    {
                        continue;
                    }

                    var type = shown.First(e => ((ILogical)e).LogicalParent == parent).GetType();

                    foreach (var sibling in parent.LogicalChildren.OfType<global::Avalonia.AvaloniaObject>())
                    {
                        if (sibling.GetType() == type && !DocsCode.GetShow(sibling) && Tw.GetClass(sibling) is { Length: > 0 } classes
                            && !shown.Any(e => Tw.GetClass(e) == classes)
                            && !(sibling is StyledElement styled && styled.Classes.Any(c => c.StartsWith("docs-", StringComparison.Ordinal))))
                        {
                            problems.Add($"{type.Name} [{classes}] sits next to shown demos but has no code");
                        }
                    }
                }
            }
        });

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems));
    }
}
