using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Tests;

/// <summary>
/// Opens every page inside the real shell at phone width and fails when something sticks out past the page
/// column without a horizontal scroller to reach it — the class of bug that used to be found only by eye.
/// </summary>
public class SampleNarrowLayoutTests
{
    private const double PhoneWidth = 420;

    [Fact]
    public void Narrow_Window_Uses_The_Compact_Layout()
    {
        var layout = SampleLayout.For(PhoneWidth, 900);

        Assert.True(layout.IsNarrow);
        Assert.True(layout.IsCompactDocs);
        Assert.False(SampleLayout.For(1280, 900).IsCompactDocs);
        Assert.True(SampleLayout.For(1280, 600).IsCompactDocs);
    }

    [Theory]
    [MemberData(nameof(SamplePageTests.Pages), MemberType = typeof(SamplePageTests))]
    public void Page_Content_Fits_Or_Scrolls_At_Phone_Width(string section, string page)
    {
        var problems = new List<string>();

        SampleHeadless.Run(() =>
        {
            var shell = new SampleShell();
            var window = new Window { Width = PhoneWidth, Height = 900, Content = shell };
            window.Show();
            window.UpdateLayout();

            var view = shell.Navigate(section, page);
            window.UpdateLayout();
            Assert.Contains(SampleLayout.CompactDocsClass, view.Classes);

            var docsPage = view.GetVisualDescendants().OfType<DocsPage>().First();
            var column = docsPage.GetVisualDescendants().OfType<ScrollViewer>().First();
            var limit = column.Viewport.Width + 1;

            Visit(column, column, limit, problems);
        });

        Assert.True(problems.Count == 0, string.Join(Environment.NewLine, problems.Distinct().Take(10)));
    }

    // Walk the visual tree; stop at anything that can scroll sideways, and report the outermost element that
    // pokes out past the column so one overflowing container is not reported once per descendant.
    private static void Visit(Visual node, ScrollViewer column, double limit, List<string> problems)
    {
        foreach (var child in node.GetVisualChildren())
        {
            if (!child.IsVisible || child is ScrollBar)
            {
                continue;
            }

            if (child is ScrollViewer { HorizontalScrollBarVisibility: not ScrollBarVisibility.Disabled } && !ReferenceEquals(child, column))
            {
                continue;
            }

            // Transform demos (rotate, scale, translate) move pixels on purpose; layout is what must fit.
            if (child.RenderTransform is not null && child is Control { Classes: var classes } && !classes.Contains("docs-paddingShell"))
            {
                continue;
            }

            var right = child.TranslatePoint(new Point(child.Bounds.Width, 0), column)?.X ?? 0;

            if (right > limit)
            {
                problems.Add($"{Describe(child)} ends at {right:0} but the column is {limit - 1:0} wide");
                continue;
            }

            Visit(child, column, limit, problems);
        }
    }

    private static string Describe(Visual visual)
    {
        var classes = visual is StyledElement { Classes.Count: > 0 } styled ? "." + string.Join(".", styled.Classes) : string.Empty;
        var tw = visual is AvaloniaObject element && Tw.GetClass(element) is { } c ? $" [{c}]" : string.Empty;
        return visual.GetType().Name + classes + tw;
    }
}
