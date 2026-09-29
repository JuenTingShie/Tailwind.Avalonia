using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class TextOverflow : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("truncate", "TextTrimming: CharacterEllipsis; TextWrapping: NoWrap"),
        new("text-ellipsis", "TextTrimming: CharacterEllipsis"),
        new("text-clip", "TextTrimming: None"),
        new("text-wrap", "TextWrapping: Wrap"),
        new("text-nowrap", "TextWrapping: NoWrap"),
    ];

    /// <summary>
    /// Initializes the text overflow docs page and seeds the utility reference table.
    /// </summary>
    public TextOverflow()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
