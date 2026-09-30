using System.Linq;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Metadata;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// Scaffolds a docs page: scrolling ground, constrained column, the section / title / lede header, then the
/// overview every page shares (what is implemented, the utility table, what is not implemented yet) and the
/// examples. A page only declares data and its <see cref="DocsSection"/> children; the layout lives here.
/// </summary>
public class DocsPage : TemplatedControl
{
    public const string DefaultSupportedTitle = "Implemented in this sample";
    public const string DefaultGapsTitle = "Not implemented yet";
    public const string DefaultGapsBody = "These forms sit outside the current parser surface.";

    public static readonly StyledProperty<string?> SectionProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(Section));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(Title));

    public static readonly StyledProperty<string?> IntroProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(Intro));

    public static readonly StyledProperty<string?> SupportedProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(Supported));

    public static readonly StyledProperty<string> SupportedTitleProperty =
        AvaloniaProperty.Register<DocsPage, string>(nameof(SupportedTitle), DefaultSupportedTitle);

    public static readonly StyledProperty<int> CollapsedRowCountProperty =
        AvaloniaProperty.Register<DocsPage, int>(nameof(CollapsedRowCount), 6);

    public static readonly StyledProperty<string> GapsTitleProperty =
        AvaloniaProperty.Register<DocsPage, string>(nameof(GapsTitle), DefaultGapsTitle);

    public static readonly StyledProperty<string?> GapsBodyProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(GapsBody), DefaultGapsBody);

    public static readonly DirectProperty<DocsPage, Control?> BodyProperty =
        AvaloniaProperty.RegisterDirect<DocsPage, Control?>(nameof(Body), o => o.Body);

    private Control? body;

    /// <summary>Category label shown above the page title.</summary>
    public string? Section
    {
        get => GetValue(SectionProperty);
        set => SetValue(SectionProperty, value);
    }

    /// <summary>Display title for the page.</summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Opening paragraph that explains what the utility family does.</summary>
    public string? Intro
    {
        get => GetValue(IntroProperty);
        set => SetValue(IntroProperty, value);
    }

    /// <summary>What this library implements for the family, shown as the green note.</summary>
    public string? Supported
    {
        get => GetValue(SupportedProperty);
        set => SetValue(SupportedProperty, value);
    }

    public string SupportedTitle
    {
        get => GetValue(SupportedTitleProperty);
        set => SetValue(SupportedTitleProperty, value);
    }

    /// <summary>Utility table rows shown before the reader expands the table.</summary>
    public int CollapsedRowCount
    {
        get => GetValue(CollapsedRowCountProperty);
        set => SetValue(CollapsedRowCountProperty, value);
    }

    public string GapsTitle
    {
        get => GetValue(GapsTitleProperty);
        set => SetValue(GapsTitleProperty, value);
    }

    public string? GapsBody
    {
        get => GetValue(GapsBodyProperty);
        set => SetValue(GapsBodyProperty, value);
    }

    /// <summary>Class-to-AXAML reference rows.</summary>
    public AvaloniaList<UtilityReferenceRow> Utilities { get; } = [];

    /// <summary>Tailwind forms the parser does not accept; tests check each concrete class really is rejected.</summary>
    public AvaloniaList<DocsGap> Gaps { get; } = [];

    /// <summary>The example sections, in order.</summary>
    [Content]
    public AvaloniaList<Control> Examples { get; } = [];

    /// <summary>The generated page body the template presents.</summary>
    public Control? Body
    {
        get => body;
        private set => SetAndRaise(BodyProperty, ref body, value);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Body is not null)
        {
            return;
        }

        var root = Build();
        LogicalChildren.Add(root);
        Body = root;
    }

    private Control Build()
    {
        var overview = new StackPanel { Classes = { "docs-overviewStack" } };

        if (!string.IsNullOrEmpty(Supported))
        {
            overview.Children.Add(new DocsNote { Classes = { "supported" }, Title = SupportedTitle, Body = Supported });
        }

        if (Utilities.Count > 0)
        {
            var table = new DocsUtilityTable { CollapsedRowCount = CollapsedRowCount };
            table.Rows.AddRange(Utilities);
            overview.Children.Add(table);
        }

        if (Gaps.Count > 0)
        {
            overview.Children.Add(new DocsNote { Classes = { "gap" }, Title = GapsTitle, Body = GapsBody, Content = BuildGaps() });
        }

        var examples = new StackPanel { Classes = { "docs-exampleStack" } };
        examples.Children.AddRange(Examples);

        var root = new StackPanel { Spacing = 52 };

        if (overview.Children.Count > 0)
        {
            root.Children.Add(overview);
        }

        if (examples.Children.Count > 0)
        {
            root.Children.Add(new StackPanel
            {
                Spacing = 22,
                Children =
                {
                    new TextBlock { Classes = { "docs-eyebrow" }, Text = "Examples" },
                    examples,
                },
            });
        }

        return root;
    }

    private Control BuildGaps()
    {
        var grouped = Gaps.Any(gap => !string.IsNullOrEmpty(gap.Title));
        var panel = new StackPanel { Spacing = grouped ? 14 : 6 };

        foreach (var gap in Gaps)
        {
            var line = new TextBlock { Classes = { "docs-tableClass" }, Text = gap.Text };

            if (string.IsNullOrEmpty(gap.Title))
            {
                panel.Children.Add(line);
                continue;
            }

            panel.Children.Add(new StackPanel
            {
                Spacing = 5,
                Children = { new TextBlock { Classes = { "docs-miniTitle" }, Text = gap.Title }, line },
            });
        }

        return panel;
    }
}

/// <summary>One line of a page's "not implemented yet" note, optionally under a small group title.</summary>
public sealed class DocsGap
{
    public string? Title { get; set; }

    /// <summary>Classes separated by " · "; parentheses hold short prose.</summary>
    public string Text { get; set; } = string.Empty;
}
