using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class FontVariantNumeric : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("tabular-nums", "tnum"),
        new("proportional-nums", "pnum"),
        new("lining-nums", "lnum"),
        new("oldstyle-nums", "onum"),
        new("slashed-zero", "zero"),
        new("ordinal", "ordn"),
        new("diagonal-fractions", "frac"),
        new("stacked-fractions", "afrc"),
        new("normal-nums", "reset"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public FontVariantNumeric()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
