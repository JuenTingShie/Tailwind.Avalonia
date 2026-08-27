using Avalonia;
using Avalonia.Logging;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private static void ApplyUtilities(AvaloniaObject element, string? classList)
    {
        var tokens = string.IsNullOrWhiteSpace(classList)
            ? Array.Empty<string>()
            : classList.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        ApplyUtilities(element, tokens);
    }

    private static void ApplyUtilities(AvaloniaObject element, string[] tokens)
    {
        var previousMask = element.GetValue(AppliedMaskProperty);
        var newMask = 0;

        var hasMargin = false;
        var hasPadding = false;
        var hasBackground = false;
        var hasForeground = false;
        var hasBorderBrush = false;
        var hasWidth = false;
        var hasMinWidth = false;
        var hasMaxWidth = false;
        var hasHeight = false;
        var hasMinHeight = false;
        var hasMaxHeight = false;
        var hasFontSize = false;
        var hasCornerRadius = false;
        var hasOpacity = false;
        var opacity = default(double);
        var backgroundVariants = new IBrush?[VariantCount];
        var foregroundVariants = new IBrush?[VariantCount];
        var borderBrushVariants = new IBrush?[VariantCount];
        var opacityVariants = new double?[VariantCount];
        var margin = default(Thickness);
        var padding = default(Thickness);
        IBrush? background = null;
        IBrush? foreground = null;
        IBrush? borderBrush = null;
        var width = default(double);
        var minWidth = default(double);
        var maxWidth = default(double);
        var height = default(double);
        var minHeight = default(double);
        var maxHeight = default(double);
        var fontSize = default(double);
        var cornerRadius = default(CornerRadius);

        foreach (var rawToken in tokens)
        {
            if (TryParseVariantToken(rawToken, out var variantKind, out var variantRemainder))
            {
                if (TryParseBrushUtility(variantRemainder, out var variantBrush))
                {
                    switch (variantBrush.Target)
                    {
                        case BrushTarget.Background:
                            backgroundVariants[(int)variantKind] = variantBrush.Brush;
                            break;

                        case BrushTarget.Foreground:
                            foregroundVariants[(int)variantKind] = variantBrush.Brush;
                            break;

                        case BrushTarget.BorderBrush:
                            borderBrushVariants[(int)variantKind] = variantBrush.Brush;
                            break;
                    }

                    continue;
                }

                if (TryParseOpacityUtility(variantRemainder, out var variantOpacity))
                {
                    opacityVariants[(int)variantKind] = variantOpacity;
                    continue;
                }

                Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                    element,
                    "Tw.Class ignored unrecognized utility token '{Token}'.",
                    rawToken);
                continue;
            }

            var token = rawToken;

            if (TryParseSpacingUtility(token, out var spacingUtility))
            {
                switch (spacingUtility.Target)
                {
                    case SpacingTarget.Margin:
                        if (!hasMargin)
                        {
                            margin = default;
                            hasMargin = true;
                        }

                        margin = ApplyEdge(margin, spacingUtility.Edge, spacingUtility.Pixels, element);
                        break;

                    case SpacingTarget.Padding:
                        if (!hasPadding)
                        {
                            padding = default;
                            hasPadding = true;
                        }

                        padding = ApplyEdge(padding, spacingUtility.Edge, spacingUtility.Pixels, element);
                        break;
                }

                continue;
            }

            if (TryParseSizingUtility(token, out var sizingUtility))
            {
                switch (sizingUtility.Target)
                {
                    case SizingTarget.Width:
                        width = sizingUtility.Pixels;
                        hasWidth = true;
                        break;

                    case SizingTarget.MinWidth:
                        minWidth = sizingUtility.Pixels;
                        hasMinWidth = true;
                        break;

                    case SizingTarget.MaxWidth:
                        maxWidth = sizingUtility.Pixels;
                        hasMaxWidth = true;
                        break;

                    case SizingTarget.Height:
                        height = sizingUtility.Pixels;
                        hasHeight = true;
                        break;

                    case SizingTarget.MinHeight:
                        minHeight = sizingUtility.Pixels;
                        hasMinHeight = true;
                        break;

                    case SizingTarget.MaxHeight:
                        maxHeight = sizingUtility.Pixels;
                        hasMaxHeight = true;
                        break;
                }

                continue;
            }

            if (TryParseCornerRadiusUtility(token, out var cornerRadiusUtility))
            {
                if (!hasCornerRadius)
                {
                    cornerRadius = default;
                    hasCornerRadius = true;
                }

                cornerRadius = ApplyCornerRadiusEdge(cornerRadius, cornerRadiusUtility.Edge, cornerRadiusUtility.Pixels);
                continue;
            }

            if (TryParseFontSizeUtility(token, out var fontSizeUtility))
            {
                fontSize = fontSizeUtility.Pixels;
                hasFontSize = true;
                continue;
            }

            if (TryParseOpacityUtility(token, out var opacityUtility))
            {
                opacity = opacityUtility;
                hasOpacity = true;
                continue;
            }

            if (!TryParseBrushUtility(token, out var brushUtility))
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                    element,
                    "Tw.Class ignored unrecognized utility token '{Token}'.",
                    token);
                continue;
            }

            switch (brushUtility.Target)
            {
                case BrushTarget.Background:
                    background = brushUtility.Brush;
                    hasBackground = true;
                    break;

                case BrushTarget.Foreground:
                    foreground = brushUtility.Brush;
                    hasForeground = true;
                    break;

                case BrushTarget.BorderBrush:
                    borderBrush = brushUtility.Brush;
                    hasBorderBrush = true;
                    break;
            }
        }

        // Only elements that combine a base value with at least one hover:/pressed:/focus:
        // variant for the same property need to go through Avalonia's Style engine (required so
        // the variant can outrank the base -- local values always beat styles otherwise). Routing
        // every bg-/text-/border-/opacity- utility through a per-element Style regardless was an
        // O(n^2) cost across a page: each Style carries a bare type selector, and Avalonia applies
        // an element's local Styles to itself AND its whole subtree, so every such Style forces
        // consideration of every same-typed descendant. Elements with no variant for a category
        // keep going through the cheap, O(1) SetValue path used before variants existed.
        var hasBackgroundVariant = Array.Exists(backgroundVariants, v => v is not null);
        var hasForegroundVariant = Array.Exists(foregroundVariants, v => v is not null);
        var hasBorderBrushVariant = Array.Exists(borderBrushVariants, v => v is not null);
        var hasOpacityVariant = Array.Exists(opacityVariants, v => v is not null);

        var backgroundDirect = hasBackground && !hasBackgroundVariant;
        var foregroundDirect = hasForeground && !hasForegroundVariant;
        var borderBrushDirect = hasBorderBrush && !hasBorderBrushVariant;
        var opacityDirect = hasOpacity && !hasOpacityVariant;

        Span<PendingUtility> pendingUtilities =
        [
            new(MarginMask, hasMargin, () => TrySetThickness(element, "Margin", margin), () => ClearThickness(element, "Margin")),
            new(PaddingMask, hasPadding, () => TrySetThickness(element, "Padding", padding), () => ClearThickness(element, "Padding")),
            new(WidthMask, hasWidth, () => TrySetDouble(element, "Width", width), () => ClearDouble(element, "Width")),
            new(MinWidthMask, hasMinWidth, () => TrySetDouble(element, "MinWidth", minWidth), () => ClearDouble(element, "MinWidth")),
            new(MaxWidthMask, hasMaxWidth, () => TrySetDouble(element, "MaxWidth", maxWidth), () => ClearDouble(element, "MaxWidth")),
            new(HeightMask, hasHeight, () => TrySetDouble(element, "Height", height), () => ClearDouble(element, "Height")),
            new(MinHeightMask, hasMinHeight, () => TrySetDouble(element, "MinHeight", minHeight), () => ClearDouble(element, "MinHeight")),
            new(MaxHeightMask, hasMaxHeight, () => TrySetDouble(element, "MaxHeight", maxHeight), () => ClearDouble(element, "MaxHeight")),
            new(FontSizeMask, hasFontSize, () => TrySetDouble(element, "FontSize", fontSize), () => ClearDouble(element, "FontSize")),
            new(BackgroundMask, backgroundDirect, () => TrySetBrush(element, "Background", background), () => ClearBrush(element, "Background")),
            new(ForegroundMask, foregroundDirect, () => TrySetBrush(element, "Foreground", foreground), () => ClearBrush(element, "Foreground")),
            new(BorderBrushMask, borderBrushDirect, () => TrySetBrush(element, "BorderBrush", borderBrush), () => ClearBrush(element, "BorderBrush")),
            new(OpacityMask, opacityDirect, () => TrySetDouble(element, "Opacity", opacity), () => ClearDouble(element, "Opacity")),
            new(CornerRadiusMask, hasCornerRadius, () => TrySetCornerRadius(element, "CornerRadius", cornerRadius), () => ClearCornerRadius(element, "CornerRadius")),
        ];

        foreach (var pending in pendingUtilities)
        {
            if (pending.HasValue && pending.TrySet())
            {
                newMask |= pending.Mask;
            }
            else if ((previousMask & pending.Mask) != 0)
            {
                pending.Clear();
            }
        }

        element.SetValue(AppliedMaskProperty, newMask);

        ApplyVariantStyles(
            element,
            new BrushCategoryState(hasBackground && hasBackgroundVariant, background, backgroundVariants),
            new BrushCategoryState(hasForeground && hasForegroundVariant, foreground, foregroundVariants),
            new BrushCategoryState(hasBorderBrush && hasBorderBrushVariant, borderBrush, borderBrushVariants),
            new OpacityCategoryState(hasOpacity && hasOpacityVariant, opacity, opacityVariants));
    }

    private readonly record struct PendingUtility(int Mask, bool HasValue, Func<bool> TrySet, Action Clear);

    private static Thickness ApplyEdge(Thickness current, SpacingEdge edge, double value, AvaloniaObject element)
    {
        var isRightToLeft = element is Visual visual && Visual.GetFlowDirection(visual) == FlowDirection.RightToLeft;

        return edge switch
        {
            SpacingEdge.All => new Thickness(value),
            SpacingEdge.X => new Thickness(value, current.Top, value, current.Bottom),
            SpacingEdge.Y => new Thickness(current.Left, value, current.Right, value),
            SpacingEdge.Top => new Thickness(current.Left, value, current.Right, current.Bottom),
            SpacingEdge.Right => new Thickness(current.Left, current.Top, value, current.Bottom),
            SpacingEdge.Bottom => new Thickness(current.Left, current.Top, current.Right, value),
            SpacingEdge.Left => new Thickness(value, current.Top, current.Right, current.Bottom),
            SpacingEdge.Start when isRightToLeft => new Thickness(current.Left, current.Top, value, current.Bottom),
            SpacingEdge.Start => new Thickness(value, current.Top, current.Right, current.Bottom),
            SpacingEdge.End when isRightToLeft => new Thickness(value, current.Top, current.Right, current.Bottom),
            SpacingEdge.End => new Thickness(current.Left, current.Top, value, current.Bottom),
            SpacingEdge.BlockStart => new Thickness(current.Left, value, current.Right, current.Bottom),
            SpacingEdge.BlockEnd => new Thickness(current.Left, current.Top, current.Right, value),
            _ => current,
        };
    }

    private static CornerRadius ApplyCornerRadiusEdge(CornerRadius current, CornerRadiusEdge edge, double value) => edge switch
    {
        CornerRadiusEdge.All => new CornerRadius(value),
        CornerRadiusEdge.Top => new CornerRadius(value, value, current.BottomRight, current.BottomLeft),
        CornerRadiusEdge.Right => new CornerRadius(current.TopLeft, value, value, current.BottomLeft),
        CornerRadiusEdge.Bottom => new CornerRadius(current.TopLeft, current.TopRight, value, value),
        CornerRadiusEdge.Left => new CornerRadius(value, current.TopRight, current.BottomRight, value),
        CornerRadiusEdge.TopLeft => new CornerRadius(value, current.TopRight, current.BottomRight, current.BottomLeft),
        CornerRadiusEdge.TopRight => new CornerRadius(current.TopLeft, value, current.BottomRight, current.BottomLeft),
        CornerRadiusEdge.BottomRight => new CornerRadius(current.TopLeft, current.TopRight, value, current.BottomLeft),
        CornerRadiusEdge.BottomLeft => new CornerRadius(current.TopLeft, current.TopRight, current.BottomRight, value),
        _ => current,
    };
}
