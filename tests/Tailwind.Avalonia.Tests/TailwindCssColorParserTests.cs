using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TailwindCssColorParserTests
{
    [Fact]
    public void Parse_Returns_Hex_Color_Unchanged()
    {
        var actual = TailwindCssColorParser.Parse("#fff");

        Assert.Equal(Color.Parse("#fff"), actual);
    }

    [Fact]
    public void Parse_Converts_Achromatic_Black_Oklch_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("oklch(0% 0 0)");

        Assert.Equal(Color.Parse("#000"), actual);
    }

    [Fact]
    public void Parse_Accepts_Alpha_And_Wraps_Hue()
    {
        var actual = TailwindCssColorParser.Parse("oklch(0% 0 720 / 50%)");

        Assert.Equal(Color.Parse("#80000000"), actual);
    }

    [Fact]
    public void Parse_Converts_Achromatic_White_Oklch_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("oklch(100% 0 0)");

        Assert.Equal(Color.Parse("#fff"), actual);
    }

    [Fact]
    public void Parse_Throws_For_Unsupported_Syntax()
    {
        var error = Assert.Throws<FormatException>(() => TailwindCssColorParser.Parse("lab(50% 0 0)"));

        Assert.Equal("Unsupported Tailwind color value 'lab(50% 0 0)'.", error.Message);
    }

    [Theory]
    [InlineData("oklch(50% 0.1 NaNdeg)")]
    [InlineData("oklch(50% 0.1 Infinitydeg)")]
    [InlineData("oklch(NaN% 0.1 20deg)")]
    [InlineData("oklch(50% NaN 20deg)")]
    [InlineData("oklch(50% 0.1 20deg / NaN%)")]
    public void Parse_Throws_For_NonFinite_Oklch_Components(string cssValue)
    {
        Assert.Throws<FormatException>(() => TailwindCssColorParser.Parse(cssValue));
    }
}