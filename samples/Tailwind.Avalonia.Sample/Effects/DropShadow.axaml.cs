using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Effects;

public partial class DropShadow : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("drop-shadow-xs", "offset 1, blur 1"),
        new("drop-shadow-sm", "offset 1, blur 2"),
        new("drop-shadow", "offset 1, blur 2"),
        new("drop-shadow-md", "offset 3, blur 3"),
        new("drop-shadow-lg", "offset 4, blur 4"),
        new("drop-shadow-xl", "offset 9, blur 7"),
        new("drop-shadow-2xl", "offset 25, blur 25"),
    ];

    /// <summary>
    /// Initializes the drop shadow docs page and seeds the utility reference table.
    /// </summary>
    public DropShadow()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
