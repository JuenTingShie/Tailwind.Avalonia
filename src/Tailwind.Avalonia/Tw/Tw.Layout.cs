using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // Container utilities: flex-row/flex-col choose a panel's orientation and gap-*, gap-x-*, gap-y-*, space-x-* and
    // space-y-* set the spacing between its children. Which Avalonia property carries the spacing depends on the
    // panel (StackPanel.Spacing, WrapPanel.ItemSpacing/LineSpacing, Grid.ColumnSpacing/RowSpacing), so the values are
    // collected first and mapped once the element type is known.
    private sealed class LayoutState
    {
        public Orientation? Orientation { get; set; }

        public double? GapX { get; set; }

        public double? GapY { get; set; }

        public bool HasValues => Orientation is not null || GapX is not null || GapY is not null;
    }

    private static bool TryApplyLayoutToken(string token, LayoutState state)
    {
        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        switch (token)
        {
            case "flex-row":
                state.Orientation = Orientation.Horizontal;
                return true;
            case "flex-col":
                state.Orientation = Orientation.Vertical;
                return true;
        }

        string? value;

        if (TryStripGapPrefix(token, "gap-x-", out value) || TryStripGapPrefix(token, "space-x-", out value))
        {
            if (!TryParseGap(value, out var x)) { return false; }
            state.GapX = x;
            return true;
        }

        if (TryStripGapPrefix(token, "gap-y-", out value) || TryStripGapPrefix(token, "space-y-", out value))
        {
            if (!TryParseGap(value, out var y)) { return false; }
            state.GapY = y;
            return true;
        }

        if (TryStripGapPrefix(token, "gap-", out value))
        {
            if (!TryParseGap(value, out var both)) { return false; }
            state.GapX = both;
            state.GapY = both;
            return true;
        }

        return false;
    }

    private static bool TryStripGapPrefix(string token, string prefix, out string? rest)
    {
        if (token.StartsWith(prefix, StringComparison.Ordinal) && token.Length > prefix.Length)
        {
            rest = token[prefix.Length..];
            return true;
        }

        rest = null;
        return false;
    }

    private static bool TryParseGap(string? value, out double pixels)
    {
        pixels = default;
        return value is not null && TryParseScaleOrArbitraryPixels(value, SpacingScale.TryGetPixels, static p => p >= 0 && double.IsFinite(p), out pixels);
    }

    private static void AddLayoutValues(AvaloniaObject element, LayoutState state, Dictionary<string, object> values)
    {
        var current = state.Orientation ?? GetCurrentOrientation(element);

        if (state.Orientation is { } orientation)
        {
            values["Orientation"] = orientation;
        }

        if (state.GapX is null && state.GapY is null)
        {
            return;
        }

        switch (element)
        {
            case Grid:
                SetIfPresent(values, "ColumnSpacing", state.GapX);
                SetIfPresent(values, "RowSpacing", state.GapY);
                break;
            case WrapPanel:
                SetIfPresent(values, "ItemSpacing", current == Orientation.Horizontal ? state.GapX : state.GapY);
                SetIfPresent(values, "LineSpacing", current == Orientation.Horizontal ? state.GapY : state.GapX);
                break;
            case StackPanel:
                SetIfPresent(values, "Spacing", current == Orientation.Horizontal ? state.GapX : state.GapY);
                break;
            default:
                // Other elements have no spacing property; requesting one makes Tw.Class log its usual warning.
                values["Spacing"] = state.GapX ?? state.GapY ?? 0d;
                break;
        }
    }

    private static void SetIfPresent(Dictionary<string, object> values, string propertyName, double? value)
    {
        if (value is { } number)
        {
            values[propertyName] = number;
        }
    }

    private static Orientation GetCurrentOrientation(AvaloniaObject element) => element switch
    {
        StackPanel stack => stack.Orientation,
        WrapPanel wrap => wrap.Orientation,
        _ => Orientation.Vertical,
    };
}
