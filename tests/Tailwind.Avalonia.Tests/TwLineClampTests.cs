using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwLineClampTests
{
    [Theory]
    [InlineData("line-clamp-1", 1)]
    [InlineData("line-clamp-3", 3)]
    [InlineData("line-clamp-12", 12)]
    public void SetClass_Clamps_Lines_With_Ellipsis(string className, int expectedLines)
    {
        var textBlock = new TextBlock { TextWrapping = TextWrapping.NoWrap };

        Tw.SetClass(textBlock, className);

        Assert.Equal(expectedLines, textBlock.MaxLines);
        Assert.Equal(TextTrimming.CharacterEllipsis, textBlock.TextTrimming);
        Assert.Equal(TextWrapping.Wrap, textBlock.TextWrapping);
    }

    [Fact]
    public void SetClass_LineClampNone_Removes_The_Limit()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "line-clamp-2 line-clamp-none");

        Assert.Equal(0, textBlock.MaxLines);
        Assert.Equal(TextTrimming.None, textBlock.TextTrimming);
    }

    [Theory]
    [InlineData("line-clamp-")]
    [InlineData("line-clamp-0")]
    [InlineData("line-clamp--2")]
    [InlineData("line-clamp-x")]
    [InlineData("line-clamp-1.5")]
    public void SetClass_Ignores_Invalid_Line_Clamp(string className)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(0, textBlock.MaxLines);
    }

    [Fact]
    public void SetClass_Clears_Line_Clamp_When_Class_Removed()
    {
        var textBlock = new TextBlock();
        var wrapping = textBlock.TextWrapping;
        var trimming = textBlock.TextTrimming;

        Tw.SetClass(textBlock, "line-clamp-2");
        Tw.SetClass(textBlock, null);

        Assert.Equal(0, textBlock.MaxLines);
        Assert.Equal(trimming, textBlock.TextTrimming);
        Assert.Equal(wrapping, textBlock.TextWrapping);
    }
}
