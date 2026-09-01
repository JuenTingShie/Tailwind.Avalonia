using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Effects;

public partial class Opacity : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("opacity-<0-100>", "<Control Opacity=\"<value / 100>\" />"),
        new("hover:opacity-<0-100>", "<Control Opacity=\"<value / 100>\" /> while :pointerover"),
        new("pressed:opacity-<0-100>", "<Control Opacity=\"<value / 100>\" /> while :pressed"),
        new("focus:opacity-<0-100>", "<Control Opacity=\"<value / 100>\" /> while :focus"),
    ];

    /// <summary>
    /// Initializes the opacity docs page and seeds the utility reference table.
    /// </summary>
    public Opacity()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
