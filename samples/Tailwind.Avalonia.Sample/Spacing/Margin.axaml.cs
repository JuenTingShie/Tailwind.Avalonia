using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Tailwind.Avalonia.Sample.Spacing;

public partial class Margin : UserControl
{
    private const int CollapsedUtilityRowCount = 5;

    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("m-<number>", "<Border Margin=\"<number>\" />"),
        new("mx-<number>", "<Border Margin=\"<number>,0,<number>,0\" />"),
        new("my-<number>", "<Border Margin=\"0,<number>,0,<number>\" />"),
        new("mt-<number>", "<Border Margin=\"0,<number>,0,0\" />"),
        new("mr-<number>", "<Border Margin=\"0,0,<number>,0\" />"),
        new("mb-<number>", "<Border Margin=\"0,0,0,<number>\" />"),
        new("ml-<number>", "<Border Margin=\"<number>,0,0,0\" />"),
        new("ms-<number>", "LTR <Border Margin=\"<number>,0,0,0\" />; RTL <Border Margin=\"0,0,<number>,0\" />"),
        new("me-<number>", "LTR <Border Margin=\"0,0,<number>,0\" />; RTL <Border Margin=\"<number>,0,0,0\" />"),
        new("mbs-<number>", "<Border Margin=\"0,<number>,0,0\" />"),
        new("mbe-<number>", "<Border Margin=\"0,0,0,<number>\" />"),
        new("msv-<number>", "<Border Margin=\"<number>,0,0,0\" />"),
        new("mev-<number>", "<Border Margin=\"0,0,<number>,0\" />"),
        new("-m-<number>", "<Border Margin=\"-<number>\" />"),
        new("-mx-<number>", "<Border Margin=\"-<number>,0,-<number>,0\" />"),
        new("-my-<number>", "<Border Margin=\"0,-<number>,0,-<number>\" />"),
        new("-mt-<number>", "<Border Margin=\"0,-<number>,0,0\" />"),
        new("-mr-<number>", "<Border Margin=\"0,0,-<number>,0\" />"),
        new("-mb-<number>", "<Border Margin=\"0,0,0,-<number>\" />"),
        new("-ml-<number>", "<Border Margin=\"-<number>,0,0,0\" />"),
        new("-ms-<number>", "LTR <Border Margin=\"-<number>,0,0,0\" />; RTL <Border Margin=\"0,0,-<number>,0\" />"),
        new("-me-<number>", "LTR <Border Margin=\"0,0,-<number>,0\" />; RTL <Border Margin=\"-<number>,0,0,0\" />"),
        new("-mbs-<number>", "<Border Margin=\"0,-<number>,0,0\" />"),
        new("-mbe-<number>", "<Border Margin=\"0,0,0,-<number>\" />"),
    ];

    private readonly SpacingUtilityReferenceTablePresenter utilityTablePresenter;

    /// <summary>
    /// Initializes the margin docs page and seeds the reference table with the compact row set.
    /// </summary>
    public Margin()
    {
        InitializeComponent();
        utilityTablePresenter = new SpacingUtilityReferenceTablePresenter(
            MarginUtilityRows,
            MarginUtilityToggleButton,
            AllUtilityRows,
            CollapsedUtilityRowCount);
    }

    // Switch between the compact and full margin utility reference rows.
    private void ToggleUtilityRows(object? sender, RoutedEventArgs e)
    {
        utilityTablePresenter.Toggle();
    }
}