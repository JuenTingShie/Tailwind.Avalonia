using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Effects;

public partial class Ring : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("ring", "BoxShadow: spread 1px"),
        new("ring-0 / 1 / 2 / 4 / 8", "BoxShadow: spread n px"),
        new("ring-[<px>]", "arbitrary width"),
        new("ring-<color>", "ring color, default blue-500"),
    ];

    /// <summary>
    /// Initializes the ring docs page and seeds the utility reference table.
    /// </summary>
    public Ring()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
