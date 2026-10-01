using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

/// <summary>
/// Classes that are not part of Tailwind CSS v4.3 (v3 names that v4 renamed or removed, and names Tailwind never
/// had) must be rejected, and their v4 replacements must work.
/// </summary>
public class TwV4AlignmentTests
{
    [Theory]
    [InlineData("rounded")]
    [InlineData("blur")]
    [InlineData("drop-shadow")]
    [InlineData("shadow-inner")]
    [InlineData("bg-gradient-to-r")]
    [InlineData("leading-tight")]
    [InlineData("leading-snug")]
    [InlineData("leading-normal")]
    [InlineData("leading-relaxed")]
    [InlineData("leading-loose")]
    [InlineData("psv-4")]
    [InlineData("pev-4")]
    [InlineData("msv-4")]
    [InlineData("mev-4")]
    [InlineData("pressed:bg-red-500")]
    [InlineData("selection-sky-500")]
    [InlineData("placeholder-slate-400")]
    public void Non_V4_Classes_Are_Ignored(string className)
    {
        var border = new Border();
        var textBlock = new TextBlock();
        var textBox = new TextBox();

        Tw.SetClass(border, className);
        Tw.SetClass(textBlock, className);
        Tw.SetClass(textBox, className);

        Assert.Equal(default, border.CornerRadius);
        Assert.Null(border.Effect);
        Assert.Null(border.Background);
        Assert.Equal(default, border.Padding);
        Assert.Equal(default, border.Margin);
        Assert.True(double.IsNaN(textBlock.LineHeight));
        Assert.Empty(border.Styles);
    }

    [Fact]
    public void Active_Variant_Targets_Pressed_State()
    {
        var button = new Button();
        Tw.SetClass(button, "bg-sky-500 active:bg-sky-700");
        Assert.NotEmpty(button.Styles);
    }

    [Fact]
    public void Selection_Variant_Sets_Selection_Brushes()
    {
        var textBox = new TextBox();
        Tw.SetClass(textBox, "selection:bg-sky-500 selection:text-white");
        Assert.Equal(Color.Parse("#00a6f4"), ((ISolidColorBrush)textBox.SelectionBrush!).Color);
        Assert.Equal(Colors.White, ((ISolidColorBrush)textBox.SelectionForegroundBrush!).Color);
    }

    [Fact]
    public void Placeholder_Variant_Sets_Placeholder_Foreground()
    {
        var textBox = new TextBox();
        Tw.SetClass(textBox, "placeholder:text-rose-400");
        Assert.NotNull(textBox.PlaceholderForeground);
    }

    [Theory]
    [InlineData("placeholder:bg-rose-400")]
    [InlineData("selection:border-sky-500")]
    public void Pseudo_Element_Variants_Reject_Other_Utilities(string className)
    {
        var textBox = new TextBox();
        Tw.SetClass(textBox, className);
        Assert.Null(textBox.PlaceholderForeground);
        Assert.Null(textBox.SelectionBrush);
    }
}
