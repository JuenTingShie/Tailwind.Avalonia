using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Themes.Fluent;

namespace Tailwind.Avalonia.PlatformTests;

public class TwScrollSnapBehaviorTests
{
    private static (Window Window, ScrollViewer Viewer) Build(string classes)
    {
        // The headless application has no theme, so ScrollViewer would have no template without one.
        if (Application.Current!.Styles.OfType<FluentTheme>().FirstOrDefault() is null)
        {
            Application.Current.Styles.Add(new FluentTheme());
        }

        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };

        for (var i = 0; i < 6; i++)
        {
            panel.Children.Add(new Border { Width = 180, Height = 90 });
        }

        var viewer = new ScrollViewer
        {
            Width = 420,
            Height = 120,
            HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            Content = panel,
        };

        Tw.SetClass(viewer, classes);
        var window = new Window { Content = viewer, Width = 600, Height = 300 };
        window.Show();
        window.UpdateLayout();
        return (window, viewer);
    }

    [Fact]
    public void SnapStart_Settles_On_The_Nearest_Card_Start()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var (_, viewer) = Build("snap-x snap-start");
            viewer.Offset = new Vector(150, 0);

            Assert.True(Tw.SnapScrollViewer(viewer));
            Assert.Equal(192, viewer.Offset.X, 1);
        });
    }

    [Fact]
    public void SnapCenter_Centers_The_Nearest_Card()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var (_, viewer) = Build("snap-x snap-center");
            viewer.Offset = new Vector(150, 0);

            Tw.SnapScrollViewer(viewer);

            // Card 2 spans 192..372, so its centre is 282; the viewport is 420 wide.
            Assert.Equal(282 - viewer.Viewport.Width / 2, viewer.Offset.X, 1);
        });
    }

    [Fact]
    public void SnapNone_Does_Not_Move()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var (_, viewer) = Build("snap-none");
            viewer.Offset = new Vector(150, 0);

            Assert.False(Tw.SnapScrollViewer(viewer));
            Assert.Equal(150, viewer.Offset.X, 1);
        });
    }
}
