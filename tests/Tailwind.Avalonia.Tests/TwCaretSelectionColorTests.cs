using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwCaretSelectionColorTests
{
    [Fact]
    public void SetClass_Applies_Caret_Color_To_TextBox()
    {
        var textBox = new TextBox();

        Tw.SetClass(textBox, "caret-red-500");

        var brush = Assert.IsAssignableFrom<ISolidColorBrush>(textBox.CaretBrush);
        Assert.Equal(Color.Parse("#fb2c36"), brush.Color);
    }

    [Fact]
    public void SetClass_Applies_Selection_Color_To_TextBox()
    {
        var textBox = new TextBox();

        Tw.SetClass(textBox, "selection:bg-sky-500");

        Assert.NotNull(textBox.SelectionBrush);
    }

    [Fact]
    public void SetClass_Applies_Caret_And_Selection_Together_With_Other_Colors()
    {
        var textBox = new TextBox();

        Tw.SetClass(textBox, "caret-white selection:bg-emerald-600 bg-slate-800 text-white");

        Assert.NotNull(textBox.CaretBrush);
        Assert.NotNull(textBox.SelectionBrush);
        Assert.NotNull(textBox.Background);
        Assert.NotNull(textBox.Foreground);
    }

    [Fact]
    public void SetClass_Applies_Selection_Color_To_SelectableTextBlock()
    {
        var textBlock = new SelectableTextBlock();

        Tw.SetClass(textBlock, "selection:bg-amber-400");

        Assert.NotNull(textBlock.SelectionBrush);
    }

    [Fact]
    public void SetClass_Clears_Caret_Color_When_Class_Removed()
    {
        var textBox = new TextBox();
        var original = textBox.CaretBrush;

        Tw.SetClass(textBox, "caret-red-500");
        Tw.SetClass(textBox, null);

        Assert.Equal(original, textBox.CaretBrush);
    }

    [Fact]
    public void SetClass_Ignores_Caret_Color_On_Elements_Without_Caret_Brush()
    {
        var border = new Border();

        var exception = Record.Exception(() => Tw.SetClass(border, "caret-red-500 p-2"));

        Assert.Null(exception);
        Assert.Equal(8, border.Padding.Left);
    }
}
