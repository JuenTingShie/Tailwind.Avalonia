using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia;
using Avalonia.Logging;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> ThicknessPropertyCache = new();
    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> BrushPropertyCache = new();
    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> DoublePropertyCache = new();
    private static readonly ConcurrentDictionary<PropertyLookupKey, AvaloniaProperty?> CornerRadiusPropertyCache = new();

    private static bool TrySetThickness(AvaloniaObject element, string propertyName, Thickness value)
    {
        var property = FindThicknessProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' Thickness property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearThickness(AvaloniaObject element, string propertyName)
    {
        var property = FindThicknessProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

    private static bool TrySetBrush(AvaloniaObject element, string propertyName, IBrush? value)
    {
        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' brush property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearBrush(AvaloniaObject element, string propertyName)
    {
        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

    private static bool TrySetDouble(AvaloniaObject element, string propertyName, double value)
    {
        var property = FindDoubleProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' numeric property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearDouble(AvaloniaObject element, string propertyName)
    {
        var property = FindDoubleProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindThicknessProperty(Type type, string propertyName)
    {
        return ThicknessPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
        {
            var property = FindPropertyField(key);
            return property?.PropertyType == typeof(Thickness) ? property : null;
        });
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindBrushProperty(Type type, string propertyName)
    {
        return BrushPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
        {
            var property = FindPropertyField(key);
            return property is not null && typeof(IBrush).IsAssignableFrom(property.PropertyType) ? property : null;
        });
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindDoubleProperty(Type type, string propertyName)
    {
        return DoublePropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
        {
            var property = FindPropertyField(key);
            return property?.PropertyType == typeof(double) ? property : null;
        });
    }

    private static bool TrySetCornerRadius(AvaloniaObject element, string propertyName, CornerRadius value)
    {
        var property = FindCornerRadiusProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' CornerRadius property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return false;
        }

        element.SetValue(property, value);
        return true;
    }

    private static void ClearCornerRadius(AvaloniaObject element, string propertyName)
    {
        var property = FindCornerRadiusProperty(element.GetType(), propertyName);

        if (property is not null)
        {
            element.ClearValue(property);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "Avalonia property lookup intentionally inspects runtime control types for public static *Property fields on the supported control surface.")]
    private static AvaloniaProperty? FindCornerRadiusProperty(Type type, string propertyName)
    {
        return CornerRadiusPropertyCache.GetOrAdd(new PropertyLookupKey(type, propertyName), static key =>
        {
            var property = FindPropertyField(key);
            return property?.PropertyType == typeof(CornerRadius) ? property : null;
        });
    }

    private static AvaloniaProperty? FindPropertyField(PropertyLookupKey key)
    {
        var field = key.Type.GetField(
            $"{key.PropertyName}Property",
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return field?.GetValue(null) as AvaloniaProperty;
    }

    private readonly record struct PropertyLookupKey
    {
        public PropertyLookupKey(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type type,
            string propertyName)
        {
            Type = type;
            PropertyName = propertyName;
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
        public Type Type { get; }

        public string PropertyName { get; }
    }
}
