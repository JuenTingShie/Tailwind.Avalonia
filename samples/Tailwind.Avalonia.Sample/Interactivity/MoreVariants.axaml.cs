using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Interactivity;

public partial class MoreVariants : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("disabled:<utility>", ":disabled"),
        new("checked:<utility>", ":checked"),
        new("focus-visible:<utility>", ":focus-visible"),
    ];

    /// <summary>
    /// Initializes the disabled, checked and focus-visible variants docs page and seeds the utility reference table.
    /// </summary>
    public MoreVariants()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
