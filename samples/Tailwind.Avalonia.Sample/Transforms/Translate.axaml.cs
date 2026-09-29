using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Transforms;

public partial class Translate : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("translate-x-<n>", "move horizontally by n * 4px"),
        new("translate-y-<n>", "move vertically by n * 4px"),
        new("-translate-x-<n>", "move left"),
        new("translate-<n>", "move on both axes"),
        new("translate-x-[<px>]", "arbitrary pixels"),
    ];

    /// <summary>
    /// Initializes the translate docs page and seeds the utility reference table.
    /// </summary>
    public Translate()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
