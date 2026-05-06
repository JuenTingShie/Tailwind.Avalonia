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
    public void SetClass_Clears_Previously_Applied_Spacing_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "p-4 m-2");
        Tw.SetClass(border, null);

        Assert.Equal(default, border.Padding);
        Assert.Equal(default, border.Margin);
    }
}