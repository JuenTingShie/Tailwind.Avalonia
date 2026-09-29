using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontFamily : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("italic", "<TextBlock FontFamily=\"Italic\" />"),
        new("not-italic", "<TextBlock FontFamily=\"Normal\" />"),
    ];

    /// <summary>
    /// Initializes the font style docs page and seeds the utility reference table.
    /// </summary>
    public FontFamily()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
