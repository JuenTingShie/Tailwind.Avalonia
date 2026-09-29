using Avalonia.Controls;
using Avalonia.Layout;

namespace Tailwind.Avalonia.Tests;

public class TwFlexDirectionGapTests
{
    [Theory]
    [InlineData("flex-row", Orientation.Horizontal)]
    [InlineData("flex-col", Orientation.Vertical)]
    public void SetClass_Sets_StackPanel_Orientation(string className, Orientation expected)
    {
        var panel = new StackPanel { Orientation = expected == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal };

        Tw.SetClass(panel, className);

        Assert.Equal(expected, panel.Orientation);
    }

    [Fact]
    public void SetClass_Sets_WrapPanel_Orientation()
    {
        var panel = new WrapPanel();

        Tw.SetClass(panel, "flex-col");

        Assert.Equal(Orientation.Vertical, panel.Orientation);
    }

    [Fact]
    public void SetClass_Gap_Sets_StackPanel_Spacing()
    {
        var panel = new StackPanel();

        Tw.SetClass(panel, "gap-4");

        Assert.Equal(16, panel.Spacing);
    }

    [Theory]
    [InlineData("gap-x-2", Orientation.Horizontal, 8)]
    [InlineData("gap-y-2", Orientation.Horizontal, 0)]
    [InlineData("gap-y-3", Orientation.Vertical, 12)]
    [InlineData("gap-x-3", Orientation.Vertical, 0)]
    [InlineData("space-x-2", Orientation.Horizontal, 8)]
    [InlineData("space-y-2", Orientation.Vertical, 8)]
    [InlineData("space-y-2", Orientation.Horizontal, 0)]
    public void SetClass_Axis_Gap_Applies_Only_Along_The_Stack_Axis(string className, Orientation orientation, double expected)
    {
        var panel = new StackPanel { Orientation = orientation };

        Tw.SetClass(panel, className);

        Assert.Equal(expected, panel.Spacing);
    }

    [Fact]
    public void SetClass_Uses_Orientation_From_The_Same_Class_List()
    {
        var panel = new StackPanel();

        Tw.SetClass(panel, "flex-row gap-x-2 gap-y-6");

        Assert.Equal(Orientation.Horizontal, panel.Orientation);
        Assert.Equal(8, panel.Spacing);
    }

    [Fact]
    public void SetClass_Gap_Sets_WrapPanel_Item_And_Line_Spacing()
    {
        var panel = new WrapPanel();

        Tw.SetClass(panel, "gap-x-2 gap-y-4");

        Assert.Equal(8, panel.ItemSpacing);
        Assert.Equal(16, panel.LineSpacing);
    }

    [Fact]
    public void SetClass_Gap_Sets_Grid_Row_And_Column_Spacing()
    {
        var grid = new Grid();

        Tw.SetClass(grid, "gap-x-2 gap-y-4");

        Assert.Equal(8, grid.ColumnSpacing);
        Assert.Equal(16, grid.RowSpacing);
    }

    [Fact]
    public void SetClass_Gap_Sets_Both_Grid_Axes()
    {
        var grid = new Grid();

        Tw.SetClass(grid, "gap-[10px]");

        Assert.Equal(10, grid.ColumnSpacing);
        Assert.Equal(10, grid.RowSpacing);
    }

    [Fact]
    public void SetClass_Clears_Gap_And_Orientation_When_Classes_Removed()
    {
        var panel = new StackPanel();
        var orientation = panel.Orientation;

        Tw.SetClass(panel, "flex-row gap-4");
        Tw.SetClass(panel, null);

        Assert.Equal(orientation, panel.Orientation);
        Assert.Equal(0, panel.Spacing);
    }

    [Fact]
    public void SetClass_Ignores_Gap_On_Elements_Without_Spacing()
    {
        var border = new Border();

        var exception = Record.Exception(() => Tw.SetClass(border, "gap-4 p-2"));

        Assert.Null(exception);
        Assert.Equal(8, border.Padding.Left);
    }

    [Theory]
    [InlineData("gap-")]
    [InlineData("gap-foo")]
    [InlineData("gap--2")]
    [InlineData("gap-x-")]
    [InlineData("flex-diagonal")]
    public void SetClass_Ignores_Invalid_Tokens(string className)
    {
        var panel = new StackPanel();

        Tw.SetClass(panel, className);

        Assert.Equal(0, panel.Spacing);
        Assert.Equal(Orientation.Vertical, panel.Orientation);
    }
}
