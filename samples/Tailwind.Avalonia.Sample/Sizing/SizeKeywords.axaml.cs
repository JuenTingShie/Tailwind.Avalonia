using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Sizing;

public partial class SizeKeywords : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("w-full / h-full", "Stretch alignment, Width/Height auto"),
        new("w-auto / h-auto", "Width/Height: auto"),
        new("size-<n>", "Width and Height: n * 4px"),
        new("size-auto", "Width and Height: auto"),
        new("max-w-none / max-h-none", "MaxWidth/MaxHeight: unlimited"),
    ];

    /// <summary>
    /// Initializes the size keywords docs page and seeds the utility reference table.
    /// </summary>
    public SizeKeywords()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
