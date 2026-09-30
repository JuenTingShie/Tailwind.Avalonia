using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Tailwind.Avalonia.Tests;

public class TwScrollSnapTests
{
    [Fact]
    public void SetClass_SnapX_And_SnapY_Set_Their_Axis_Only()
    {
        var x = new ScrollViewer();
        var y = new ScrollViewer();

        Tw.SetClass(x, "snap-x");
        Tw.SetClass(y, "snap-y");

        Assert.Equal(SnapPointsType.Mandatory, x.HorizontalSnapPointsType);
        Assert.Equal(SnapPointsType.None, x.VerticalSnapPointsType);
        Assert.Equal(SnapPointsType.Mandatory, y.VerticalSnapPointsType);
        Assert.Equal(SnapPointsType.None, y.HorizontalSnapPointsType);
    }

    [Fact]
    public void SetClass_SnapBoth_And_SnapNone()
    {
        var viewer = new ScrollViewer();

        Tw.SetClass(viewer, "snap-both");
        Assert.Equal(SnapPointsType.Mandatory, viewer.HorizontalSnapPointsType);
        Assert.Equal(SnapPointsType.Mandatory, viewer.VerticalSnapPointsType);

        Tw.SetClass(viewer, "snap-none");
        Assert.Equal(SnapPointsType.None, viewer.HorizontalSnapPointsType);
        Assert.Equal(SnapPointsType.None, viewer.VerticalSnapPointsType);
    }

    [Theory]
    [InlineData("snap-start", SnapPointsAlignment.Near)]
    [InlineData("snap-center", SnapPointsAlignment.Center)]
    [InlineData("snap-end", SnapPointsAlignment.Far)]
    public void SetClass_SnapAlignment_Sets_Both_Axes(string token, SnapPointsAlignment expected)
    {
        var viewer = new ScrollViewer();

        Tw.SetClass(viewer, token);

        Assert.Equal(expected, viewer.HorizontalSnapPointsAlignment);
        Assert.Equal(expected, viewer.VerticalSnapPointsAlignment);
    }

    [Fact]
    public void SetClass_Snap_Removal_Restores_Defaults()
    {
        var viewer = new ScrollViewer();

        Tw.SetClass(viewer, "snap-both snap-center");
        Tw.SetClass(viewer, "");

        Assert.Equal(SnapPointsType.None, viewer.HorizontalSnapPointsType);
        Assert.Equal(SnapPointsAlignment.Near, viewer.HorizontalSnapPointsAlignment);
    }
}
