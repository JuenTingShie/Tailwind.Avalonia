using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwTests
{
    [Fact]
    public void SetClass_Applies_Physical_Spacing_Utilities()
    {
        var border = new Border();

        Tw.SetClass(border, "p-4 px-2 pt-3 m-1 mx-2");

        Assert.Equal(new Thickness(8, 12, 8, 16), border.Padding);
        Assert.Equal(new Thickness(8, 4, 8, 4), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Logical_Spacing_For_RightToLeft()
    {
        var border = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(border, "ps-6 pe-2 py-4");

        Assert.Equal(new Thickness(8, 16, 24, 16), border.Padding);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Spacing_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "p-4 m-2");
        Tw.SetClass(border, null);

        Assert.Equal(default, border.Padding);
        Assert.Equal(default, border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Color_Utilities()
    {
        var resources = new ColorResourceDictionary();
        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(textBlock, "text-orange-800");

        Assert.Equal(
            Assert.IsType<SolidColorBrush>(resources["BrushBlue700"]).Color,
            Assert.IsType<SolidColorBrush>(border.Background).Color);
        Assert.Equal(
            Assert.IsType<SolidColorBrush>(resources["BrushGreen800"]).Color,
            Assert.IsType<SolidColorBrush>(border.BorderBrush).Color);
        Assert.Equal(
            Assert.IsType<SolidColorBrush>(resources["BrushOrange800"]).Color,
            Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Color_Utilities_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(border, null);

        Assert.Null(border.Background);
        Assert.Null(border.BorderBrush);
    }

    [Fact]
    public void SetClass_Applies_Transparent_And_Opacity_Color_Utilities()
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));
        Assert.True(TailwindColorPalette.TryGetColor("orange-800", out var orange800));

        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700/50 border-transparent");
        Tw.SetClass(textBlock, "text-orange-800/25");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal((byte)128, background.A);
        Assert.Equal(blue700.R, background.R);
        Assert.Equal(blue700.G, background.G);
        Assert.Equal(blue700.B, background.B);
        Assert.Equal(Colors.Transparent, borderBrush);
        Assert.Equal((byte)64, foreground.A);
        Assert.Equal(orange800.R, foreground.R);
        Assert.Equal(orange800.G, foreground.G);
        Assert.Equal(orange800.B, foreground.B);
    }
}