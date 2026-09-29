using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwTextOverflowTests
{
    [Fact]
    public void SetClass_Truncate_Sets_Ellipsis_And_NoWrap()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "truncate");

        Assert.Equal(TextTrimming.CharacterEllipsis, textBlock.TextTrimming);
        Assert.Equal(TextWrapping.NoWrap, textBlock.TextWrapping);
    }

    [Theory]
    [InlineData("text-ellipsis", true)]
    [InlineData("text-clip", false)]
    public void SetClass_Applies_Text_Trimming(string className, bool ellipsis)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(ellipsis ? TextTrimming.CharacterEllipsis : TextTrimming.None, textBlock.TextTrimming);
    }

    [Theory]
    [InlineData("text-wrap", TextWrapping.Wrap)]
    [InlineData("text-nowrap", TextWrapping.NoWrap)]
    public void SetClass_Applies_Text_Wrapping(string className, TextWrapping expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(expected, textBlock.TextWrapping);
    }

    [Fact]
    public void SetClass_Later_Wrap_Overrides_Truncate_Wrapping()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "truncate text-wrap");

        Assert.Equal(TextWrapping.Wrap, textBlock.TextWrapping);
        Assert.Equal(TextTrimming.CharacterEllipsis, textBlock.TextTrimming);
    }

    [Fact]
    public void SetClass_Clears_Text_Overflow_When_Class_Removed()
    {
        var textBlock = new TextBlock();
        var trimming = textBlock.TextTrimming;
        var wrapping = textBlock.TextWrapping;

        Tw.SetClass(textBlock, "truncate");
        Tw.SetClass(textBlock, null);

        Assert.Equal(trimming, textBlock.TextTrimming);
        Assert.Equal(wrapping, textBlock.TextWrapping);
    }

    [Fact]
    public void SetClass_Keeps_Text_Color_Alongside_Text_Overflow()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "truncate text-red-500 text-sm");

        Assert.Equal(TextTrimming.CharacterEllipsis, textBlock.TextTrimming);
        Assert.NotNull(textBlock.Foreground);
        Assert.Equal(14, textBlock.FontSize);
    }
}
