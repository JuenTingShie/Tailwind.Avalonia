using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwShapePaintTests
{
    [Fact]
    public void SetClass_Fill_And_Stroke_Colors_Apply_To_Shapes()
    {
        Assert.True(TailwindColorPalette.TryGetColor("sky-500", out var sky));
        Assert.True(TailwindColorPalette.TryGetColor("rose-600", out var rose));
        var ellipse = new Ellipse();

        Tw.SetClass(ellipse, "fill-sky-500 stroke-rose-600");

        Assert.Equal(sky, Assert.IsType<SolidColorBrush>(ellipse.Fill).Color);
        Assert.Equal(rose, Assert.IsType<SolidColorBrush>(ellipse.Stroke).Color);
    }

    [Theory]
    [InlineData("stroke-0", 0)]
    [InlineData("stroke-1", 1)]
    [InlineData("stroke-2", 2)]
    [InlineData("stroke-[3.5px]", 3.5)]
    public void SetClass_Stroke_Number_Sets_Thickness(string token, double expected)
    {
        var path = new global::Avalonia.Controls.Shapes.Path();

        Tw.SetClass(path, token);

        Assert.Equal(expected, path.StrokeThickness);
    }

    [Fact]
    public void SetClass_FillNone_StrokeNone_Use_Transparent_And_Removal_Restores()
    {
        var rect = new Rectangle();

        Tw.SetClass(rect, "fill-none stroke-none");

        Assert.Equal(Colors.Transparent, Assert.IsAssignableFrom<ISolidColorBrush>(rect.Fill).Color);
        Assert.Equal(Colors.Transparent, Assert.IsAssignableFrom<ISolidColorBrush>(rect.Stroke).Color);

        Tw.SetClass(rect, "");

        Assert.Null(rect.Fill);
        Assert.Null(rect.Stroke);
    }
}
