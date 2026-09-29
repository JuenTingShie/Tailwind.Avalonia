using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Backgrounds;

public partial class Gradients : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("bg-gradient-to-t / tr / r / br / b / bl / l / tl", "LinearGradientBrush direction"),
        new("bg-linear-to-<direction>", "same, Tailwind v4 spelling"),
        new("from-<color>", "first stop"),
        new("via-<color>", "middle stop"),
        new("to-<color>", "last stop"),
    ];

    /// <summary>
    /// Initializes the gradients docs page and seeds the utility reference table.
    /// </summary>
    public Gradients()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
