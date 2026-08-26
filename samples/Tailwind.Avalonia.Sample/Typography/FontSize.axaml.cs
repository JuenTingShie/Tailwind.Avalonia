using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontSize : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("text-xs", "<TextBlock tw:Tw.Class=\"text-xs\" />"),
        new("text-sm", "<TextBlock tw:Tw.Class=\"text-sm\" />"),
        new("text-base", "<TextBlock tw:Tw.Class=\"text-base\" />"),
        new("text-lg", "<TextBlock tw:Tw.Class=\"text-lg\" />"),
        new("text-xl", "<TextBlock tw:Tw.Class=\"text-xl\" />"),
        new("text-2xl", "<TextBlock tw:Tw.Class=\"text-2xl\" />"),
        new("text-3xl", "<TextBlock tw:Tw.Class=\"text-3xl\" />"),
        new("text-4xl", "<TextBlock tw:Tw.Class=\"text-4xl\" />"),
        new("text-5xl", "<TextBlock tw:Tw.Class=\"text-5xl\" />"),
        new("text-6xl", "<TextBlock tw:Tw.Class=\"text-6xl\" />"),
        new("text-7xl", "<TextBlock tw:Tw.Class=\"text-7xl\" />"),
        new("text-8xl", "<TextBlock tw:Tw.Class=\"text-8xl\" />"),
        new("text-9xl", "<TextBlock tw:Tw.Class=\"text-9xl\" />"),
        new("text-[<value>]", "<TextBlock tw:Tw.Class=\"text-[<value>]\" />"),
    ];

    /// <summary>
    /// Initializes the font-size docs page and seeds the utility reference table.
    /// </summary>
    public FontSize()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
