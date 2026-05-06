using System;
using System.Collections.Concurrent;
using System.Reflection;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public class Tw : AvaloniaObject
{
    private const int MarginMask = 1;
    private const int PaddingMask = 2;

    private static readonly ConcurrentDictionary<(Type Type, string PropertyName), AvaloniaProperty?> ThicknessPropertyCache = new();

    public static readonly AttachedProperty<string?> ClassProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, string?>(
            "Class",
            default,
            false,
            BindingMode.OneWay);

    private static readonly AttachedProperty<int> AppliedMaskProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, int>("AppliedMask");

    static Tw()
    {
        ClassProperty.Changed.AddClassHandler<AvaloniaObject>(HandleClassChanged);
        Visual.FlowDirectionProperty.Changed.AddClassHandler<Visual>(HandleFlowDirectionChanged);
    }

    public static void SetClass(AvaloniaObject element, string? value) => element.SetValue(ClassProperty, value);

    public static string? GetClass(AvaloniaObject element) => element.GetValue(ClassProperty);

    private static void HandleClassChanged(AvaloniaObject element, AvaloniaPropertyChangedEventArgs args)
    {
        ApplyUtilities(element, args.GetNewValue<string?>());
    }

    private static void HandleFlowDirectionChanged(Visual visual, AvaloniaPropertyChangedEventArgs args)
    {
        var classList = GetClass(visual);

        if (!string.IsNullOrWhiteSpace(classList) && ContainsLogicalUtilities(classList))
        {
            ApplyUtilities(visual, classList);
        }
    }

    private static void ApplyUtilities(AvaloniaObject element, string? classList)
    {
        var previousMask = element.GetValue(AppliedMaskProperty);
        var newMask = 0;

        var hasMargin = false;
        var hasPadding = false;
        var margin = default(Thickness);
        var padding = default(Thickness);

        if (!string.IsNullOrWhiteSpace(classList))
        {
            foreach (var token in classList.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!TryParseUtility(token, out var utility))
                {
                    continue;
                }

                switch (utility.Target)
                {
                    case SpacingTarget.Margin:
                        if (!hasMargin)
                        {
                            margin = default;
                            hasMargin = true;
                        }

                        margin = ApplyEdge(margin, utility.Edge, utility.Pixels, element);
                        break;

                    case SpacingTarget.Padding:
                        if (!hasPadding)
                        {
                            padding = default;
                            hasPadding = true;
                        }

                        padding = ApplyEdge(padding, utility.Edge, utility.Pixels, element);
                        break;
                }
            }
        }

        if (hasMargin && TrySetThickness(element, "Margin", margin))
        {
            newMask |= MarginMask;
        }
        else if ((previousMask & MarginMask) != 0)
        {
            ClearThickness(element, "Margin");
        }

        if (hasPadding && TrySetThickness(element, "Padding", padding))
        {
            newMask |= PaddingMask;
        }
        else if ((previousMask & PaddingMask) != 0)
        {
            ClearThickness(element, "Padding");
        }

        element.SetValue(AppliedMaskProperty, newMask);
    }

    private static Thickness ApplyEdge(Thickness current, SpacingEdge edge, double value, AvaloniaObject element)
    {
        var isRightToLeft = element is Visual visual && Visual.GetFlowDirection(visual) == FlowDirection.RightToLeft;

        return edge switch
        {
            SpacingEdge.All => new Thickness(value),
            SpacingEdge.X => new Thickness(value, current.Top, value, current.Bottom),
            SpacingEdge.Y => new Thickness(current.Left, value, current.Right, value),
            SpacingEdge.Top => new Thickness(current.Left, value, current.Right, current.Bottom),
            SpacingEdge.Right => new Thickness(current.Left, current.Top, value, current.Bottom),
            SpacingEdge.Bottom => new Thickness(current.Left, current.Top, current.Right, value),
            SpacingEdge.Left => new Thickness(value, current.Top, current.Right, current.Bottom),
            SpacingEdge.Start when isRightToLeft => new Thickness(current.Left, current.Top, value, current.Bottom),
            SpacingEdge.Start => new Thickness(value, current.Top, current.Right, current.Bottom),
            SpacingEdge.End when isRightToLeft => new Thickness(value, current.Top, current.Right, current.Bottom),
            SpacingEdge.End => new Thickness(current.Left, current.Top, value, current.Bottom),
            SpacingEdge.BlockStart => new Thickness(current.Left, value, current.Right, current.Bottom),
            SpacingEdge.BlockEnd => new Thickness(current.Left, current.Top, current.Right, value),
            _ => current,
        };
    }

    private static bool TryParseUtility(string token, out SpacingUtility utility)
    {
        utility = default;

        if (token.Contains(':') ||
            token.Contains('[') ||
            token.Contains('('))
        {
            return false;
        }

        var negative = token.StartsWith("-", StringComparison.Ordinal);
        var candidate = negative ? token[1..] : token;

        foreach (var descriptor in UtilityDescriptors.All)
        {
            if (!candidate.StartsWith(descriptor.Prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var scaleToken = candidate[descriptor.Prefix.Length..];

            if (scaleToken.Length == 0)
            {
                return false;
            }

            if (negative && descriptor.Target != SpacingTarget.Margin)
            {
                return false;
            }

            if (!SpacingScale.TryGetPixels(scaleToken, out var pixels))
            {
                return false;
            }

            utility = new SpacingUtility(descriptor.Target, descriptor.Edge, negative ? -pixels : pixels);
            return true;
        }

        return false;
    }

    private static bool ContainsLogicalUtilities(string classList)
    {
        foreach (var token in classList.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = token.StartsWith("-", StringComparison.Ordinal) ? token[1..] : token;

            if (candidate.StartsWith("ps-", StringComparison.Ordinal) ||
                candidate.StartsWith("pe-", StringComparison.Ordinal) ||
                candidate.StartsWith("ms-", StringComparison.Ordinal) ||
                candidate.StartsWith("me-", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TrySetThickness(AvaloniaObject element, string propertyName, Thickness value)
    {
        var property = FindThicknessProperty(element.GetType(), propertyName);

        if (property is null)
        {
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

    private static AvaloniaProperty? FindThicknessProperty(Type type, string propertyName)
    {
        return ThicknessPropertyCache.GetOrAdd((type, propertyName), static key =>
        {
            var (candidateType, candidatePropertyName) = key;
            var fieldName = $"{candidatePropertyName}Property";

            while (candidateType is not null)
            {
                var field = candidateType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (field?.GetValue(null) is AvaloniaProperty property && property.PropertyType == typeof(Thickness))
                {
                    return property;
                }

                candidateType = candidateType.BaseType;
            }

            return null;
        });
    }

    private readonly record struct SpacingUtility(SpacingTarget Target, SpacingEdge Edge, double Pixels);

    private readonly record struct UtilityDescriptor(string Prefix, SpacingTarget Target, SpacingEdge Edge);

    private enum SpacingTarget
    {
        Margin,
        Padding,
    }

    private enum SpacingEdge
    {
        All,
        X,
        Y,
        Top,
        Right,
        Bottom,
        Left,
        Start,
        End,
        BlockStart,
        BlockEnd,
    }

    private static class UtilityDescriptors
    {
        public static readonly UtilityDescriptor[] All =
        {
            new("mbs-", SpacingTarget.Margin, SpacingEdge.BlockStart),
            new("mbe-", SpacingTarget.Margin, SpacingEdge.BlockEnd),
            new("pbs-", SpacingTarget.Padding, SpacingEdge.BlockStart),
            new("pbe-", SpacingTarget.Padding, SpacingEdge.BlockEnd),
            new("mx-", SpacingTarget.Margin, SpacingEdge.X),
            new("my-", SpacingTarget.Margin, SpacingEdge.Y),
            new("ms-", SpacingTarget.Margin, SpacingEdge.Start),
            new("me-", SpacingTarget.Margin, SpacingEdge.End),
            new("mt-", SpacingTarget.Margin, SpacingEdge.Top),
            new("mr-", SpacingTarget.Margin, SpacingEdge.Right),
            new("mb-", SpacingTarget.Margin, SpacingEdge.Bottom),
            new("ml-", SpacingTarget.Margin, SpacingEdge.Left),
            new("px-", SpacingTarget.Padding, SpacingEdge.X),
            new("py-", SpacingTarget.Padding, SpacingEdge.Y),
            new("ps-", SpacingTarget.Padding, SpacingEdge.Start),
            new("pe-", SpacingTarget.Padding, SpacingEdge.End),
            new("pt-", SpacingTarget.Padding, SpacingEdge.Top),
            new("pr-", SpacingTarget.Padding, SpacingEdge.Right),
            new("pb-", SpacingTarget.Padding, SpacingEdge.Bottom),
            new("pl-", SpacingTarget.Padding, SpacingEdge.Left),
            new("m-", SpacingTarget.Margin, SpacingEdge.All),
            new("p-", SpacingTarget.Padding, SpacingEdge.All),
        };
    }
}