using Avalonia.Controls;
using Avalonia.Layout;

namespace Tailwind.Avalonia.Tests;

public class TwSelfAlignmentTests
{
    [Theory]
    [InlineData("self-start", VerticalAlignment.Top)]
    [InlineData("self-center", VerticalAlignment.Center)]
    [InlineData("self-end", VerticalAlignment.Bottom)]
    [InlineData("self-stretch", VerticalAlignment.Stretch)]
    public void SetClass_Self_Aligns_Vertically(string className, VerticalAlignment expected)
    {
        var border = new Border { HorizontalAlignment = HorizontalAlignment.Right };

        Tw.SetClass(border, className);

        Assert.Equal(expected, border.VerticalAlignment);
        Assert.Equal(HorizontalAlignment.Right, border.HorizontalAlignment);
    }

    [Theory]
    [InlineData("justify-self-start", HorizontalAlignment.Left)]
    [InlineData("justify-self-center", HorizontalAlignment.Center)]
    [InlineData("justify-self-end", HorizontalAlignment.Right)]
    [InlineData("justify-self-stretch", HorizontalAlignment.Stretch)]
    public void SetClass_JustifySelf_Aligns_Horizontally(string className, HorizontalAlignment expected)
    {
        var border = new Border { VerticalAlignment = VerticalAlignment.Bottom };

        Tw.SetClass(border, className);

        Assert.Equal(expected, border.HorizontalAlignment);
        Assert.Equal(VerticalAlignment.Bottom, border.VerticalAlignment);
    }

    [Fact]
    public void SetClass_PlaceSelf_Aligns_Both_Axes()
    {
        var border = new Border();

        Tw.SetClass(border, "place-self-end");

        Assert.Equal(VerticalAlignment.Bottom, border.VerticalAlignment);
        Assert.Equal(HorizontalAlignment.Right, border.HorizontalAlignment);
    }

    [Fact]
    public void SetClass_Later_Class_Overrides_Place_Self_On_One_Axis()
    {
        var border = new Border();

        Tw.SetClass(border, "place-self-end self-center");

        Assert.Equal(VerticalAlignment.Center, border.VerticalAlignment);
        Assert.Equal(HorizontalAlignment.Right, border.HorizontalAlignment);
    }

    [Fact]
    public void SetClass_Clears_Alignment_When_Class_Removed()
    {
        var border = new Border();
        var vertical = border.VerticalAlignment;
        var horizontal = border.HorizontalAlignment;

        Tw.SetClass(border, "place-self-center");
        Tw.SetClass(border, null);

        Assert.Equal(vertical, border.VerticalAlignment);
        Assert.Equal(horizontal, border.HorizontalAlignment);
    }
}
