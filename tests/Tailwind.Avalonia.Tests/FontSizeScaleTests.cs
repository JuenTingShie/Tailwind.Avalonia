namespace Tailwind.Avalonia.Tests;

public class FontSizeScaleTests
{
    [Theory]
    [InlineData("xs", 12.0)]
    [InlineData("base", 16.0)]
    [InlineData("4xl", 36.0)]
    [InlineData("9xl", 128.0)]
    public void TryGetPixels_Returns_Expected_Value(string token, double expectedPixels)
    {
        var success = FontSizeScale.TryGetPixels(token, out var actualPixels);

        Assert.True(success);
        Assert.Equal(expectedPixels, actualPixels);
    }

    [Fact]
    public void TryGetPixels_Returns_False_For_Unknown_Token()
    {
        var success = FontSizeScale.TryGetPixels("mega", out _);

        Assert.False(success);
    }

    [Theory]
    [InlineData("xs", "Xs")]
    [InlineData("base", "Base")]
    [InlineData("2xl", "2xl")]
    public void ToResourceSuffix_Returns_Avalonia_Safe_Key_Suffix(string token, string expectedSuffix)
    {
        var actualSuffix = FontSizeScale.ToResourceSuffix(token);

        Assert.Equal(expectedSuffix, actualSuffix);
    }
}