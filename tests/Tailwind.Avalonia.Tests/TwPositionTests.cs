using Avalonia.Controls;

namespace Tailwind.Avalonia.Tests;

public class TwPositionTests
{
    [Theory]
    [InlineData("left-4", 16, double.NaN, double.NaN, double.NaN)]
    [InlineData("top-2", double.NaN, 8, double.NaN, double.NaN)]
    [InlineData("right-[10px]", double.NaN, double.NaN, 10, double.NaN)]
    [InlineData("bottom-1", double.NaN, double.NaN, double.NaN, 4)]
    [InlineData("-left-2", -8, double.NaN, double.NaN, double.NaN)]
    [InlineData("inset-0", 0, 0, 0, 0)]
    [InlineData("inset-x-2", 8, double.NaN, 8, double.NaN)]
    [InlineData("inset-y-3", double.NaN, 12, double.NaN, 12)]
    public void SetClass_Positions_Element_In_Canvas(string className, double left, double top, double right, double bottom)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.Equal(left, Canvas.GetLeft(border));
        Assert.Equal(top, Canvas.GetTop(border));
        Assert.Equal(right, Canvas.GetRight(border));
        Assert.Equal(bottom, Canvas.GetBottom(border));
    }

    [Fact]
    public void SetClass_Later_Class_Overrides_Inset_On_One_Side()
    {
        var border = new Border();

        Tw.SetClass(border, "inset-2 top-6");

        Assert.Equal(24, Canvas.GetTop(border));
        Assert.Equal(8, Canvas.GetLeft(border));
    }

    [Fact]
    public void SetClass_Clears_Position_When_Classes_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "left-4 top-2");
        Tw.SetClass(border, null);

        Assert.True(double.IsNaN(Canvas.GetLeft(border)));
        Assert.True(double.IsNaN(Canvas.GetTop(border)));
    }

    [Theory]
    [InlineData("top-")]
    [InlineData("top-foo")]
    [InlineData("-top-[-2px]")]
    [InlineData("inset-x-")]
    public void SetClass_Ignores_Invalid_Position_Tokens(string className)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.True(double.IsNaN(Canvas.GetTop(border)));
        Assert.True(double.IsNaN(Canvas.GetLeft(border)));
    }

    [Fact]
    public void SetClass_Places_Child_At_Offset_Inside_Canvas()
    {
        var canvas = new Canvas { Width = 200, Height = 200 };
        var child = new Border { Width = 20, Height = 20 };
        canvas.Children.Add(child);
        Tw.SetClass(child, "left-5 top-2");

        canvas.Measure(new global::Avalonia.Size(200, 200));
        canvas.Arrange(new global::Avalonia.Rect(0, 0, 200, 200));

        Assert.Equal(new global::Avalonia.Point(20, 8), child.Bounds.Position);
    }
}
