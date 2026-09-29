using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class TextAlign : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("text-left", "<TextBlock TextAlignment=\"Left\" />"),
        new("text-center", "<TextBlock TextAlignment=\"Center\" />"),
        new("text-right", "<TextBlock TextAlignment=\"Right\" />"),
        new("text-justify", "<TextBlock TextAlignment=\"Justify\" />"),
        new("text-start", "<TextBlock TextAlignment=\"Start\" />"),
        new("text-end", "<TextBlock TextAlignment=\"End\" />"),
    ];

    /// <summary>
    /// Initializes the text-align docs page and seeds the utility reference table.
    /// </summary>
    public TextAlign()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
