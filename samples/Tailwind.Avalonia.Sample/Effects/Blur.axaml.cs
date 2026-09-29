using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Effects;

public partial class Blur : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("blur / blur-sm", "BlurEffect radius 8"),
        new("blur-xs", "radius 4"),
        new("blur-md", "radius 12"),
        new("blur-lg", "radius 16"),
        new("blur-xl", "radius 24"),
        new("blur-2xl", "radius 40"),
        new("blur-3xl", "radius 64"),
        new("blur-none", "radius 0"),
        new("blur-[<px>]", "arbitrary radius"),
    ];

    /// <summary>
    /// Initializes the blur docs page and seeds the utility reference table.
    /// </summary>
    public Blur()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
