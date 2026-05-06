namespace Tailwind.Avalonia.Tests;

public class SpacingScaleTests
{
    [Theory]
    [InlineData("px", 1.0)]
    [InlineData("0.5", 2.0)]
    [InlineData("4", 16.0)]
    [InlineData("96", 384.0)]
    public void TryGetPixels_Returns_Expected_Value(string token, double expectedPixels)
    {
        var success = SpacingScale.TryGetPixels(token, out var actualPixels);

        Assert.True(success);
        Assert.Equal(expectedPixels, actualPixels);
    }

    [Fact]
    public void TryGetPixels_Returns_False_For_Unknown_Token()
    {
        var success = SpacingScale.TryGetPixels("999", out _);

        Assert.False(success);
    }

    [Theory]
    [InlineData("px", "Px")]
    [InlineData("0.5", "0_5")]
    [InlineData("4", "4")]
    public void ToResourceSuffix_Returns_Avalonia_Safe_Key_Suffix(string token, string expectedSuffix)
    {
        var actualSuffix = SpacingScale.ToResourceSuffix(token);

        Assert.Equal(expectedSuffix, actualSuffix);
    }
}