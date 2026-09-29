using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class Display : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("hidden", "IsVisible: false"),
        new("block", "IsVisible: true"),
    ];

    /// <summary>
    /// Initializes the display docs page and seeds the utility reference table.
    /// </summary>
    public Display()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
