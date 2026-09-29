using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwRingTests
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
    [InlineData("ring", 1)]
    [InlineData("ring-0", 0)]
    [InlineData("ring-1", 1)]
    [InlineData("ring-2", 2)]
    [InlineData("ring-4", 4)]
    [InlineData("ring-8", 8)]
    [InlineData("ring-[3px]", 3)]
    public void SetClass_Applies_Ring_Width_As_Spread(string className, double spread)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        var shadows = Shadows(border);

        if (spread == 0)
        {
            Assert.Empty(shadows);
            return;
        }

        var ring = Assert.Single(shadows);
        Assert.Equal(spread, ring.Spread);
        Assert.Equal(0, ring.Blur);
        Assert.Equal(0, ring.OffsetX);
        Assert.Equal(0, ring.OffsetY);
    }

    [Fact]
    public void SetClass_Applies_Ring_Color()
    {
        var border = new Border();

        Tw.SetClass(border, "ring-2 ring-red-500");

        Assert.Equal(Color.Parse("#fb2c36"), Assert.Single(Shadows(border)).Color);
    }

    [Fact]
    public void SetClass_Ring_Color_Order_Does_Not_Matter()
    {
        var border = new Border();

        Tw.SetClass(border, "ring-emerald-500 ring-4");

        var ring = Assert.Single(Shadows(border));
        Assert.Equal(4, ring.Spread);
        Assert.Equal(Color.Parse("#00bc7d"), ring.Color);
    }

    [Fact]
    public void SetClass_Ring_Color_Without_Width_Sets_Nothing()
    {
        var border = new Border();

        Tw.SetClass(border, "ring-red-500");

        Assert.Empty(Shadows(border));
    }

    [Fact]
    public void SetClass_Ring_Is_Placed_In_Front_Of_Shadow()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-md ring-2");

        var shadows = Shadows(border);
        Assert.Equal(3, shadows.Length);
        Assert.Equal(2, shadows[0].Spread);
        Assert.Equal(0, shadows[0].Blur);
    }

    [Fact]
    public void SetClass_Ring_Zero_Keeps_Shadow()
    {
        var border = new Border();

        Tw.SetClass(border, "shadow-sm ring-0");

        Assert.NotEmpty(Shadows(border));
        Assert.All(Shadows(border), shadow => Assert.NotEqual(0, shadow.Blur + shadow.OffsetY));
    }

    [Fact]
    public void SetClass_Clears_Ring_When_Classes_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "ring-2 ring-blue-500");
        Tw.SetClass(border, null);

        Assert.Empty(Shadows(border));
    }

    [Theory]
    [InlineData("ring-")]
    [InlineData("ring-3")]
    [InlineData("ring-[-2px]")]
    [InlineData("ring-notacolor")]
    public void SetClass_Ignores_Invalid_Ring_Tokens(string className)
    {
        var border = new Border();

        Tw.SetClass(border, $"ring-2 {className}");

        Assert.Equal(2, Assert.Single(Shadows(border)).Spread);
    }
}
