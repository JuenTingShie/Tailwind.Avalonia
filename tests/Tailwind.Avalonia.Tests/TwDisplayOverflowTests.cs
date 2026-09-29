using Avalonia.Controls;

namespace Tailwind.Avalonia.Tests;

public class TwDisplayOverflowTests
{
    [Fact]
    public void SetClass_Hidden_Collapses_Element()
    {
        var border = new Border();

        Tw.SetClass(border, "hidden");

        Assert.False(border.IsVisible);
    }

    [Fact]
    public void SetClass_Block_Shows_Element_After_Hidden()
    {
        var border = new Border();

        Tw.SetClass(border, "hidden block");

        Assert.True(border.IsVisible);
    }

    [Fact]
    public void SetClass_Removing_Hidden_Restores_Visibility()
    {
        var border = new Border();

        Tw.SetClass(border, "hidden");
        Tw.SetClass(border, "p-2");

        Assert.True(border.IsVisible);
    }

    [Theory]
    [InlineData("overflow-hidden", true)]
    [InlineData("overflow-clip", true)]
    [InlineData("overflow-visible", false)]
    public void SetClass_Applies_Overflow_As_ClipToBounds(string className, bool expected)
    {
        var border = new Border { ClipToBounds = !expected };

        Tw.SetClass(border, className);

        Assert.Equal(expected, border.ClipToBounds);
    }

    [Fact]
    public void SetClass_Removing_Overflow_Restores_Default_Clip()
    {
        var border = new Border();
        var original = border.ClipToBounds;

        Tw.SetClass(border, "overflow-hidden");
        Tw.SetClass(border, null);

        Assert.Equal(original, border.ClipToBounds);
    }

    [Fact]
    public void SetClass_Combines_Hidden_With_Other_Utilities()
    {
        var border = new Border();

        Tw.SetClass(border, "hidden p-4 rounded-lg");

        Assert.False(border.IsVisible);
        Assert.Equal(16, border.Padding.Left);
    }
}
