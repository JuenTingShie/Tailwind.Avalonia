using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Svg;

public partial class FillStroke : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("fill-<color>", "Shape.Fill"),
        new("stroke-<color>", "Shape.Stroke"),
        new("fill-none / stroke-none", "transparent brush"),
        new("stroke-0 / 1 / 2", "Shape.StrokeThickness"),
        new("stroke-[<px>]", "Shape.StrokeThickness"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public FillStroke()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
