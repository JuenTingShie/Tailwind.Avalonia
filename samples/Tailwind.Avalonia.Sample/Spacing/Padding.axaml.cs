using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Spacing;

public partial class Padding : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("p-<number>", "<Border Padding=\"<number>\" />"),
        new("p-[<value>]", "<Border Padding=\"<parsed value>\" />"),
        new("px-<number>", "<Border Padding=\"<number>,0,<number>,0\" />"),
        new("px-[<value>]", "<Border Padding=\"<parsed>,0,<parsed>,0\" />"),
        new("py-<number>", "<Border Padding=\"0,<number>,0,<number>\" />"),
        new("py-[<value>]", "<Border Padding=\"0,<parsed>,0,<parsed>\" />"),
        new("pt-<number>", "<Border Padding=\"0,<number>,0,0\" />"),
        new("pt-[<value>]", "<Border Padding=\"0,<parsed>,0,0\" />"),
        new("pr-<number>", "<Border Padding=\"0,0,<number>,0\" />"),
        new("pr-[<value>]", "<Border Padding=\"0,0,<parsed>,0\" />"),
        new("pb-<number>", "<Border Padding=\"0,0,0,<number>\" />"),
        new("pb-[<value>]", "<Border Padding=\"0,0,0,<parsed>\" />"),
        new("pl-<number>", "<Border Padding=\"<number>,0,0,0\" />"),
        new("pl-[<value>]", "<Border Padding=\"<parsed>,0,0,0\" />"),
        new("ps-<number>", "LTR <Border Padding=\"<number>,0,0,0\" />; RTL <Border Padding=\"0,0,<number>,0\" />"),
        new("ps-[<value>]", "LTR <Border Padding=\"<parsed>,0,0,0\" />; RTL <Border Padding=\"0,0,<parsed>,0\" />"),
        new("pe-<number>", "LTR <Border Padding=\"0,0,<number>,0\" />; RTL <Border Padding=\"<number>,0,0,0\" />"),
        new("pe-[<value>]", "LTR <Border Padding=\"0,0,<parsed>,0\" />; RTL <Border Padding=\"<parsed>,0,0,0\" />"),
        new("pbs-<number>", "<Border Padding=\"0,<number>,0,0\" />"),
        new("pbs-[<value>]", "<Border Padding=\"0,<parsed>,0,0\" />"),
        new("pbe-<number>", "<Border Padding=\"0,0,0,<number>\" />"),
        new("pbe-[<value>]", "<Border Padding=\"0,0,0,<parsed>\" />"),
        new("psv-<number>", "<Border Padding=\"<number>,0,0,0\" />"),
        new("psv-[<value>]", "<Border Padding=\"<parsed>,0,0,0\" />"),
        new("pev-<number>", "<Border Padding=\"0,0,<number>,0\" />"),
        new("pev-[<value>]", "<Border Padding=\"0,0,<parsed>,0\" />"),
    ];

    /// <summary>
    /// Initializes the padding docs page and seeds the utility reference table.
    /// </summary>
    public Padding()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
