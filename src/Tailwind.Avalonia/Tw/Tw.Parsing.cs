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
            token.Contains(':'))
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

            // Reject Tailwind's custom-property shorthand (e.g. bg-(--my-color)), which this
            // library deliberately does not support, but allow a '(' that is part of a CSS
            // color function inside a bracket arbitrary value (e.g. bg-[rgb(255,0,0)]). A
            // token only qualifies as a bracket arbitrary value when it starts with '[' right
            // after the utility prefix. Note: ApplyUtilities splits the class list on
            // whitespace, so only space-free function syntax can ever survive tokenization —
            // bg-[rgb(255,0,0)] works, bg-[rgb(255, 0, 0)] cannot; that tokenizer limitation is
            // out of scope here.
            if (colorToken.Contains('(') && !colorToken.StartsWith("[", StringComparison.Ordinal))
            {
                return false;
            }

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

    private static bool TryParseBorderWidthUtility(string token, out SpacingUtility utility)
    {
        utility = default;

        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        foreach (var descriptor in BorderWidthUtilityDescriptors.All)
        {
            if (token.Equals(descriptor.Prefix, StringComparison.Ordinal))
            {
                utility = new SpacingUtility(SpacingTarget.BorderWidth, descriptor.Edge, 1.0);
                return true;
            }

            var valuedPrefix = descriptor.Prefix + "-";

            if (!token.StartsWith(valuedPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            var scaleToken = token[valuedPrefix.Length..];

            if (scaleToken.Length == 0)
            {
                continue;
            }

            // Try a bare non-negative integer first (e.g. border-2 = 2px, unlike the spacing scale's
            // 4px-per-step multiplier), then an arbitrary value (e.g. border-[3px]).
            if (TryParseScaleOrArbitraryPixels(scaleToken, TryParseBareBorderWidthPixels, static p => p >= 0, out var pixels))
            {
                utility = new SpacingUtility(SpacingTarget.BorderWidth, descriptor.Edge, pixels);
                return true;
            }
        }

        return false;
    }

    private static bool TryParseBareBorderWidthPixels(string token, out double pixels)
    {
        pixels = default;

        if (token.Length == 0)
        {
            return false;
        }

        foreach (var ch in token)
        {
            if (!char.IsAsciiDigit(ch))
            {
                return false;
            }
        }

        pixels = double.Parse(token, NumberStyles.None, CultureInfo.InvariantCulture);
        return true;
    }

    private static bool TryParseCornerRadiusUtility(string token, out CornerRadiusUtility utility)
    {
        utility = default;

        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        if (token.Equals("rounded", StringComparison.Ordinal))
        {
            utility = new CornerRadiusUtility(CornerRadiusEdge.All, 4.0);
            return true;
        }

        foreach (var descriptor in CornerRadiusUtilityDescriptors.All)
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

            // Try a scale-table token first (e.g. rounded-lg), then an arbitrary value (e.g. rounded-[6px]).
            if (TryParseScaleOrArbitraryPixels(scaleToken, CornerRadiusScale.TryGetPixels, static p => p >= 0, out var pixels))
            {
                utility = new CornerRadiusUtility(descriptor.Edge, pixels);
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
        var unitPart = contentWithUnit[index..].Trim().ToLowerInvariant();

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
