using Avalonia.Controls;
using Avalonia.Interactivity;

using Tailwind.Avalonia.Sample.Spacing;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontSize : UserControl
{
    private const int CollapsedUtilityRowCount = 5;

    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("text-xs", "<TextBlock FontSize=\"{StaticResource FontSizeXs}\" />"),
        new("text-sm", "<TextBlock FontSize=\"{StaticResource FontSizeSm}\" />"),
        new("text-base", "<TextBlock FontSize=\"{StaticResource FontSizeBase}\" />"),
        new("text-lg", "<TextBlock FontSize=\"{StaticResource FontSizeLg}\" />"),
        new("text-xl", "<TextBlock FontSize=\"{StaticResource FontSizeXl}\" />"),
        new("text-2xl", "<TextBlock FontSize=\"{StaticResource FontSize2xl}\" />"),
        new("text-3xl", "<TextBlock FontSize=\"{StaticResource FontSize3xl}\" />"),
        new("text-4xl", "<TextBlock FontSize=\"{StaticResource FontSize4xl}\" />"),
        new("text-5xl", "<TextBlock FontSize=\"{StaticResource FontSize5xl}\" />"),
        new("text-6xl", "<TextBlock FontSize=\"{StaticResource FontSize6xl}\" />"),
        new("text-7xl", "<TextBlock FontSize=\"{StaticResource FontSize7xl}\" />"),
        new("text-8xl", "<TextBlock FontSize=\"{StaticResource FontSize8xl}\" />"),
        new("text-9xl", "<TextBlock FontSize=\"{StaticResource FontSize9xl}\" />"),
        new("text-[<value>]", "<TextBlock FontSize=\"<parsed absolute value>\" />"),
    ];

    private readonly SpacingUtilityReferenceTablePresenter utilityTablePresenter;

    /// <summary>
    /// Initializes the font-size docs page and seeds the compact utility reference table.
    /// </summary>
    public FontSize()
    {
        InitializeComponent();
        utilityTablePresenter = new SpacingUtilityReferenceTablePresenter(
            FontSizeUtilityRows,
            FontSizeUtilityToggleButton,
            AllUtilityRows,
            CollapsedUtilityRowCount);
    }

    // Switch between the compact and full font-size utility reference rows.
    private void ToggleUtilityRows(object? sender, RoutedEventArgs e)
    {
        utilityTablePresenter.Toggle();
    }
}