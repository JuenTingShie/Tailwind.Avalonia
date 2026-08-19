using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TailwindColorPaletteTests
{
    [Theory]
    [InlineData("mauve-500")]
    [InlineData("olive-500")]
    [InlineData("mist-500")]
    [InlineData("taupe-500")]
    public void TryGetColor_Returns_False_For_Non_Tailwind_Color_Families(string tokenName)
    {
        var resolved = TailwindColorPalette.TryGetColor(tokenName, out _);

        Assert.False(resolved);
    }

    [Fact]
    public void TryGetColor_Still_Resolves_Real_Tailwind_Families()
    {
        Assert.True(TailwindColorPalette.TryGetColor("red-500", out _));
        Assert.True(TailwindColorPalette.TryGetColor("black", out var black));
        Assert.Equal(Colors.Black, black);
    }

    [Theory]
    [InlineData("-red-500")]
    [InlineData("red--500")]
    [InlineData("red-500-")]
    [InlineData("red500")]
    public void TryGetColor_Returns_False_For_Malformed_Tokens_That_Would_Otherwise_Collapse_To_A_Valid_Suffix(string tokenName)
    {
        var resolved = TailwindColorPalette.TryGetColor(tokenName, out _);

        Assert.False(resolved);
    }
}
