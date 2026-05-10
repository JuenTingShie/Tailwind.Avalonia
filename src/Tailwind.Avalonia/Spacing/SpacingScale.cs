namespace Tailwind.Avalonia;

internal static class SpacingScale
{
    public const double BaseUnit = 4.0;

    public static readonly (string Token, double Pixels)[] OrderedValues =
    {
        ("0", 0.0),
        ("px", 1.0),
        ("0.5", 2.0),
        ("1", 4.0),
        ("1.5", 6.0),
        ("2", 8.0),
        ("2.5", 10.0),
        ("3", 12.0),
        ("3.5", 14.0),
        ("4", 16.0),
        ("5", 20.0),
        ("6", 24.0),
        ("7", 28.0),
        ("8", 32.0),
        ("9", 36.0),
        ("10", 40.0),
        ("11", 44.0),
        ("12", 48.0),
        ("14", 56.0),
        ("16", 64.0),
        ("20", 80.0),
        ("24", 96.0),
        ("28", 112.0),
        ("32", 128.0),
        ("36", 144.0),
        ("40", 160.0),
        ("44", 176.0),
        ("48", 192.0),
        ("52", 208.0),
        ("56", 224.0),
        ("60", 240.0),
        ("64", 256.0),
        ("72", 288.0),
        ("80", 320.0),
        ("96", 384.0),
    };

    private static readonly Dictionary<string, double> TokenToPixels = CreateLookup();

    public static bool TryGetPixels(string token, out double pixels) => TokenToPixels.TryGetValue(token, out pixels);

    public static string ToResourceSuffix(string token)
    {
        if (token.Equals("px", StringComparison.Ordinal))
        {
            return "Px";
        }

        return token.Replace('.', '_');
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