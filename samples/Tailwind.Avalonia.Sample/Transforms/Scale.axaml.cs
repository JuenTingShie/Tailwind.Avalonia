using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Transforms;

public partial class Scale : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("scale-<n>", "scale x and y by n percent"),
        new("scale-x-<n>", "scale horizontally"),
        new("scale-y-<n>", "scale vertically"),
        new("-scale-x-100", "mirror horizontally"),
        new("scale-[1.25]", "arbitrary multiplier"),
    ];

    /// <summary>
    /// Initializes the scale docs page and seeds the utility reference table.
    /// </summary>
    public Scale()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
