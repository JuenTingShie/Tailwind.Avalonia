using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace Tailwind.Avalonia.PlatformTests;

public class TwTransitionTests
{
    private static T Single<T>(Border border) where T : class, ITransition
    {
        Assert.NotNull(border.Transitions);
        return Assert.Single(border.Transitions.OfType<T>());
    }

    [Fact]
    public void SetClass_TransitionColors_Animates_The_Brush_Properties()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition-colors");

            var properties = border.Transitions!.OfType<BrushTransition>().Select(static t => t.Property).ToArray();
            Assert.Contains(Border.BackgroundProperty, properties);
            Assert.Contains(Border.BorderBrushProperty, properties);
            Assert.DoesNotContain(border.Transitions!.OfType<DoubleTransition>(), static t => true);
        });
    }

    [Fact]
    public void SetClass_TransitionOpacity_Animates_Opacity_Only()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition-opacity");

            var transition = Assert.Single(border.Transitions!);
            var opacity = Assert.IsType<DoubleTransition>(transition);
            Assert.Equal(Border.OpacityProperty, opacity.Property);
        });
    }

    [Fact]
    public void SetClass_TransitionTransform_Animates_RenderTransform()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition-transform");

            var transform = Assert.IsType<TransformOperationsTransition>(Assert.Single(border.Transitions!));
            Assert.Equal(Border.RenderTransformProperty, transform.Property);
        });
    }

    [Fact]
    public void SetClass_TransitionShadow_Animates_BoxShadow()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition-shadow");

            var shadow = Assert.IsType<BoxShadowsTransition>(Assert.Single(border.Transitions!));
            Assert.Equal(Border.BoxShadowProperty, shadow.Property);
        });
    }

    [Fact]
    public void SetClass_Transition_Covers_Colors_Opacity_Transform_And_Shadow()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition");

            Assert.NotEmpty(border.Transitions!.OfType<BrushTransition>());
            Single<DoubleTransition>(border);
            Single<TransformOperationsTransition>(border);
            Single<BoxShadowsTransition>(border);
        });
    }

    [Fact]
    public void SetClass_Default_Duration_Is_150ms()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition-opacity");

            Assert.Equal(TimeSpan.FromMilliseconds(150), Single<DoubleTransition>(border).Duration);
        });
    }

    [Theory]
    [InlineData("duration-300", 300)]
    [InlineData("duration-75", 75)]
    [InlineData("duration-[250ms]", 250)]
    [InlineData("duration-[0.5s]", 500)]
    public void SetClass_Applies_Duration(string className, double milliseconds)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, $"transition-opacity {className}");

            Assert.Equal(TimeSpan.FromMilliseconds(milliseconds), Single<DoubleTransition>(border).Duration);
        });
    }

    [Fact]
    public void SetClass_Applies_Delay()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "delay-200 transition-opacity");

            Assert.Equal(TimeSpan.FromMilliseconds(200), Single<DoubleTransition>(border).Delay);
        });
    }

    [Theory]
    [InlineData("ease-linear", typeof(LinearEasing))]
    [InlineData("ease-in", typeof(SplineEasing))]
    [InlineData("ease-out", typeof(SplineEasing))]
    [InlineData("ease-in-out", typeof(SplineEasing))]
    public void SetClass_Applies_Easing(string className, Type expected)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, $"transition-opacity {className}");

            Assert.IsType(expected, Single<DoubleTransition>(border).Easing);
        });
    }

    [Fact]
    public void SetClass_TransitionNone_Removes_Transitions()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition-opacity transition-none");

            Assert.Empty(border.Transitions!);
        });
    }

    [Fact]
    public void SetClass_Clears_Transitions_When_Classes_Removed()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "transition duration-500");
            Tw.SetClass(border, null);

            Assert.Null(border.Transitions);
        });
    }

    [Fact]
    public void SetClass_Duration_Without_Transition_Adds_Nothing()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "duration-500 ease-out");

            Assert.Null(border.Transitions);
        });
    }

    [Theory]
    [InlineData("duration-")]
    [InlineData("duration-abc")]
    [InlineData("duration--5")]
    [InlineData("delay-1.5")]
    [InlineData("ease-bounce")]
    public void SetClass_Ignores_Invalid_Timing_Tokens(string className)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, $"transition-opacity {className}");

            var transition = Single<DoubleTransition>(border);
            Assert.Equal(TimeSpan.FromMilliseconds(150), transition.Duration);
            Assert.Equal(TimeSpan.Zero, transition.Delay);
        });
    }

    [Fact]
    public void SetClass_Colors_Animate_On_TextBlock_Foreground()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var textBlock = new TextBlock();

            Tw.SetClass(textBlock, "transition-colors");

            var properties = textBlock.Transitions!.OfType<BrushTransition>().Select(static t => t.Property).ToArray();
            Assert.Contains(TextBlock.ForegroundProperty, properties);
            Assert.Contains(TextBlock.BackgroundProperty, properties);
        });
    }
}
