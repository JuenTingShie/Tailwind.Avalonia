using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwTests
{
    [Fact]
    public void SetClass_Applies_Physical_Spacing_Utilities()
    {
        var border = new Border();

        Tw.SetClass(border, "p-4 px-2 pt-3 m-1 mx-2");

        Assert.Equal(new Thickness(8, 12, 8, 16), border.Padding);
        Assert.Equal(new Thickness(8, 4, 8, 4), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Logical_Spacing_For_RightToLeft()
    {
        var border = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(border, "ps-6 pe-2 py-4");

        Assert.Equal(new Thickness(8, 16, 24, 16), border.Padding);
    }

    [Fact]
    public void SetClass_Reapplies_Logical_Spacing_When_FlowDirection_Changes()
    {
        var border = new Border();

        Tw.SetClass(border, "ps-6 pe-2 py-4");

        Assert.Equal(new Thickness(24, 16, 8, 16), border.Padding);

        border.FlowDirection = FlowDirection.RightToLeft;

        Assert.Equal(new Thickness(8, 16, 24, 16), border.Padding);
    }

    [Fact]
    public void SetClass_Arranges_Logical_Padding_On_Expected_Rtl_Side()
    {
        var psChild = new Canvas();
        var psBorder = new Border
        {
            Child = psChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(psBorder, "ps-6");
        psBorder.Measure(new Size(100, 20));
        psBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(0, 0, 76, 20), psChild.Bounds);

        var peChild = new Canvas();
        var peBorder = new Border
        {
            Child = peChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(peBorder, "pe-2");
        peBorder.Measure(new Size(100, 20));
        peBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(8, 0, 92, 20), peChild.Bounds);
    }

    [Fact]
    public void SetClass_Uses_Physical_Left_For_Visual_Start_Padding()
    {
        var ltrChild = new Canvas();
        var ltrBorder = new Border
        {
            Child = ltrChild,
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlChild = new Canvas();
        var rtlBorder = new Border
        {
            Child = rtlChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "psv-6");
        Tw.SetClass(rtlBorder, "psv-6");

        Assert.Equal(new Thickness(24, 0, 0, 0), ltrBorder.Padding);
        Assert.Equal(new Thickness(24, 0, 0, 0), rtlBorder.Padding);

        ltrBorder.Measure(new Size(100, 20));
        ltrBorder.Arrange(new Rect(0, 0, 100, 20));
        rtlBorder.Measure(new Size(100, 20));
        rtlBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(24, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(24, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Uses_Physical_Right_For_Visual_End_Padding()
    {
        var ltrChild = new Canvas();
        var ltrBorder = new Border
        {
            Child = ltrChild,
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlChild = new Canvas();
        var rtlBorder = new Border
        {
            Child = rtlChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "pev-6");
        Tw.SetClass(rtlBorder, "pev-6");

        Assert.Equal(new Thickness(0, 0, 24, 0), ltrBorder.Padding);
        Assert.Equal(new Thickness(0, 0, 24, 0), rtlBorder.Padding);

        ltrBorder.Measure(new Size(100, 20));
        ltrBorder.Arrange(new Rect(0, 0, 100, 20));
        rtlBorder.Measure(new Size(100, 20));
        rtlBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(0, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(0, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Applies_Block_Padding_Utilities_Independent_Of_FlowDirection()
    {
        var ltrBorder = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlBorder = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "pbs-6 pbe-2");
        Tw.SetClass(rtlBorder, "pbs-6 pbe-2");

        Assert.Equal(new Thickness(0, 24, 0, 8), ltrBorder.Padding);
        Assert.Equal(new Thickness(0, 24, 0, 8), rtlBorder.Padding);
    }

    [Fact]
    public void SetClass_Uses_Physical_Left_For_Visual_Start_Margin()
    {
        var ltrChild = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };
        var ltrHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { ltrChild },
        };

        var rtlChild = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };
        var rtlHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { rtlChild },
        };

        Tw.SetClass(ltrChild, "msv-6");
        Tw.SetClass(rtlChild, "msv-6");

        Assert.Equal(new Thickness(24, 0, 0, 0), ltrChild.Margin);
        Assert.Equal(new Thickness(24, 0, 0, 0), rtlChild.Margin);

        ltrHost.Measure(new Size(100, 20));
        ltrHost.Arrange(new Rect(0, 0, 100, 20));
        rtlHost.Measure(new Size(100, 20));
        rtlHost.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(24, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(24, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Uses_Physical_Right_For_Visual_End_Margin()
    {
        var ltrChild = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };
        var ltrHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { ltrChild },
        };

        var rtlChild = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };
        var rtlHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { rtlChild },
        };

        Tw.SetClass(ltrChild, "mev-6");
        Tw.SetClass(rtlChild, "mev-6");

        Assert.Equal(new Thickness(0, 0, 24, 0), ltrChild.Margin);
        Assert.Equal(new Thickness(0, 0, 24, 0), rtlChild.Margin);

        ltrHost.Measure(new Size(100, 20));
        ltrHost.Arrange(new Rect(0, 0, 100, 20));
        rtlHost.Measure(new Size(100, 20));
        rtlHost.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(0, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(0, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Applies_Block_Margin_Utilities_Independent_Of_FlowDirection()
    {
        var ltrBorder = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlBorder = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "mbs-6 mbe-2");
        Tw.SetClass(rtlBorder, "mbs-6 mbe-2");

        Assert.Equal(new Thickness(0, 24, 0, 8), ltrBorder.Margin);
        Assert.Equal(new Thickness(0, 24, 0, 8), rtlBorder.Margin);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Spacing_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "p-4 m-2");
        Tw.SetClass(border, null);

        Assert.Equal(default, border.Padding);
        Assert.Equal(default, border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Color_Utilities()
    {
        var resources = new ColorResourceDictionary();
        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(textBlock, "text-orange-800");

        Assert.Equal(
            Assert.IsType<SolidColorBrush>(resources["BrushBlue700"]).Color,
            Assert.IsType<SolidColorBrush>(border.Background).Color);
        Assert.Equal(
            Assert.IsType<SolidColorBrush>(resources["BrushGreen800"]).Color,
            Assert.IsType<SolidColorBrush>(border.BorderBrush).Color);
        Assert.Equal(
            Assert.IsType<SolidColorBrush>(resources["BrushOrange800"]).Color,
            Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }

    [Fact]
    public void SetClass_Applies_Sizing_Utilities()
    {
        var border = new Border();

        Tw.SetClass(border, "w-24 min-w-16 max-w-32 h-12 min-h-8 max-h-20");

        Assert.Equal(96d, border.Width);
        Assert.Equal(64d, border.MinWidth);
        Assert.Equal(128d, border.MaxWidth);
        Assert.Equal(48d, border.Height);
        Assert.Equal(32d, border.MinHeight);
        Assert.Equal(80d, border.MaxHeight);
    }

    [Fact]
    public void SetClass_Uses_Last_Sizing_Utility_For_Each_Property()
    {
        var border = new Border();

        Tw.SetClass(border, "w-16 w-24 min-w-8 min-w-12 max-w-40 max-w-32 h-10 h-12 min-h-4 min-h-6 max-h-24 max-h-20");

        Assert.Equal(96d, border.Width);
        Assert.Equal(48d, border.MinWidth);
        Assert.Equal(128d, border.MaxWidth);
        Assert.Equal(48d, border.Height);
        Assert.Equal(24d, border.MinHeight);
        Assert.Equal(80d, border.MaxHeight);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Sizing_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "w-24 min-w-16 max-w-32 h-12 min-h-8 max-h-20");
        Tw.SetClass(border, null);

        Assert.True(double.IsNaN(border.Width));
        Assert.Equal(0d, border.MinWidth);
        Assert.True(double.IsPositiveInfinity(border.MaxWidth));
        Assert.True(double.IsNaN(border.Height));
        Assert.Equal(0d, border.MinHeight);
        Assert.True(double.IsPositiveInfinity(border.MaxHeight));
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Color_Utilities_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(border, null);

        Assert.Null(border.Background);
        Assert.Null(border.BorderBrush);
    }

    [Fact]
    public void SetClass_Applies_Transparent_And_Opacity_Color_Utilities()
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));
        Assert.True(TailwindColorPalette.TryGetColor("orange-800", out var orange800));

        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700/50 border-transparent");
        Tw.SetClass(textBlock, "text-orange-800/25");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal((byte)128, background.A);
        Assert.Equal(blue700.R, background.R);
        Assert.Equal(blue700.G, background.G);
        Assert.Equal(blue700.B, background.B);
        Assert.Equal(Colors.Transparent, borderBrush);
        Assert.Equal((byte)64, foreground.A);
        Assert.Equal(orange800.R, foreground.R);
        Assert.Equal(orange800.G, foreground.G);
        Assert.Equal(orange800.B, foreground.B);
    }

    [Fact]
    public void SetClass_Applies_Font_Size_Utilities_And_Preserves_Text_Color_Parsing()
    {
        var resources = new ColorResourceDictionary();
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-base text-sky-300");

        Assert.Equal(16d, textBlock.FontSize);
        Assert.Equal(
            Assert.IsType<SolidColorBrush>(resources["BrushSky300"]).Color,
            Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }

    [Fact]
    public void SetClass_Applies_Font_Size_Without_Changing_Foreground_When_No_Color_Utility_Is_Present()
    {
        var textBlock = new TextBlock();
        var originalForeground = textBlock.Foreground;

        Tw.SetClass(textBlock, "text-4xl");

        Assert.Equal(36d, textBlock.FontSize);
        Assert.Equal(originalForeground, textBlock.Foreground);
    }

    [Fact]
    public void SetClass_Uses_Last_Font_Size_Utility()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-sm text-2xl text-lg");

        Assert.Equal(18d, textBlock.FontSize);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Font_Size_When_Class_Removed()
    {
        var textBlock = new TextBlock();
        var defaultFontSize = textBlock.FontSize;

        Tw.SetClass(textBlock, "text-7xl");
        Tw.SetClass(textBlock, null);

        Assert.Equal(defaultFontSize, textBlock.FontSize);
    }
}