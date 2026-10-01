using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

/// <summary>Scales and defaults that follow Tailwind CSS v4.3 values.</summary>
public class TwV4ValueTests
{
    [Theory]
    [InlineData("p-13", 52)]
    [InlineData("p-17", 68)]
    [InlineData("w-0.75", 3)]
    [InlineData("p-100", 400)]
    public void Spacing_Accepts_Any_Quarter_Step(string className, double expected)
    {
        var border = new Border();
        Tw.SetClass(border, className + " " + className.Replace("p-", "w-"));
        Assert.Equal(expected, className.StartsWith("p-", StringComparison.Ordinal) ? border.Padding.Left : border.Width);
    }

    [Theory]
    [InlineData("p-1.3")]
    [InlineData("p-1.")]
    [InlineData("p-.5")]
    public void Spacing_Rejects_Non_Quarter_Steps(string className)
    {
        var border = new Border();
        Tw.SetClass(border, className);
        Assert.Equal(default, border.Padding);
    }

    [Theory]
    [InlineData("text-xs", 12, 16)]
    [InlineData("text-sm", 14, 20)]
    [InlineData("text-base", 16, 24)]
    [InlineData("text-xl", 20, 28)]
    [InlineData("text-4xl", 36, 40)]
    [InlineData("text-6xl", 60, 60)]
    public void Named_Text_Sizes_Set_Their_Line_Height(string className, double size, double lineHeight)
    {
        var text = new TextBlock();
        Tw.SetClass(text, className);
        Assert.Equal(size, text.FontSize);
        Assert.Equal(lineHeight, text.LineHeight, 3);
    }

    [Theory]
    [InlineData("leading-8 text-sm", 32)]
    [InlineData("text-sm leading-8", 32)]
    [InlineData("text-sm/6", 24)]
    public void Leading_Wins_Over_The_Size_Line_Height(string className, double expected)
    {
        var text = new TextBlock();
        Tw.SetClass(text, className);
        Assert.Equal(expected, text.LineHeight, 3);
    }

    [Fact]
    public void Arbitrary_Text_Size_Leaves_Line_Height_Alone()
    {
        var text = new TextBlock();
        Tw.SetClass(text, "text-[14px]");
        Assert.True(double.IsNaN(text.LineHeight));
    }

    [Fact]
    public void Ring_Defaults_To_Current_Color()
    {
        var border = new Border();
        Tw.SetClass(border, "ring-2");
        border.ApplyStyling();
        var ring = border.BoxShadow[0];
        Assert.Equal(2, ring.Spread);
        Assert.Equal(Colors.Black, ring.Color);
    }

    [Fact]
    public void Ring_Uses_The_Text_Color_In_The_Same_List()
    {
        var border = new Border();
        Tw.SetClass(border, "ring text-sky-500");
        Assert.Equal(Color.Parse("#00a6f4"), border.BoxShadow[0].Color);
    }

    [Theory]
    [InlineData("ring-3", 3)]
    [InlineData("ring-6", 6)]
    public void Ring_Accepts_Any_Width(string className, double expected)
    {
        var border = new Border();
        Tw.SetClass(border, className + " ring-red-500");
        Assert.Equal(expected, border.BoxShadow[0].Spread);
    }

    [Theory]
    [InlineData("decoration-3", 3)]
    [InlineData("underline-offset-6", 6)]
    public void Decoration_Metrics_Accept_Any_Number(string className, double expected)
    {
        var text = new TextBlock();
        Tw.SetClass(text, "underline " + className);
        var decoration = Assert.Single(text.TextDecorations!);
        Assert.Equal(expected, className.StartsWith("decoration", StringComparison.Ordinal) ? decoration.StrokeThickness : decoration.StrokeOffset);
    }

    [Fact]
    public void Stroke_Accepts_Any_Width()
    {
        var shape = new Rectangle();
        Tw.SetClass(shape, "stroke-5");
        Assert.Equal(5, shape.StrokeThickness);
    }

    [Fact]
    public void Drop_Shadow_None_Removes_The_Shadow()
    {
        var border = new Border();
        Tw.SetClass(border, "drop-shadow-none");
        var effect = Assert.IsType<DropShadowEffect>(border.Effect);
        Assert.Equal(0, effect.Opacity);
    }
}
