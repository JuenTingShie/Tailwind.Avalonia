using System.Globalization;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
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
            !double.IsFinite(percent) ||
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
        var alpha = (byte)Math.Clamp((int)Math.Round(255 * opacity), 0, byte.MaxValue);
        return Color.FromArgb(alpha, color.R, color.G, color.B);
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
}
