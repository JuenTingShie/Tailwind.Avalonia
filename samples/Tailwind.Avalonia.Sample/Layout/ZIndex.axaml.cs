using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class ZIndex : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("z-0", "ZIndex: 0"),
        new("z-10", "ZIndex: 10"),
        new("z-20", "ZIndex: 20"),
        new("z-30", "ZIndex: 30"),
        new("z-40", "ZIndex: 40"),
        new("z-50", "ZIndex: 50"),
        new("z-auto", "ZIndex: 0"),
        new("z-[25]", "ZIndex: 25"),
    ];

    /// <summary>
    /// Initializes the z-index docs page and seeds the utility reference table.
    /// </summary>
    public ZIndex()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
