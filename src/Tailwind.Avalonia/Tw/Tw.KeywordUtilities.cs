using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Logging;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // Keyword utilities map a fixed token (e.g. "font-bold") to one or more Avalonia property values. Unlike the
    // numeric utilities above they need no parsing, so they are described by a table and applied generically.
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
        table["truncate"] =
        [
            new KeywordAssignment("TextTrimming", TextTrimming.CharacterEllipsis),
            new KeywordAssignment("TextWrapping", TextWrapping.NoWrap),
        ];

        return table;
    }

    private static bool TryParseKeywordUtility(string token, out KeywordAssignment[] assignments) =>
        KeywordUtilities.TryGetValue(token, out assignments!);

    private static void ApplyKeywordUtilities(AvaloniaObject element, Dictionary<string, object>? values)
    {
        var previous = element.GetValue(AppliedKeywordPropertiesProperty);
        var applied = new List<string>();

        if (values is not null)
        {
            foreach (var (propertyName, value) in values)
            {
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

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindKeywordProperty(Type type, string propertyName)
    {
        return KeywordPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key => FindPropertyField(key));
    }
}
