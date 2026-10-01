using Avalonia;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // bg-linear-to-* and from-*, via-*, to-* collect into one LinearGradientBrush that replaces the
    // element's Background, since Avalonia has a single Background brush instead of CSS's separate background-image.
    private sealed class GradientState
    {
        public (RelativePoint Start, RelativePoint End)? Direction { get; set; }

        public Color? From { get; set; }

        public Color? Via { get; set; }

        public Color? To { get; set; }

        public bool IsUsable => Direction is not null && (From is not null || Via is not null || To is not null);
    }

    private static bool TryApplyGradientToken(string token, GradientState state)
    {
        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        if (TryParseGradientDirection(token, out var direction))
        {
            state.Direction = direction;
            return true;
        }

        foreach (var (prefix, assign) in GradientStopPrefixes)
        {
            if (token.StartsWith(prefix, StringComparison.Ordinal) && token.Length > prefix.Length)
            {
                if (TryParseBrushUtility("bg-" + token[prefix.Length..], out var utility) &&
                    utility.Brush is ISolidColorBrush solid)
                {
                    var color = Color.FromArgb((byte)Math.Round(solid.Color.A * solid.Opacity), solid.Color.R, solid.Color.G, solid.Color.B);
                    assign(state, color);
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    private static readonly (string Prefix, Action<GradientState, Color> Assign)[] GradientStopPrefixes =
    [
        ("from-", static (state, color) => state.From = color),
        ("via-", static (state, color) => state.Via = color),
        ("to-", static (state, color) => state.To = color),
    ];

    private static bool TryParseGradientDirection(string token, out (RelativePoint Start, RelativePoint End) direction)
    {
        var suffix = token switch
        {
            _ when token.StartsWith("bg-linear-to-", StringComparison.Ordinal) => token["bg-linear-to-".Length..],
            _ => null,
        };

        (double X, double Y, double X2, double Y2)? line = suffix switch
        {
            "t" => (0.5, 1, 0.5, 0),
            "tr" => (0, 1, 1, 0),
            "r" => (0, 0.5, 1, 0.5),
            "br" => (0, 0, 1, 1),
            "b" => (0.5, 0, 0.5, 1),
            "bl" => (1, 0, 0, 1),
            "l" => (1, 0.5, 0, 0.5),
            "tl" => (1, 1, 0, 0),
            _ => null,
        };

        direction = line is { } l
            ? (new RelativePoint(l.X, l.Y, RelativeUnit.Relative), new RelativePoint(l.X2, l.Y2, RelativeUnit.Relative))
            : default;
        return line is not null;
    }

    private static IBrush BuildGradientBrush(GradientState state)
    {
        var stops = new GradientStops();
        var transparent = Colors.Transparent;

        if (state.Via is { } via)
        {
            stops.Add(new GradientStop(state.From ?? transparent, 0));
            stops.Add(new GradientStop(via, 0.5));
            stops.Add(new GradientStop(state.To ?? transparent, 1));
        }
        else
        {
            stops.Add(new GradientStop(state.From ?? transparent, 0));
            stops.Add(new GradientStop(state.To ?? transparent, 1));
        }

        var (start, end) = state.Direction!.Value;
        return new LinearGradientBrush { StartPoint = start, EndPoint = end, GradientStops = stops };
    }
}
