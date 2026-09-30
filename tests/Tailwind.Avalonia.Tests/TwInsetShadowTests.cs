using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwInsetShadowTests
{
    private static BoxShadow[] Shadows(Border border)
    {
        var list = new List<BoxShadow>();

        foreach (var shadow in border.BoxShadow)
        {
            list.Add(shadow);
        }

        return list.ToArray();
    }

    [Theory]
    [InlineData("inset-shadow-2xs", 1, 0)]
    [InlineData("inset-shadow-xs", 1, 1)]
    [InlineData("inset-shadow-sm", 2, 4)]
    [InlineData("shadow-inner", 2, 4)]
    public void SetClass_Applies_Inset_Shadow(string token, double offsetY, double blur)
    {
        var border = new Border();

        Tw.SetClass(border, token);

        var shadow = Assert.Single(Shadows(border));
        Assert.True(shadow.IsInset);
        Assert.Equal(offsetY, shadow.OffsetY);
        Assert.Equal(blur, shadow.Blur);
    }

    [Fact]
    public void SetClass_Combines_Inset_With_Outer_Shadow_And_Ring()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-md ring-2 inset-shadow-sm");

        var shadows = Shadows(border);
        Assert.Equal(4, shadows.Length);
        Assert.Equal(2, shadows[0].Spread);
        Assert.False(shadows[0].IsInset);
        Assert.True(shadows[^1].IsInset);
    }

    [Fact]
    public void SetClass_Inset_Shadow_None_And_Removal_Restore_Default()
    {
        var border = new Border();

        Tw.SetClass(border, "inset-shadow-sm");
        Tw.SetClass(border, "inset-shadow-none");

        Assert.Empty(Shadows(border));
    }
}
