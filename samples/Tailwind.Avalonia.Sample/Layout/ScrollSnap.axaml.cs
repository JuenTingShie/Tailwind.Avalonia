using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class ScrollSnap : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("snap-x", "ScrollViewer.HorizontalSnapPointsType: Mandatory"),
        new("snap-y", "ScrollViewer.VerticalSnapPointsType: Mandatory"),
        new("snap-both", "both axes: Mandatory"),
        new("snap-none", "both axes: None"),
        new("snap-start", "SnapPointsAlignment: Near"),
        new("snap-center", "SnapPointsAlignment: Center"),
        new("snap-end", "SnapPointsAlignment: Far"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public ScrollSnap()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
