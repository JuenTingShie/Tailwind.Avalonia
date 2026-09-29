using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Interactivity;

public partial class CaretSelectionColor : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("caret-<color>", "CaretBrush"),
        new("selection-<color>", "SelectionBrush"),
    ];

    /// <summary>
    /// Initializes the caret and selection color docs page and seeds the utility reference table.
    /// </summary>
    public CaretSelectionColor()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
