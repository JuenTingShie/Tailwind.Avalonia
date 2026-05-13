namespace Tailwind.Avalonia;

internal static class FontSizeScale
{
    public static readonly (string Token, double Pixels)[] OrderedValues =
    {
        ("xs", 12.0),
        ("sm", 14.0),
        ("base", 16.0),
        ("lg", 18.0),
        ("xl", 20.0),
        ("2xl", 24.0),
        ("3xl", 30.0),
        ("4xl", 36.0),
        ("5xl", 48.0),
        ("6xl", 60.0),
        ("7xl", 72.0),
        ("8xl", 96.0),
        ("9xl", 128.0),
    };

    private static readonly Dictionary<string, double> TokenToPixels = CreateLookup();

    public static bool TryGetPixels(string token, out double pixels) => TokenToPixels.TryGetValue(token, out pixels);

    public static string ToResourceSuffix(string token)
    {
        if (token.Length == 0)
        {
            return string.Empty;
        }

        if (!char.IsLetter(token[0]))
        {
            return token;
        }

        return string.Concat(char.ToUpperInvariant(token[0]), token[1..]);
    }

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