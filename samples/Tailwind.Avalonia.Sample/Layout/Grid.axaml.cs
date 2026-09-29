using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class Grid : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("grid-cols-<n>", "Grid.ColumnDefinitions: n star columns"),
        new("grid-rows-<n>", "Grid.RowDefinitions: n star rows"),
        new("col-start-<n>", "Grid.Column: n - 1"),
        new("row-start-<n>", "Grid.Row: n - 1"),
        new("col-span-<n>", "Grid.ColumnSpan: n"),
        new("row-span-<n>", "Grid.RowSpan: n"),
        new("col-span-full", "Grid.ColumnSpan: all columns"),
        new("row-span-full", "Grid.RowSpan: all rows"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public Grid()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
