using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Interactivity;

public partial class PlaceholderColor : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("placeholder-<color>", "TextBox.PlaceholderForeground"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public PlaceholderColor()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
