using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class ColorUtilities : UserControl
{
    // These are C# string literals bound straight to TextBlock.Text, so the
    // placeholder brackets are written literally - XML entities would render
    // as "&lt;color&gt;" on screen.
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("bg-<color>", "<Control tw:Tw.Class=\"bg-blue-500\" />"),
        new("text-<color>", "<Control tw:Tw.Class=\"text-blue-500\" />"),
        new("border-<color>", "<Border tw:Tw.Class=\"border-blue-500\" />"),
        new("*<color>/<opacity>", "<Control tw:Tw.Class=\"bg-blue-500/50\" />"),
        new("bg-[#<hex>]", "<Control tw:Tw.Class=\"bg-[#3b82f6]\" />"),
        new("text-[#<hex>]", "<Control tw:Tw.Class=\"text-[#3b82f6]\" />"),
        new("border-[#<hex>]", "<Border tw:Tw.Class=\"border-[#3b82f6]\" />"),
        new("text-[#<hex>]/<opacity>", "<Control tw:Tw.Class=\"text-[#3b82f6]/50\" />"),
    ];

    /// <summary>
    /// Initializes the color docs page and seeds the utility reference table.
    /// </summary>
    public ColorUtilities()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
