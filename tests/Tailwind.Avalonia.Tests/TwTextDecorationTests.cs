using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwTextDecorationTests
{
    private static TextDecorationLocation[] Locations(TextBlock textBlock) =>
        textBlock.TextDecorations?.Select(static d => d.Location).ToArray() ?? [];

    [Theory]
    [InlineData("underline", TextDecorationLocation.Underline)]
    [InlineData("overline", TextDecorationLocation.Overline)]
    [InlineData("line-through", TextDecorationLocation.Strikethrough)]
    public void SetClass_Applies_Single_Decoration(string className, TextDecorationLocation expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal([expected], Locations(textBlock));
    }

    [Fact]
    public void SetClass_Combines_Underline_And_LineThrough()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "underline line-through");

        Assert.Equal([TextDecorationLocation.Underline, TextDecorationLocation.Strikethrough], Locations(textBlock));
    }

    [Fact]
    public void SetClass_NoUnderline_Clears_Earlier_Decorations()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "underline line-through no-underline");

        Assert.Empty(Locations(textBlock));
    }

    [Fact]
    public void SetClass_Decoration_After_NoUnderline_Applies_Again()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "no-underline underline");

        Assert.Equal([TextDecorationLocation.Underline], Locations(textBlock));
    }

    [Fact]
    public void SetClass_Repeated_Underline_Is_Not_Duplicated()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "underline underline");

        Assert.Single(Locations(textBlock));
    }

    [Fact]
    public void SetClass_Clears_Decoration_When_Class_Removed()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "underline");
        Tw.SetClass(textBlock, null);

        Assert.Empty(Locations(textBlock));
    }

    [Fact]
    public void SetClass_Combines_Decoration_With_Other_Text_Utilities()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "underline italic font-bold");

        Assert.Equal([TextDecorationLocation.Underline], Locations(textBlock));
        Assert.Equal(FontStyle.Italic, textBlock.FontStyle);
        Assert.Equal(FontWeight.Bold, textBlock.FontWeight);
    }
}
