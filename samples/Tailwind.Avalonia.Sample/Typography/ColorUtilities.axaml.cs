using Avalonia.Controls;
using Avalonia.Interactivity;

using Tailwind.Avalonia.Sample.Spacing;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class ColorUtilities : UserControl
{
    private const int CollapsedUtilityRowCount = 4;

    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("bg-&lt;color&gt;", "<Control Background=\"{StaticResource BrushBlue500}\" />"),
        new("text-&lt;color&gt;", "<Control Foreground=\"{StaticResource BrushBlue500}\" />"),
        new("border-&lt;color&gt;", "<Border BorderBrush=\"{StaticResource BrushBlue500}\" />"),
        new("*&lt;color&gt;/&lt;opacity&gt;", "<SolidColorBrush Color=\"{StaticResource ColorBlue500}\" Opacity=\"0.5\" />"),
        new("bg-[#&lt;hex&gt;]", "<Control Background=\"arbitrary hex brush\" />"),
        new("text-[#&lt;hex&gt;]", "<Control Foreground=\"arbitrary hex brush\" />"),
        new("border-[#&lt;hex&gt;]", "<Border BorderBrush=\"arbitrary hex brush\" />"),
        new("text-[#&lt;hex&gt;]/&lt;opacity&gt;", "<Control Foreground=\"arbitrary hex brush with opacity\" />"),
    ];

    private readonly SpacingUtilityReferenceTablePresenter utilityTablePresenter;

    public ColorUtilities()
    {
        InitializeComponent();
        utilityTablePresenter = new SpacingUtilityReferenceTablePresenter(
            ColorUtilityRows,
            ColorUtilityToggleButton,
            AllUtilityRows,
            CollapsedUtilityRowCount);
    }

    private void ToggleUtilityRows(object? sender, RoutedEventArgs e)
    {
        utilityTablePresenter.Toggle();
    }
}