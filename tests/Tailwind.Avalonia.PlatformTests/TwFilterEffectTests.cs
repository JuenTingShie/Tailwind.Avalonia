using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.PlatformTests;

// Effects are thread-affine Avalonia objects, so these tests run on the headless UI thread.
public class TwFilterEffectTests
{
    [Theory]
    [InlineData("blur-none", 0)]
    [InlineData("blur-xs", 4)]
    [InlineData("blur-sm", 8)]
    [InlineData("blur", 8)]
    [InlineData("blur-md", 12)]
    [InlineData("blur-lg", 16)]
    [InlineData("blur-xl", 24)]
    [InlineData("blur-2xl", 40)]
    [InlineData("blur-3xl", 64)]
    [InlineData("blur-[5px]", 5)]
    [InlineData("blur-[2.5px]", 2.5)]
    public void SetClass_Applies_Blur_Radius(string className, double radius)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, className);

            var blur = Assert.IsType<BlurEffect>(border.Effect);
            Assert.Equal(radius, blur.Radius);
        });
    }

    [Theory]
    [InlineData("drop-shadow-xs")]
    [InlineData("drop-shadow-sm")]
    [InlineData("drop-shadow")]
    [InlineData("drop-shadow-md")]
    [InlineData("drop-shadow-lg")]
    [InlineData("drop-shadow-xl")]
    [InlineData("drop-shadow-2xl")]
    public void SetClass_Applies_Drop_Shadow(string className)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, className);

            var shadow = Assert.IsType<DropShadowEffect>(border.Effect);
            Assert.True(shadow.BlurRadius > 0);
            Assert.True(shadow.OffsetY > 0);
        });
    }

    [Fact]
    public void SetClass_Last_Effect_Wins()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "blur-md drop-shadow-lg");

            Assert.IsType<DropShadowEffect>(border.Effect);
        });
    }

    [Fact]
    public void SetClass_Each_Element_Gets_Its_Own_Effect_Instance()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var first = new Border();
            var second = new Border();

            Tw.SetClass(first, "blur-sm");
            Tw.SetClass(second, "blur-sm");

            Assert.NotSame(first.Effect, second.Effect);
        });
    }

    [Fact]
    public void SetClass_Clears_Effect_When_Class_Removed()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "blur-lg");
            Tw.SetClass(border, null);

            Assert.Null(border.Effect);
        });
    }

    [Theory]
    [InlineData("blur-")]
    [InlineData("blur-foo")]
    [InlineData("blur-[-4px]")]
    [InlineData("blur-[px]")]
    [InlineData("drop-shadow-foo")]
    public void SetClass_Ignores_Invalid_Filter_Tokens(string className)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, className);

            Assert.Null(border.Effect);
        });
    }

    [Fact]
    public void SetClass_Combines_Filter_With_Other_Utilities()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "blur-sm p-4 bg-red-500");

            Assert.NotNull(border.Effect);
            Assert.Equal(16, border.Padding.Left);
            Assert.NotNull(border.Background);
        });
    }
}
