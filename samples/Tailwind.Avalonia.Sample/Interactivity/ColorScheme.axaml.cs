using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Interactivity;

public partial class ColorScheme : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("scheme-light", "RequestedThemeVariant: Light"),
        new("scheme-dark", "RequestedThemeVariant: Dark"),
        new("scheme-normal", "RequestedThemeVariant: Default"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public ColorScheme()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
