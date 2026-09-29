using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontWeight : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("font-thin", "<TextBlock FontWeight=\"Thin\" />"),
        new("font-extralight", "<TextBlock FontWeight=\"ExtraLight\" />"),
        new("font-light", "<TextBlock FontWeight=\"Light\" />"),
        new("font-normal", "<TextBlock FontWeight=\"Normal\" />"),
        new("font-medium", "<TextBlock FontWeight=\"Medium\" />"),
        new("font-semibold", "<TextBlock FontWeight=\"SemiBold\" />"),
        new("font-bold", "<TextBlock FontWeight=\"Bold\" />"),
        new("font-extrabold", "<TextBlock FontWeight=\"ExtraBold\" />"),
        new("font-black", "<TextBlock FontWeight=\"Black\" />"),
    ];

    /// <summary>
    /// Initializes the font weight docs page and seeds the utility reference table.
    /// </summary>
    public FontWeight()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
