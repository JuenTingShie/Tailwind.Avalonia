using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Sizing;

public partial class Height : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("h-<number>", "<Border Height=\"<number>\" />"),
        new("h-[<value>]", "<Border Height=\"<parsed value>\" />"),
        new("min-h-<number>", "<Border MinHeight=\"<number>\" />"),
        new("min-h-[<value>]", "<Border MinHeight=\"<parsed value>\" />"),
        new("max-h-<number>", "<Border MaxHeight=\"<number>\" />"),
        new("max-h-[<value>]", "<Border MaxHeight=\"<parsed value>\" />"),
    ];

    /// <summary>
    /// Initializes the height docs page and seeds the utility reference table.
    /// </summary>
    public Height()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
