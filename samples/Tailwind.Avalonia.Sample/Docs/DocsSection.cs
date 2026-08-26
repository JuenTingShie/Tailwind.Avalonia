using Avalonia;
using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// One titled block inside a docs page: heading, optional summary, then content.
/// </summary>
public class DocsSection : ContentControl
{
    public static readonly StyledProperty<string?> HeadingProperty =
        AvaloniaProperty.Register<DocsSection, string?>(nameof(Heading));

    public static readonly StyledProperty<string?> SummaryProperty =
        AvaloniaProperty.Register<DocsSection, string?>(nameof(Summary));

    /// <summary>
    /// Heading text for the block.
    /// </summary>
    public string? Heading
    {
        get => GetValue(HeadingProperty);
        set => SetValue(HeadingProperty, value);
    }

    /// <summary>
    /// Optional paragraph rendered under the heading; hidden when empty.
    /// </summary>
    public string? Summary
    {
        get => GetValue(SummaryProperty);
        set => SetValue(SummaryProperty, value);
    }
}
