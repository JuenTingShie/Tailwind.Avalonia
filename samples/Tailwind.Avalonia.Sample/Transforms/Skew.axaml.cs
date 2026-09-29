using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Transforms;

public partial class Skew : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("skew-x-<deg>", "skew along x by n degrees"),
        new("skew-y-<deg>", "skew along y"),
        new("-skew-x-<deg>", "skew the other way"),
        new("skew-<deg>", "skew on both axes"),
    ];

    /// <summary>
    /// Initializes the skew docs page and seeds the utility reference table.
    /// </summary>
    public Skew()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
