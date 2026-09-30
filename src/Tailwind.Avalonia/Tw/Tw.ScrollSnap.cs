using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // Avalonia's own snap points only react to touch/pan gestures, so a mouse wheel or scrollbar drag never snaps.
    // While a ScrollViewer has snap-x / snap-y enabled, this helper settles the offset on the nearest child boundary
    // shortly after scrolling stops, which gives the CSS scroll-snap feel for every input device.
    private static readonly ConditionalWeakTable<ScrollViewer, ScrollSnapHelper> ScrollSnapHelpers = new();

    private static void SyncScrollSnap(AvaloniaObject element)
    {
        if (element is not ScrollViewer viewer)
        {
            return;
        }

        var enabled = viewer.HorizontalSnapPointsType != SnapPointsType.None
            || viewer.VerticalSnapPointsType != SnapPointsType.None;

        if (enabled)
        {
            if (!ScrollSnapHelpers.TryGetValue(viewer, out _))
            {
                ScrollSnapHelpers.Add(viewer, new ScrollSnapHelper(viewer));
            }
        }
        else if (ScrollSnapHelpers.TryGetValue(viewer, out var helper))
        {
            helper.Dispose();
            ScrollSnapHelpers.Remove(viewer);
        }
    }

    internal static bool SnapScrollViewer(ScrollViewer viewer)
    {
        if (viewer.Content is not Panel panel)
        {
            return false;
        }

        var offset = viewer.Offset;
        var x = viewer.HorizontalSnapPointsType == SnapPointsType.None
            ? offset.X
            : Nearest(panel, viewer, horizontal: true, offset.X);
        var y = viewer.VerticalSnapPointsType == SnapPointsType.None
            ? offset.Y
            : Nearest(panel, viewer, horizontal: false, offset.Y);

        if (Math.Abs(x - offset.X) < 0.5 && Math.Abs(y - offset.Y) < 0.5)
        {
            return false;
        }

        viewer.Offset = new Vector(x, y);
        return true;
    }

    private static double Nearest(Panel panel, ScrollViewer viewer, bool horizontal, double current)
    {
        var alignment = horizontal ? viewer.HorizontalSnapPointsAlignment : viewer.VerticalSnapPointsAlignment;
        var viewport = horizontal ? viewer.Viewport.Width : viewer.Viewport.Height;
        var max = Math.Max(0, (horizontal ? viewer.Extent.Width : viewer.Extent.Height) - viewport);
        var best = current;
        var bestDistance = double.MaxValue;

        foreach (var child in panel.Children)
        {
            if (!child.IsVisible)
            {
                continue;
            }

            var origin = child.TranslatePoint(default, panel) ?? default;
            var start = horizontal ? origin.X : origin.Y;
            var size = horizontal ? child.Bounds.Width : child.Bounds.Height;
            var target = alignment switch
            {
                SnapPointsAlignment.Center => start + size / 2 - viewport / 2,
                SnapPointsAlignment.Far => start + size - viewport,
                _ => start,
            };

            target = Math.Clamp(target, 0, max);
            var distance = Math.Abs(target - current);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = target;
            }
        }

        return best;
    }

    private sealed class ScrollSnapHelper : IDisposable
    {
        private readonly ScrollViewer viewer;
        private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(120) };

        public ScrollSnapHelper(ScrollViewer viewer)
        {
            this.viewer = viewer;
            timer.Tick += OnTick;
            viewer.ScrollChanged += OnScrollChanged;
        }

        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            timer.Stop();
            SnapScrollViewer(viewer);
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Tick -= OnTick;
            viewer.ScrollChanged -= OnScrollChanged;
        }
    }
}
