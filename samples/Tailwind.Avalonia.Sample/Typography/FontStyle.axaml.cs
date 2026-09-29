using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontStyle : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("italic", "<TextBlock FontStyle=\"Italic\" />"),
        new("not-italic", "<TextBlock FontStyle=\"Normal\" />"),
    ];

    /// <summary>
    /// Initializes the font style docs page and seeds the utility reference table.
    /// </summary>
    public FontStyle()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
