using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace Tailwind.Avalonia.Tests;

public class TwMiscInteractivityTests
{
    [Theory]
    [InlineData("overscroll-auto", true)]
    [InlineData("overscroll-contain", false)]
    [InlineData("overscroll-none", false)]
    public void SetClass_Overscroll_Sets_Scroll_Chaining(string token, bool expected)
    {
        var viewer = new ScrollViewer();

        Tw.SetClass(viewer, token);

        Assert.Equal(expected, viewer.IsScrollChainingEnabled);
    }

    [Fact]
    public void SetClass_Overscroll_Removal_Restores_Default()
    {
        var viewer = new ScrollViewer();

        Tw.SetClass(viewer, "overscroll-none");
        Tw.SetClass(viewer, "");

        Assert.True(viewer.IsScrollChainingEnabled);
    }

    [Fact]
    public void SetClass_Scheme_Sets_Requested_Theme_Variant()
    {
        var border = new ThemeVariantScope();

        Tw.SetClass(border, "scheme-dark");
        Assert.Equal(ThemeVariant.Dark, border.RequestedThemeVariant);

        Tw.SetClass(border, "scheme-light");
        Assert.Equal(ThemeVariant.Light, border.RequestedThemeVariant);

        Tw.SetClass(border, "scheme-normal");
        Assert.Equal(ThemeVariant.Default, border.RequestedThemeVariant);
    }

    [Fact]
    public void SetClass_Placeholder_Color_Sets_PlaceholderForeground()
    {
        Assert.True(TailwindColorPalette.TryGetColor("slate-400", out var slate));
        var box = new TextBox();

        Tw.SetClass(box, "placeholder:text-slate-400");

        Assert.Equal(slate, Assert.IsAssignableFrom<ISolidColorBrush>(box.PlaceholderForeground).Color);
    }

    [Fact]
    public void SetClass_TransformNone_Clears_Transform()
    {
        var border = new Border();

        Tw.SetClass(border, "transform-none");

        var transform = Assert.IsType<TransformOperations>(border.RenderTransform);
        Assert.True(transform.IsIdentity);
    }
}
