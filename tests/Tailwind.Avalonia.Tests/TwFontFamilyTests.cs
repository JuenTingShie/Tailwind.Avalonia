using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwFontFamilyTests
{
    [Theory]
    [InlineData("font-sans", "Inter")]
    [InlineData("font-serif", "Georgia")]
    [InlineData("font-mono", "Cascadia Mono")]
    public void SetClass_FontFamily_Sets_Stack(string token, string first)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, token);

        Assert.Equal(first, textBlock.FontFamily.FamilyNames[0]);
        Assert.True(textBlock.FontFamily.FamilyNames.Count > 1);
    }

    [Fact]
    public void SetClass_FontFamily_Removal_Restores_Default()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "font-mono");
        Tw.SetClass(textBlock, "font-bold");

        Assert.False(textBlock.IsSet(TextBlock.FontFamilyProperty));
    }
}
