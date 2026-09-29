using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class Position : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("top-<n> / right-<n> / bottom-<n> / left-<n>", "Canvas.Top / Right / Bottom / Left = n * 4px"),
        new("-top-<n>", "negative offset"),
        new("inset-<n>", "all four Canvas sides"),
        new("inset-x-<n>", "Canvas.Left and Canvas.Right"),
        new("inset-y-<n>", "Canvas.Top and Canvas.Bottom"),
    ];

    /// <summary>
    /// Initializes the position docs page and seeds the utility reference table.
    /// </summary>
    public Position()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
