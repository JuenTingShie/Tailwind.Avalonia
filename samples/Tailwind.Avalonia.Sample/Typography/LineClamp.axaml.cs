using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class LineClamp : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("line-clamp-<n>", "MaxLines: n; TextTrimming: CharacterEllipsis; TextWrapping: Wrap"),
        new("line-clamp-none", "MaxLines: 0; TextTrimming: None"),
    ];

    /// <summary>
    /// Initializes the line clamp docs page and seeds the utility reference table.
    /// </summary>
    public LineClamp()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
