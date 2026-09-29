using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwBoxShadowTests
{
    [Theory]
    [InlineData("shadow-2xs", 1)]
    [InlineData("shadow-xs", 1)]
    [InlineData("shadow-sm", 2)]
    [InlineData("shadow-md", 2)]
    [InlineData("shadow-lg", 2)]
    [InlineData("shadow-xl", 2)]
    [InlineData("shadow-2xl", 1)]
    public void SetClass_Applies_Shadow_Scale_With_Expected_Layer_Count(string className, int expectedLayers)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.Equal(expectedLayers, border.BoxShadow.Count);
    }

    [Fact]
    public void SetClass_Applies_Shadow_Md_Layers_From_Tailwind_Scale()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-md");

        var first = border.BoxShadow[0];
        var second = border.BoxShadow[1];

        Assert.Equal(4d, first.OffsetY);
        Assert.Equal(6d, first.Blur);
        Assert.Equal(-1d, first.Spread);
        Assert.Equal(Color.FromArgb(26, 0, 0, 0), first.Color);
        Assert.Equal(2d, second.OffsetY);
        Assert.Equal(4d, second.Blur);
        Assert.Equal(-2d, second.Spread);
    }

    [Fact]
    public void SetClass_Applies_Shadow_2xl_Layer_From_Tailwind_Scale()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-2xl");

        var layer = border.BoxShadow[0];

        Assert.Equal(25d, layer.OffsetY);
        Assert.Equal(50d, layer.Blur);
        Assert.Equal(-12d, layer.Spread);
        Assert.Equal(Color.FromArgb(64, 0, 0, 0), layer.Color);
    }

    [Fact]
    public void SetClass_Shadow_None_Removes_Shadow()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-lg shadow-none");

        Assert.Equal(0, border.BoxShadow.Count);
    }

    [Fact]
    public void SetClass_Last_Shadow_Wins()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-sm shadow-xl");

        Assert.Equal(20d, border.BoxShadow[0].OffsetY);
    }

    [Fact]
    public void SetClass_Clears_Shadow_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-md");
        Tw.SetClass(border, null);

        Assert.Equal(0, border.BoxShadow.Count);
    }

    [Theory]
    [InlineData("shadow")]
    [InlineData("shadow-huge")]
    [InlineData("shadow-red-500")]
    [InlineData("shadow-[0_4px_6px_black]")]
    public void SetClass_Ignores_Unsupported_Shadow_Tokens(string className)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.Equal(0, border.BoxShadow.Count);
    }

    [Fact]
    public void SetClass_Ignores_Shadow_On_Element_Without_BoxShadow_Property()
    {
        var textBlock = new TextBlock();

        var exception = Record.Exception(() => Tw.SetClass(textBlock, "shadow-md"));

        Assert.Null(exception);
    }
}
