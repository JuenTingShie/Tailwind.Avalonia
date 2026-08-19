using Avalonia.Media;

using System.Globalization;

namespace Tailwind.Avalonia;

internal static class TailwindCssColorParser
{
    public static Color Parse(string cssValue)
    {
        if (cssValue.Length > 0 && cssValue[0] == '#')
        {
            return Color.Parse(cssValue);
        }

        if (cssValue.StartsWith("oklch(", StringComparison.OrdinalIgnoreCase))
        {
            return ParseOklch(cssValue);
        }

        throw new FormatException($"Unsupported Tailwind color value '{cssValue}'.");
    }

    private static Color ParseOklch(string cssValue)
    {
        var inner = cssValue.AsSpan(6, cssValue.Length - 7).Trim();
        var alpha = 1.0;

        var slashIndex = inner.IndexOf('/');

        if (slashIndex >= 0)
        {
            alpha = ParseAlpha(inner[(slashIndex + 1)..].Trim().ToString());
            inner = inner[..slashIndex].Trim();
        }

        var components = inner.ToString().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        if (components.Length != 3)
        {
            throw new FormatException($"Invalid OKLCH value '{cssValue}'.");
        }

        var lightness = ParseLightness(components[0]);
        var chroma = ParseChroma(components[1]);
        var hue = ParseHue(components[2]);

        return ToSrgb(alpha, lightness, chroma, hue);
    }

    private static Color ToSrgb(double alpha, double lightness, double chroma, double hue)
    {
        var hueRadians = hue * Math.PI / 180.0;
        var a = chroma * Math.Cos(hueRadians);
        var b = chroma * Math.Sin(hueRadians);

        // Oklab to linear sRGB conversion from Bjorn Ottosson's reference implementation.
        var lPrime = lightness + 0.3963377774 * a + 0.2158037573 * b;
        var mPrime = lightness - 0.1055613458 * a - 0.0638541728 * b;
        var sPrime = lightness - 0.0894841775 * a - 1.2914855480 * b;

        var l = lPrime * lPrime * lPrime;
        var m = mPrime * mPrime * mPrime;
        var s = sPrime * sPrime * sPrime;

        var redLinear = +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s;
        var greenLinear = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s;
        var blueLinear = -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s;

        return Color.FromArgb(
            ToByte(alpha),
            ToByte(ToSrgbComponent(redLinear)),
            ToByte(ToSrgbComponent(greenLinear)),
            ToByte(ToSrgbComponent(blueLinear)));
    }

    private static double ToSrgbComponent(double linear)
    {
        var value = linear <= 0.0031308
            ? 12.92 * linear
            : 1.055 * Math.Pow(linear, 1.0 / 2.4) - 0.055;

        return Clamp01(value);
    }

    private static byte ToByte(double value)
    {
        return (byte)Math.Round(Clamp01(value) * 255.0, MidpointRounding.AwayFromZero);
    }

    private static double ParseLightness(string token) => ParsePercentOrNumber(token, 1.0);

    private static double ParseChroma(string token) => ParsePercentOrNumber(token, 0.4);

    private static double ParseAlpha(string token) => ParsePercentOrNumber(token, 1.0);

    private static double ParseHue(string token)
    {
        var normalized = token.EndsWith("deg", StringComparison.OrdinalIgnoreCase)
            ? token[..^3]
            : token;

        if (normalized.Length == 0)
        {
            throw new FormatException($"Invalid hue value '{token}': numeric component is missing.");
        }

        var value = double.Parse(normalized, CultureInfo.InvariantCulture);

        if (!double.IsFinite(value))
        {
            throw new FormatException($"Invalid hue value '{token}': value must be finite.");
        }

        value %= 360.0;

        if (value < 0)
        {
            value += 360.0;
        }

        return value;
    }

    private static double ParsePercentOrNumber(string token, double percentageScale)
    {
        var value = token.EndsWith('%')
            ? double.Parse(token[..^1], CultureInfo.InvariantCulture) / 100.0 * percentageScale
            : double.Parse(token, CultureInfo.InvariantCulture);

        if (!double.IsFinite(value))
        {
            throw new FormatException($"Invalid numeric component '{token}': value must be finite.");
        }

        return value;
    }

    private static double Clamp01(double value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 1)
        {
            return 1;
        }

        return value;
    }
}