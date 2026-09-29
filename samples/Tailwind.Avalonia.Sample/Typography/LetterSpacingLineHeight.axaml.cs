using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class LetterSpacingLineHeight : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("tracking-tighter", "LetterSpacing: -0.05em"),
        new("tracking-tight", "LetterSpacing: -0.025em"),
        new("tracking-normal", "LetterSpacing: 0"),
        new("tracking-wide", "LetterSpacing: 0.025em"),
        new("tracking-wider", "LetterSpacing: 0.05em"),
        new("tracking-widest", "LetterSpacing: 0.1em"),
        new("tracking-[3px]", "LetterSpacing: 3px"),
        new("leading-none", "LineHeight: 1 x font size"),
        new("leading-tight", "LineHeight: 1.25 x font size"),
        new("leading-snug", "LineHeight: 1.375 x font size"),
        new("leading-normal", "LineHeight: 1.5 x font size"),
        new("leading-relaxed", "LineHeight: 1.625 x font size"),
        new("leading-loose", "LineHeight: 2 x font size"),
        new("leading-6", "LineHeight: 24px"),
        new("leading-[18px]", "LineHeight: 18px"),
    ];

    /// <summary>
    /// Initializes the tracking and leading docs page and seeds the utility reference table.
    /// </summary>
    public LetterSpacingLineHeight()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
