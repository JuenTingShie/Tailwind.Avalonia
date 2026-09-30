using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

/// <summary>Utilities documented by Tailwind v4 that extend existing families with new value forms.</summary>
public class TwParserExtensionTests
{
    [Theory]
    [InlineData("-z-10", -10)]
    [InlineData("-z-50", -50)]
    [InlineData("z-[-3]", -3)]
    public void Negative_Z_Index(string className, int expected)
    {
        var border = new Border();
        Tw.SetClass(border, className);
        Assert.Equal(expected, border.ZIndex);
    }

    [Theory]
    [InlineData("-z-[5]")]
    [InlineData("-z-0")]
    [InlineData("-z-")]
    public void Negative_Z_Index_Rejects_Malformed(string className)
    {
        var border = new Border { ZIndex = 7 };
        Tw.SetClass(border, className);
        Assert.Equal(7, border.ZIndex);
    }

    [Theory]
    [InlineData("rotate-[0.5turn]", 180)]
    [InlineData("rotate-[3.14159265rad]", 180)]
    [InlineData("rotate-[100grad]", 90)]
    [InlineData("rotate-[-0.25turn]", -90)]
    [InlineData("skew-x-[0.1turn]", 36)]
    public void Angle_Units(string className, double degrees)
    {
        var border = new Border();
        Tw.SetClass(border, className);

        var matrix = border.RenderTransform!.Value;

        if (className.StartsWith("skew", StringComparison.Ordinal))
        {
            Assert.Equal(Math.Tan(degrees * Math.PI / 180), matrix.M21, 4);
        }
        else
        {
            Assert.Equal(Math.Cos(degrees * Math.PI / 180), matrix.M11, 4);
            Assert.Equal(Math.Sin(degrees * Math.PI / 180), matrix.M12, 4);
        }
    }

    [Theory]
    [InlineData("opacity-[.67]", 0.67)]
    [InlineData("opacity-[0.5]", 0.5)]
    [InlineData("opacity-[35%]", 0.35)]
    [InlineData("opacity-[1]", 1)]
    public void Arbitrary_Opacity(string className, double expected)
    {
        var border = new Border();
        Tw.SetClass(border, className);
        Assert.Equal(expected, border.Opacity, 6);
    }

    [Theory]
    [InlineData("opacity-[1.5]")]
    [InlineData("opacity-[150%]")]
    [InlineData("opacity-[-0.2]")]
    [InlineData("opacity-[1.]")]
    [InlineData("opacity-[]")]
    public void Arbitrary_Opacity_Rejects_Out_Of_Range(string className)
    {
        var border = new Border();
        Tw.SetClass(border, className);
        Assert.Equal(1, border.Opacity);
    }

    [Fact]
    public void Arbitrary_Opacity_Works_As_A_Color_Modifier()
    {
        var border = new Border();
        Tw.SetClass(border, "bg-sky-500/[.5]");
        var brush = Assert.IsAssignableFrom<ISolidColorBrush>(border.Background);
        Assert.Equal(128, brush.Color.A);
    }

    [Fact]
    public void Arbitrary_Line_Clamp()
    {
        var text = new TextBlock();
        Tw.SetClass(text, "line-clamp-[5]");
        Assert.Equal(5, text.MaxLines);
        Assert.Equal(TextTrimming.CharacterEllipsis, text.TextTrimming);
    }

    [Theory]
    [InlineData("font-[600]", 600)]
    [InlineData("font-[1000]", 1000)]
    [InlineData("font-[1]", 1)]
    public void Arbitrary_Font_Weight(string className, int expected)
    {
        var text = new TextBlock();
        Tw.SetClass(text, className);
        Assert.Equal((FontWeight)expected, text.FontWeight);
    }

    [Theory]
    [InlineData("font-[0]")]
    [InlineData("font-[1001]")]
    [InlineData("font-[bold]")]
    public void Arbitrary_Font_Weight_Rejects_Invalid(string className)
    {
        var text = new TextBlock();
        Tw.SetClass(text, className);
        Assert.Equal(FontWeight.Normal, text.FontWeight);
    }

    [Theory]
    [InlineData("text-base leading-[1.5]", 24)]
    [InlineData("text-base leading-[2em]", 32)]
    [InlineData("text-base leading-[2rem]", 32)]
    [InlineData("text-base leading-[18px]", 18)]
    [InlineData("text-base leading-6", 24)]
    public void Line_Height_Values(string className, double expected)
    {
        var text = new TextBlock();
        Tw.SetClass(text, className);
        Assert.Equal(expected, text.LineHeight, 3);
    }

    [Theory]
    [InlineData("text-sm/6", 14, 24)]
    [InlineData("text-lg/7", 18, 28)]
    [InlineData("text-base/[1.5]", 16, 24)]
    [InlineData("text-[20px]/[30px]", 20, 30)]
    [InlineData("text-sm/6 leading-8", 14, 32)]
    public void Font_Size_With_Line_Height(string className, double size, double lineHeight)
    {
        var text = new TextBlock();
        Tw.SetClass(text, className);
        Assert.Equal(size, text.FontSize);
        Assert.Equal(lineHeight, text.LineHeight, 3);
    }

    [Fact]
    public void Text_Color_With_Opacity_Is_Still_A_Color()
    {
        var text = new TextBlock();
        Tw.SetClass(text, "text-sky-500/50");
        var brush = Assert.IsAssignableFrom<ISolidColorBrush>(text.Foreground);
        Assert.Equal(128, brush.Color.A);
    }

    [Fact]
    public void Size_Full_Stretches_Both_Axes()
    {
        var border = new Border { Width = 40, Height = 40 };
        Tw.SetClass(border, "size-full");
        Assert.True(double.IsNaN(border.Width));
        Assert.True(double.IsNaN(border.Height));
        Assert.Equal(HorizontalAlignment.Stretch, border.HorizontalAlignment);
        Assert.Equal(VerticalAlignment.Stretch, border.VerticalAlignment);
    }

    [Theory]
    [InlineData("max-w-full")]
    [InlineData("max-h-full")]
    public void Max_Full_Lifts_The_Maximum(string className)
    {
        var border = new Border { MaxWidth = 10, MaxHeight = 10 };
        Tw.SetClass(border, "max-w-8 max-h-8 " + className);
        var value = className.StartsWith("max-w", StringComparison.Ordinal) ? border.MaxWidth : border.MaxHeight;
        Assert.True(double.IsPositiveInfinity(value));
    }

    [Fact]
    public void Invisible_Hides_But_Keeps_Layout()
    {
        var border = new Border();
        Tw.SetClass(border, "opacity-50 pointer-events-auto invisible");
        Assert.Equal(0, border.Opacity);
        Assert.False(border.IsHitTestVisible);
        Assert.True(border.IsVisible);
    }

    [Fact]
    public void Visible_Undoes_Invisible()
    {
        var border = new Border();
        Tw.SetClass(border, "invisible opacity-50 visible");
        Assert.Equal(0.5, border.Opacity);
        Assert.True(border.IsHitTestVisible);
    }

    [Fact]
    public void Removing_Invisible_Restores_Defaults()
    {
        var border = new Border();
        Tw.SetClass(border, "invisible");
        Tw.SetClass(border, null);
        Assert.Equal(1, border.Opacity);
        Assert.True(border.IsHitTestVisible);
    }
}
