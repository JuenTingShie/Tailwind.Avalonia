using Avalonia.Controls;

namespace Tailwind.Avalonia.Tests;

public class TwZIndexTests
{
    [Theory]
    [InlineData("z-0", 0)]
    [InlineData("z-10", 10)]
    [InlineData("z-50", 50)]
    [InlineData("z-7", 7)]
    [InlineData("z-999", 999)]
    [InlineData("z-[25]", 25)]
    [InlineData("z-[-3]", -3)]
    [InlineData("z-auto", 0)]
    public void SetClass_Applies_Z_Index(string className, int expected)
    {
        var border = new Border { ZIndex = 99 };

        Tw.SetClass(border, className);

        Assert.Equal(expected, border.ZIndex);
    }

    [Theory]
    [InlineData("z-")]
    [InlineData("z-foo")]
    [InlineData("z-1.5")]
    [InlineData("z-[5")]
    [InlineData("z-5]")]
    [InlineData("z-[]")]
    [InlineData("z-99999999999")]
    public void SetClass_Ignores_Invalid_Z_Index(string className)
    {
        var border = new Border { ZIndex = 4 };

        Tw.SetClass(border, className);

        Assert.Equal(4, border.ZIndex);
    }

    [Fact]
    public void SetClass_Clears_Z_Index_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "z-20");
        Tw.SetClass(border, null);

        Assert.Equal(0, border.ZIndex);
    }

    [Fact]
    public void SetClass_Last_Z_Index_Wins()
    {
        var border = new Border();

        Tw.SetClass(border, "z-10 z-30");

        Assert.Equal(30, border.ZIndex);
    }
}
