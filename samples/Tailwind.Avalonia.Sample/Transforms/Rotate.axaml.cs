using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Transforms;

public partial class Rotate : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("rotate-<deg>", "RenderTransform: rotate n degrees"),
        new("-rotate-<deg>", "RenderTransform: rotate -n degrees"),
        new("rotate-[<deg>deg]", "RenderTransform: arbitrary angle"),
        new("origin-center", "RenderTransformOrigin: 50% 50%"),
        new("origin-top-left", "RenderTransformOrigin: 0 0"),
        new("origin-<side>", "top, right, bottom, left and the corners"),
        new("transform-none", "RenderTransform: identity"),
    ];

    /// <summary>
    /// Initializes the rotate docs page and seeds the utility reference table.
    /// </summary>
    public Rotate()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
