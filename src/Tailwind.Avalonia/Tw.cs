using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Data;
using Avalonia.Logging;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public class Tw : AvaloniaObject
{
    private const int MarginMask = 1;
    private const int PaddingMask = 2;
    private const int BackgroundMask = 4;
    private const int ForegroundMask = 8;
    private const int BorderBrushMask = 16;
    private const int WidthMask = 32;
    private const int MinWidthMask = 64;
    private const int MaxWidthMask = 128;
    private const int HeightMask = 256;
    private const int MinHeightMask = 512;
    private const int MaxHeightMask = 1024;
    private const int FontSizeMask = 2048;
    private const string LogArea = "Tailwind.Avalonia";

    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> ThicknessPropertyCache = new();
    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> BrushPropertyCache = new();
    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> DoublePropertyCache = new();

    public static readonly AttachedProperty<string?> ClassProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, string?>(
            "Class",
            default,
            false,
            BindingMode.OneWay);

    private static readonly AttachedProperty<int> AppliedMaskProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, int>("AppliedMask");

    private static readonly AttachedProperty<bool> AttachHandlerRegisteredProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, bool>("AttachHandlerRegistered");

    static Tw()
    {
        ClassProperty.Changed.AddClassHandler<AvaloniaObject>(HandleClassChanged);
        Visual.FlowDirectionProperty.Changed.AddClassHandler<Visual>(HandleFlowDirectionChanged);
    }

    public static void SetClass(AvaloniaObject element, string? value) => element.SetValue(ClassProperty, value);

    public static string? GetClass(AvaloniaObject element) => element.GetValue(ClassProperty);

    private static void HandleClassChanged(AvaloniaObject element, AvaloniaPropertyChangedEventArgs args)
    {
        if (element is Visual visual)
        {
            EnsureAttachHandlerRegistered(visual);
        }

        ApplyUtilities(element, args.GetNewValue<string?>());
    }

    private static void HandleFlowDirectionChanged(Visual visual, AvaloniaPropertyChangedEventArgs args)
    {
        var classList = GetClass(visual);

        if (!string.IsNullOrWhiteSpace(classList) && ContainsLogicalUtilities(classList))
        {
            ApplyUtilities(visual, classList);
        }
    }

    private static void HandleAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
    {
        if (sender is not Visual visual)
        {
            return;
        }

        var classList = GetClass(visual);

        if (!string.IsNullOrWhiteSpace(classList) && ContainsLogicalUtilities(classList))
        {
            ApplyUtilities(visual, classList);
        }
    }

    private static void EnsureAttachHandlerRegistered(Visual visual)
    {
        if (visual.GetValue(AttachHandlerRegisteredProperty))
        {
            return;
        }

        visual.AttachedToVisualTree += HandleAttachedToVisualTree;
        visual.SetValue(AttachHandlerRegisteredProperty, true);
    }

    private static void ApplyUtilities(AvaloniaObject element, string? classList)
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

        if (!string.IsNullOrWhiteSpace(classList))
        {
            foreach (var token in classList.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            {
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

                if (TryParseFontSizeUtility(token, out var fontSizeUtility))
                {
                    fontSize = fontSizeUtility.Pixels;
                    hasFontSize = true;
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
        }

        if (hasMargin && TrySetThickness(element, "Margin", margin))
        {
            newMask |= MarginMask;
        }
        else if ((previousMask & MarginMask) != 0)
        {
            ClearThickness(element, "Margin");
        }

        if (hasPadding && TrySetThickness(element, "Padding", padding))
        {
            newMask |= PaddingMask;
        }
        else if ((previousMask & PaddingMask) != 0)
        {
            ClearThickness(element, "Padding");
        }

        if (hasBackground && TrySetBrush(element, "Background", background))
        {
            newMask |= BackgroundMask;
        }
        else if ((previousMask & BackgroundMask) != 0)
        {
            ClearBrush(element, "Background");
        }

        if (hasForeground && TrySetBrush(element, "Foreground", foreground))
        {
            newMask |= ForegroundMask;
        }
        else if ((previousMask & ForegroundMask) != 0)
        {
            ClearBrush(element, "Foreground");
        }

        if (hasBorderBrush && TrySetBrush(element, "BorderBrush", borderBrush))
        {
            newMask |= BorderBrushMask;
        }
        else if ((previousMask & BorderBrushMask) != 0)
        {
            ClearBrush(element, "BorderBrush");
        }

        if (hasWidth && TrySetDouble(element, "Width", width))
        {
            newMask |= WidthMask;
        }
        else if ((previousMask & WidthMask) != 0)
        {
            ClearDouble(element, "Width");
        }

        if (hasMinWidth && TrySetDouble(element, "MinWidth", minWidth))
        {
            newMask |= MinWidthMask;
        }
        else if ((previousMask & MinWidthMask) != 0)
        {
            ClearDouble(element, "MinWidth");
        }

        if (hasMaxWidth && TrySetDouble(element, "MaxWidth", maxWidth))
        {
            newMask |= MaxWidthMask;
        }
        else if ((previousMask & MaxWidthMask) != 0)
        {
            ClearDouble(element, "MaxWidth");
        }

        if (hasHeight && TrySetDouble(element, "Height", height))
        {
            newMask |= HeightMask;
        }
        else if ((previousMask & HeightMask) != 0)
        {
            ClearDouble(element, "Height");
        }

        if (hasMinHeight && TrySetDouble(element, "MinHeight", minHeight))
        {
            newMask |= MinHeightMask;
        }
        else if ((previousMask & MinHeightMask) != 0)
        {
            ClearDouble(element, "MinHeight");
        }

        if (hasMaxHeight && TrySetDouble(element, "MaxHeight", maxHeight))
        {
            newMask |= MaxHeightMask;
        }
        else if ((previousMask & MaxHeightMask) != 0)
        {
            ClearDouble(element, "MaxHeight");
        }

        if (hasFontSize && TrySetDouble(element, "FontSize", fontSize))
        {
            newMask |= FontSizeMask;
        }
        else if ((previousMask & FontSizeMask) != 0)
        {
            ClearDouble(element, "FontSize");
        }

        element.SetValue(AppliedMaskProperty, newMask);
    }

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

    private static bool TryParseSpacingUtility(string token, out SpacingUtility utility)
    {
        utility = default;

        if (token.Contains(':') ||
            token.Contains('('))
        {
            return false;
        }

        var negative = token.StartsWith("-", StringComparison.Ordinal);
        var candidate = negative ? token[1..] : token;

        foreach (var descriptor in UtilityDescriptors.All)
        {
            if (!candidate.StartsWith(descriptor.Prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var scaleToken = candidate[descriptor.Prefix.Length..];

            if (scaleToken.Length == 0)
            {
                return false;
            }

            if (negative && descriptor.Target != SpacingTarget.Margin)
            {
                return false;
            }

            // Try to parse as a scale token first
            if (SpacingScale.TryGetPixels(scaleToken, out var pixels))
            {
                utility = new SpacingUtility(descriptor.Target, descriptor.Edge, negative ? -pixels : pixels);
                return true;
            }

            // Try to parse as an arbitrary value (e.g., p-[1.5rem], m-[20px])
            if (TryParseArbitraryDouble(scaleToken, out var arbitraryPixels))
            {
                utility = new SpacingUtility(descriptor.Target, descriptor.Edge, negative ? -arbitraryPixels : arbitraryPixels);
                return true;
            }
        }

        return false;
    }

    private static bool TryParseBrushUtility(string token, out BrushUtility utility)
    {
        utility = default;

        if (token.StartsWith("-", StringComparison.Ordinal) ||
            token.Contains(':') ||
            token.Contains('('))
        {
            return false;
        }

        foreach (var descriptor in BrushUtilityDescriptors.All)
        {
            if (!token.StartsWith(descriptor.Prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var colorToken = token[descriptor.Prefix.Length..];

            if (!TryResolveUtilityColor(colorToken, out var color))
            {
                return false;
            }

            utility = new BrushUtility(descriptor.Target, new SolidColorBrush(color));
            return true;
        }

        return false;
    }

    private static bool TryParseSizingUtility(string token, out SizingUtility utility)
    {
        utility = default;

        if (token.StartsWith("-", StringComparison.Ordinal) ||
            token.Contains(':') ||
            token.Contains('('))
        {
            return false;
        }

        foreach (var descriptor in SizingUtilityDescriptors.All)
        {
            if (!token.StartsWith(descriptor.Prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var scaleToken = token[descriptor.Prefix.Length..];

            if (scaleToken.Length == 0)
            {
                return false;
            }

            // Try to parse as a scale token first
            if (SpacingScale.TryGetPixels(scaleToken, out var pixels))
            {
                utility = new SizingUtility(descriptor.Target, pixels);
                return true;
            }

            // Try to parse as an arbitrary value (e.g., w-[100px], h-[50%])
            if (TryParseArbitraryDouble(scaleToken, out var arbitraryPixels))
            {
                utility = new SizingUtility(descriptor.Target, arbitraryPixels);
                return true;
            }
        }

        return false;
    }

    private static bool TryParseFontSizeUtility(string token, out FontSizeUtility utility)
    {
        utility = default;

        if (!token.StartsWith("text-", StringComparison.Ordinal) ||
            token.Contains(':') ||
            token.Contains('('))
        {
            return false;
        }

        var sizeToken = token["text-".Length..];

        if (sizeToken.Length == 0)
        {
            return false;
        }

        if (FontSizeScale.TryGetPixels(sizeToken, out var pixels))
        {
            utility = new FontSizeUtility(pixels);
            return true;
        }

        if (TryParseArbitraryFontSize(sizeToken, out var arbitraryPixels))
        {
            utility = new FontSizeUtility(arbitraryPixels);
            return true;
        }

        return false;
    }

    private static bool TryResolveUtilityColor(string token, out Color color)
    {
        color = default;

        var separatorIndex = token.IndexOf('/');
        var colorToken = separatorIndex >= 0 ? token[..separatorIndex] : token;

        if (colorToken.Length == 0)
        {
            return false;
        }

        // Try to resolve from Tailwind palette first
        if (!TailwindColorPalette.TryGetColor(colorToken, out color))
        {
            // Try to parse as an arbitrary color (e.g., bg-[#ff0000], text-[#123456])
            if (!TryParseArbitraryColor(colorToken, out color))
            {
                return false;
            }
        }

        if (separatorIndex < 0)
        {
            return true;
        }

        var opacityToken = token[(separatorIndex + 1)..];

        if (!TryParseOpacity(opacityToken, out var opacity))
        {
            return false;
        }

        color = ApplyOpacity(color, opacity);
        return true;
    }

    private static bool TryParseOpacity(string token, out double opacity)
    {
        opacity = default;

        if (token.Length == 0 ||
            !double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var percent) ||
            percent < 0 ||
            percent > 100)
        {
            return false;
        }

        opacity = percent / 100d;
        return true;
    }

    private static Color ApplyOpacity(Color color, double opacity)
    {
        var alpha = (byte)Math.Clamp((int)Math.Round(color.A * opacity), 0, byte.MaxValue);
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }

    private static bool TryParseArbitraryDouble(string token, out double value)
    {
        value = default;

        // Must be enclosed in square brackets
        if (!token.StartsWith('[') || !token.EndsWith(']'))
        {
            return false;
        }

        var contentWithUnit = token[1..^1].Trim();

        if (contentWithUnit.Length == 0)
        {
            return false;
        }

        // Extract numeric and unit parts
        var index = 0;
        var hasDecimal = false;

        if (contentWithUnit[index] == '-')
        {
            index++;
        }

        var digitStart = index;

        while (index < contentWithUnit.Length)
        {
            var ch = contentWithUnit[index];

            if (char.IsDigit(ch))
            {
                index++;
            }
            else if (ch == '.' && !hasDecimal)
            {
                hasDecimal = true;
                index++;
            }
            else
            {
                break;
            }
        }

        if (index == digitStart)
        {
            return false;
        }

        var numericPart = contentWithUnit[..index];
        var unitPart = contentWithUnit[index..].Trim();

        if (!double.TryParse(numericPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericValue))
        {
            return false;
        }

        // Convert based on unit
        value = unitPart switch
        {
            "px" => numericValue,
            "rem" => numericValue * 16.0, // 1rem = 16px
            "em" => numericValue * 16.0,  // Treat em as rem for simplicity
            "%" => default,                 // Percentage values not supported for sizing/spacing
            "" => numericValue,             // Unitless (treated as px)
            _ => default,
        };

        return unitPart switch
        {
            "px" or "rem" or "em" or "" => true,
            _ => false,
        };
    }

    private static bool TryParseArbitraryFontSize(string token, out double value)
    {
        value = default;

        if (token.StartsWith("[%", StringComparison.Ordinal) ||
            token.EndsWith("%]", StringComparison.Ordinal))
        {
            return false;
        }

        return TryParseArbitraryDouble(token, out value) && value >= 0;
    }

    private static bool TryParseArbitraryColor(string token, out Color color)
    {
        color = default;

        // Must be enclosed in square brackets
        if (!token.StartsWith('[') || !token.EndsWith(']'))
        {
            return false;
        }

        var colorValue = token[1..^1].Trim();

        if (colorValue.Length == 0)
        {
            return false;
        }

        // Try to parse hex color (#rrggbb or #rrggbbaa)
        if (colorValue.StartsWith('#'))
        {
            return TryParseHexColor(colorValue, out color);
        }

        // Could be extended to support rgb(), hsl(), etc. in the future
        return false;
    }

    private static bool TryParseHexColor(string hexColor, out Color color)
    {
        color = default;

        // Remove the # prefix
        if (!hexColor.StartsWith('#'))
        {
            return false;
        }

        var hex = hexColor[1..];

        // Expand shorthand hex notation (#rgb → #rrggbb, #rgba → #rrggbbaa)
        if (hex.Length == 3 || hex.Length == 4)
        {
            var expanded = new char[hex.Length * 2];
            for (var i = 0; i < hex.Length; i++)
            {
                expanded[i * 2] = hex[i];
                expanded[i * 2 + 1] = hex[i];
            }
            hex = new string(expanded);
        }

        // Support #rrggbb (6 chars) and #rrggbbaa (8 chars)
        if (hex.Length != 6 && hex.Length != 8)
        {
            return false;
        }

        try
        {
            var r = byte.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber);
            var a = hex.Length == 8 ? byte.Parse(hex[6..8], System.Globalization.NumberStyles.HexNumber) : (byte)255;

            color = Color.FromArgb(a, r, g, b);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool ContainsLogicalUtilities(string classList)
    {
        foreach (var token in classList.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = token.StartsWith("-", StringComparison.Ordinal) ? token[1..] : token;

            if (candidate.StartsWith("ps-", StringComparison.Ordinal) ||
                candidate.StartsWith("pe-", StringComparison.Ordinal) ||
                candidate.StartsWith("ms-", StringComparison.Ordinal) ||
                candidate.StartsWith("me-", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TrySetThickness(AvaloniaObject element, string propertyName, Thickness value)
    {
        var property = FindThicknessProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' Thickness property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearThickness(AvaloniaObject element, string propertyName)
    {
        var property = FindThicknessProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

    private static bool TrySetBrush(AvaloniaObject element, string propertyName, IBrush? value)
    {
        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' brush property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearBrush(AvaloniaObject element, string propertyName)
    {
        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

    private static bool TrySetDouble(AvaloniaObject element, string propertyName, double value)
    {
        var property = FindDoubleProperty(element.GetType(), propertyName);

        if (property is null)
        {
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearDouble(AvaloniaObject element, string propertyName)
    {
        var property = FindDoubleProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindThicknessProperty(Type type, string propertyName)
    {
        return ThicknessPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
        {
            var property = FindPropertyField(key);
            return property?.PropertyType == typeof(Thickness) ? property : null;
        });
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindBrushProperty(Type type, string propertyName)
    {
        return BrushPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
        {
            var property = FindPropertyField(key);
            return property is not null && typeof(IBrush).IsAssignableFrom(property.PropertyType) ? property : null;
        });
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindDoubleProperty(Type type, string propertyName)
    {
        return DoublePropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
        {
            var property = FindPropertyField(key);
            return property?.PropertyType == typeof(double) ? property : null;
        });
    }

    private static AvaloniaProperty? FindPropertyField(PropertyLookupKey key)
    {
        var field = key.Type.GetField(
            $"{key.PropertyName}Property",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return field?.GetValue(null) as AvaloniaProperty;
    }

    private readonly record struct SpacingUtility(SpacingTarget Target, SpacingEdge Edge, double Pixels);
    private readonly record struct BrushUtility(BrushTarget Target, IBrush Brush);
    private readonly record struct SizingUtility(SizingTarget Target, double Pixels);
    private readonly record struct FontSizeUtility(double Pixels);

    private readonly record struct PropertyLookupKey
    {
        public PropertyLookupKey(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type type,
            string propertyName)
        {
            Type = type;
            PropertyName = propertyName;
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
        public Type Type { get; }

        public string PropertyName { get; }
    }

    private readonly record struct UtilityDescriptor(string Prefix, SpacingTarget Target, SpacingEdge Edge);
    private readonly record struct BrushUtilityDescriptor(string Prefix, BrushTarget Target);
    private readonly record struct SizingUtilityDescriptor(string Prefix, SizingTarget Target);

    private enum SpacingTarget
    {
        Margin,
        Padding,
    }

    private enum BrushTarget
    {
        Background,
        Foreground,
        BorderBrush,
    }

    private enum SizingTarget
    {
        Width,
        MinWidth,
        MaxWidth,
        Height,
        MinHeight,
        MaxHeight,
    }

    private enum SpacingEdge
    {
        All,
        X,
        Y,
        Top,
        Right,
        Bottom,
        Left,
        Start,
        End,
        BlockStart,
        BlockEnd,
    }

    private static class UtilityDescriptors
    {
        public static readonly UtilityDescriptor[] All =
        {
            new("mbs-", SpacingTarget.Margin, SpacingEdge.BlockStart),
            new("mbe-", SpacingTarget.Margin, SpacingEdge.BlockEnd),
            new("pbs-", SpacingTarget.Padding, SpacingEdge.BlockStart),
            new("pbe-", SpacingTarget.Padding, SpacingEdge.BlockEnd),
            new("mx-", SpacingTarget.Margin, SpacingEdge.X),
            new("my-", SpacingTarget.Margin, SpacingEdge.Y),
            new("msv-", SpacingTarget.Margin, SpacingEdge.Left),
            new("mev-", SpacingTarget.Margin, SpacingEdge.Right),
            new("ms-", SpacingTarget.Margin, SpacingEdge.Start),
            new("me-", SpacingTarget.Margin, SpacingEdge.End),
            new("mt-", SpacingTarget.Margin, SpacingEdge.Top),
            new("mr-", SpacingTarget.Margin, SpacingEdge.Right),
            new("mb-", SpacingTarget.Margin, SpacingEdge.Bottom),
            new("ml-", SpacingTarget.Margin, SpacingEdge.Left),
            new("px-", SpacingTarget.Padding, SpacingEdge.X),
            new("py-", SpacingTarget.Padding, SpacingEdge.Y),
            new("psv-", SpacingTarget.Padding, SpacingEdge.Left),
            new("pev-", SpacingTarget.Padding, SpacingEdge.Right),
            new("ps-", SpacingTarget.Padding, SpacingEdge.Start),
            new("pe-", SpacingTarget.Padding, SpacingEdge.End),
            new("pt-", SpacingTarget.Padding, SpacingEdge.Top),
            new("pr-", SpacingTarget.Padding, SpacingEdge.Right),
            new("pb-", SpacingTarget.Padding, SpacingEdge.Bottom),
            new("pl-", SpacingTarget.Padding, SpacingEdge.Left),
            new("m-", SpacingTarget.Margin, SpacingEdge.All),
            new("p-", SpacingTarget.Padding, SpacingEdge.All),
        };
    }

    private static class BrushUtilityDescriptors
    {
        public static readonly BrushUtilityDescriptor[] All =
        {
            new("bg-", BrushTarget.Background),
            new("text-", BrushTarget.Foreground),
            new("border-", BrushTarget.BorderBrush),
        };
    }

    private static class SizingUtilityDescriptors
    {
        public static readonly SizingUtilityDescriptor[] All =
        {
            new("min-w-", SizingTarget.MinWidth),
            new("max-w-", SizingTarget.MaxWidth),
            new("min-h-", SizingTarget.MinHeight),
            new("max-h-", SizingTarget.MaxHeight),
            new("w-", SizingTarget.Width),
            new("h-", SizingTarget.Height),
        };
    }
}