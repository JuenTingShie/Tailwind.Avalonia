using Avalonia;
using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// Scaffolds a docs page: scrolling ground, constrained column, and the
/// section / title / lede header every page opens with.
/// </summary>
public class DocsPage : ContentControl
{
    public static readonly StyledProperty<string?> SectionProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(Section));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(Title));

    public static readonly StyledProperty<string?> IntroProperty =
        AvaloniaProperty.Register<DocsPage, string?>(nameof(Intro));

    /// <summary>
    /// Category label shown above the page title.
    /// </summary>
    public string? Section
    {
        get => GetValue(SectionProperty);
        set => SetValue(SectionProperty, value);
    }

    /// <summary>
    /// Display title for the page.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Opening paragraph that explains what the utility family does.
    /// </summary>
    public string? Intro
    {
        get => GetValue(IntroProperty);
        set => SetValue(IntroProperty, value);
    }
}
