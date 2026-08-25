using System.Globalization;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
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

            // Try a scale-table token first (e.g. p-4), then an arbitrary value (e.g. p-[1.5rem]).
            if (TryParseScaleOrArbitraryPixels(scaleToken, SpacingScale.TryGetPixels, isValid: null, out var pixels))
            {
                // Reject if negative prefix is combined with already-negative arbitrary value (e.g., -m-[-10px])
                // This prevents confusing double-negative behavior where two negatives cancel out
                if (negative && pixels < 0)
                {
                    return false;
                }

                utility = new SpacingUtility(descriptor.Target, descriptor.Edge, negative ? -pixels : pixels);
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

            // Try a scale-table token first (e.g. w-4), then an arbitrary value (e.g. w-[100px]).
            if (TryParseScaleOrArbitraryPixels(scaleToken, SpacingScale.TryGetPixels, static p => p >= 0, out var pixels))
            {
                utility = new SizingUtility(descriptor.Target, pixels);
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

        // Try a scale-table token first (e.g. text-lg), then an arbitrary value (e.g. text-[14px]).
        if (TryParseScaleOrArbitraryPixels(sizeToken, FontSizeScale.TryGetPixels, static p => p >= 0, out var pixels))
        {
            utility = new FontSizeUtility(pixels);
            return true;
        }

        return false;
    }

    private static bool TryParseOpacityUtility(string token, out double opacity)
    {
        opacity = default;

        if (!token.StartsWith("opacity-", StringComparison.Ordinal) ||
            token.Contains(':') ||
            token.Contains('('))
        {
            return false;
        }

        var valueToken = token["opacity-".Length..];

        return TryParseOpacity(valueToken, out opacity);
    }

    private delegate bool ScalePixelLookup(string token, out double pixels);

    private static bool TryParseScaleOrArbitraryPixels(string token, ScalePixelLookup scaleLookup, Predicate<double>? isValid, out double pixels) =>
        (scaleLookup(token, out pixels) || TryParseArbitraryDouble(token, out pixels)) && (isValid is null || isValid(pixels));

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

        // Convert based on unit; "px"/"" are unitless-or-pixels, "rem"/"em" scale to pixels.
        double? converted = unitPart switch
        {
            "px" or "" => numericValue,
            "rem" or "em" => numericValue * 16.0, // 1rem/1em = 16px
            _ => null, // e.g. "%" - percentage values not supported for sizing/spacing
        };

        if (converted is not { } convertedValue || !double.IsFinite(convertedValue))
        {
            return false;
        }

        value = convertedValue;
        return true;
    }
}
