using Avalonia;
using Avalonia.Collections;
using Avalonia.Layout;
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
        var hasBorderWidth = false;
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
        double? defaultLineHeight = null;
        bool? invisible = null;
        var hasTextAlignment = false;
        Dictionary<string, object>? keywordValues = null;
        var hasBoxShadow = false;
        TransformState? transformState = null;
        LayoutState? layoutState = null;
        GradientState? gradientState = null;
        BackgroundImageState? backgroundImageState = null;
        RingState? ringState = null;
        TransitionState? transitionState = null;
        var widthFill = false;
        var heightFill = false;
        var hasTextDecoration = false;
        var textDecorationLocations = new List<TextDecorationLocation>();
        double? decorationThickness = null;
        string? decorationStyle = null;
        string? animationName = null;
        var hasNumericVariants = false;
        var numericFeatures = new List<string>();
        double? decorationOffset = null;
        IBrush? decorationBrush = null;
        var hasLetterSpacing = false;
        var hasLineHeight = false;
        var letterSpacing = default(TextMetricUtility);
        var lineHeight = default(TextMetricUtility);
        var boxShadow = default(BoxShadows);
        var insetShadow = default(BoxShadows);
        var hasInsetShadow = false;
        var textAlignment = default(TextAlignment);
        var opacity = default(double);
        var backgroundVariants = new IBrush?[VariantCount];
        var foregroundVariants = new IBrush?[VariantCount];
        var borderBrushVariants = new IBrush?[VariantCount];
        var opacityVariants = new double?[VariantCount];
        var margin = default(Thickness);
        var padding = default(Thickness);
        var borderWidth = default(Thickness);
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
            // placeholder: and selection: target pseudo-elements, which Avalonia exposes as plain brush properties:
            // placeholder:text-* -> PlaceholderForeground, selection:bg-* -> SelectionBrush and
            // selection:text-* -> SelectionForegroundBrush.
            if (TryParsePseudoElementBrush(rawToken, out var pseudoProperty, out var pseudoBrush))
            {
                keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
                keywordValues[pseudoProperty] = pseudoBrush;
                continue;
            }

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

            if (TryParseSpacingUtility(token, out var spacingUtility) || TryParseBorderWidthUtility(token, out spacingUtility))
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

                    case SpacingTarget.BorderWidth:
                        if (!hasBorderWidth)
                        {
                            borderWidth = default;
                            hasBorderWidth = true;
                        }

                        borderWidth = ApplyEdge(borderWidth, spacingUtility.Edge, spacingUtility.Pixels, element);
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
                        widthFill = sizingUtility.Fill;
                        break;

                    case SizingTarget.Size:
                        width = sizingUtility.Pixels;
                        height = sizingUtility.Pixels;
                        hasWidth = true;
                        hasHeight = true;
                        widthFill = sizingUtility.Fill;
                        heightFill = sizingUtility.Fill;
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
                        heightFill = sizingUtility.Fill;
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

                if (fontSizeUtility.LineHeight is { } pairedLineHeight)
                {
                    lineHeight = pairedLineHeight;
                    hasLineHeight = true;
                }

                defaultLineHeight = fontSizeUtility.DefaultLineHeight;

                continue;
            }

            if (TryParseLetterSpacingUtility(token, out var letterSpacingUtility))
            {
                letterSpacing = letterSpacingUtility;
                hasLetterSpacing = true;
                continue;
            }

            if (TryParseLineHeightUtility(token, out var lineHeightUtility))
            {
                lineHeight = lineHeightUtility;
                hasLineHeight = true;
                continue;
            }

            if (backgroundImageState is null ? TryApplyBackgroundImageToken(token, backgroundImageState = new BackgroundImageState()) : TryApplyBackgroundImageToken(token, backgroundImageState))
            {
                continue;
            }

            if (gradientState is null ? TryApplyGradientToken(token, gradientState = new GradientState()) : TryApplyGradientToken(token, gradientState))
            {
                continue;
            }

            if (ringState is null ? TryApplyRingToken(token, ringState = new RingState()) : TryApplyRingToken(token, ringState))
            {
                continue;
            }

            if (transitionState is null ? TryApplyTransitionToken(token, transitionState = new TransitionState()) : TryApplyTransitionToken(token, transitionState))
            {
                continue;
            }

            if (layoutState is null ? TryApplyLayoutToken(token, layoutState = new LayoutState()) : TryApplyLayoutToken(token, layoutState))
            {
                continue;
            }

            if (transformState is null ? TryApplyTransformToken(token, transformState = new TransformState()) : TryApplyTransformToken(token, transformState))
            {
                continue;
            }

            if (TryParseAnimationUtility(token, out var animation))
            {
                animationName = animation;
                continue;
            }

            if (TryParseNumericVariantUtility(token, out var numericFeature))
            {
                hasNumericVariants = true;

                if (numericFeature is null)
                {
                    numericFeatures.Clear();
                }
                else if (!numericFeatures.Contains(numericFeature))
                {
                    numericFeatures.Add(numericFeature);
                }

                continue;
            }

            if (token is "decoration-solid" or "decoration-dotted" or "decoration-dashed")
            {
                decorationStyle = token["decoration-".Length..];
                continue;
            }

            if (TryParseDecorationMetricUtility(token, out var isThickness, out var decorationMetric))
            {
                if (isThickness)
                {
                    decorationThickness = decorationMetric;
                }
                else
                {
                    decorationOffset = decorationMetric;
                }

                continue;
            }

            if (TryParseTextDecorationUtility(token, out var decorationLocation))
            {
                hasTextDecoration = true;

                if (decorationLocation is null)
                {
                    textDecorationLocations.Clear();
                }
                else if (!textDecorationLocations.Contains(decorationLocation.Value))
                {
                    textDecorationLocations.Add(decorationLocation.Value);
                }

                continue;
            }

            if (TryParseKeywordUtility(token, out var keywordAssignments))
            {
                keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);

                foreach (var assignment in keywordAssignments)
                {
                    keywordValues[assignment.PropertyName] = assignment.Value;
                }

                continue;
            }

            if (TryParseInsetShadowUtility(token, out var insetShadowUtility))
            {
                insetShadow = insetShadowUtility;
                hasInsetShadow = true;
                continue;
            }

            if (TryParseBoxShadowUtility(token, out var boxShadowUtility))
            {
                boxShadow = boxShadowUtility.Shadows;
                hasBoxShadow = true;
                continue;
            }

            if (TryParseTextAlignUtility(token, out var textAlignUtility))
            {
                textAlignment = textAlignUtility.Alignment;
                hasTextAlignment = true;
                continue;
            }

            // invisible hides the element but keeps its place in layout (CSS visibility: hidden); visible undoes it.
            if (token is "invisible" or "visible")
            {
                invisible = token == "invisible";
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

                // Caret and selection colors are plain brush properties on text input controls (TextBox and
                // SelectableTextBlock), applied through the keyword mechanism so removed classes are cleared.
                case BrushTarget.CaretBrush:
                    keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
                    keywordValues["CaretBrush"] = brushUtility.Brush;
                    break;

                case BrushTarget.PlaceholderForeground:
                    keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
                    keywordValues["PlaceholderForeground"] = brushUtility.Brush;
                    break;

                case BrushTarget.Fill:
                    keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
                    keywordValues["Fill"] = brushUtility.Brush;
                    break;

                case BrushTarget.Stroke:
                    keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
                    keywordValues["Stroke"] = brushUtility.Brush;
                    break;

                case BrushTarget.DecorationBrush:
                    decorationBrush = brushUtility.Brush;
                    break;

                case BrushTarget.SelectionBrush:
                    keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
                    keywordValues["SelectionBrush"] = brushUtility.Brush;
                    break;
            }
        }

        // A named text size supplies the theme line height unless a leading-* or text-<size>/<n> class sets one,
        // in any order (Tailwind reads it through --tw-leading, so leading-* always wins).
        // Only where a LineHeight exists, so text-lg on a control without one warns about FontSize alone.
        if (!hasLineHeight && defaultLineHeight is { } themeLineHeight &&
            AvaloniaPropertyRegistry.Instance.FindRegistered(element, "LineHeight") is not null)
        {
            lineHeight = new TextMetricUtility(themeLineHeight, false);
            hasLineHeight = true;
        }

        // invisible: fully transparent and ignored by the pointer, but still measured and arranged. It wins over
        // opacity-* and pointer-events-* in the same list, and over their state variants, as CSS visibility does.
        if (invisible == true)
        {
            opacity = 0;
            hasOpacity = true;
            Array.Clear(opacityVariants);
            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
            keywordValues["IsHitTestVisible"] = false;
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

        if (transitionState is { HasTransition: true } transition)
        {
            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
            keywordValues["Transitions"] = BuildTransitions(element, transition)!;
        }

        // A ring (width plus color) is combined with any shadow-* utility into one BoxShadow value.
        if (ringState is { Width: not null } ring)
        {
            boxShadow = ApplyRing(ring, ResolveCurrentColor(element, hasForeground ? foreground : null), hasBoxShadow, boxShadow);
            hasBoxShadow = true;
        }

        // Inset shadows are appended after the outer shadows and ring so both can be active together.
        if (hasInsetShadow && insetShadow.Count > 0)
        {
            var layers = new List<BoxShadow>();

            if (hasBoxShadow)
            {
                foreach (var shadow in boxShadow)
                {
                    layers.Add(shadow);
                }
            }

            foreach (var shadow in insetShadow)
            {
                layers.Add(shadow);
            }

            boxShadow = new BoxShadows(layers[0], layers.Skip(1).ToArray());
            hasBoxShadow = true;
        }

        // A gradient replaces the Background color, because Avalonia has one Background brush.
        if (gradientState is { IsUsable: true } gradient)
        {
            background = BuildGradientBrush(gradient);
            hasBackground = true;
        }

        // A background image replaces the Background as well and wins over a gradient.
        if (backgroundImageState is { IsUsable: true } backgroundImage && TryBuildImageBrush(element, backgroundImage) is { } imageBrush)
        {
            background = imageBrush;
            hasBackground = true;
        }

        // w-full and h-full leave the size to layout and stretch the element inside its parent.
        if (hasWidth && widthFill)
        {
            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
            keywordValues.TryAdd("HorizontalAlignment", HorizontalAlignment.Stretch);
        }

        if (hasHeight && heightFill)
        {
            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
            keywordValues.TryAdd("VerticalAlignment", VerticalAlignment.Stretch);
        }

        if (layoutState is { HasValues: true } layout)
        {
            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
            AddLayoutValues(element, layout, keywordValues);
        }

        if (transformState is { } transform)
        {
            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);

            if (transform.HasTransform)
            {
                keywordValues["RenderTransform"] = BuildTransform(transform);
            }

            if (transform.Origin is { } origin)
            {
                keywordValues["RenderTransformOrigin"] = origin;
            }
        }

        if (hasNumericVariants && numericFeatures.Count > 0)
        {
            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
            keywordValues["FontFeatures"] = FontFeatureCollection.Parse(string.Join(", ", numericFeatures));
        }

        if (hasTextDecoration)
        {
            var decorations = new TextDecorationCollection();

            foreach (var location in textDecorationLocations)
            {
                var decoration = new TextDecoration { Location = location };

                if (decorationThickness is { } thickness)
                {
                    decoration.StrokeThicknessUnit = TextDecorationUnit.Pixel;
                    decoration.StrokeThickness = thickness;
                }

                if (decorationOffset is { } offset)
                {
                    decoration.StrokeOffsetUnit = TextDecorationUnit.Pixel;
                    decoration.StrokeOffset = offset;
                }

                if (decorationBrush is not null)
                {
                    decoration.Stroke = decorationBrush;
                }

                // Dash lengths are multiples of the line thickness. Dotted uses round caps so the dots are round.
                if (decorationStyle == "dotted")
                {
                    decoration.StrokeDashArray = new AvaloniaList<double> { 0, 2 };
                    decoration.StrokeLineCap = PenLineCap.Round;
                }
                else if (decorationStyle == "dashed")
                {
                    decoration.StrokeDashArray = new AvaloniaList<double> { 4, 3 };
                }

                decorations.Add(decoration);
            }

            keywordValues ??= new Dictionary<string, object>(StringComparer.Ordinal);
            keywordValues["TextDecorations"] = decorations;
        }

        var backgroundDirect = hasBackground && !hasBackgroundVariant;
        var foregroundDirect = hasForeground && !hasForegroundVariant;
        var borderBrushDirect = hasBorderBrush && !hasBorderBrushVariant;
        var opacityDirect = hasOpacity && !hasOpacityVariant;

        // Relative metrics (em / line-height multipliers) resolve against the class-list font size,
        // falling back to the element's current FontSize.
        var effectiveFontSize = hasFontSize ? fontSize : GetCurrentFontSize(element);

        Span<PendingUtility> pendingUtilities =
        [
            new(MarginMask, hasMargin, () => TrySetThickness(element, "Margin", margin), () => ClearThickness(element, "Margin")),
            new(PaddingMask, hasPadding, () => TrySetThickness(element, "Padding", padding), () => ClearThickness(element, "Padding")),
            new(BorderWidthMask, hasBorderWidth, () => TrySetThickness(element, "BorderThickness", borderWidth), () => ClearThickness(element, "BorderThickness")),
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
            new(BoxShadowMask, hasBoxShadow, () => TrySetBoxShadows(element, "BoxShadow", boxShadow), () => ClearBoxShadows(element, "BoxShadow")),
            new(TextAlignmentMask, hasTextAlignment, () => TrySetTextAlignment(element, "TextAlignment", textAlignment), () => ClearTextAlignment(element, "TextAlignment")),
            new(LetterSpacingMask, hasLetterSpacing, () => TrySetDouble(element, "LetterSpacing", letterSpacing.Resolve(effectiveFontSize)), () => ClearDouble(element, "LetterSpacing")),
            new(LineHeightMask, hasLineHeight, () => TrySetDouble(element, "LineHeight", lineHeight.Resolve(effectiveFontSize)), () => ClearDouble(element, "LineHeight")),
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

        ApplyKeywordUtilities(element, keywordValues);
        SyncAnimation(element, animationName, TimingFrom(transitionState));
        SyncScrollSnap(element);

        ApplyVariantStyles(
            element,
            new BrushCategoryState(hasBackground && hasBackgroundVariant, background, backgroundVariants),
            new BrushCategoryState(hasForeground && hasForegroundVariant, foreground, foregroundVariants),
            new BrushCategoryState(hasBorderBrush && hasBorderBrushVariant, borderBrush, borderBrushVariants),
            new OpacityCategoryState(hasOpacity && hasOpacityVariant, opacity, opacityVariants));
    }

    private static double GetCurrentFontSize(AvaloniaObject element)
    {
        var property = FindDoubleProperty(element.GetType(), "FontSize");
        return property is not null && element.GetValue(property) is double size ? size : 12;
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
