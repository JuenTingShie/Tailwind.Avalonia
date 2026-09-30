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
        new("decoration-<0|1|2|4|8>", "TextDecoration.StrokeThickness (px)"),
        new("decoration-[<px>]", "TextDecoration.StrokeThickness (px)"),
        new("decoration-<color>", "TextDecoration.Stroke"),
        new("underline-offset-<0|1|2|4|8>", "TextDecoration.StrokeOffset (px)"),
        new("decoration-solid / dotted / dashed", "TextDecoration.StrokeDashArray"),
        new("decoration-auto / underline-offset-auto", "font recommended metrics"),
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
