using Avalonia.Controls;
using Avalonia.Interactivity;

using Tailwind.Avalonia.Sample.Spacing;

namespace Tailwind.Avalonia.Sample.Sizing;

public partial class Height : UserControl
{
    private const int CollapsedUtilityRowCount = 2;

    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("h-<number>", "<Border Height=\"<number>\" />"),
        new("min-h-<number>", "<Border MinHeight=\"<number>\" />"),
        new("max-h-<number>", "<Border MaxHeight=\"<number>\" />"),
    ];

    private readonly SpacingUtilityReferenceTablePresenter utilityTablePresenter;

    /// <summary>
    /// Initializes the height docs page and seeds the reference table with the compact row set.
    /// </summary>
    public Height()
    {
        InitializeComponent();
        utilityTablePresenter = new SpacingUtilityReferenceTablePresenter(
            HeightUtilityRows,
            HeightUtilityToggleButton,
            AllUtilityRows,
            CollapsedUtilityRowCount);
    }

    // Switch between the compact and full height utility reference rows.
    private void ToggleUtilityRows(object? sender, RoutedEventArgs e)
    {
        utilityTablePresenter.Toggle();
    }
}