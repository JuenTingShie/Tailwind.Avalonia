using Avalonia.Controls;
using Avalonia.Interactivity;
using Tailwind.Avalonia.Sample.Spacing;

namespace Tailwind.Avalonia.Sample.Sizing;

public partial class Width : UserControl
{
    private const int CollapsedUtilityRowCount = 2;

    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("w-<number>", "<Border Width=\"<number>\" />"),
        new("min-w-<number>", "<Border MinWidth=\"<number>\" />"),
        new("max-w-<number>", "<Border MaxWidth=\"<number>\" />"),
    ];

    private readonly SpacingUtilityReferenceTablePresenter utilityTablePresenter;

    /// <summary>
    /// Initializes the width docs page and seeds the reference table with the compact row set.
    /// </summary>
    public Width()
    {
        InitializeComponent();
        utilityTablePresenter = new SpacingUtilityReferenceTablePresenter(
            WidthUtilityRows,
            WidthUtilityToggleButton,
            AllUtilityRows,
            CollapsedUtilityRowCount);
    }

    // Switch between the compact and full width utility reference rows.
    private void ToggleUtilityRows(object? sender, RoutedEventArgs e)
    {
        utilityTablePresenter.Toggle();
    }
}