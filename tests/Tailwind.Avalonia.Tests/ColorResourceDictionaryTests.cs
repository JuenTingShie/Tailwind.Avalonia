using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class ColorResourceDictionaryTests
{
    [Fact]
    public void TailwindPalette_Contains_Expected_Token_Count_And_New_Families()
    {
        Assert.Equal(288, TailwindColorPalette.Tokens.Count);
        Assert.Contains(TailwindColorPalette.Tokens, token => token.ResourceSuffix == "Blue700");
        Assert.Contains(TailwindColorPalette.Tokens, token => token.ResourceSuffix == "Taupe500");
        Assert.Contains(TailwindColorPalette.Tokens, token => token.ResourceSuffix == "White" && token.Color == Color.Parse("#fff"));
        Assert.Contains(TailwindColorPalette.Tokens, token => token.ResourceSuffix == "Black" && token.Color == Color.Parse("#000"));
    }

    [Fact]
    public void ResourceDictionary_Exposes_Color_And_Brush_Pairs()
    {
        var dictionary = new ColorResourceDictionary();
        var color = Assert.IsType<Color>(dictionary["ColorBlue700"]);
        var brush = Assert.IsType<SolidColorBrush>(dictionary["BrushBlue700"]);

        Assert.Equal(576, dictionary.Count);
        Assert.Equal(color, brush.Color);
        Assert.Equal(Color.Parse("#fff"), Assert.IsType<Color>(dictionary["ColorWhite"]));
        Assert.Equal(Color.Parse("#000"), Assert.IsType<Color>(dictionary["ColorBlack"]));
    }
}