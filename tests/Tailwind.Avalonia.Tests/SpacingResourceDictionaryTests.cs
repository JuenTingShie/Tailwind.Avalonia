using Avalonia;

namespace Tailwind.Avalonia.Tests;

public class SpacingResourceDictionaryTests
{
    [Fact]
    public void ResourceDictionary_Exposes_Spacing_And_Sizing_StaticResources()
    {
        var dictionary = new SpacingResourceDictionary();

        Assert.Equal(939, dictionary.Count);
        Assert.Equal(4d, Assert.IsType<double>(dictionary["SpacingBase"]));
        Assert.Equal(new Thickness(16), Assert.IsType<Thickness>(dictionary["Padding4"]));
        Assert.Equal(new Thickness(0, 0, 0, -32), Assert.IsType<Thickness>(dictionary["NegativeMarginBottom8"]));
        Assert.Equal(96d, Assert.IsType<double>(dictionary["Width24"]));
        Assert.Equal(2d, Assert.IsType<double>(dictionary["MinWidth0_5"]));
        Assert.Equal(160d, Assert.IsType<double>(dictionary["MaxWidth40"]));
        Assert.Equal(64d, Assert.IsType<double>(dictionary["Height16"]));
        Assert.Equal(24d, Assert.IsType<double>(dictionary["MinHeight6"]));
        Assert.Equal(384d, Assert.IsType<double>(dictionary["MaxHeight96"]));
    }
}