using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;

namespace Tailwind.Avalonia.PlatformTests;

public class TwAnimationTests
{
    [Theory]
    [InlineData("animate-spin", "spin")]
    [InlineData("animate-ping", "ping")]
    [InlineData("animate-pulse", "pulse")]
    [InlineData("animate-bounce", "bounce")]
    public void SetClass_Animate_Registers_The_Named_Animation(string token, string name)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, token);

            Assert.Equal(name, Tw.GetActiveAnimation(border));
        });
    }

    [Fact]
    public void SetClass_AnimateNone_And_Removal_Stop_The_Animation()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "animate-spin");
            Tw.SetClass(border, "animate-none");
            Assert.Null(Tw.GetActiveAnimation(border));

            Tw.SetClass(border, "animate-pulse");
            Tw.SetClass(border, "");
            Assert.Null(Tw.GetActiveAnimation(border));
        });
    }

    [Fact]
    public void SetClass_Animate_Does_Not_Restart_When_Unchanged()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "animate-spin");
            Tw.SetClass(border, "animate-spin p-2");

            Assert.Equal("spin", Tw.GetActiveAnimation(border));
        });
    }

    private static (Window Window, Border Border) ShowAnimated(string classes)
    {
        var border = new Border { Width = 40, Height = 40 };
        Tw.SetClass(border, classes);
        var window = new Window { Content = border };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        for (var i = 0; i < 5; i++)
        {
            Thread.Sleep(30);
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(5);
            Dispatcher.UIThread.RunJobs();
        }

        return (window, border);
    }

    [Fact]
    public void Spin_Applies_A_Rotation_While_Attached_And_Restores_When_Stopped()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var (window, border) = ShowAnimated("animate-spin");

            Assert.True(Tw.IsAnimationRunning(border));
            Assert.Null(Tw.GetAnimationError(border));
            Assert.NotNull(border.RenderTransform);

            Tw.SetClass(border, "");
            Dispatcher.UIThread.RunJobs();

            Assert.False(Tw.IsAnimationRunning(border));
            Assert.Null(border.RenderTransform);
            window.Close();
        });
    }

    [Fact]
    public void Pulse_Fades_The_Element_While_Running_And_Restores_Opacity_When_Stopped()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var (window, border) = ShowAnimated("animate-pulse");

            Assert.Null(Tw.GetAnimationError(border));
            Assert.True(border.Opacity < 1, $"opacity was {border.Opacity}");

            Tw.SetClass(border, "");
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, border.Opacity);
            window.Close();
        });
    }

    [Fact]
    public void Detaching_Stops_The_Animation_And_Reattaching_Restarts_It()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var (window, border) = ShowAnimated("animate-pulse");
            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            Assert.False(Tw.IsAnimationRunning(border));
            Assert.Equal(1, border.Opacity);

            window.Content = border;
            Dispatcher.UIThread.RunJobs();

            Assert.True(Tw.IsAnimationRunning(border));
            window.Close();
        });
    }
}
