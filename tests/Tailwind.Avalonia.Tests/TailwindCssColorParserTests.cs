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

    [Fact]
    public void Parse_Converts_Comma_Separated_Rgb_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("rgb(255, 0, 0)");

        Assert.Equal(Color.Parse("#ff0000"), actual);
    }

    [Fact]
    public void Parse_Converts_Space_Separated_Rgb_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("rgb(255 0 0)");

        Assert.Equal(Color.Parse("#ff0000"), actual);
    }

    [Fact]
    public void Parse_Converts_Percentage_Rgb_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("rgb(100% 0% 0%)");

        Assert.Equal(Color.Parse("#ff0000"), actual);
    }

    [Fact]
    public void Parse_Converts_Legacy_Comma_Rgba_With_Alpha_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("rgba(255, 0, 0, 0.5)");

        Assert.Equal(Color.Parse("#80ff0000"), actual);
    }

    [Fact]
    public void Parse_Converts_Modern_Slash_Alpha_Rgb_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("rgb(255 0 0 / 0.5)");

        Assert.Equal(Color.Parse("#80ff0000"), actual);
    }

    [Fact]
    public void Parse_Converts_Comma_Separated_Hsl_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("hsl(0, 100%, 50%)");

        Assert.Equal(Color.Parse("#ff0000"), actual);
    }

    [Fact]
    public void Parse_Converts_Space_Separated_Hsl_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("hsl(120 100% 50%)");

        Assert.Equal(Color.Parse("#00ff00"), actual);
    }

    [Fact]
    public void Parse_Converts_Legacy_Comma_Hsla_With_Alpha_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("hsla(240, 100%, 50%, 0.5)");

        Assert.Equal(Color.Parse("#800000ff"), actual);
    }

    [Fact]
    public void Parse_Converts_Modern_Slash_Alpha_Hsl_To_Expected_Color()
    {
        var actual = TailwindCssColorParser.Parse("hsl(0 100% 50% / 0.5)");

        Assert.Equal(Color.Parse("#80ff0000"), actual);
    }

    [Fact]
    public void Parse_Handles_Trailing_Comma_In_Rgb()
    {
        var actual = TailwindCssColorParser.Parse("rgb(255, 0, 0,)");

        Assert.Equal(Color.Parse("#ff0000"), actual);
    }

    [Theory]
    [InlineData("rgb(255, 0 0)")]
    [InlineData("rgb(255, 0)")]
    [InlineData("rgb(255, 0, 0, 0.5 / 0.5)")]
    [InlineData("hsl(0, 100% 50%)")]
    public void Parse_Throws_For_Mixed_Or_Malformed_Separators(string cssValue)
    {
        Assert.Throws<FormatException>(() => TailwindCssColorParser.Parse(cssValue));
    }

    [Theory]
    [InlineData("hsl(50%, 50%, 50%)")]
    [InlineData("hsl(0, 50, 50%)")]
    public void Parse_Throws_When_Hsl_Saturation_Or_Lightness_Missing_Percent(string cssValue)
    {
        Assert.Throws<FormatException>(() => TailwindCssColorParser.Parse(cssValue));
    }

    [Theory]
    [InlineData("rgb(NaN, 0, 0)")]
    [InlineData("hsl(0, NaN%, 50%)")]
    public void Parse_Throws_For_NonFinite_Rgb_Or_Hsl_Components(string cssValue)
    {
        Assert.Throws<FormatException>(() => TailwindCssColorParser.Parse(cssValue));
    }
}