using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Interactivity;

public partial class PointerEvents : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("pointer-events-none", "IsHitTestVisible: false"),
        new("pointer-events-auto", "IsHitTestVisible: true"),
    ];

    /// <summary>
    /// Initializes the pointer events docs page and seeds the utility reference table.
    /// </summary>
    public PointerEvents()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
