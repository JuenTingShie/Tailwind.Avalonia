using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwFontWeightTests
{
    [Theory]
    [InlineData("font-thin", FontWeight.Thin)]
    [InlineData("font-extralight", FontWeight.ExtraLight)]
    [InlineData("font-light", FontWeight.Light)]
    [InlineData("font-normal", FontWeight.Normal)]
    [InlineData("font-medium", FontWeight.Medium)]
    [InlineData("font-semibold", FontWeight.SemiBold)]
    [InlineData("font-bold", FontWeight.Bold)]
    [InlineData("font-extrabold", FontWeight.ExtraBold)]
    [InlineData("font-black", FontWeight.Black)]
    public void SetClass_Applies_Font_Weight(string className, FontWeight expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(expected, textBlock.FontWeight);
    }

    [Fact]
    public void SetClass_Last_Font_Weight_Wins()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "font-light font-bold");

        Assert.Equal(FontWeight.Bold, textBlock.FontWeight);
    }

    [Fact]
    public void SetClass_Applies_Font_Weight_To_Button()
    {
        var button = new Button();

        Tw.SetClass(button, "font-semibold");

        Assert.Equal(FontWeight.SemiBold, button.FontWeight);
    }

    [Fact]
    public void SetClass_Combines_Font_Weight_With_Other_Text_Utilities()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "font-bold text-lg text-center");

        Assert.Equal(FontWeight.Bold, textBlock.FontWeight);
        Assert.Equal(18d, textBlock.FontSize);
        Assert.Equal(TextAlignment.Center, textBlock.TextAlignment);
    }

    [Fact]
    public void SetClass_Clears_Font_Weight_When_Class_Removed()
    {
        var textBlock = new TextBlock();
        var defaultWeight = textBlock.FontWeight;

        Tw.SetClass(textBlock, "font-black");
        Tw.SetClass(textBlock, null);

        Assert.Equal(defaultWeight, textBlock.FontWeight);
    }

    [Fact]
    public void SetClass_Clears_Font_Weight_When_Replaced_By_Other_Classes()
    {
        var textBlock = new TextBlock();
        var defaultWeight = textBlock.FontWeight;

        Tw.SetClass(textBlock, "font-black text-lg");
        Tw.SetClass(textBlock, "text-lg");

        Assert.Equal(defaultWeight, textBlock.FontWeight);
        Assert.Equal(18d, textBlock.FontSize);
    }

    [Fact]
    public void SetClass_Ignores_Font_Weight_On_Element_Without_Property()
    {
        var border = new Border();

        var exception = Record.Exception(() => Tw.SetClass(border, "font-bold"));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData("font-heavy")]
    [InlineData("hover:font-bold")]
    public void SetClass_Ignores_Unsupported_Font_Weight_Tokens(string className)
    {
        var textBlock = new TextBlock();
        var defaultWeight = textBlock.FontWeight;

        Tw.SetClass(textBlock, className);

        Assert.Equal(defaultWeight, textBlock.FontWeight);
    }
}
