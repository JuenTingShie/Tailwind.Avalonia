namespace Tailwind.Avalonia;

internal static class CornerRadiusScale
{
    public static readonly (string Token, double Pixels)[] OrderedValues =
    {
        ("none", 0.0),
        ("xs", 2.0),
        ("sm", 4.0),
        ("md", 6.0),
        ("lg", 8.0),
        ("xl", 12.0),
        ("2xl", 16.0),
        ("3xl", 24.0),
        ("4xl", 32.0),
        ("full", 9999.0),
    };

    private static readonly Dictionary<string, double> TokenToPixels = CreateLookup();

    public static bool TryGetPixels(string token, out double pixels) => TokenToPixels.TryGetValue(token, out pixels);

    private static Dictionary<string, double> CreateLookup()
    {
        var lookup = new Dictionary<string, double>(StringComparer.Ordinal);

        foreach (var (token, pixels) in OrderedValues)
        {
            lookup[token] = pixels;
        }

        return lookup;
    }
}
