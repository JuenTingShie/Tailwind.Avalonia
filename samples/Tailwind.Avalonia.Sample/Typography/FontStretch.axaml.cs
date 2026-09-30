using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontStretch : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("font-stretch-ultra-condensed", "FontStretch: UltraCondensed"),
        new("font-stretch-extra-condensed", "FontStretch: ExtraCondensed"),
        new("font-stretch-condensed", "FontStretch: Condensed"),
        new("font-stretch-semi-condensed", "FontStretch: SemiCondensed"),
        new("font-stretch-normal", "FontStretch: Normal"),
        new("font-stretch-semi-expanded", "FontStretch: SemiExpanded"),
        new("font-stretch-expanded", "FontStretch: Expanded"),
        new("font-stretch-extra-expanded", "FontStretch: ExtraExpanded"),
        new("font-stretch-ultra-expanded", "FontStretch: UltraExpanded"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public FontStretch()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
