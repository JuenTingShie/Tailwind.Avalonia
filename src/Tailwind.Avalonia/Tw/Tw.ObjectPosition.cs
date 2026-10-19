using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Logging;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // object-position places an Image's bitmap inside its box. Avalonia's Image has no alignment property, so the two
    // axes are handled by what Avalonia does offer:
    //
    //   * Where the bitmap is smaller than the box (object-contain, object-scale-down, object-none on a big box) the
    //     Image control shrinks to the bitmap and layout places it in the box, so the position becomes the control's
    //     HorizontalAlignment / VerticalAlignment. Those only have start, center and end, so a percentage snaps to
    //     the nearest of 0%, 50% and 100%.
    //   * Where the bitmap overflows the box (object-cover, object-none on a small box) Image crops around the
    //     center. Wrapping Image.Source in a thin IImage decorator moves the cropped window, so any percentage is
    //     exact. The wrapper is installed with SetCurrentValue so a Source binding keeps working, and it is removed
    //     again with the class.
    //
    // object-center is Avalonia's own behavior, so it assigns and installs nothing.
    private readonly record struct ObjectPosition(double FractionX, double FractionY)
    {
        public static ObjectPosition Center { get; } = new(0.5, 0.5);

        public bool IsDefault => this == Center;
    }

    private static readonly Dictionary<string, ObjectPosition> ObjectPositionPresets = new(StringComparer.Ordinal)
    {
        ["object-top-left"] = new(0, 0),
        ["object-top"] = new(0.5, 0),
        ["object-top-right"] = new(1, 0),
        ["object-left"] = new(0, 0.5),
        ["object-center"] = new(0.5, 0.5),
        ["object-right"] = new(1, 0.5),
        ["object-bottom-left"] = new(0, 1),
        ["object-bottom"] = new(0.5, 1),
        ["object-bottom-right"] = new(1, 1),
    };

    private static readonly ConditionalWeakTable<Image, ObjectPositionHelper> ObjectPositionHelpers = new();

    private static bool TryParseObjectPositionUtility(string token, out ObjectPosition position)
    {
        if (ObjectPositionPresets.TryGetValue(token, out position))
        {
            return true;
        }

        // object-[<x>_<y>] takes CSS object-position values with `_` standing for a space. This library accepts the
        // keywords and percentages; lengths (object-[10px_20px]) and object-(<custom-property>) are not supported.
        const string Prefix = "object-[";

        if (token.StartsWith(Prefix, StringComparison.Ordinal) && token.EndsWith(']'))
        {
            return TryParseObjectPositionValue(token[Prefix.Length..^1], out position);
        }

        position = default;
        return false;
    }

    private enum PositionAxis
    {
        Either,
        Horizontal,
        Vertical,
    }

    private readonly record struct PositionComponent(PositionAxis Axis, double Fraction);

    private static bool TryParseObjectPositionValue(string value, out ObjectPosition position)
    {
        position = default;

        var parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is < 1 or > 2 || !TryParsePositionComponent(parts[0], out var first))
        {
            return false;
        }

        var center = new PositionComponent(PositionAxis.Either, 0.5);
        PositionComponent horizontal;
        PositionComponent vertical;

        if (parts.Length == 1)
        {
            // A single keyword names one axis and centers the other (top -> center top); a percentage is the x position.
            (horizontal, vertical) = first.Axis == PositionAxis.Vertical ? (center, first) : (first, center);
        }
        else
        {
            if (!TryParsePositionComponent(parts[1], out var second))
            {
                return false;
            }

            // Keywords may come in either order (top_left == left_top); otherwise the order is x then y.
            (horizontal, vertical) = first.Axis == PositionAxis.Vertical || second.Axis == PositionAxis.Horizontal
                ? (second, first)
                : (first, second);
        }

        if (horizontal.Axis == PositionAxis.Vertical || vertical.Axis == PositionAxis.Horizontal)
        {
            return false;
        }

        position = new ObjectPosition(horizontal.Fraction, vertical.Fraction);
        return true;
    }

    private static bool TryParsePositionComponent(string text, out PositionComponent component)
    {
        component = default;

        switch (text)
        {
            case "left": component = new(PositionAxis.Horizontal, 0); return true;
            case "right": component = new(PositionAxis.Horizontal, 1); return true;
            case "top": component = new(PositionAxis.Vertical, 0); return true;
            case "bottom": component = new(PositionAxis.Vertical, 1); return true;
            case "center": component = new(PositionAxis.Either, 0.5); return true;
        }

        if (text.EndsWith('%') &&
            double.TryParse(text[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var percent) &&
            double.IsFinite(percent))
        {
            component = new(PositionAxis.Either, percent / 100.0);
            return true;
        }

        return false;
    }

    // The alignment half of a position: the control's alignment on the axes where the bitmap is smaller than the box.
    // Center is Avalonia's default (a Stretch-aligned Image that shrinks to its bitmap is centered), so it assigns
    // nothing and leaves any explicit alignment alone. Values between the anchors snap to the nearest one.
    private static void AssignObjectPositionAlignment(Dictionary<string, object> keywordValues, ObjectPosition position, ref object? horizontalAssigned, ref object? verticalAssigned)
    {
        // The last object-position class replaces the whole position, so withdraw what an earlier one assigned,
        // unless a later utility (self-*, justify-self-*) has since taken the property over.
        Withdraw(keywordValues, "HorizontalAlignment", ref horizontalAssigned);
        Withdraw(keywordValues, "VerticalAlignment", ref verticalAssigned);

        horizontalAssigned = SnapToAnchor(position.FractionX) switch
        {
            0 => HorizontalAlignment.Left,
            2 => HorizontalAlignment.Right,
            _ => (object?)null,
        };

        verticalAssigned = SnapToAnchor(position.FractionY) switch
        {
            0 => VerticalAlignment.Top,
            2 => VerticalAlignment.Bottom,
            _ => (object?)null,
        };

        if (horizontalAssigned is not null)
        {
            keywordValues["HorizontalAlignment"] = horizontalAssigned;
        }

        if (verticalAssigned is not null)
        {
            keywordValues["VerticalAlignment"] = verticalAssigned;
        }

        static void Withdraw(Dictionary<string, object> values, string property, ref object? assigned)
        {
            if (assigned is not null && values.TryGetValue(property, out var current) && current.Equals(assigned))
            {
                values.Remove(property);
            }

            assigned = null;
        }

        // 0 = start, 1 = center, 2 = end.
        static int SnapToAnchor(double fraction) => (int)Math.Round(Math.Clamp(fraction, 0, 1) * 2, MidpointRounding.AwayFromZero);
    }

    private static void SyncObjectPosition(AvaloniaObject element, ObjectPosition? position)
    {
        if (element is not Image image)
        {
            if (position is not null)
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                    element,
                    "Tw.Class ignored an object-position utility: {Element} is not an Image.",
                    element.GetType().Name);
            }

            return;
        }

        if (position is { } value)
        {
            if (ObjectPositionHelpers.TryGetValue(image, out var helper))
            {
                helper.Position = value;
            }
            else
            {
                ObjectPositionHelpers.Add(image, new ObjectPositionHelper(image, value));
            }
        }
        else if (ObjectPositionHelpers.TryGetValue(image, out var existing))
        {
            existing.Dispose();
            ObjectPositionHelpers.Remove(image);
        }
    }

    /// <summary>
    /// Moves the cropped window of a bitmap that overflows its box. <paramref name="sourceRect"/> and
    /// <paramref name="destRect"/> are the rectangles Image passes to Draw (the bitmap is centered and cropped to the
    /// viewport). On each axis where the scaled bitmap is longer than the viewport the window slides to
    /// <paramref name="fractionX"/> / <paramref name="fractionY"/> of the overflow; axes where it fits are untouched,
    /// because layout already placed the control there. Returns false for degenerate input, in which case the caller
    /// draws the original rectangles.
    /// </summary>
    internal static bool TryPlaceCroppedImage(Size sourceSize, Size viewport, Rect sourceRect, Rect destRect, double fractionX, double fractionY, out Rect placedSource)
    {
        placedSource = sourceRect;

        if (!SlideAxis(fractionX, sourceSize.Width, viewport.Width, sourceRect.Width, destRect.Width, out var sourceX) ||
            !SlideAxis(fractionY, sourceSize.Height, viewport.Height, sourceRect.Height, destRect.Height, out var sourceY))
        {
            return false;
        }

        placedSource = new Rect(sourceX ?? sourceRect.X, sourceY ?? sourceRect.Y, sourceRect.Width, sourceRect.Height);
        return true;
    }

    private static bool SlideAxis(double fraction, double sourceLength, double viewportLength, double sourceRectLength, double destLength, out double? sourceStart)
    {
        sourceStart = null;

        if (!(sourceRectLength > 0) || !(destLength > 0) || !(viewportLength > 0))
        {
            return false;
        }

        // Image scales the bitmap by destLength / sourceRectLength and shows a window of sourceRectLength of it. When
        // the window is shorter than the whole bitmap the bitmap overflows, and CSS puts the point `fraction` of the
        // way along the bitmap at the point `fraction` of the way along the box: the window starts that fraction of
        // the way through the hidden part. Fractions outside 0..1 would push the bitmap off the box, so they clamp.
        var hidden = sourceLength - sourceRectLength;

        if (hidden > 1e-6)
        {
            sourceStart = Math.Clamp(fraction, 0, 1) * hidden;
        }

        return true;
    }

    private sealed class PositionedImageSource : IImage
    {
        private readonly Image owner;
        private readonly ObjectPosition position;

        public PositionedImageSource(IImage inner, Image owner, ObjectPosition position)
        {
            Inner = inner;
            this.owner = owner;
            this.position = position;
        }

        public IImage Inner { get; }

        public Size Size => Inner.Size;

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            if (TryPlaceCroppedImage(Size, owner.Bounds.Size, sourceRect, destRect, position.FractionX, position.FractionY, out var placedSource))
            {
                Inner.Draw(context, placedSource, destRect);
                return;
            }

            Inner.Draw(context, sourceRect, destRect);
        }
    }

    private sealed class ObjectPositionHelper : IDisposable
    {
        private readonly Image image;
        private ObjectPosition position;
        private bool updating;

        // Image redraws when a DrawingImage changes in place through an internal Avalonia interface that a wrapper
        // cannot implement, so the helper forwards that notification itself. Other IImage types that change in place
        // need their Source reassigned to redraw while an object-position utility is active.
        private DrawingImage? watched;

        public ObjectPositionHelper(Image image, ObjectPosition position)
        {
            this.image = image;
            this.position = position;
            image.PropertyChanged += OnPropertyChanged;
            Refresh();
        }

        public ObjectPosition Position
        {
            set
            {
                if (position == value)
                {
                    return;
                }

                position = value;
                Refresh();
            }
        }

        public void Dispose()
        {
            image.PropertyChanged -= OnPropertyChanged;
            Watch(null);

            if (image.Source is PositionedImageSource wrapped)
            {
                SetSource(wrapped.Inner);
            }
        }

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            // A new Source (set directly, or pushed by a binding) replaces our wrapper, so wrap the new image.
            if (!updating && e.Property == Image.SourceProperty)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var source = image.Source is PositionedImageSource wrapped ? wrapped.Inner : image.Source;

            Watch(source as DrawingImage);
            SetSource(source is null || position.IsDefault ? source : new PositionedImageSource(source, image, position));
        }

        private void Watch(DrawingImage? drawing)
        {
            if (ReferenceEquals(watched, drawing))
            {
                return;
            }

            if (watched is not null)
            {
                watched.Invalidated -= OnInnerInvalidated;
            }

            watched = drawing;

            if (watched is not null)
            {
                watched.Invalidated += OnInnerInvalidated;
            }
        }

        private void OnInnerInvalidated(object? sender, EventArgs e) => image.InvalidateVisual();

        private void SetSource(IImage? target)
        {
            if (ReferenceEquals(target, image.Source))
            {
                return;
            }

            updating = true;

            try
            {
                image.SetCurrentValue(Image.SourceProperty, target);
            }
            finally
            {
                updating = false;
            }
        }
    }
}
