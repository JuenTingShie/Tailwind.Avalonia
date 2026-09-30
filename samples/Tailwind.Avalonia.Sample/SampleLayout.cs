using Avalonia;
using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// The sample's responsive rules in one place: which window sizes count as narrow or compact, and how a docs
/// page is switched into its compact style (the <c>docs-mobile</c> class that DocsStyles keys its overrides on).
/// </summary>
internal readonly record struct SampleLayout(bool IsNarrow, bool IsCompactDocs)
{
    public const string CompactDocsClass = "docs-mobile";

    /// <summary>Below this width the navigation becomes an overlay and pages use compact docs styles.</summary>
    public const double NarrowWidth = 960;

    /// <summary>Below this height pages use compact docs styles even on a wide window.</summary>
    public const double CompactHeight = 640;

    public const double NarrowPaneLength = 304;
    public const double WidePaneLength = 336;

    public double PaneLength => IsNarrow ? NarrowPaneLength : WidePaneLength;

    public Thickness HeaderPadding => IsNarrow ? new Thickness(10, 0) : new Thickness(12, 0);

    public Thickness ContentPadding => IsCompactDocs ? new Thickness(10) : new Thickness(18);

    public static SampleLayout For(double width, double height)
    {
        var narrow = width > 0 && width < NarrowWidth;
        return new SampleLayout(narrow, narrow || (height > 0 && height < CompactHeight));
    }

    /// <summary>Adds or removes the compact docs class on a page.</summary>
    public void ApplyTo(Control page) => page.Classes.Set(CompactDocsClass, IsCompactDocs);
}
