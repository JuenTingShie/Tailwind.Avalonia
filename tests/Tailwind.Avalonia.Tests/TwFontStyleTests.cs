using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwFontStyleTests
{
    [Theory]
    [InlineData("italic", FontStyle.Italic)]
    [InlineData("not-italic", FontStyle.Normal)]
    public void SetClass_Applies_Font_Style(string className, FontStyle expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(expected, textBlock.FontStyle);
    }

    [Fact]
    public void SetClass_Last_Font_Style_Wins()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "italic not-italic");

        Assert.Equal(FontStyle.Normal, textBlock.FontStyle);
    }

    [Fact]
    public void SetClass_Combines_Italic_With_Font_Weight()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "italic font-bold");

        Assert.Equal(FontStyle.Italic, textBlock.FontStyle);
        Assert.Equal(FontWeight.Bold, textBlock.FontWeight);
    }

    [Fact]
    public void SetClass_Clears_Font_Style_When_Class_Removed()
    {
        var textBlock = new TextBlock();
        var defaultStyle = textBlock.FontStyle;

        Tw.SetClass(textBlock, "italic");
        Tw.SetClass(textBlock, null);

        Assert.Equal(defaultStyle, textBlock.FontStyle);
    }

    [Fact]
    public void SetClass_Keeps_Font_Weight_When_Only_Font_Style_Is_Removed()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "italic font-bold");
        Tw.SetClass(textBlock, "font-bold");

        Assert.Equal(FontStyle.Normal, textBlock.FontStyle);
        Assert.Equal(FontWeight.Bold, textBlock.FontWeight);
    }

    [Fact]
    public void SetClass_Ignores_Font_Style_On_Element_Without_Property()
    {
        var border = new Border();

        var exception = Record.Exception(() => Tw.SetClass(border, "italic"));

        Assert.Null(exception);
    }
}
