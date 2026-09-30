using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// Sits inside a horizontal ScrollViewer and measures its child at the viewport width instead of infinity,
/// so panels still wrap and stretch on wide windows, while a child that is wider than the viewport
/// (fixed-width examples on a narrow window) grows the extent and becomes scrollable.
/// </summary>
public class StageFit : Decorator
{
    private ScrollViewer? scroller;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        scroller = this.FindAncestorOfType<ScrollViewer>();

        if (scroller is not null)
        {
            scroller.PropertyChanged += OnScrollerPropertyChanged;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (scroller is not null)
        {
            scroller.PropertyChanged -= OnScrollerPropertyChanged;
            scroller = null;
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void OnScrollerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ScrollViewer.ViewportProperty || e.Property == BoundsProperty)
        {
            InvalidateMeasure();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Child is null)
        {
            return default;
        }

        var width = scroller is { Viewport.Width: > 0 } ? scroller.Viewport.Width : (scroller?.Bounds.Width ?? 0);

        if (width <= 0 || double.IsInfinity(width))
        {
            width = double.IsInfinity(availableSize.Width) ? double.PositiveInfinity : availableSize.Width;
        }

        // Layout clamps a child's desired size to the space it was offered, so a fixed-width example only reveals
        // its natural width when measured without a limit. Content that can wrap (WrapPanel, wrapping text)
        // would report an absurdly wide natural width, so it keeps the viewport width and simply wraps.
        Child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
        var natural = Child.DesiredSize;

        if (double.IsInfinity(width) || (natural.Width > width && HasFlexibleWrap(Child)) || natural.Width <= width)
        {
            Child.Measure(new Size(width, availableSize.Height));
            var desired = Child.DesiredSize;
            return new Size(double.IsInfinity(width) ? desired.Width : global::System.Math.Max(width, desired.Width), desired.Height);
        }

        return new Size(natural.Width, natural.Height);
    }

    private static bool HasFlexibleWrap(Control root)
    {
        foreach (var visual in root.GetVisualDescendants())
        {
            var wraps = visual is WrapPanel || visual is TextBlock { TextWrapping: TextWrapping.Wrap or TextWrapping.WrapWithOverflow };

            if (!wraps)
            {
                continue;
            }

            var fixedWidth = false;

            for (Visual? current = visual; current is not null && !ReferenceEquals(current, root.GetVisualParent()); current = current.GetVisualParent())
            {
                if (current is Layoutable { Width: > 0 } || current is ScrollViewer)
                {
                    fixedWidth = true;
                    break;
                }
            }

            if (!fixedWidth)
            {
                return true;
            }
        }

        return false;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Child?.Arrange(new Rect(finalSize));
        return finalSize;
    }
}
