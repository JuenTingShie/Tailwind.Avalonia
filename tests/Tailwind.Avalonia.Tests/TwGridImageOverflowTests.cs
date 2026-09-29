using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwGridImageOverflowTests
{
    [Fact]
    public void SetClass_Grid_Placement_Uses_ZeroBased_Start_And_Span()
    {
        var child = new Border();

        Tw.SetClass(child, "col-start-2 row-start-3 col-span-2 row-span-4");

        Assert.Equal(1, Grid.GetColumn(child));
        Assert.Equal(2, Grid.GetRow(child));
        Assert.Equal(2, Grid.GetColumnSpan(child));
        Assert.Equal(4, Grid.GetRowSpan(child));
    }

    [Fact]
    public void SetClass_Grid_Placement_Removal_Restores_Defaults()
    {
        var child = new Border();

        Tw.SetClass(child, "col-span-3");
        Tw.SetClass(child, "");

        Assert.Equal(1, Grid.GetColumnSpan(child));
    }

    [Fact]
    public void SetClass_GridCols_And_GridRows_Create_Star_Tracks()
    {
        var grid = new Grid();

        Tw.SetClass(grid, "grid-cols-3 grid-rows-2");

        Assert.Equal(3, grid.ColumnDefinitions.Count);
        Assert.Equal(2, grid.RowDefinitions.Count);
        Assert.All(grid.ColumnDefinitions, c => Assert.Equal(GridUnitType.Star, c.Width.GridUnitType));

        Tw.SetClass(grid, "grid-cols-2");

        Assert.Equal(2, grid.ColumnDefinitions.Count);
        Assert.Empty(grid.RowDefinitions);
    }

    [Fact]
    public void SetClass_GridCols_Ignored_On_NonGrid()
    {
        var panel = new StackPanel();

        Tw.SetClass(panel, "grid-cols-3");

        Assert.Empty(panel.Children);
    }

    [Theory]
    [InlineData("col-span-0")]
    [InlineData("col-start-x")]
    [InlineData("grid-cols-0")]
    public void SetClass_Invalid_Grid_Tokens_Are_Ignored(string token)
    {
        var child = new Border();

        Tw.SetClass(child, token);

        Assert.Equal(1, Grid.GetColumnSpan(child));
        Assert.Equal(0, Grid.GetColumn(child));
    }

    [Theory]
    [InlineData("object-contain", Stretch.Uniform)]
    [InlineData("object-cover", Stretch.UniformToFill)]
    [InlineData("object-fill", Stretch.Fill)]
    [InlineData("object-none", Stretch.None)]
    public void SetClass_ObjectFit_Sets_Image_Stretch(string token, Stretch expected)
    {
        var image = new Image();

        Tw.SetClass(image, token);

        Assert.Equal(expected, image.Stretch);
    }

    [Fact]
    public void SetClass_ObjectScaleDown_Sets_DownOnly()
    {
        var image = new Image();

        Tw.SetClass(image, "object-scale-down");

        Assert.Equal(StretchDirection.DownOnly, image.StretchDirection);
    }

    [Fact]
    public void SetClass_OverflowXY_Sets_ScrollBar_Visibility()
    {
        var viewer = new ScrollViewer();

        Tw.SetClass(viewer, "overflow-x-scroll overflow-y-hidden");

        Assert.Equal(ScrollBarVisibility.Visible, viewer.HorizontalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Hidden, viewer.VerticalScrollBarVisibility);
    }

    [Fact]
    public void SetClass_Whitespace_Aliases_Set_TextWrapping()
    {
        var text = new TextBlock();

        Tw.SetClass(text, "whitespace-nowrap");
        Assert.Equal(TextWrapping.NoWrap, text.TextWrapping);

        Tw.SetClass(text, "whitespace-normal");
        Assert.Equal(TextWrapping.Wrap, text.TextWrapping);
    }
}
