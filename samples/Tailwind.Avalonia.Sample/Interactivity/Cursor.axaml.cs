using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Interactivity;

public partial class Cursor : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("cursor-default", "Cursor: Arrow"),
        new("cursor-pointer", "Cursor: Hand"),
        new("cursor-text", "Cursor: IBeam"),
        new("cursor-wait", "Cursor: Wait"),
        new("cursor-progress", "Cursor: AppStarting"),
        new("cursor-crosshair", "Cursor: Cross"),
        new("cursor-move", "Cursor: SizeAll"),
        new("cursor-help", "Cursor: Help"),
        new("cursor-not-allowed", "Cursor: No"),
        new("cursor-none", "Cursor: None"),
        new("cursor-ew-resize", "Cursor: SizeWestEast"),
        new("cursor-ns-resize", "Cursor: SizeNorthSouth"),
    ];

    /// <summary>
    /// Initializes the cursor docs page and seeds the utility reference table.
    /// </summary>
    public Cursor()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
