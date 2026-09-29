using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class TextDecoration : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("underline", "TextDecorations: Underline"),
        new("overline", "TextDecorations: Overline"),
        new("line-through", "TextDecorations: Strikethrough"),
        new("no-underline", "TextDecorations: none"),
    ];

    /// <summary>
    /// Initializes the text decoration docs page and seeds the utility reference table.
    /// </summary>
    public TextDecoration()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
