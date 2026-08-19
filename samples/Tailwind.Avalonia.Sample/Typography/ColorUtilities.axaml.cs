using Avalonia.Controls;
using Avalonia.Interactivity;

using Tailwind.Avalonia.Sample.Spacing;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class ColorUtilities : UserControl
{
    private const int CollapsedUtilityRowCount = 4;

    private static readonly SpacingUtilityReferenceRow[] AllUtilityRows =
    [
        new("bg-&lt;color&gt;", "<Control tw:Tw.Class=\"bg-blue-500\" />"),
        new("text-&lt;color&gt;", "<Control tw:Tw.Class=\"text-blue-500\" />"),
        new("border-&lt;color&gt;", "<Border tw:Tw.Class=\"border-blue-500\" />"),
        new("*&lt;color&gt;/&lt;opacity&gt;", "<Control tw:Tw.Class=\"bg-blue-500/50\" />"),
        new("bg-[#&lt;hex&gt;]", "<Control tw:Tw.Class=\"bg-[#3b82f6]\" />"),
        new("text-[#&lt;hex&gt;]", "<Control tw:Tw.Class=\"text-[#3b82f6]\" />"),
        new("border-[#&lt;hex&gt;]", "<Border tw:Tw.Class=\"border-[#3b82f6]\" />"),
        new("text-[#&lt;hex&gt;]/&lt;opacity&gt;", "<Control tw:Tw.Class=\"text-[#3b82f6]/50\" />"),
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