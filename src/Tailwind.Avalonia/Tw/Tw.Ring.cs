using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // ring-<n> draws an outline of that width outside the element and ring-<color> colors it. Avalonia has no outline
    // property, so the ring is a spread-only BoxShadow, placed in front of any shadow-* utility like Tailwind does.
    private sealed class RingState
    {
        public double? Width { get; set; }

        public Color Color { get; set; } = Color.FromRgb(0x3b, 0x82, 0xf6);
    }

    private static bool TryApplyRingToken(string token, RingState state)
    {
        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        if (token == "ring")
        {
            state.Width = 1;
            return true;
        }

        if (!token.StartsWith("ring-", StringComparison.Ordinal) || token.Length == "ring-".Length)
        {
            return false;
        }

        var value = token["ring-".Length..];

        if (value is "0" or "1" or "2" or "4" or "8")
        {
            state.Width = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            return true;
        }

        if (TryParseArbitraryDouble(value, out var pixels))
        {
            if (pixels < 0)
            {
                return false;
            }

            state.Width = pixels;
            return true;
        }

        if (TryParseBrushUtility("bg-" + value, out var utility) && utility.Brush is ISolidColorBrush solid)
        {
            state.Color = Color.FromArgb((byte)Math.Round(solid.Color.A * solid.Opacity), solid.Color.R, solid.Color.G, solid.Color.B);
            return true;
        }

        return false;
    }

    private static BoxShadows ApplyRing(RingState ring, bool hasShadow, BoxShadows existing)
    {
        if (ring.Width is not { } width || width <= 0)
        {
            return hasShadow ? existing : default;
        }

        var ringShadow = new BoxShadow { OffsetX = 0, OffsetY = 0, Blur = 0, Spread = width, Color = ring.Color };

        if (!hasShadow)
        {
            return new BoxShadows(ringShadow);
        }

        var rest = new List<BoxShadow>();

        foreach (var shadow in existing)
        {
            rest.Add(shadow);
        }

        return new BoxShadows(ringShadow, rest.ToArray());
    }
}
