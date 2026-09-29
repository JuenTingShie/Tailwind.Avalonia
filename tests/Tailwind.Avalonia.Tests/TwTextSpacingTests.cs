using Avalonia.Controls;

namespace Tailwind.Avalonia.Tests;

public class TwTextSpacingTests
{
    [Theory]
    [InlineData("tracking-tighter", -0.8)]
    [InlineData("tracking-tight", -0.4)]
    [InlineData("tracking-normal", 0)]
    [InlineData("tracking-wide", 0.4)]
    [InlineData("tracking-wider", 0.8)]
    [InlineData("tracking-widest", 1.6)]
    public void SetClass_Tracking_Scales_With_Class_Font_Size(string className, double expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, $"text-base {className}");

        Assert.Equal(expected, textBlock.LetterSpacing, 3);
    }

    [Fact]
    public void SetClass_Tracking_Uses_Element_Font_Size_When_No_Size_Class()
    {
        var textBlock = new TextBlock { FontSize = 20 };

        Tw.SetClass(textBlock, "tracking-widest");

        Assert.Equal(2, textBlock.LetterSpacing, 3);
    }

    [Theory]
    [InlineData("tracking-[3px]", 3)]
    [InlineData("tracking-[-1.5px]", -1.5)]
    public void SetClass_Applies_Arbitrary_Tracking(string className, double expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(expected, textBlock.LetterSpacing);
    }

    [Theory]
    [InlineData("leading-none", 16)]
    [InlineData("leading-tight", 20)]
    [InlineData("leading-snug", 22)]
    [InlineData("leading-normal", 24)]
    [InlineData("leading-relaxed", 26)]
    [InlineData("leading-loose", 32)]
    public void SetClass_Leading_Keywords_Scale_With_Font_Size(string className, double expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, $"text-base {className}");

        Assert.Equal(expected, textBlock.LineHeight, 3);
    }

    [Theory]
    [InlineData("leading-6", 24)]
    [InlineData("leading-10", 40)]
    [InlineData("leading-[18px]", 18)]
    public void SetClass_Applies_Fixed_Leading(string className, double expected)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(expected, textBlock.LineHeight);
    }

    [Fact]
    public void SetClass_Clears_Tracking_And_Leading_When_Classes_Removed()
    {
        var textBlock = new TextBlock();
        var letterSpacing = textBlock.LetterSpacing;
        var lineHeight = textBlock.LineHeight;

        Tw.SetClass(textBlock, "tracking-wide leading-6");
        Tw.SetClass(textBlock, null);

        Assert.Equal(letterSpacing, textBlock.LetterSpacing);
        Assert.Equal(lineHeight, textBlock.LineHeight);
    }

    [Fact]
    public void SetClass_Ignores_Unknown_Tracking_And_Leading()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "tracking-foo leading-bar");

        Assert.Equal(0, textBlock.LetterSpacing);
        Assert.True(double.IsNaN(textBlock.LineHeight));
    }
}
