using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Effects;

public partial class BoxShadow : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("shadow-2xs", "<Border BoxShadow=\"0 1 0 0 #0D000000\" />"),
        new("shadow-xs", "<Border BoxShadow=\"0 1 2 0 #0D000000\" />"),
        new("shadow-sm", "<Border BoxShadow=\"0 1 3 0 #1A000000, 0 1 2 -1 #1A000000\" />"),
        new("shadow-md", "<Border BoxShadow=\"0 4 6 -1 #1A000000, 0 2 4 -2 #1A000000\" />"),
        new("shadow-lg", "<Border BoxShadow=\"0 10 15 -3 #1A000000, 0 4 6 -4 #1A000000\" />"),
        new("shadow-xl", "<Border BoxShadow=\"0 20 25 -5 #1A000000, 0 8 10 -6 #1A000000\" />"),
        new("shadow-2xl", "<Border BoxShadow=\"0 25 50 -12 #40000000\" />"),
        new("shadow-none", "<Border BoxShadow=\"none\" />"),
    ];

    /// <summary>
    /// Initializes the box-shadow docs page and seeds the utility reference table.
    /// </summary>
    public BoxShadow()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
