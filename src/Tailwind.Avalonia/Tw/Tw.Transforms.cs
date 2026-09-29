using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // Transform utilities (translate, rotate, scale, skew, origin) accumulate into one state object per class list
    // and are applied as a single RenderTransform, so several transform classes on one element combine.
    private sealed class TransformState
    {
        public bool HasTransform { get; set; }

        public double TranslateX { get; set; }

        public double TranslateY { get; set; }

        public double ScaleX { get; set; } = 1;

        public double ScaleY { get; set; } = 1;

        public double RotateDegrees { get; set; }

        public double SkewXDegrees { get; set; }

        public double SkewYDegrees { get; set; }

        public RelativePoint? Origin { get; set; }
    }

    private static bool TryApplyTransformToken(string token, TransformState state)
    {
        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        if (token.StartsWith("origin-", StringComparison.Ordinal))
        {
            if (TryParseTransformOrigin(token["origin-".Length..], out var origin))
            {
                state.Origin = origin;
                return true;
            }

            return false;
        }

        var negative = token.StartsWith('-');
        var name = negative ? token[1..] : token;
        var sign = negative ? -1 : 1;

        if (TryStripPrefix(name, "translate-x-", out var value))
        {
            if (!TryParseTranslate(value, out var pixels)) { return false; }
            state.TranslateX = sign * pixels;
        }
        else if (TryStripPrefix(name, "translate-y-", out value))
        {
            if (!TryParseTranslate(value, out var pixels)) { return false; }
            state.TranslateY = sign * pixels;
        }
        else if (TryStripPrefix(name, "translate-", out value))
        {
            if (!TryParseTranslate(value, out var pixels)) { return false; }
            state.TranslateX = sign * pixels;
            state.TranslateY = sign * pixels;
        }
        else if (TryStripPrefix(name, "rotate-", out value))
        {
            if (!TryParseDegrees(value, out var degrees)) { return false; }
            state.RotateDegrees = sign * degrees;
        }
        else if (TryStripPrefix(name, "scale-x-", out value))
        {
            if (!TryParseScale(value, out var factor)) { return false; }
            state.ScaleX = sign * factor;
        }
        else if (TryStripPrefix(name, "scale-y-", out value))
        {
            if (!TryParseScale(value, out var factor)) { return false; }
            state.ScaleY = sign * factor;
        }
        else if (TryStripPrefix(name, "scale-", out value))
        {
            if (!TryParseScale(value, out var factor)) { return false; }
            state.ScaleX = sign * factor;
            state.ScaleY = sign * factor;
        }
        else if (TryStripPrefix(name, "skew-x-", out value))
        {
            if (!TryParseDegrees(value, out var degrees)) { return false; }
            state.SkewXDegrees = sign * degrees;
        }
        else if (TryStripPrefix(name, "skew-y-", out value))
        {
            if (!TryParseDegrees(value, out var degrees)) { return false; }
            state.SkewYDegrees = sign * degrees;
        }
        else if (TryStripPrefix(name, "skew-", out value))
        {
            if (!TryParseDegrees(value, out var degrees)) { return false; }
            state.SkewXDegrees = sign * degrees;
            state.SkewYDegrees = sign * degrees;
        }
        else
        {
            return false;
        }

        state.HasTransform = true;
        return true;
    }

    private static bool TryStripPrefix(string token, string prefix, out string rest)
    {
        if (token.StartsWith(prefix, StringComparison.Ordinal) && token.Length > prefix.Length)
        {
            rest = token[prefix.Length..];
            return true;
        }

        rest = string.Empty;
        return false;
    }

    private static bool TryParseTranslate(string value, out double pixels) =>
        (SpacingScale.TryGetPixels(value, out pixels) || TryParseArbitraryDouble(value, out pixels)) &&
        double.IsFinite(pixels) && pixels >= 0;

    private static bool TryParseNumber(string text, out double number) =>
        double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number) &&
        double.IsFinite(number) &&
        !text.StartsWith('-') && !text.StartsWith('+') && !text.EndsWith('.');

    // Degrees: a bare non-negative number (rotate-45) or an arbitrary value such as rotate-[12.5deg].
    private static bool TryParseDegrees(string value, out double degrees)
    {
        if (value.Length > 2 && value[0] == '[' && value[^1] == ']')
        {
            var inner = value[1..^1];
            inner = inner.EndsWith("deg", StringComparison.Ordinal) ? inner[..^3] : inner;
            var isNegative = inner.StartsWith('-');
            var ok = TryParseNumber(isNegative ? inner[1..] : inner, out degrees);
            degrees = isNegative ? -degrees : degrees;
            return ok;
        }

        return TryParseNumber(value, out degrees);
    }

    // Scale: a bare percentage (scale-150 = 1.5) or an arbitrary multiplier such as scale-[1.25].
    private static bool TryParseScale(string value, out double factor)
    {
        if (value.Length > 2 && value[0] == '[' && value[^1] == ']')
        {
            var inner = value[1..^1];
            var isNegative = inner.StartsWith('-');
            var ok = TryParseNumber(isNegative ? inner[1..] : inner, out factor);
            factor = isNegative ? -factor : factor;
            return ok;
        }

        if (TryParseNumber(value, out var percent))
        {
            factor = percent / 100;
            return true;
        }

        factor = default;
        return false;
    }

    private static bool TryParseTransformOrigin(string value, out RelativePoint origin)
    {
        (double X, double Y)? point = value switch
        {
            "center" => (0.5, 0.5),
            "top" => (0.5, 0),
            "top-right" => (1, 0),
            "right" => (1, 0.5),
            "bottom-right" => (1, 1),
            "bottom" => (0.5, 1),
            "bottom-left" => (0, 1),
            "left" => (0, 0.5),
            "top-left" => (0, 0),
            _ => null,
        };

        origin = point is { } p ? new RelativePoint(p.X, p.Y, RelativeUnit.Relative) : default;
        return point is not null;
    }

    private static ITransform BuildTransform(TransformState state)
    {
        var builder = new TransformOperations.Builder(4);

        // CSS applies translate outermost, then rotate, then scale. Avalonia multiplies operations for row vectors,
        // so they are appended in the reverse order to get the same result (a translation is not rotated or scaled).
        if (state.SkewXDegrees != 0 || state.SkewYDegrees != 0)
        {
            builder.AppendSkew(state.SkewXDegrees * Math.PI / 180, state.SkewYDegrees * Math.PI / 180);
        }

        if (state.ScaleX != 1 || state.ScaleY != 1)
        {
            builder.AppendScale(state.ScaleX, state.ScaleY);
        }

        if (state.RotateDegrees != 0)
        {
            builder.AppendRotate(state.RotateDegrees * Math.PI / 180);
        }

        if (state.TranslateX != 0 || state.TranslateY != 0)
        {
            builder.AppendTranslate(state.TranslateX, state.TranslateY);
        }

        return builder.Build();
    }
}
