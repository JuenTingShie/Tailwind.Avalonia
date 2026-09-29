using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwTransformTests
{
    private static Matrix MatrixOf(Border border) => border.RenderTransform!.Value;

    [Theory]
    [InlineData("rotate-90", 90)]
    [InlineData("-rotate-45", -45)]
    [InlineData("rotate-[30deg]", 30)]
    [InlineData("rotate-[-12.5deg]", -12.5)]
    public void SetClass_Applies_Rotation(string className, double degrees)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        var radians = degrees * Math.PI / 180;
        var matrix = MatrixOf(border);
        Assert.Equal(Math.Cos(radians), matrix.M11, 6);
        Assert.Equal(Math.Sin(radians), matrix.M12, 6);
    }

    [Theory]
    [InlineData("scale-150", 1.5, 1.5)]
    [InlineData("scale-x-50", 0.5, 1)]
    [InlineData("scale-y-200", 1, 2)]
    [InlineData("-scale-x-100", -1, 1)]
    [InlineData("scale-[1.25]", 1.25, 1.25)]
    public void SetClass_Applies_Scale(string className, double x, double y)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        var matrix = MatrixOf(border);
        Assert.Equal(x, matrix.M11, 6);
        Assert.Equal(y, matrix.M22, 6);
    }

    [Theory]
    [InlineData("translate-x-4", 16, 0)]
    [InlineData("translate-y-2", 0, 8)]
    [InlineData("-translate-x-2", -8, 0)]
    [InlineData("translate-3", 12, 12)]
    [InlineData("translate-x-[10px]", 10, 0)]
    public void SetClass_Applies_Translate(string className, double x, double y)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        var matrix = MatrixOf(border);
        Assert.Equal(x, matrix.M31, 6);
        Assert.Equal(y, matrix.M32, 6);
    }

    [Fact]
    public void SetClass_Applies_Skew()
    {
        var border = new Border();

        Tw.SetClass(border, "skew-x-12");

        var matrix = MatrixOf(border);
        Assert.Equal(Math.Tan(12 * Math.PI / 180), matrix.M21, 6);
    }

    [Fact]
    public void SetClass_Combines_Multiple_Transforms_Into_One()
    {
        var border = new Border();

        Tw.SetClass(border, "translate-x-4 rotate-90 scale-50");

        var matrix = MatrixOf(border);
        Assert.Equal(16, matrix.M31, 6);
        Assert.Equal(0.5, matrix.M12, 6);
        Assert.Equal(-0.5, matrix.M21, 6);
    }

    [Theory]
    [InlineData("origin-center", 0.5, 0.5)]
    [InlineData("origin-top-left", 0, 0)]
    [InlineData("origin-bottom-right", 1, 1)]
    [InlineData("origin-top", 0.5, 0)]
    [InlineData("origin-left", 0, 0.5)]
    public void SetClass_Applies_Transform_Origin(string className, double x, double y)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.Equal(new RelativePoint(x, y, RelativeUnit.Relative), border.RenderTransformOrigin);
    }

    [Fact]
    public void SetClass_Clears_Transform_When_Classes_Removed()
    {
        var border = new Border();
        var origin = border.RenderTransformOrigin;

        Tw.SetClass(border, "rotate-45 origin-top-left");
        Tw.SetClass(border, null);

        Assert.Null(border.RenderTransform);
        Assert.Equal(origin, border.RenderTransformOrigin);
    }

    [Theory]
    [InlineData("rotate-")]
    [InlineData("rotate-foo")]
    [InlineData("rotate--45")]
    [InlineData("scale-x-")]
    [InlineData("translate-x-foo")]
    [InlineData("translate-x-[-5px]")]
    [InlineData("origin-middle")]
    [InlineData("skew-z-3")]
    public void SetClass_Ignores_Invalid_Transform_Tokens(string className)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.Null(border.RenderTransform);
    }

    [Fact]
    public void SetClass_Combines_Transform_With_Other_Utilities()
    {
        var border = new Border();

        Tw.SetClass(border, "rotate-12 p-4 bg-red-500");

        Assert.NotNull(border.RenderTransform);
        Assert.Equal(16, border.Padding.Left);
        Assert.NotNull(border.Background);
    }
}
