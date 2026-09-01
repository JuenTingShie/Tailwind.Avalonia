using System.Globalization;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private static bool TryResolveUtilityColor(string token, out Color color)
    {
        color = default;

        // Handle opacity modifiers correctly by detecting bracket-enclosed arbitrary values first.
        // For arbitrary colors like bg-[#ff0000]/50, we must extract the full [...] section
        // before checking for the slash, otherwise we split incorrectly at the / inside potential expressions.
        var bracketCloseIndex = token.IndexOf(']');
        var slashIndex = token.IndexOf('/');

        string colorToken;
        string? opacityToken = null;

        if (bracketCloseIndex >= 0 && (slashIndex < 0 || slashIndex > bracketCloseIndex))
        {
            // Arbitrary value in brackets, possibly with opacity after it
            colorToken = token[..(bracketCloseIndex + 1)];
            if (slashIndex > bracketCloseIndex)
            {
                opacityToken = token[(slashIndex + 1)..];
            }
        }
        else if (slashIndex >= 0)
        {
            // Palette color with opacity modifier (no brackets)
            colorToken = token[..slashIndex];
            opacityToken = token[(slashIndex + 1)..];
        }
        else
        {
            // No opacity modifier
            colorToken = token;
        }

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

        if (opacityToken is null)
        {
            return true;
        }

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
        var alpha = (byte)Math.Clamp((int)Math.Round(255 * opacity, MidpointRounding.AwayFromZero), 0, byte.MaxValue);
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

        // Try to parse CSS color functions (rgb, hsl, oklch)
        if (colorValue.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase) ||
            colorValue.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase) ||
            colorValue.StartsWith("hsl(", StringComparison.OrdinalIgnoreCase) ||
            colorValue.StartsWith("hsla(", StringComparison.OrdinalIgnoreCase) ||
            colorValue.StartsWith("oklch(", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                color = TailwindCssColorParser.Parse(colorValue);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

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
