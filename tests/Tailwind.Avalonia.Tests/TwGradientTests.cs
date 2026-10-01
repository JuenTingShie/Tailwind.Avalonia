using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwGradientTests
{
    private static LinearGradientBrush GradientOf(Border border) =>
        Assert.IsType<LinearGradientBrush>(border.Background);

    [Fact]
    public void SetClass_Builds_Two_Stop_Gradient()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-linear-to-r from-red-500 to-blue-500");

        var brush = GradientOf(border);
        Assert.Equal(2, brush.GradientStops.Count);
        Assert.Equal(Color.Parse("#fb2c36"), brush.GradientStops[0].Color);
        Assert.Equal(0, brush.GradientStops[0].Offset);
        Assert.Equal(1, brush.GradientStops[1].Offset);
    }

    [Fact]
    public void SetClass_Via_Adds_Middle_Stop()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-linear-to-b from-red-500 via-white to-blue-500");

        var brush = GradientOf(border);
        Assert.Equal(3, brush.GradientStops.Count);
        Assert.Equal(0.5, brush.GradientStops[1].Offset);
        Assert.Equal(Colors.White, brush.GradientStops[1].Color);
    }

    [Theory]
    [InlineData("bg-linear-to-r", 0, 0.5, 1, 0.5)]
    [InlineData("bg-linear-to-l", 1, 0.5, 0, 0.5)]
    [InlineData("bg-linear-to-t", 0.5, 1, 0.5, 0)]
    [InlineData("bg-linear-to-b", 0.5, 0, 0.5, 1)]
    [InlineData("bg-linear-to-tr", 0, 1, 1, 0)]
    [InlineData("bg-linear-to-br", 0, 0, 1, 1)]
    [InlineData("bg-linear-to-bl", 1, 0, 0, 1)]
    [InlineData("bg-linear-to-tl", 1, 1, 0, 0)]
    [InlineData("bg-linear-to-r", 0, 0.5, 1, 0.5)]
    public void SetClass_Applies_Direction(string direction, double x1, double y1, double x2, double y2)
    {
        var border = new Border();

        Tw.SetClass(border, $"{direction} from-white to-black");

        var brush = GradientOf(border);
        Assert.Equal(new RelativePoint(x1, y1, RelativeUnit.Relative), brush.StartPoint);
        Assert.Equal(new RelativePoint(x2, y2, RelativeUnit.Relative), brush.EndPoint);
    }

    [Fact]
    public void SetClass_Missing_Stop_Is_Transparent()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-linear-to-r from-red-500");

        var brush = GradientOf(border);
        Assert.Equal(Colors.Transparent, brush.GradientStops[1].Color);
    }

    [Fact]
    public void SetClass_Supports_Opacity_And_Arbitrary_Colors()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-linear-to-r from-[#ff0000] to-blue-500/50");

        var brush = GradientOf(border);
        Assert.Equal(Color.Parse("#ff0000"), brush.GradientStops[0].Color);
        Assert.Equal(128, brush.GradientStops[1].Color.A);
    }

    [Fact]
    public void SetClass_Gradient_Replaces_Background_Color()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-red-500 bg-linear-to-r from-white to-black");

        Assert.IsType<LinearGradientBrush>(border.Background);
    }

    [Fact]
    public void SetClass_Stops_Without_Direction_Do_Not_Set_Background()
    {
        var border = new Border();

        Tw.SetClass(border, "from-red-500 to-blue-500");

        Assert.Null(border.Background);
    }

    [Fact]
    public void SetClass_Direction_Without_Stops_Does_Not_Set_Background()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-linear-to-r");

        Assert.Null(border.Background);
    }

    [Fact]
    public void SetClass_Clears_Gradient_When_Classes_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-linear-to-r from-red-500 to-blue-500");
        Tw.SetClass(border, null);

        Assert.Null(border.Background);
    }

    [Fact]
    public void SetClass_Ignores_Invalid_Stop_Colors()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-linear-to-r from-notacolor to-blue-500");

        var brush = GradientOf(border);
        Assert.Equal(Colors.Transparent, brush.GradientStops[0].Color);
    }
}
