using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Effects;

public partial class Transitions : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("transition / transition-all", "colors, opacity, transform and shadow"),
        new("transition-colors", "Background, Foreground, BorderBrush"),
        new("transition-opacity", "Opacity"),
        new("transition-transform", "RenderTransform"),
        new("transition-shadow", "BoxShadow"),
        new("transition-none", "no transitions"),
        new("duration-<ms>", "length, default 150ms"),
        new("delay-<ms>", "start delay"),
        new("ease-linear / in / out / in-out", "easing"),
    ];

    /// <summary>
    /// Initializes the transitions docs page and seeds the utility reference table.
    /// </summary>
    public Transitions()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
