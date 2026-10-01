using Avalonia.Media;

namespace Tailwind.Avalonia;

/// <summary>
/// Tailwind v4 box-shadow scale (shadow-2xs ... shadow-2xl, shadow-none), expressed as Avalonia <see cref="BoxShadows"/>.
/// </summary>
internal static class BoxShadowScale
{
    public static bool TryGetShadows(string token, out BoxShadows shadows)
    {
        switch (token)
        {
            case "2xs":
                shadows = new BoxShadows(Layer(0, 1, 0, 0, 0.05));
                return true;

            case "xs":
                shadows = new BoxShadows(Layer(0, 1, 2, 0, 0.05));
                return true;

            case "sm":
                shadows = new BoxShadows(Layer(0, 1, 3, 0, 0.1), [Layer(0, 1, 2, -1, 0.1)]);
                return true;

            case "md":
                shadows = new BoxShadows(Layer(0, 4, 6, -1, 0.1), [Layer(0, 2, 4, -2, 0.1)]);
                return true;

            case "lg":
                shadows = new BoxShadows(Layer(0, 10, 15, -3, 0.1), [Layer(0, 4, 6, -4, 0.1)]);
                return true;

            case "xl":
                shadows = new BoxShadows(Layer(0, 20, 25, -5, 0.1), [Layer(0, 8, 10, -6, 0.1)]);
                return true;

            case "2xl":
                shadows = new BoxShadows(Layer(0, 25, 50, -12, 0.25));
                return true;

            case "none":
                shadows = default;
                return true;

            default:
                shadows = default;
                return false;
        }
    }

    // inset-shadow-2xs / xs / sm draw the shadow inside the box.
    public static bool TryGetInsetShadows(string token, out BoxShadows shadows)
    {
        switch (token)
        {
            case "2xs":
                shadows = new BoxShadows(Inset(0, 1, 0, 0.05));
                return true;

            case "xs":
                shadows = new BoxShadows(Inset(0, 1, 1, 0.05));
                return true;

            case "sm":
                shadows = new BoxShadows(Inset(0, 2, 4, 0.05));
                return true;

            case "none":
                shadows = default;
                return true;

            default:
                shadows = default;
                return false;
        }
    }

    private static BoxShadow Inset(double offsetX, double offsetY, double blur, double opacity)
    {
        var shadow = Layer(offsetX, offsetY, blur, 0, opacity);
        shadow.IsInset = true;
        return shadow;
    }

    private static BoxShadow Layer(double offsetX, double offsetY, double blur, double spread, double opacity) => new()
    {
        OffsetX = offsetX,
        OffsetY = offsetY,
        Blur = blur,
        Spread = spread,
        Color = Color.FromArgb((byte)Math.Round(255 * opacity, MidpointRounding.AwayFromZero), 0, 0, 0),
    };
}
