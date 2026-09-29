using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Logging;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // Keyword utilities map a fixed token (e.g. "font-bold") to one or more Avalonia property values. Unlike the
    // numeric utilities above they need no parsing, so they are described by a table and applied generically.
    // A Value that is a Func<object> is created when the utility is applied, for values that need a platform
    // service (such as cursors) and so cannot be built up front.
    private readonly record struct KeywordAssignment(string PropertyName, object Value);

    private static readonly AttachedProperty<string[]?> AppliedKeywordPropertiesProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, string[]?>("AppliedKeywordProperties");

    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> KeywordPropertyCache = new();

    private static readonly Dictionary<string, KeywordAssignment[]> KeywordUtilities = CreateKeywordUtilities();

    private static Dictionary<string, KeywordAssignment[]> CreateKeywordUtilities()
    {
        var table = new Dictionary<string, KeywordAssignment[]>(StringComparer.Ordinal);

        void Add(string token, string propertyName, object value) =>
            table[token] = [new KeywordAssignment(propertyName, value)];

        Add("font-thin", "FontWeight", FontWeight.Thin);
        Add("font-extralight", "FontWeight", FontWeight.ExtraLight);
        Add("font-light", "FontWeight", FontWeight.Light);
        Add("font-normal", "FontWeight", FontWeight.Normal);
        Add("font-medium", "FontWeight", FontWeight.Medium);
        Add("font-semibold", "FontWeight", FontWeight.SemiBold);
        Add("font-bold", "FontWeight", FontWeight.Bold);
        Add("font-extrabold", "FontWeight", FontWeight.ExtraBold);
        Add("font-black", "FontWeight", FontWeight.Black);

        Add("italic", "FontStyle", FontStyle.Italic);
        Add("not-italic", "FontStyle", FontStyle.Normal);

        Add("text-ellipsis", "TextTrimming", TextTrimming.CharacterEllipsis);
        Add("text-clip", "TextTrimming", TextTrimming.None);
        Add("text-wrap", "TextWrapping", TextWrapping.Wrap);
        Add("text-nowrap", "TextWrapping", TextWrapping.NoWrap);
        Add("hidden", "IsVisible", false);
        Add("block", "IsVisible", true);
        Add("overflow-hidden", "ClipToBounds", true);
        Add("overflow-clip", "ClipToBounds", true);
        Add("overflow-visible", "ClipToBounds", false);
        Add("z-auto", "ZIndex", 0);

        // Filters map to Visual.Effect. Effects are animatable objects that belong to the UI thread, so each element
        // gets its own instance created when the utility is applied. An element has a single Effect, so a blur and a
        // drop shadow on the same element replace each other and the last class wins.
        void AddBlur(string token, double radius) =>
            table[token] = [new KeywordAssignment("Effect", (Func<object>)(() => new BlurEffect { Radius = radius }))];

        AddBlur("blur-none", 0);
        AddBlur("blur-xs", 4);
        AddBlur("blur-sm", 8);
        AddBlur("blur", 8);
        AddBlur("blur-md", 12);
        AddBlur("blur-lg", 16);
        AddBlur("blur-xl", 24);
        AddBlur("blur-2xl", 40);
        AddBlur("blur-3xl", 64);

        void AddDropShadow(string token, double offsetY, double blurRadius, double opacity) =>
            table[token] =
            [
                new KeywordAssignment("Effect", (Func<object>)(() => new DropShadowEffect
                {
                    OffsetX = 0,
                    OffsetY = offsetY,
                    BlurRadius = blurRadius,
                    Color = Colors.Black,
                    Opacity = opacity,
                })),
            ];

        AddDropShadow("drop-shadow-xs", 1, 1, 0.05);
        AddDropShadow("drop-shadow-sm", 1, 2, 0.15);
        AddDropShadow("drop-shadow", 1, 2, 0.1);
        AddDropShadow("drop-shadow-md", 3, 3, 0.12);
        AddDropShadow("drop-shadow-lg", 4, 4, 0.15);
        AddDropShadow("drop-shadow-xl", 9, 7, 0.1);
        AddDropShadow("drop-shadow-2xl", 25, 25, 0.15);

        // Self-alignment follows CSS grid semantics: align-self is the block (vertical) axis, justify-self the inline
        // (horizontal) axis and place-self sets both. Avalonia aligns an element inside its parent with these properties.
        void AddSelfAlignment(string suffix, VerticalAlignment vertical, HorizontalAlignment horizontal)
        {
            table["self-" + suffix] = [new KeywordAssignment("VerticalAlignment", vertical)];
            table["justify-self-" + suffix] = [new KeywordAssignment("HorizontalAlignment", horizontal)];
            table["place-self-" + suffix] =
            [
                new KeywordAssignment("VerticalAlignment", vertical),
                new KeywordAssignment("HorizontalAlignment", horizontal),
            ];
        }

        AddSelfAlignment("start", VerticalAlignment.Top, HorizontalAlignment.Left);
        AddSelfAlignment("center", VerticalAlignment.Center, HorizontalAlignment.Center);
        AddSelfAlignment("end", VerticalAlignment.Bottom, HorizontalAlignment.Right);
        AddSelfAlignment("stretch", VerticalAlignment.Stretch, HorizontalAlignment.Stretch);

        Add("pointer-events-none", "IsHitTestVisible", false);
        Add("pointer-events-auto", "IsHitTestVisible", true);

        void AddCursor(string token, StandardCursorType type) =>
            table[token] = [new KeywordAssignment("Cursor", (Func<object>)(() => new Cursor(type)))];

        AddCursor("cursor-auto", StandardCursorType.Arrow);
        AddCursor("cursor-default", StandardCursorType.Arrow);
        AddCursor("cursor-pointer", StandardCursorType.Hand);
        AddCursor("cursor-text", StandardCursorType.Ibeam);
        AddCursor("cursor-wait", StandardCursorType.Wait);
        AddCursor("cursor-progress", StandardCursorType.AppStarting);
        AddCursor("cursor-crosshair", StandardCursorType.Cross);
        AddCursor("cursor-move", StandardCursorType.SizeAll);
        AddCursor("cursor-help", StandardCursorType.Help);
        AddCursor("cursor-not-allowed", StandardCursorType.No);
        AddCursor("cursor-none", StandardCursorType.None);
        AddCursor("cursor-ew-resize", StandardCursorType.SizeWestEast);
        AddCursor("cursor-ns-resize", StandardCursorType.SizeNorthSouth);
        table["truncate"] =
        [
            new KeywordAssignment("TextTrimming", TextTrimming.CharacterEllipsis),
            new KeywordAssignment("TextWrapping", TextWrapping.NoWrap),
        ];

        return table;
    }

    private static bool TryParseKeywordUtility(string token, out KeywordAssignment[] assignments)
    {
        if (KeywordUtilities.TryGetValue(token, out assignments!))
        {
            return true;
        }

        // blur-[<px>] takes an arbitrary radius.
        if (token.StartsWith("blur-[", StringComparison.Ordinal) &&
            TryParseArbitraryDouble(token["blur-".Length..], out var blurRadius) &&
            blurRadius >= 0)
        {
            assignments = [new KeywordAssignment("Effect", (Func<object>)(() => new BlurEffect { Radius = blurRadius }))];
            return true;
        }

        if (TryParsePositionUtility(token, out var position))
        {
            assignments = position;
            return true;
        }

        // z-<integer> and z-[<integer>] are open-ended, so they are parsed rather than listed in the table.
        if (token.StartsWith("z-", StringComparison.Ordinal) && TryParseZIndex(token["z-".Length..], out var zIndex))
        {
            assignments = [new KeywordAssignment("ZIndex", zIndex)];
            return true;
        }

        // line-clamp-<n> limits the visible lines and ends the last one with an ellipsis; line-clamp-none removes the limit.
        if (token.StartsWith("line-clamp-", StringComparison.Ordinal))
        {
            var value = token["line-clamp-".Length..];

            if (value == "none")
            {
                assignments =
                [
                    new KeywordAssignment("MaxLines", 0),
                    new KeywordAssignment("TextTrimming", TextTrimming.None),
                ];
                return true;
            }

            if (int.TryParse(value, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var lines) && lines > 0)
            {
                assignments =
                [
                    new KeywordAssignment("MaxLines", lines),
                    new KeywordAssignment("TextTrimming", TextTrimming.CharacterEllipsis),
                    new KeywordAssignment("TextWrapping", TextWrapping.Wrap),
                ];
                return true;
            }
        }

        return false;
    }

    private static bool TryParseZIndex(string value, out int zIndex)
    {
        if (value.Length > 2 && value[0] == '[' && value[^1] == ']')
        {
            value = value[1..^1];
        }
        else if (value.StartsWith('[') || value.EndsWith(']'))
        {
            zIndex = default;
            return false;
        }

        return int.TryParse(value, System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out zIndex);
    }

    private static void ApplyKeywordUtilities(AvaloniaObject element, Dictionary<string, object>? values)
    {
        var previous = element.GetValue(AppliedKeywordPropertiesProperty);
        var applied = new List<string>();

        if (values is not null)
        {
            foreach (var (propertyName, rawValue) in values)
            {
                if (!TryResolveKeywordValue(element, propertyName, rawValue, out var value))
                {
                    continue;
                }

                var property = FindKeywordProperty(element.GetType(), propertyName);

                if (property is null || !property.PropertyType.IsInstanceOfType(value))
                {
                    Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                        element,
                        "Tw.Class could not find a compatible '{PropertyName}' property on {ElementType}; the utility was ignored.",
                        propertyName,
                        element.GetType());
                    continue;
                }

                element.SetValue(property, value);
                applied.Add(propertyName);
            }
        }

        if (previous is not null)
        {
            foreach (var propertyName in previous)
            {
                if (applied.Contains(propertyName))
                {
                    continue;
                }

                var property = FindKeywordProperty(element.GetType(), propertyName);

                if (property is not null)
                {
                    element.ClearValue(property);
                }
            }
        }

        element.SetValue(AppliedKeywordPropertiesProperty, applied.Count > 0 ? applied.ToArray() : null);
    }

    internal static bool TryResolveKeywordValue(AvaloniaObject element, string propertyName, object rawValue, out object value)
    {
        if (rawValue is not Func<object> factory)
        {
            value = rawValue;
            return true;
        }

        try
        {
            value = factory();
            return true;
        }
        catch (InvalidOperationException exception)
        {
            // Platform services (for example the cursor factory) are missing on this target or in this host.
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not create the '{PropertyName}' value on this platform ({Reason}); the utility was ignored.",
                propertyName,
                exception.Message);
            value = null!;
            return false;
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindKeywordProperty(Type type, string propertyName)
    {
        // Canvas position utilities set Canvas' attached properties, which are not declared on the element's own type.
        return propertyName switch
        {
            "Canvas.Left" => Canvas.LeftProperty,
            "Canvas.Top" => Canvas.TopProperty,
            "Canvas.Right" => Canvas.RightProperty,
            "Canvas.Bottom" => Canvas.BottomProperty,
            _ => KeywordPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key => FindPropertyField(key)),
        };
    }
}
