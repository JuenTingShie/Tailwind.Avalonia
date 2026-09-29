using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class FlexDirectionGap : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("flex-row", "Orientation: Horizontal"),
        new("flex-col", "Orientation: Vertical"),
        new("gap-<n>", "StackPanel.Spacing / WrapPanel.ItemSpacing+LineSpacing / Grid.ColumnSpacing+RowSpacing"),
        new("gap-x-<n>", "horizontal spacing"),
        new("gap-y-<n>", "vertical spacing"),
        new("space-x-<n> / space-y-<n>", "StackPanel.Spacing along that axis"),
    ];

    /// <summary>
    /// Initializes the direction and gap docs page and seeds the utility reference table.
    /// </summary>
    public FlexDirectionGap()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
