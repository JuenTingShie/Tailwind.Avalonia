using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Tailwind.Avalonia.PlatformTests;

/// <summary>
/// Avalonia's Image always centers its bitmap, so object-position cannot be read back from a property. These tests lay
/// an Image out inside a sized box with Avalonia's own layout and check two things: where layout puts the control
/// (the axes where the bitmap is smaller than the box) and which window of the bitmap reaches IImage.Draw (the axes
/// where it overflows and is cropped).
/// </summary>
public class TwObjectPositionTests
{
    // A 200x100 bitmap that records every Draw call instead of drawing.
    private sealed class RecordingImage : IImage
    {
        public Size Size { get; } = new(200, 100);

        public List<(Rect Source, Rect Dest)> Calls { get; } = [];

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect) => Calls.Add((sourceRect, destRect));
    }

    // Minimal observable standing in for a view-model property pushed through a binding.
    private sealed class PushedSource : IObservable<IImage?>
    {
        private IObserver<IImage?>? observer;

        public IDisposable Subscribe(IObserver<IImage?> observer)
        {
            this.observer = observer;
            return new Unsubscriber();
        }

        public void Push(IImage? value) => observer?.OnNext(value);

        private sealed class Unsubscriber : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    private readonly record struct Shown(Rect Bounds, Rect Source, Rect Dest);

    // Lays the image out as the only child of a box (its "slot"), renders it and returns what happened.
    private static Shown Show(Image image, RecordingImage recording, double boxWidth, double boxHeight)
    {
        // A test may show the same image again after the binding pushes a new Source, so release it from its old box.
        if (image.Parent is Border oldBox)
        {
            oldBox.Child = null;
        }

        var box = new Border { Width = boxWidth, Height = boxHeight, Child = image };
        var host = new Canvas { Width = 400, Height = 400 };
        host.Children.Add(box);
        host.Measure(new Size(400, 400));
        host.Arrange(new Rect(0, 0, 400, 400));

        recording.Calls.Clear();
        new RenderTargetBitmap(new PixelSize(400, 400)).Render(host);

        var call = Assert.Single(recording.Calls);
        return new Shown(image.Bounds, call.Source, call.Dest);
    }

    private static Shown Place(string classes, Stretch stretch, double boxWidth, double boxHeight)
    {
        Shown result = default;

        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Source = recording, Stretch = stretch };

            Tw.SetClass(image, classes);
            result = Show(image, recording, boxWidth, boxHeight);
        });

        return result;
    }

    private static void AssertRect(Rect expected, Rect actual)
    {
        Assert.Equal(expected.X, actual.X, 3);
        Assert.Equal(expected.Y, actual.Y, 3);
        Assert.Equal(expected.Width, actual.Width, 3);
        Assert.Equal(expected.Height, actual.Height, 3);
    }

    // 200x100 bitmap in a 100x100 box with Stretch.Uniform (object-contain): the bitmap shrinks to 100x50, leaving 50px
    // of free space vertically, so layout decides where the 100x50 control sits.
    [Theory]
    [InlineData("", 25)]
    [InlineData("object-center", 25)]
    [InlineData("object-left", 25)]
    [InlineData("object-right", 25)]
    [InlineData("object-top", 0)]
    [InlineData("object-top-left", 0)]
    [InlineData("object-top-right", 0)]
    [InlineData("object-bottom", 50)]
    [InlineData("object-bottom-left", 50)]
    [InlineData("object-bottom-right", 50)]
    public void Contain_Places_The_Control_In_The_Free_Space(string classes, double boundsY)
    {
        var shown = Place(classes, Stretch.Uniform, 100, 100);

        AssertRect(new Rect(0, boundsY, 100, 50), shown.Bounds);
        AssertRect(new Rect(0, 0, 200, 100), shown.Source);
        AssertRect(new Rect(0, 0, 100, 50), shown.Dest);
    }

    // The same bitmap with Stretch.UniformToFill (object-cover): it keeps its size and the box shows a 100x100 window,
    // so object-position chooses which part of the bitmap is visible.
    [Theory]
    [InlineData("", 50)]
    [InlineData("object-center", 50)]
    [InlineData("object-top", 50)]
    [InlineData("object-bottom", 50)]
    [InlineData("object-left", 0)]
    [InlineData("object-top-left", 0)]
    [InlineData("object-bottom-left", 0)]
    [InlineData("object-right", 100)]
    [InlineData("object-top-right", 100)]
    [InlineData("object-bottom-right", 100)]
    public void Cover_Chooses_The_Visible_Window_Of_The_Bitmap(string classes, double sourceX)
    {
        var shown = Place(classes, Stretch.UniformToFill, 100, 100);

        AssertRect(new Rect(0, 0, 100, 100), shown.Bounds);
        AssertRect(new Rect(sourceX, 0, 100, 100), shown.Source);
        AssertRect(new Rect(0, 0, 100, 100), shown.Dest);
    }

    // Stretch.None in a 300x300 box: the 200x100 bitmap is smaller than the box on both axes.
    [Theory]
    [InlineData("", 50, 100)]
    [InlineData("object-center", 50, 100)]
    [InlineData("object-top-left", 0, 0)]
    [InlineData("object-top", 50, 0)]
    [InlineData("object-top-right", 100, 0)]
    [InlineData("object-left", 0, 100)]
    [InlineData("object-right", 100, 100)]
    [InlineData("object-bottom-left", 0, 200)]
    [InlineData("object-bottom", 50, 200)]
    [InlineData("object-bottom-right", 100, 200)]
    public void None_Positions_A_Smaller_Bitmap_On_Both_Axes(string classes, double boundsX, double boundsY)
    {
        var shown = Place(classes, Stretch.None, 300, 300);

        AssertRect(new Rect(boundsX, boundsY, 200, 100), shown.Bounds);
        AssertRect(new Rect(0, 0, 200, 100), shown.Source);
    }

    // Stretch.None in a 150x150 box: the bitmap overflows horizontally (cropped, so the window moves) and fits
    // vertically (the control is placed by layout) at the same time.
    [Theory]
    [InlineData("object-top-left", 0, 0)]
    [InlineData("object-top", 25, 0)]
    [InlineData("object-top-right", 50, 0)]
    [InlineData("object-left", 0, 25)]
    [InlineData("object-center", 25, 25)]
    [InlineData("object-right", 50, 25)]
    [InlineData("object-bottom-left", 0, 50)]
    [InlineData("object-bottom", 25, 50)]
    [InlineData("object-bottom-right", 50, 50)]
    public void None_Mixes_A_Cropped_Axis_And_A_Placed_Axis(string classes, double sourceX, double boundsY)
    {
        var shown = Place(classes, Stretch.None, 150, 150);

        AssertRect(new Rect(0, boundsY, 150, 100), shown.Bounds);
        AssertRect(new Rect(sourceX, 0, 150, 100), shown.Source);
        AssertRect(new Rect(0, 0, 150, 100), shown.Dest);
    }

    [Theory]
    [InlineData("object-[left_top]", 0, 0)]
    [InlineData("object-[top_left]", 0, 0)]
    [InlineData("object-[right_bottom]", 50, 50)]
    [InlineData("object-[bottom_right]", 50, 50)]
    [InlineData("object-[center_center]", 25, 25)]
    [InlineData("object-[top]", 25, 0)]
    [InlineData("object-[bottom]", 25, 50)]
    [InlineData("object-[left]", 0, 25)]
    [InlineData("object-[right]", 50, 25)]
    [InlineData("object-[25%]", 12.5, 25)]
    [InlineData("object-[25%_0%]", 12.5, 0)]
    [InlineData("object-[0%_100%]", 0, 50)]
    [InlineData("object-[100%_100%]", 50, 50)]
    public void Arbitrary_Values_Follow_Css_Object_Position(string classes, double sourceX, double boundsY)
    {
        var shown = Place(classes, Stretch.None, 150, 150);

        AssertRect(new Rect(0, boundsY, 150, 100), shown.Bounds);
        AssertRect(new Rect(sourceX, 0, 150, 100), shown.Source);
    }

    [Theory]
    [InlineData("object-[25%_0%]", 0)]
    [InlineData("object-[25%_24%]", 0)]
    [InlineData("object-[25%_25%]", 25)]
    [InlineData("object-[25%_40%]", 25)]
    [InlineData("object-[25%_60%]", 25)]
    [InlineData("object-[25%_74%]", 25)]
    [InlineData("object-[25%_75%]", 50)]
    [InlineData("object-[25%_100%]", 50)]
    public void A_Percentage_Snaps_To_The_Nearest_Anchor_Where_Only_Alignment_Applies(string classes, double boundsY)
    {
        // Vertically the bitmap fits, so the control can only be start / center / end aligned: below 25% snaps to the
        // start, 25% up to just under 75% to the center, and 75% and above to the end (ties round up).
        var shown = Place(classes, Stretch.None, 150, 150);

        AssertRect(new Rect(0, boundsY, 150, 100), shown.Bounds);
    }

    [Theory]
    [InlineData("object-[200%_0%]", 100)]
    [InlineData("object-[-50%_0%]", 0)]
    public void Percentages_Outside_The_Box_Clamp_To_The_Edges(string classes, double sourceX)
    {
        var shown = Place(classes, Stretch.UniformToFill, 100, 100);

        AssertRect(new Rect(sourceX, 0, 100, 100), shown.Source);
    }

    // The common Tailwind form puts the box size on the Image itself (size-24 object-cover object-top).
    [Theory]
    [InlineData("size-[100]", "object-contain", 25)]
    [InlineData("size-[100]", "object-contain object-top", 0)]
    [InlineData("size-[100]", "object-contain object-bottom", 50)]
    public void An_Image_Carrying_Its_Own_Size_Is_Positioned_Inside_That_Size(string size, string classes, double boundsY)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Source = recording };

            Tw.SetClass(image, $"{size} {classes}");

            var host = new Canvas { Width = 400, Height = 400 };
            host.Children.Add(image);
            host.Measure(new Size(400, 400));
            host.Arrange(new Rect(0, 0, 400, 400));

            // The Image keeps the 100x100 box as its slot, so layout puts the fitted 100x50 control inside it.
            Assert.Equal(100, image.Width);
            AssertRect(new Rect(0, boundsY, 100, 50), image.Bounds);
        });
    }

    [Theory]
    [InlineData("object-cover object-left", 0)]
    [InlineData("object-cover object-center", 50)]
    [InlineData("object-cover object-right", 100)]
    public void An_Image_Carrying_Its_Own_Size_Crops_At_The_Chosen_Position(string classes, double sourceX)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Source = recording };

            Tw.SetClass(image, $"size-[100] {classes}");

            var host = new Canvas { Width = 400, Height = 400 };
            host.Children.Add(image);
            host.Measure(new Size(400, 400));
            host.Arrange(new Rect(0, 0, 400, 400));
            new RenderTargetBitmap(new PixelSize(400, 400)).Render(host);

            var call = Assert.Single(recording.Calls);
            AssertRect(new Rect(0, 0, 100, 100), image.Bounds);
            AssertRect(new Rect(sourceX, 0, 100, 100), call.Source);
            AssertRect(new Rect(0, 0, 100, 100), call.Dest);
        });
    }

    [Fact]
    public void The_Last_Position_Class_Wins_As_A_Whole()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var image = new Image { Source = new RecordingImage() };

            // object-left after object-top replaces the whole position (left, center), as one CSS property would, so
            // the vertical alignment object-top assigned is withdrawn.
            Tw.SetClass(image, "object-top object-left");

            Assert.Equal(HorizontalAlignment.Left, image.HorizontalAlignment);
            Assert.Equal(VerticalAlignment.Stretch, image.VerticalAlignment);
        });
    }

    [Theory]
    [InlineData("object-top justify-self-end", HorizontalAlignment.Right, VerticalAlignment.Top)]
    [InlineData("justify-self-end object-top", HorizontalAlignment.Right, VerticalAlignment.Top)]
    [InlineData("object-left justify-self-end", HorizontalAlignment.Right, VerticalAlignment.Stretch)]
    [InlineData("justify-self-end object-left", HorizontalAlignment.Left, VerticalAlignment.Stretch)]
    [InlineData("justify-self-end object-center", HorizontalAlignment.Right, VerticalAlignment.Stretch)]
    public void Alignment_Composes_With_Self_Alignment_In_Token_Order(string classes, HorizontalAlignment horizontal, VerticalAlignment vertical)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var image = new Image { Source = new RecordingImage() };

            Tw.SetClass(image, classes);

            Assert.Equal(horizontal, image.HorizontalAlignment);
            Assert.Equal(vertical, image.VerticalAlignment);
        });
    }

    [Theory]
    [InlineData("object-[foo]")]
    [InlineData("object-[top_top]")]
    [InlineData("object-[left_right]")]
    [InlineData("object-[10px]")]
    [InlineData("object-[10px_20px]")]
    [InlineData("object-[1_2_3]")]
    [InlineData("object-[]")]
    [InlineData("object-(--pos)")]
    public void Invalid_Values_Change_Nothing(string classes)
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Source = recording };

            Tw.SetClass(image, classes);

            Assert.Same(recording, image.Source);
            Assert.Equal(HorizontalAlignment.Stretch, image.HorizontalAlignment);
            Assert.Equal(VerticalAlignment.Stretch, image.VerticalAlignment);
        });
    }

    [Fact]
    public void Center_Installs_And_Assigns_Nothing_Because_It_Is_Avalonias_Default()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Source = recording, HorizontalAlignment = HorizontalAlignment.Right };

            Tw.SetClass(image, "object-center");

            Assert.Same(recording, image.Source);
            Assert.Equal(HorizontalAlignment.Right, image.HorizontalAlignment);
        });
    }

    [Fact]
    public void Removing_The_Class_Restores_The_Source_And_The_Alignment()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Source = recording };

            Tw.SetClass(image, "object-top-left");
            Assert.NotSame(recording, image.Source);
            Assert.Equal(recording.Size, image.Source!.Size);
            Assert.Equal(HorizontalAlignment.Left, image.HorizontalAlignment);
            Assert.Equal(VerticalAlignment.Top, image.VerticalAlignment);

            Tw.SetClass(image, "object-top-left object-center");
            Assert.Same(recording, image.Source);
            Assert.Equal(HorizontalAlignment.Stretch, image.HorizontalAlignment);
            Assert.Equal(VerticalAlignment.Stretch, image.VerticalAlignment);

            Tw.SetClass(image, "object-bottom");
            Assert.NotSame(recording, image.Source);
            Assert.Equal(VerticalAlignment.Bottom, image.VerticalAlignment);

            Tw.SetClass(image, string.Empty);
            Assert.Same(recording, image.Source);
            Assert.Equal(VerticalAlignment.Stretch, image.VerticalAlignment);
        });
    }

    [Fact]
    public void Changing_The_Position_Rewraps_Without_Stacking_Wrappers()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Source = recording, Stretch = Stretch.UniformToFill };

            Tw.SetClass(image, "object-left");
            Tw.SetClass(image, "object-right");

            var shown = Show(image, recording, 100, 100);
            AssertRect(new Rect(100, 0, 100, 100), shown.Source);

            Tw.SetClass(image, string.Empty);
            Assert.Same(recording, image.Source);
        });
    }

    [Fact]
    public void A_New_Source_Is_Positioned_Too()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var first = new RecordingImage();
            var second = new RecordingImage();
            var image = new Image { Source = first, Stretch = Stretch.UniformToFill };

            Tw.SetClass(image, "object-left");
            image.Source = second;

            var shown = Show(image, second, 100, 100);
            AssertRect(new Rect(0, 0, 100, 100), shown.Source);
            Assert.Empty(first.Calls);

            Tw.SetClass(image, string.Empty);
            Assert.Same(second, image.Source);
        });
    }

    [Fact]
    public void A_Bound_Source_Keeps_Updating_And_Stays_Positioned()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var first = new RecordingImage();
            var second = new RecordingImage();
            var pushed = new PushedSource();
            var image = new Image { Stretch = Stretch.UniformToFill };
            image.Bind(Image.SourceProperty, pushed);
            pushed.Push(first);

            Tw.SetClass(image, "object-right");
            AssertRect(new Rect(100, 0, 100, 100), Show(image, first, 100, 100).Source);

            // The binding pushes a new image: it must replace our wrapper and then be wrapped itself.
            pushed.Push(second);
            AssertRect(new Rect(100, 0, 100, 100), Show(image, second, 100, 100).Source);

            Tw.SetClass(image, string.Empty);
            Assert.Same(second, image.Source);
        });
    }

    [Fact]
    public void A_Null_Source_Is_Tolerated_Until_One_Arrives()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var recording = new RecordingImage();
            var image = new Image { Stretch = Stretch.UniformToFill };

            Tw.SetClass(image, "object-left");
            Assert.Null(image.Source);

            image.Source = recording;
            AssertRect(new Rect(0, 0, 100, 100), Show(image, recording, 100, 100).Source);
        });
    }

    [Fact]
    public void A_NonImage_Element_Ignores_The_Utility()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "object-top-left");

            Assert.Equal(HorizontalAlignment.Stretch, border.HorizontalAlignment);
            Assert.Equal(VerticalAlignment.Stretch, border.VerticalAlignment);
        });
    }
}
