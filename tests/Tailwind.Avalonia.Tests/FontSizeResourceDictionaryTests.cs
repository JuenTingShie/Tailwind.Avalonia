namespace Tailwind.Avalonia.Tests;

public class FontSizeResourceDictionaryTests
{
    [Fact]
    public void ResourceDictionary_Exposes_Font_Size_StaticResources()
    {
        var dictionary = new FontSizeResourceDictionary();

        Assert.Equal(13, dictionary.Count);
        Assert.Equal(12d, Assert.IsType<double>(dictionary["FontSizeXs"]));
        Assert.Equal(16d, Assert.IsType<double>(dictionary["FontSizeBase"]));
        Assert.Equal(24d, Assert.IsType<double>(dictionary["FontSize2xl"]));
        Assert.Equal(128d, Assert.IsType<double>(dictionary["FontSize9xl"]));
    }
}