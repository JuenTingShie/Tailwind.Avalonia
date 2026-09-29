using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwTextAlignTests
{
    [Theory]
    [InlineData("text-left", TextAlignment.Left)]
    [InlineData("text-center", TextAlignment.Center)]
    [InlineData("text-right", TextAlignment.Right)]
    [InlineData("text-justify", TextAlignment.Justify)]
    [InlineData("text-start", TextAlignment.Start)]
    [InlineData("text-end", TextAlignment.End)]
    public void SetClass_Applies_Text_Alignment(string className, TextAlignment expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(expected, textBlock.TextAlignment);
    }

    [Fact]
    public void SetClass_Last_Text_Alignment_Wins()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-left text-center");

        Assert.Equal(TextAlignment.Center, textBlock.TextAlignment);
    }

    [Fact]
    public void SetClass_Combines_Text_Alignment_With_Font_Size_And_Color()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-center text-lg text-[#ff0000]");
        textBlock.ApplyStyling();

        Assert.Equal(TextAlignment.Center, textBlock.TextAlignment);
        Assert.Equal(18d, textBlock.FontSize);
        Assert.Equal(Color.Parse("#ff0000"), Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }

    [Fact]
    public void SetClass_Clears_Text_Alignment_When_Class_Removed()
    {
        var textBlock = new TextBlock();
        var defaultAlignment = textBlock.TextAlignment;

        Tw.SetClass(textBlock, "text-right");
        Tw.SetClass(textBlock, null);

        Assert.Equal(defaultAlignment, textBlock.TextAlignment);
    }

    [Fact]
    public void SetClass_Ignores_Text_Alignment_On_Element_Without_Property()
    {
        var border = new Border();

        var exception = Record.Exception(() => Tw.SetClass(border, "text-center"));

        Assert.Null(exception);
    }

    [Fact]
    public void SetClass_Ignores_Unknown_Text_Keyword()
    {
        var textBlock = new TextBlock();
        var defaultAlignment = textBlock.TextAlignment;

        Tw.SetClass(textBlock, "text-middle");

        Assert.Equal(defaultAlignment, textBlock.TextAlignment);
    }
}
