using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class Overflow : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("overflow-hidden", "ClipToBounds: true"),
        new("overflow-clip", "ClipToBounds: true"),
        new("overflow-visible", "ClipToBounds: false"),
    ];

    /// <summary>
    /// Initializes the overflow docs page and seeds the utility reference table.
    /// </summary>
    public Overflow()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
