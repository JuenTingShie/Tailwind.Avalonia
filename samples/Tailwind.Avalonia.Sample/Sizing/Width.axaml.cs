using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Sizing;

public partial class Width : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("w-<number>", "<Border Width=\"<number>\" />"),
        new("w-[<value>]", "<Border Width=\"<parsed value>\" />"),
        new("min-w-<number>", "<Border MinWidth=\"<number>\" />"),
        new("min-w-[<value>]", "<Border MinWidth=\"<parsed value>\" />"),
        new("max-w-<number>", "<Border MaxWidth=\"<number>\" />"),
        new("max-w-[<value>]", "<Border MaxWidth=\"<parsed value>\" />"),
    ];

    /// <summary>
    /// Initializes the width docs page and seeds the utility reference table.
    /// </summary>
    public Width()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
