using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Spacing;

public partial class Margin : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("m-<number>", "<Border Margin=\"<number>\" />"),
        new("m-[<value>]", "<Border Margin=\"<parsed value>\" />"),
        new("mx-<number>", "<Border Margin=\"<number>,0,<number>,0\" />"),
        new("mx-[<value>]", "<Border Margin=\"<parsed>,0,<parsed>,0\" />"),
        new("my-<number>", "<Border Margin=\"0,<number>,0,<number>\" />"),
        new("my-[<value>]", "<Border Margin=\"0,<parsed>,0,<parsed>\" />"),
        new("mt-<number>", "<Border Margin=\"0,<number>,0,0\" />"),
        new("mt-[<value>]", "<Border Margin=\"0,<parsed>,0,0\" />"),
        new("mr-<number>", "<Border Margin=\"0,0,<number>,0\" />"),
        new("mr-[<value>]", "<Border Margin=\"0,0,<parsed>,0\" />"),
        new("mb-<number>", "<Border Margin=\"0,0,0,<number>\" />"),
        new("mb-[<value>]", "<Border Margin=\"0,0,0,<parsed>\" />"),
        new("ml-<number>", "<Border Margin=\"<number>,0,0,0\" />"),
        new("ml-[<value>]", "<Border Margin=\"<parsed>,0,0,0\" />"),
        new("ms-<number>", "LTR <Border Margin=\"<number>,0,0,0\" />; RTL <Border Margin=\"0,0,<number>,0\" />"),
        new("ms-[<value>]", "LTR <Border Margin=\"<parsed>,0,0,0\" />; RTL <Border Margin=\"0,0,<parsed>,0\" />"),
        new("me-<number>", "LTR <Border Margin=\"0,0,<number>,0\" />; RTL <Border Margin=\"<number>,0,0,0\" />"),
        new("me-[<value>]", "LTR <Border Margin=\"0,0,<parsed>,0\" />; RTL <Border Margin=\"<parsed>,0,0,0\" />"),
        new("mbs-<number>", "<Border Margin=\"0,<number>,0,0\" />"),
        new("mbs-[<value>]", "<Border Margin=\"0,<parsed>,0,0\" />"),
        new("mbe-<number>", "<Border Margin=\"0,0,0,<number>\" />"),
        new("mbe-[<value>]", "<Border Margin=\"0,0,0,<parsed>\" />"),
        new("msv-<number>", "<Border Margin=\"<number>,0,0,0\" />"),
        new("msv-[<value>]", "<Border Margin=\"<parsed>,0,0,0\" />"),
        new("mev-<number>", "<Border Margin=\"0,0,<number>,0\" />"),
        new("mev-[<value>]", "<Border Margin=\"0,0,<parsed>,0\" />"),
        new("-m-<number>", "<Border Margin=\"-<number>\" />"),
        new("-m-[<value>]", "<Border Margin=\"-<parsed value>\" />"),
        new("-mx-<number>", "<Border Margin=\"-<number>,0,-<number>,0\" />"),
        new("-mx-[<value>]", "<Border Margin=\"-<parsed>,0,-<parsed>,0\" />"),
        new("-my-<number>", "<Border Margin=\"0,-<number>,0,-<number>\" />"),
        new("-my-[<value>]", "<Border Margin=\"0,-<parsed>,0,-<parsed>\" />"),
        new("-mt-<number>", "<Border Margin=\"0,-<number>,0,0\" />"),
        new("-mt-[<value>]", "<Border Margin=\"0,-<parsed>,0,0\" />"),
        new("-mr-<number>", "<Border Margin=\"0,0,-<number>,0\" />"),
        new("-mr-[<value>]", "<Border Margin=\"0,0,-<parsed>,0\" />"),
        new("-mb-<number>", "<Border Margin=\"0,0,0,-<number>\" />"),
        new("-mb-[<value>]", "<Border Margin=\"0,0,0,-<parsed>\" />"),
        new("-ml-<number>", "<Border Margin=\"-<number>,0,0,0\" />"),
        new("-ml-[<value>]", "<Border Margin=\"-<parsed>,0,0,0\" />"),
        new("-ms-<number>", "LTR <Border Margin=\"-<number>,0,0,0\" />; RTL <Border Margin=\"0,0,-<number>,0\" />"),
        new("-ms-[<value>]", "LTR <Border Margin=\"-<parsed>,0,0,0\" />; RTL <Border Margin=\"0,0,-<parsed>,0\" />"),
        new("-me-<number>", "LTR <Border Margin=\"0,0,-<number>,0\" />; RTL <Border Margin=\"-<number>,0,0,0\" />"),
        new("-me-[<value>]", "LTR <Border Margin=\"0,0,-<parsed>,0\" />; RTL <Border Margin=\"-<parsed>,0,0,0\" />"),
        new("-mbs-<number>", "<Border Margin=\"0,-<number>,0,0\" />"),
        new("-mbs-[<value>]", "<Border Margin=\"0,-<parsed>,0,0\" />"),
        new("-mbe-<number>", "<Border Margin=\"0,0,0,-<number>\" />"),
        new("-mbe-[<value>]", "<Border Margin=\"0,0,0,-<parsed>\" />"),
    ];

    /// <summary>
    /// Initializes the margin docs page and seeds the utility reference table.
    /// </summary>
    public Margin()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
