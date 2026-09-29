using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class SelfAlignment : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("self-start / center / end / stretch", "VerticalAlignment: Top / Center / Bottom / Stretch"),
        new("justify-self-start / center / end / stretch", "HorizontalAlignment: Left / Center / Right / Stretch"),
        new("place-self-start / center / end / stretch", "both axes"),
    ];

    /// <summary>
    /// Initializes the self alignment docs page and seeds the utility reference table.
    /// </summary>
    public SelfAlignment()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
