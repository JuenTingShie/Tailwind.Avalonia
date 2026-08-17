using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwArbitraryValuesTests
{
    [Fact]
    public void SetClass_Applies_Arbitrary_Spacing_With_Px_Unit()
    {
        var border = new Border();

        Tw.SetClass(border, "p-[10px] m-[20px]");

        Assert.Equal(new Thickness(10), border.Padding);
        Assert.Equal(new Thickness(20), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Spacing_With_Rem_Unit()
    {
        var border = new Border();

        Tw.SetClass(border, "p-[1.5rem] m-[2rem]");

        // 1.5rem = 1.5 * 16 = 24px, 2rem = 2 * 16 = 32px
        Assert.Equal(new Thickness(24), border.Padding);
        Assert.Equal(new Thickness(32), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Spacing_With_Em_Unit()
    {
        var border = new Border();

        Tw.SetClass(border, "px-[1em] py-[2em]");

        // 1em = 16px, 2em = 32px
        Assert.Equal(new Thickness(16, 32, 16, 32), border.Padding);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Spacing_Unitless()
    {
        var border = new Border();

        Tw.SetClass(border, "p-[15] m-[25]");

        // Unitless values treated as px
        Assert.Equal(new Thickness(15), border.Padding);
        Assert.Equal(new Thickness(25), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Spacing_With_Decimal()
    {
        var border = new Border();

        Tw.SetClass(border, "p-[12.5px] m-[7.25rem]");

        // 12.5px = 12.5, 7.25rem = 7.25 * 16 = 116px
        Assert.Equal(new Thickness(12.5), border.Padding);
        Assert.Equal(new Thickness(116), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Negative_Spacing()
    {
        var border = new Border();

        Tw.SetClass(border, "-m-[10px]");

        Assert.Equal(new Thickness(-10), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Spacing_To_All_Edges()
    {
        var border = new Border();

        Tw.SetClass(border, "pt-[5px] pr-[10px] pb-[15px] pl-[20px]");

        Assert.Equal(new Thickness(20, 5, 10, 15), border.Padding);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Spacing_X_And_Y()
    {
        var border = new Border();

        Tw.SetClass(border, "px-[8px] py-[12px]");

        Assert.Equal(new Thickness(8, 12, 8, 12), border.Padding);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Sizing_With_Px_Unit()
    {
        var border = new Border();

        Tw.SetClass(border, "w-[100px] h-[50px] min-w-[25px] max-h-[200px]");

        Assert.Equal(100d, border.Width);
        Assert.Equal(50d, border.Height);
        Assert.Equal(25d, border.MinWidth);
        Assert.Equal(200d, border.MaxHeight);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Sizing_With_Rem_Unit()
    {
        var border = new Border();

        Tw.SetClass(border, "w-[6.25rem] h-[3.125rem]");

        // 6.25rem = 6.25 * 16 = 100px, 3.125rem = 3.125 * 16 = 50px
        Assert.Equal(100d, border.Width);
        Assert.Equal(50d, border.Height);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Sizing_With_Em_Unit()
    {
        var border = new Border();

        Tw.SetClass(border, "w-[5em] h-[2.5em]");

        // 5em = 5 * 16 = 80px, 2.5em = 2.5 * 16 = 40px
        Assert.Equal(80d, border.Width);
        Assert.Equal(40d, border.Height);
    }

    [Fact]
    public void SetClass_Ignores_Arbitrary_Sizing_With_Percentage()
    {
        var border = new Border();
        var defaultWidth = border.Width;
        var defaultHeight = border.Height;

        Tw.SetClass(border, "w-[50%] h-[100%]");

        // Percentage values are not supported for sizing in Avalonia
        Assert.Equal(defaultWidth, border.Width);
        Assert.Equal(defaultHeight, border.Height);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Sizing_Unitless()
    {
        var border = new Border();

        Tw.SetClass(border, "w-[120] h-[80]");

        // Unitless values treated as px
        Assert.Equal(120d, border.Width);
        Assert.Equal(80d, border.Height);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Sizing_All_Properties()
    {
        var border = new Border();

        Tw.SetClass(border, "w-[100px] min-w-[50px] max-w-[150px] h-[75px] min-h-[40px] max-h-[120px]");

        Assert.Equal(100d, border.Width);
        Assert.Equal(50d, border.MinWidth);
        Assert.Equal(150d, border.MaxWidth);
        Assert.Equal(75d, border.Height);
        Assert.Equal(40d, border.MinHeight);
        Assert.Equal(120d, border.MaxHeight);
    }

    [Theory]
    [InlineData("text-[14px]", 14d)]
    [InlineData("text-[1.5rem]", 24d)]
    [InlineData("text-[2em]", 32d)]
    [InlineData("text-[18]", 18d)]
    public void SetClass_Applies_Arbitrary_Font_Size_With_Supported_Units(string className, double expectedFontSize)
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, className);

        Assert.Equal(expectedFontSize, textBlock.FontSize);
    }

    [Fact]
    public void SetClass_Ignores_Arbitrary_Font_Size_With_Percentage()
    {
        var textBlock = new TextBlock();
        var defaultFontSize = textBlock.FontSize;

        Tw.SetClass(textBlock, "text-[120%]");

        Assert.Equal(defaultFontSize, textBlock.FontSize);
    }

    [Fact]
    public void SetClass_Ignores_CustomProperty_Shorthand_For_Sizing()
    {
        var border = new Border();

        Tw.SetClass(border, "w-(--my-width) h-(--my-height) min-w-(--my-min-width)");

        Assert.True(double.IsNaN(border.Width));
        Assert.True(double.IsNaN(border.Height));
        Assert.Equal(0d, border.MinWidth);
    }

    [Fact]
    public void SetClass_Ignores_CustomProperty_Shorthand_For_Spacing()
    {
        var border = new Border();

        Tw.SetClass(border, "p-(--my-padding) m-(--my-margin) -m-(--my-negative-margin)");

        Assert.Equal(default, border.Padding);
        Assert.Equal(default, border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_Hex_6Digit()
    {
        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-[#ff0000] border-[#00ff00]");
        Tw.SetClass(textBlock, "text-[#0000ff]");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal(Colors.Red, background);
        Assert.Equal(Colors.Lime, borderBrush);
        Assert.Equal(Colors.Blue, foreground);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_Hex_8Digit_With_Alpha()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-[#ff0000ff] border-[#00ff0080]");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;

        Assert.Equal((byte)255, background.A);
        Assert.Equal(Colors.Red.R, background.R);
        Assert.Equal(Colors.Red.G, background.G);
        Assert.Equal(Colors.Red.B, background.B);

        Assert.Equal((byte)128, borderBrush.A);
        Assert.Equal(Colors.Lime.R, borderBrush.R);
        Assert.Equal(Colors.Lime.G, borderBrush.G);
        Assert.Equal(Colors.Lime.B, borderBrush.B);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_With_Opacity()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-[#ff0000]/50");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;

        Assert.Equal((byte)128, background.A);
        Assert.Equal(Colors.Red.R, background.R);
        Assert.Equal(Colors.Red.G, background.G);
        Assert.Equal(Colors.Red.B, background.B);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_With_Different_Opacities()
    {
        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-[#ffffff]/75");
        Tw.SetClass(textBlock, "text-[#000000]/25");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        // 75% opacity
        Assert.Equal((byte)191, background.A);
        Assert.Equal(Colors.White.R, background.R);
        Assert.Equal(Colors.White.G, background.G);
        Assert.Equal(Colors.White.B, background.B);

        // 25% opacity
        Assert.Equal((byte)64, foreground.A);
        Assert.Equal(Colors.Black.R, foreground.R);
        Assert.Equal(Colors.Black.G, foreground.G);
        Assert.Equal(Colors.Black.B, foreground.B);
    }

    [Fact]
    public void SetClass_Ignores_CustomProperty_Shorthand_For_Colors()
    {
        var border = new Border();
        var textBlock = new TextBlock();
        var originalForeground = textBlock.Foreground;

        Tw.SetClass(border, "bg-(--my-bg) border-(--my-border)");
        Tw.SetClass(textBlock, "text-(--my-text)");

        Assert.Null(border.Background);
        Assert.Null(border.BorderBrush);
        Assert.Equal(originalForeground, textBlock.Foreground);
    }

    [Fact]
    public void SetClass_Ignores_Invalid_Arbitrary_Values()
    {
        var border = new Border();

        Tw.SetClass(border, "p-[invalid] w-[abc] bg-[#gggggg]");

        // Invalid arbitrary values should be ignored, properties remain default
        Assert.Equal(default, border.Padding);
        Assert.True(double.IsNaN(border.Width));
        Assert.Null(border.Background);
    }

    [Fact]
    public void SetClass_Applies_Mixed_Scale_And_Arbitrary_Values()
    {
        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "p-4 px-[10px] bg-blue-700");
        Tw.SetClass(textBlock, "text-[#ffffff]");

        // p-4 sets all sides to 16px, then px-[10px] overrides horizontal to 10px
        // Final: left/right = 10px (from px-[10px]), top/bottom = 16px (from p-4)
        Assert.Equal(new Thickness(10, 16, 10, 16), border.Padding);

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));
        Assert.Equal(blue700, background);
        Assert.Equal(Colors.White, foreground);
    }

    [Fact]
    public void SetClass_Applies_Multiple_Arbitrary_Values_In_Sequence()
    {
        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "p-[8px] m-[10px] w-[200px] h-[150px] bg-[#333333]");
        Tw.SetClass(textBlock, "text-[#cccccc]");

        Assert.Equal(new Thickness(8), border.Padding);
        Assert.Equal(new Thickness(10), border.Margin);
        Assert.Equal(200d, border.Width);
        Assert.Equal(150d, border.Height);

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal(0x33, background.R);
        Assert.Equal(0x33, background.G);
        Assert.Equal(0x33, background.B);
        Assert.Equal(0xcc, foreground.R);
        Assert.Equal(0xcc, foreground.G);
        Assert.Equal(0xcc, foreground.B);
    }

    [Fact]
    public void SetClass_Arbitrary_Color_Takes_Precedence_Over_Tailwind_Palette()
    {
        var border = new Border();

        // First apply Tailwind palette color, then override with arbitrary
        Tw.SetClass(border, "bg-blue-700 bg-[#ff0000]");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;

        // Should use the arbitrary value (red) as last value wins
        Assert.Equal(Colors.Red, background);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Arbitrary_Values_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "p-[20px] w-[100px] bg-[#ff0000]");
        Tw.SetClass(border, null);

        Assert.Equal(default, border.Padding);
        Assert.True(double.IsNaN(border.Width));
        Assert.Null(border.Background);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_Case_Insensitive_Hex()
    {
        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-[#FF0000] border-[#00FF00]");
        Tw.SetClass(textBlock, "text-[#0000fF]");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal(Colors.Red, background);
        Assert.Equal(Colors.Lime, borderBrush);
        Assert.Equal(Colors.Blue, foreground);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_Hex_3Digit_Shorthand()
    {
        var border = new Border();
        var textBlock = new TextBlock();

        // #rgb shorthand expands to #rrggbb (e.g., #fff → #ffffff, #f00 → #ff0000)
        Tw.SetClass(border, "bg-[#f00] border-[#0f0]");
        Tw.SetClass(textBlock, "text-[#00f]");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal(Colors.Red, background);
        Assert.Equal(Colors.Lime, borderBrush);
        Assert.Equal(Colors.Blue, foreground);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_Hex_4Digit_Shorthand_With_Alpha()
    {
        var border = new Border();

        // #rgba shorthand expands to #rrggbbaa (e.g., #f00f → #ff0000ff, #f008 → #ff000088)
        Tw.SetClass(border, "bg-[#f00f] border-[#0f08]");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;

        Assert.Equal((byte)255, background.A);
        Assert.Equal(Colors.Red.R, background.R);
        Assert.Equal(Colors.Red.G, background.G);
        Assert.Equal(Colors.Red.B, background.B);

        Assert.Equal((byte)136, borderBrush.A);  // 0x88 = 136
        Assert.Equal(Colors.Lime.R, borderBrush.R);
        Assert.Equal(Colors.Lime.G, borderBrush.G);
        Assert.Equal(Colors.Lime.B, borderBrush.B);
    }

    [Fact]
    public void SetClass_Applies_Arbitrary_Color_Hex_3Digit_Shorthand_Case_Insensitive()
    {
        var border = new Border();

        // Test case insensitivity with shorthand
        Tw.SetClass(border, "bg-[#FFF] border-[#000]");

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;

        Assert.Equal(Colors.White, background);
        Assert.Equal(Colors.Black, borderBrush);
    }
}
