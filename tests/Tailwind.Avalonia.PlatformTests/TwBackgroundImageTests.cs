using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.PlatformTests;

public class TwBackgroundImageTests
{
    private const string Pattern = "avares://Tailwind.Avalonia.PlatformTests/Assets/pattern.png";

    private static ImageBrush BrushOf(Border border) => Assert.IsType<ImageBrush>(border.Background);

    [Fact]
    public void SetClass_Url_Loads_An_ImageBrush_With_Css_Defaults()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, $"bg-[url({Pattern})]");

            var brush = BrushOf(border);
            Assert.NotNull(brush.Source);
            Assert.Equal(Stretch.None, brush.Stretch);
            Assert.Equal(TileMode.Tile, brush.TileMode);
            Assert.Equal(AlignmentX.Left, brush.AlignmentX);
            Assert.Equal(AlignmentY.Top, brush.AlignmentY);
            Assert.Equal(RelativeUnit.Absolute, brush.DestinationRect.Unit);
            // The headless bitmap decoder may report a stub size, so compare with what the brush's own source reports.
            Assert.Equal(((global::Avalonia.Media.Imaging.Bitmap)brush.Source!).Size.Width, brush.DestinationRect.Rect.Width, 1);
            Assert.Equal(((global::Avalonia.Media.Imaging.Bitmap)brush.Source!).Size.Height, brush.DestinationRect.Rect.Height, 1);
        });
    }

    [Theory]
    [InlineData("bg-cover", Stretch.UniformToFill)]
    [InlineData("bg-contain", Stretch.Uniform)]
    [InlineData("bg-auto", Stretch.None)]
    public void SetClass_Size_Keywords_Set_Stretch(string size, Stretch expected)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, $"bg-[url({Pattern})] {size}");

            Assert.Equal(expected, BrushOf(border).Stretch);
        });
    }

    [Fact]
    public void SetClass_NoRepeat_And_Position_Keywords()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, $"bg-[url({Pattern})] bg-no-repeat bg-bottom-right");

            var brush = BrushOf(border);
            Assert.Equal(TileMode.None, brush.TileMode);
            Assert.Equal(AlignmentX.Right, brush.AlignmentX);
            Assert.Equal(AlignmentY.Bottom, brush.AlignmentY);
        });
    }

    [Fact]
    public void SetClass_Center_Sets_Both_Axes()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, $"bg-[url({Pattern})] bg-cover bg-center");

            var brush = BrushOf(border);
            Assert.Equal(AlignmentX.Center, brush.AlignmentX);
            Assert.Equal(AlignmentY.Center, brush.AlignmentY);
        });
    }

    [Fact]
    public void SetClass_Missing_Or_NonAvares_Url_Is_Ignored_And_Removal_Restores()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "bg-[url(https://example.com/x.png)]");
            Assert.Null(border.Background);

            Tw.SetClass(border, "bg-[url(avares://Tailwind.Avalonia.PlatformTests/Assets/missing.png)]");
            Assert.Null(border.Background);

            Tw.SetClass(border, $"bg-[url({Pattern})]");
            Assert.NotNull(border.Background);
            Tw.SetClass(border, "");
            Assert.Null(border.Background);
        });
    }
}
