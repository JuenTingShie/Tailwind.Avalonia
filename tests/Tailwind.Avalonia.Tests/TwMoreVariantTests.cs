using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwMoreVariantTests
{
    private static Color ColorOf(Border border) => Assert.IsType<SolidColorBrush>(border.Background).Color;

    private static void SetPseudoClass(Border border, string pseudoClass, bool on)
    {
        var classes = (IPseudoClasses)border.Classes;

        if (on)
        {
            classes.Add(pseudoClass);
        }
        else
        {
            classes.Remove(pseudoClass);
        }

        border.ApplyStyling();
    }

    [Theory]
    [InlineData("disabled:bg-red-500", ":disabled")]
    [InlineData("checked:bg-red-500", ":checked")]
    [InlineData("focus-visible:bg-red-500", ":focus-visible")]
    public void SetClass_Applies_Variant_While_Pseudo_Class_Is_Active(string variantClass, string pseudoClass)
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-500", out var blue500));
        Assert.True(TailwindColorPalette.TryGetColor("red-500", out var red500));
        var border = new Border();

        Tw.SetClass(border, $"bg-blue-500 {variantClass}");

        border.ApplyStyling();
        Assert.Equal(blue500, ColorOf(border));

        SetPseudoClass(border, pseudoClass, true);
        Assert.Equal(red500, ColorOf(border));

        SetPseudoClass(border, pseudoClass, false);
        Assert.Equal(blue500, ColorOf(border));
    }

    [Fact]
    public void SetClass_Applies_Opacity_Variant_For_Disabled()
    {
        var border = new Border();

        Tw.SetClass(border, "opacity-100 disabled:opacity-40");

        border.ApplyStyling();
        Assert.Equal(1, border.Opacity);

        SetPseudoClass(border, ":disabled", true);
        Assert.Equal(0.4, border.Opacity, 3);
    }

    [Fact]
    public void SetClass_Disabled_Beats_Hover_When_Both_Are_Active()
    {
        Assert.True(TailwindColorPalette.TryGetColor("gray-400", out var gray400));
        var border = new Border();

        Tw.SetClass(border, "bg-blue-500 hover:bg-blue-700 disabled:bg-gray-400");

        SetPseudoClass(border, ":pointerover", true);
        SetPseudoClass(border, ":disabled", true);

        Assert.Equal(gray400, ColorOf(border));
    }

    [Fact]
    public void SetClass_Checked_Variant_Follows_ToggleButton_State()
    {
        Assert.True(TailwindColorPalette.TryGetColor("emerald-500", out var emerald500));
        var toggle = new ToggleButton();

        Tw.SetClass(toggle, "checked:bg-emerald-500");

        toggle.ApplyStyling();
        Assert.Null(toggle.Background);

        toggle.IsChecked = true;
        toggle.ApplyStyling();

        Assert.Equal(emerald500, Assert.IsType<SolidColorBrush>(toggle.Background).Color);
    }

    [Fact]
    public void SetClass_Disabled_Variant_Follows_IsEnabled()
    {
        Assert.True(TailwindColorPalette.TryGetColor("gray-400", out var gray400));
        var button = new Button();

        Tw.SetClass(button, "disabled:bg-gray-400");

        button.IsEnabled = false;
        button.ApplyStyling();

        Assert.Equal(gray400, Assert.IsType<SolidColorBrush>(button.Background).Color);
    }
}
