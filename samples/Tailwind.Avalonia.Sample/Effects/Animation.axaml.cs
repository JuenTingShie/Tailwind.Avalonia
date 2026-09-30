using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Effects;

public partial class Animation : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("animate-spin", "RotateTransform.Angle 0 to 360, 1s linear"),
        new("animate-ping", "ScaleTransform 1 to 2 and Opacity 1 to 0, 1s"),
        new("animate-pulse", "Opacity 1, 0.5, 1 over 2s"),
        new("animate-bounce", "TranslateTransform.Y bounce, 1s"),
        new("animate-none", "stops the animation"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public Animation()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
