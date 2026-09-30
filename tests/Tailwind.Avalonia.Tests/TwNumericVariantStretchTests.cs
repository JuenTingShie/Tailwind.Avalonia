using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwNumericVariantStretchTests
{
    private static string[] Tags(TextBlock text) =>
        (text.FontFeatures ?? []).Select(f => f.Tag!).ToArray();

    [Fact]
    public void SetClass_Numeric_Variants_Combine_Into_Feature_List()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "tabular-nums slashed-zero");

        Assert.Equal(["tnum", "zero"], Tags(text));
    }

    [Fact]
    public void SetClass_NormalNums_Resets_And_Removal_Restores_Default()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "tabular-nums normal-nums lining-nums");
        Assert.Equal(["lnum"], Tags(text));

        Tw.SetClass(text, "");
        Assert.Empty(Tags(text));
    }

    [Theory]
    [InlineData("font-stretch-condensed", FontStretch.Condensed)]
    [InlineData("font-stretch-ultra-expanded", FontStretch.UltraExpanded)]
    [InlineData("font-stretch-normal", FontStretch.Normal)]
    public void SetClass_FontStretch_Sets_Stretch(string token, FontStretch expected)
    {
        var text = new TextBlock();

        Tw.SetClass(text, token);

        Assert.Equal(expected, text.FontStretch);
    }
}
