namespace Tailwind.Avalonia.Tests;

public class CornerRadiusScaleTests
{
    [Theory]
    [InlineData("none", 0.0)]
    [InlineData("xs", 2.0)]
    [InlineData("sm", 4.0)]
    [InlineData("md", 6.0)]
    [InlineData("lg", 8.0)]
    [InlineData("xl", 12.0)]
    [InlineData("2xl", 16.0)]
    [InlineData("3xl", 24.0)]
    [InlineData("4xl", 32.0)]
    [InlineData("full", 9999.0)]
    public void TryGetPixels_Returns_Expected_Value(string token, double expectedPixels)
    {
        var success = CornerRadiusScale.TryGetPixels(token, out var actualPixels);

        Assert.True(success);
        Assert.Equal(expectedPixels, actualPixels);
    }

    [Fact]
    public void TryGetPixels_Returns_False_For_Unknown_Token()
    {
        var success = CornerRadiusScale.TryGetPixels("999", out _);

        Assert.False(success);
    }
}
