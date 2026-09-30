using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Interactivity;

public partial class StructuralVariants : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("first:<utility>", ":nth-child(1)"),
        new("last:<utility>", ":nth-last-child(1)"),
        new("odd:<utility>", ":nth-child(2n+1)"),
        new("even:<utility>", ":nth-child(2n)"),
    ];

    /// <summary>
    /// Initializes the first, last, odd and even variants docs page and seeds the utility reference table.
    /// </summary>
    public StructuralVariants()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
