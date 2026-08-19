using Avalonia.Controls;
using Avalonia.Interactivity;

using Tailwind.Avalonia.Sample.Spacing;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontSize : UserControl
{
    private const int CollapsedUtilityRowCount = 5;

    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("text-xs", "<TextBlock tw:Tw.Class=\"text-xs\" />"),
        new("text-sm", "<TextBlock tw:Tw.Class=\"text-sm\" />"),
        new("text-base", "<TextBlock tw:Tw.Class=\"text-base\" />"),
        new("text-lg", "<TextBlock tw:Tw.Class=\"text-lg\" />"),
        new("text-xl", "<TextBlock tw:Tw.Class=\"text-xl\" />"),
        new("text-2xl", "<TextBlock tw:Tw.Class=\"text-2xl\" />"),
        new("text-3xl", "<TextBlock tw:Tw.Class=\"text-3xl\" />"),
        new("text-4xl", "<TextBlock tw:Tw.Class=\"text-4xl\" />"),
        new("text-5xl", "<TextBlock tw:Tw.Class=\"text-5xl\" />"),
        new("text-6xl", "<TextBlock tw:Tw.Class=\"text-6xl\" />"),
        new("text-7xl", "<TextBlock tw:Tw.Class=\"text-7xl\" />"),
        new("text-8xl", "<TextBlock tw:Tw.Class=\"text-8xl\" />"),
        new("text-9xl", "<TextBlock tw:Tw.Class=\"text-9xl\" />"),
        new("text-[<value>]", "<TextBlock tw:Tw.Class=\"text-[<value>]\" />"),
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