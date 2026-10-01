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
            // CSS padding cannot be negative, so reject arbitrary negatives for padding (margin may be negative).
            Predicate<double>? isValid = descriptor.Target == SpacingTarget.Padding ? static p => p >= 0 : null;

            if (TryParseScaleOrArbitraryPixels(scaleToken, SpacingScale.TryGetPixels, isValid, out var pixels))
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

            var isSize = descriptor.Target is SizingTarget.Width or SizingTarget.Height or SizingTarget.Size;
            var isMax = descriptor.Target is SizingTarget.MaxWidth or SizingTarget.MaxHeight;

            // auto clears an explicit size, full stretches to the parent, none lifts a maximum.
            if (isSize && scaleToken == "auto")
            {
                utility = new SizingUtility(descriptor.Target, double.NaN);
                return true;
            }

            if (descriptor.Target is SizingTarget.Width or SizingTarget.Height or SizingTarget.Size && scaleToken == "full")
            {
                utility = new SizingUtility(descriptor.Target, double.NaN, Fill: true);
                return true;
            }

            // max-w-full / max-h-full: an Avalonia child is already held to its parent's slot, so "no larger than the
            // parent" is the same as no explicit maximum.
            if (isMax && scaleToken == "full")
            {
                utility = new SizingUtility(descriptor.Target, double.PositiveInfinity);
                return true;
            }

            if (isMax && scaleToken == "none")
            {
                utility = new SizingUtility(descriptor.Target, double.PositiveInfinity);
                return true;
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

        // Very long digit strings parse to Infinity instead of failing, and Avalonia rejects a non-finite Thickness.
        return double.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out pixels) && double.IsFinite(pixels);
    }

    private static bool TryParseCornerRadiusUtility(string token, out CornerRadiusUtility utility)
    {
        utility = default;

        if (token.Contains(':') || token.Contains('('))
        {
            return false;
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

        // text-<size>/<line-height> sets both (text-sm/6, text-lg/[28px], text-base/[1.5]). A size that is not a
        // font size (text-sky-500/50) is left for the color parser.
        var slash = sizeToken.LastIndexOf('/');

        if (slash > 0 &&
            TryParseScaleOrArbitraryPixels(sizeToken[..slash], FontSizeScale.TryGetPixels, static p => p >= 0, out var sizedPixels) &&
            TryParseLineHeightValue(sizeToken[(slash + 1)..], out var lineHeight))
        {
            utility = new FontSizeUtility(sizedPixels, lineHeight);
            return true;
        }

        // Try a scale-table token first (e.g. text-lg), then an arbitrary value (e.g. text-[14px]).
        if (TryParseScaleOrArbitraryPixels(sizeToken, FontSizeScale.TryGetPixels, static p => p >= 0, out var pixels))
        {
            // A named size brings its theme line height (text-sm is 14px on a 20px line); arbitrary sizes do not.
            utility = FontSizeScale.TryGetLineHeight(sizeToken, out var themeLineHeight)
                ? new FontSizeUtility(pixels, DefaultLineHeight: themeLineHeight)
                : new FontSizeUtility(pixels);
            return true;
        }

        return false;
    }

    // underline, overline and line-through combine; no-underline (reported as a null location) clears them all.
    private static bool TryParseTextDecorationUtility(string token, out TextDecorationLocation? location)
    {
        location = null;

        switch (token)
        {
            case "underline":
                location = TextDecorationLocation.Underline;
                return true;
            case "overline":
                location = TextDecorationLocation.Overline;
                return true;
            case "line-through":
                location = TextDecorationLocation.Strikethrough;
                return true;
            case "no-underline":
                return true;
            default:
                return false;
        }
    }

    // decoration-<n> / decoration-[<px>] set the line thickness, underline-offset-<n> the distance from the text;
    // decoration-auto and underline-offset-auto return to the font's recommended metrics (reported as NaN).
    private static bool TryParseDecorationMetricUtility(string token, out bool isThickness, out double? value)
    {
        isThickness = false;
        value = null;
        string rest;

        if (token.StartsWith("decoration-", StringComparison.Ordinal))
        {
            isThickness = true;
            rest = token["decoration-".Length..];
        }
        else if (token.StartsWith("underline-offset-", StringComparison.Ordinal))
        {
            rest = token["underline-offset-".Length..];
        }
        else
        {
            return false;
        }

        if (rest == "auto" || (isThickness && rest == "from-font"))
        {
            value = null;
            return true;
        }

        // decoration-<number> and underline-offset-<number> are that many pixels.
        if (int.TryParse(rest, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var metric))
        {
            value = metric;
            return true;
        }

        if (rest.StartsWith('[') && TryParseArbitraryDouble(rest, out var arbitrary) && arbitrary >= 0)
        {
            value = arbitrary;
            return true;
        }

        return false;
    }

    // font-variant-numeric utilities combine into one OpenType feature list; normal-nums resets it.
    private static bool TryParseNumericVariantUtility(string token, out string? feature)
    {
        // font-features-[smcp,liga=0] is the arbitrary font-feature-settings form; underscores stand for spaces.
        if (token.StartsWith("font-features-[", StringComparison.Ordinal) && token.EndsWith(']') && token.Length > "font-features-[]".Length)
        {
            feature = token["font-features-[".Length..^1].Replace('_', ' ').Replace("'", string.Empty).Replace("\"", string.Empty);
            return true;
        }

        feature = token switch
        {
            "normal-nums" => null,
            "ordinal" => "ordn",
            "slashed-zero" => "zero",
            "lining-nums" => "lnum",
            "oldstyle-nums" => "onum",
            "proportional-nums" => "pnum",
            "tabular-nums" => "tnum",
            "diagonal-fractions" => "frac",
            "stacked-fractions" => "afrc",
            _ => null,
        };

        return feature is not null || token == "normal-nums";
    }

    private static bool TryParseLetterSpacingUtility(string token, out TextMetricUtility utility)
    {
        utility = default;

        if (!token.StartsWith("tracking-", StringComparison.Ordinal) || token.Contains(':'))
        {
            return false;
        }

        var value = token["tracking-".Length..];
        double? em = value switch
        {
            "tighter" => -0.05,
            "tight" => -0.025,
            "normal" => 0,
            "wide" => 0.025,
            "wider" => 0.05,
            "widest" => 0.1,
            _ => null,
        };

        if (em is { } known)
        {
            utility = new TextMetricUtility(known, true);
            return true;
        }

        if (TryParseArbitraryDouble(value, out var pixels))
        {
            utility = new TextMetricUtility(pixels, false);
            return true;
        }

        return false;
    }

    private static bool TryParseLineHeightUtility(string token, out TextMetricUtility utility)
    {
        utility = default;

        if (!token.StartsWith("leading-", StringComparison.Ordinal) || token.Contains(':'))
        {
            return false;
        }

        var value = token["leading-".Length..];
        double? multiplier = value switch
        {
            "none" => 1,
            _ => null,
        };

        if (multiplier is { } known)
        {
            utility = new TextMetricUtility(known, true);
            return true;
        }

        return TryParseLineHeightValue(value, out utility);
    }

    // A line height value: a spacing-scale step (6 = 24px), an arbitrary length ([18px], [2rem]) or, as in CSS,
    // a unitless or em value that multiplies the font size ([1.5], [1.2em]).
    private static bool TryParseLineHeightValue(string value, out TextMetricUtility utility)
    {
        utility = default;

        if (value.Length > 2 && value[0] == '[' && value[^1] == ']')
        {
            var inner = value[1..^1].Trim();
            var isEm = inner.EndsWith("em", StringComparison.Ordinal) && !inner.EndsWith("rem", StringComparison.Ordinal);
            var number = isEm ? inner[..^2] : inner;

            if ((isEm || (number.Length > 0 && (char.IsDigit(number[^1]) || number[^1] == '.'))) &&
                number.Length > 0 && number[^1] != '.' &&
                double.TryParse(number, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var multiplier) &&
                double.IsFinite(multiplier))
            {
                utility = new TextMetricUtility(multiplier, true);
                return true;
            }
        }

        if (TryParseScaleOrArbitraryPixels(value, SpacingScale.TryGetPixels, static p => p >= 0, out var pixels))
        {
            utility = new TextMetricUtility(pixels, false);
            return true;
        }

        return false;
    }

    private static bool TryParseTextAlignUtility(string token, out TextAlignUtility utility)
    {
        TextAlignment? alignment = token switch
        {
            "text-left" => TextAlignment.Left,
            "text-center" => TextAlignment.Center,
            "text-right" => TextAlignment.Right,
            "text-justify" => TextAlignment.Justify,
            "text-start" => TextAlignment.Start,
            "text-end" => TextAlignment.End,
            _ => null,
        };

        utility = alignment is { } value ? new TextAlignUtility(value) : default;
        return alignment is not null;
    }

    private static bool TryParseInsetShadowUtility(string token, out BoxShadows shadows)
    {
        shadows = default;

        return token.StartsWith("inset-shadow-", StringComparison.Ordinal) &&
            BoxShadowScale.TryGetInsetShadows(token["inset-shadow-".Length..], out shadows);
    }

    private static bool TryParseBoxShadowUtility(string token, out BoxShadowUtility utility)
    {
        utility = default;

        if (!token.StartsWith("shadow-", StringComparison.Ordinal) ||
            !BoxShadowScale.TryGetShadows(token["shadow-".Length..], out var shadows))
        {
            return false;
        }

        utility = new BoxShadowUtility(shadows);
        return true;
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

        // CSS numbers need a digit after the decimal point: "1." is malformed even though double.TryParse accepts it.
        if (numericPart[^1] == '.')
        {
            return false;
        }

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
