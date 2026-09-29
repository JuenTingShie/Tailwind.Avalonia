using Avalonia.Controls;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // top-*, right-*, bottom-*, left-* and inset-* position an element inside a Canvas through its attached properties,
    // which is the Avalonia equivalent of absolute positioning. Outside a Canvas the values are stored but have no effect.
    private static bool TryParsePositionUtility(string token, out KeywordAssignment[] assignments)
    {
        assignments = [];

        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        var negative = token.StartsWith('-');
        var name = negative ? token[1..] : token;

        string[]? properties = null;
        string? value = null;

        foreach (var (prefix, targets) in PositionPrefixes)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal) && name.Length > prefix.Length)
            {
                properties = targets;
                value = name[prefix.Length..];
                break;
            }
        }

        if (properties is null || value is null)
        {
            return false;
        }

        if (!TryParseScaleOrArbitraryPixels(value, SpacingScale.TryGetPixels, static p => double.IsFinite(p), out var pixels))
        {
            return false;
        }

        // Reject a negative prefix on an already negative arbitrary value, like the margin utilities do.
        if (negative && pixels < 0)
        {
            return false;
        }

        var number = negative ? -pixels : pixels;
        assignments = properties.Select(property => new KeywordAssignment(property, number)).ToArray();
        return true;
    }

    // Longer prefixes first so inset-x- is not read as inset-.
    private static readonly (string Prefix, string[] Targets)[] PositionPrefixes =
    [
        ("inset-x-", ["Canvas.Left", "Canvas.Right"]),
        ("inset-y-", ["Canvas.Top", "Canvas.Bottom"]),
        ("inset-", ["Canvas.Left", "Canvas.Top", "Canvas.Right", "Canvas.Bottom"]),
        ("top-", ["Canvas.Top"]),
        ("right-", ["Canvas.Right"]),
        ("bottom-", ["Canvas.Bottom"]),
        ("left-", ["Canvas.Left"]),
    ];
}
