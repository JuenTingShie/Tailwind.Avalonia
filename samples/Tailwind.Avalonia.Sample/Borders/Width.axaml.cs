using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Borders;

public partial class Width : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("border", "<Border BorderThickness=\"1\" />"),
        new("border-<number>", "<Border BorderThickness=\"<number>\" />"),
        new("border-[<value>]", "<Border BorderThickness=\"<parsed value>\" />"),
        new("border-t", "<Border BorderThickness=\"0,1,0,0\" />"),
        new("border-t-<number>", "<Border BorderThickness=\"0,<number>,0,0\" />"),
        new("border-r", "<Border BorderThickness=\"0,0,1,0\" />"),
        new("border-r-<number>", "<Border BorderThickness=\"0,0,<number>,0\" />"),
        new("border-b", "<Border BorderThickness=\"0,0,0,1\" />"),
        new("border-b-<number>", "<Border BorderThickness=\"0,0,0,<number>\" />"),
        new("border-l", "<Border BorderThickness=\"1,0,0,0\" />"),
        new("border-l-<number>", "<Border BorderThickness=\"<number>,0,0,0\" />"),
        new("border-x", "<Border BorderThickness=\"1,0,1,0\" />"),
        new("border-x-<number>", "<Border BorderThickness=\"<number>,0,<number>,0\" />"),
        new("border-y", "<Border BorderThickness=\"0,1,0,1\" />"),
        new("border-y-<number>", "<Border BorderThickness=\"0,<number>,0,<number>\" />"),
        new("border-s", "LTR <Border BorderThickness=\"1,0,0,0\" />; RTL <Border BorderThickness=\"0,0,1,0\" />"),
        new("border-s-<number>", "LTR <Border BorderThickness=\"<number>,0,0,0\" />; RTL <Border BorderThickness=\"0,0,<number>,0\" />"),
        new("border-e", "LTR <Border BorderThickness=\"0,0,1,0\" />; RTL <Border BorderThickness=\"1,0,0,0\" />"),
        new("border-e-<number>", "LTR <Border BorderThickness=\"0,0,<number>,0\" />; RTL <Border BorderThickness=\"<number>,0,0,0\" />"),
        new("border-bs", "<Border BorderThickness=\"0,1,0,0\" />"),
        new("border-bs-<number>", "<Border BorderThickness=\"0,<number>,0,0\" />"),
        new("border-be", "<Border BorderThickness=\"0,0,0,1\" />"),
        new("border-be-<number>", "<Border BorderThickness=\"0,0,0,<number>\" />"),
    ];

    /// <summary>
    /// Initializes the border-width docs page and seeds the utility reference table.
    /// </summary>
    public Width()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
