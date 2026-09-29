using Avalonia.Controls;
using Avalonia.Layout;

namespace Tailwind.Avalonia.Tests;

public class TwSizingKeywordTests
{
    [Fact]
    public void SetClass_WidthFull_Stretches_Horizontally()
    {
        var border = new Border { HorizontalAlignment = HorizontalAlignment.Left, Width = 50 };

        Tw.SetClass(border, "w-full");

        Assert.Equal(HorizontalAlignment.Stretch, border.HorizontalAlignment);
        Assert.True(double.IsNaN(border.Width));
    }

    [Fact]
    public void SetClass_HeightFull_Stretches_Vertically()
    {
        var border = new Border { VerticalAlignment = VerticalAlignment.Top, Height = 50 };

        Tw.SetClass(border, "h-full");

        Assert.Equal(VerticalAlignment.Stretch, border.VerticalAlignment);
        Assert.True(double.IsNaN(border.Height));
    }

    [Fact]
    public void SetClass_Later_Fixed_Width_Overrides_Full()
    {
        var border = new Border { HorizontalAlignment = HorizontalAlignment.Left };

        Tw.SetClass(border, "w-full w-40");

        Assert.Equal(160, border.Width);
        Assert.Equal(HorizontalAlignment.Left, border.HorizontalAlignment);
    }

    [Fact]
    public void SetClass_Explicit_Alignment_Wins_Over_Full()
    {
        var border = new Border();

        Tw.SetClass(border, "justify-self-center w-full");

        Assert.Equal(HorizontalAlignment.Center, border.HorizontalAlignment);
    }

    [Fact]
    public void SetClass_Auto_Clears_Explicit_Size()
    {
        var border = new Border { Width = 80, Height = 40 };

        Tw.SetClass(border, "w-auto h-auto");

        Assert.True(double.IsNaN(border.Width));
        Assert.True(double.IsNaN(border.Height));
    }

    [Fact]
    public void SetClass_Size_Sets_Width_And_Height()
    {
        var border = new Border();

        Tw.SetClass(border, "size-10");

        Assert.Equal(40, border.Width);
        Assert.Equal(40, border.Height);
    }

    [Fact]
    public void SetClass_Size_Supports_Arbitrary_Values_And_Auto()
    {
        var border = new Border { Width = 10, Height = 10 };

        Tw.SetClass(border, "size-[24px]");
        Assert.Equal(24, border.Width);

        Tw.SetClass(border, "size-auto");
        Assert.True(double.IsNaN(border.Width));
        Assert.True(double.IsNaN(border.Height));
    }

    [Fact]
    public void SetClass_MaxWidthNone_Lifts_Maximum()
    {
        var border = new Border { MaxWidth = 100, MaxHeight = 100 };

        Tw.SetClass(border, "max-w-none max-h-none");

        Assert.True(double.IsPositiveInfinity(border.MaxWidth));
        Assert.True(double.IsPositiveInfinity(border.MaxHeight));
    }

    [Fact]
    public void SetClass_Clears_Full_Width_When_Class_Removed()
    {
        var border = new Border();
        var alignment = border.HorizontalAlignment;

        Tw.SetClass(border, "w-full");
        Tw.SetClass(border, null);

        Assert.Equal(alignment, border.HorizontalAlignment);
    }

    [Theory]
    [InlineData("min-w-auto")]
    [InlineData("min-w-full")]
    [InlineData("max-w-auto")]
    [InlineData("w-none")]
    [InlineData("size-full")]
    [InlineData("size-")]
    public void SetClass_Ignores_Unsupported_Size_Keywords(string className)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.True(double.IsNaN(border.Width));
        Assert.True(double.IsPositiveInfinity(border.MaxWidth));
        Assert.Equal(0, border.MinWidth);
    }
}
