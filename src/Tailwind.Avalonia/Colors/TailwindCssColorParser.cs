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

        if (cssValue.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase)
            || cssValue.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase))
        {
            return ParseRgb(cssValue);
        }

        if (cssValue.StartsWith("hsl(", StringComparison.OrdinalIgnoreCase)
            || cssValue.StartsWith("hsla(", StringComparison.OrdinalIgnoreCase))
        {
            return ParseHsl(cssValue);
        }

        throw new FormatException($"Unsupported Tailwind color value '{cssValue}'.");
    }

    private static Color ParseRgb(string cssValue)
    {
        var (components, alpha) = SplitFunctionComponents(cssValue, "RGB");

        if (components.Length != 3)
        {
            throw new FormatException($"Invalid RGB value '{cssValue}'.");
        }

        var red = ParseRgbComponent(components[0]);
        var green = ParseRgbComponent(components[1]);
        var blue = ParseRgbComponent(components[2]);

        return Color.FromArgb(ToByte(alpha), ToByte(red), ToByte(green), ToByte(blue));
    }

    private static Color ParseHsl(string cssValue)
    {
        var (components, alpha) = SplitFunctionComponents(cssValue, "HSL");

        if (components.Length != 3)
        {
            throw new FormatException($"Invalid HSL value '{cssValue}'.");
        }

        var hue = ParseHue(components[0]);
        var saturation = ParseRequiredPercent(components[1]);
        var lightness = ParseRequiredPercent(components[2]);

        var (red, green, blue) = HslToRgb(hue, saturation, lightness);

        return Color.FromArgb(ToByte(alpha), ToByte(red), ToByte(green), ToByte(blue));
    }

    private static (double Red, double Green, double Blue) HslToRgb(double hue, double saturation, double lightness)
    {
        var chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        var hPrime = hue / 60.0;
        var x = chroma * (1 - Math.Abs(hPrime % 2 - 1));
        var m = lightness - chroma / 2;

        var (r, g, b) = (int)hPrime switch
        {
            0 => (chroma, x, 0.0),
            1 => (x, chroma, 0.0),
            2 => (0.0, chroma, x),
            3 => (0.0, x, chroma),
            4 => (x, 0.0, chroma),
            _ => (chroma, 0.0, x),
        };

        return (r + m, g + m, b + m);
    }

    private static (string[] Components, double Alpha) SplitFunctionComponents(string cssValue, string label)
    {
        var open = cssValue.IndexOf('(');
        var close = cssValue.LastIndexOf(')');

        if (open < 0 || close < 0 || close <= open)
        {
            throw new FormatException($"Invalid {label} value '{cssValue}': malformed function syntax.");
        }

        var inner = cssValue.AsSpan(open + 1, close - open - 1).Trim();

        var alpha = 1.0;
        var slashIndex = inner.IndexOf('/');
        var hasSlashAlpha = slashIndex >= 0;

        if (hasSlashAlpha)
        {
            alpha = ParseAlpha(inner[(slashIndex + 1)..].Trim().ToString());
            inner = inner[..slashIndex].Trim();
        }

        var colorPart = inner.ToString();

        string[] components = colorPart.Contains(',')
            ? colorPart.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : colorPart.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        if (components.Length == 4)
        {
            if (hasSlashAlpha)
            {
                throw new FormatException($"Invalid {label} value '{cssValue}': alpha specified twice.");
            }

            alpha = ParseAlpha(components[3]);
            components = components[..3];
        }

        return (components, alpha);
    }

    private static double ParseRgbComponent(string token)
    {
        var value = token.EndsWith('%')
            ? double.Parse(token[..^1], CultureInfo.InvariantCulture) / 100.0
            : double.Parse(token, CultureInfo.InvariantCulture) / 255.0;

        if (!double.IsFinite(value))
        {
            throw new FormatException($"Invalid numeric component '{token}': value must be finite.");
        }

        return value;
    }

    private static double ParseRequiredPercent(string token)
    {
        if (!token.EndsWith('%'))
        {
            throw new FormatException($"Invalid component '{token}': percentage value expected.");
        }

        return ParsePercentOrNumber(token, 1.0);
    }

    private static Color ParseOklch(string cssValue)
    {
        var open = cssValue.IndexOf('(');
        var close = cssValue.LastIndexOf(')');

        if (open < 0 || close < 0 || close <= open)
        {
            throw new FormatException($"Invalid OKLCH value '{cssValue}': malformed function syntax.");
        }

        var inner = cssValue.AsSpan(open + 1, close - open - 1).Trim();
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