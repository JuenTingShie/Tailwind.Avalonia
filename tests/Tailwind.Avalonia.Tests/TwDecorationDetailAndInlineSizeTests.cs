using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwDecorationDetailAndInlineSizeTests
{
    [Fact]
    public void SetClass_Decoration_Thickness_Offset_And_Color()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "underline decoration-4 underline-offset-2 decoration-red-500");

        var decoration = Assert.Single(text.TextDecorations!);
        Assert.Equal(TextDecorationLocation.Underline, decoration.Location);
        Assert.Equal(4, decoration.StrokeThickness);
        Assert.Equal(TextDecorationUnit.Pixel, decoration.StrokeThicknessUnit);
        Assert.Equal(2, decoration.StrokeOffset);
        Assert.Equal(TextDecorationUnit.Pixel, decoration.StrokeOffsetUnit);
        Assert.IsAssignableFrom<ISolidColorBrush>(decoration.Stroke);
    }

    [Fact]
    public void SetClass_Decoration_Arbitrary_Thickness()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "line-through decoration-[3px]");

        Assert.Equal(3, Assert.Single(text.TextDecorations!).StrokeThickness);
    }

    [Fact]
    public void SetClass_Decoration_Auto_Keeps_Font_Metrics()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "underline decoration-4 decoration-auto");

        Assert.Equal(TextDecorationUnit.FontRecommended, Assert.Single(text.TextDecorations!).StrokeThicknessUnit);
    }

    [Fact]
    public void SetClass_Decoration_Without_Location_Sets_Nothing()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "decoration-4");

        Assert.Null(text.TextDecorations);
    }

    [Fact]
    public void SetClass_Inline_And_Block_Sizes_Map_To_Width_And_Height()
    {
        var border = new Border();

        Tw.SetClass(border, "inline-20 block-10 min-inline-4 max-inline-40 min-block-2 max-block-32");

        Assert.Equal(80, border.Width);
        Assert.Equal(40, border.Height);
        Assert.Equal(16, border.MinWidth);
        Assert.Equal(160, border.MaxWidth);
        Assert.Equal(8, border.MinHeight);
        Assert.Equal(128, border.MaxHeight);
    }
}

public class TwDecorationStyleTests
{
    [Fact]
    public void SetClass_Dotted_Sets_Round_Dots()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "underline decoration-dotted decoration-2");

        var decoration = Assert.Single(text.TextDecorations!);
        Assert.Equal([0d, 2d], decoration.StrokeDashArray!.ToArray());
        Assert.Equal(PenLineCap.Round, decoration.StrokeLineCap);
    }

    [Fact]
    public void SetClass_Dashed_Sets_Dashes()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "line-through decoration-dashed");

        Assert.Equal([4d, 3d], Assert.Single(text.TextDecorations!).StrokeDashArray!.ToArray());
    }

    [Fact]
    public void SetClass_Solid_Overrides_Earlier_Style()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "underline decoration-dashed decoration-solid");

        var decoration = Assert.Single(text.TextDecorations!);
        Assert.True(decoration.StrokeDashArray is null or { Count: 0 });
    }
}
