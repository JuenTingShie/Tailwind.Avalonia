using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// Marks preview elements whose markup a <see cref="DocsExample"/> prints as code. The snippet is built from
/// the element's own <c>tw:Tw.Class</c>, so the code shown can never drift from what the preview renders.
/// </summary>
/// <example>
/// <code>&lt;Border Width="96" docs:DocsCode.Show="True" docs:DocsCode.Attributes="Width" tw:Tw.Class="h-[60px]" /&gt;</code>
/// prints <c>&lt;Border Width="96" tw:Tw.Class="h-[60px]" /&gt;</c>.
/// </example>
public static class DocsCode
{
    public static readonly AttachedProperty<bool> ShowProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>("Show", typeof(DocsCode));

    /// <summary>Comma-separated plain properties (for example "Width,Height") printed before tw:Tw.Class.</summary>
    public static readonly AttachedProperty<string?> AttributesProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, string?>("Attributes", typeof(DocsCode));

    public static bool GetShow(AvaloniaObject element) => element.GetValue(ShowProperty);

    public static void SetShow(AvaloniaObject element, bool value) => element.SetValue(ShowProperty, value);

    public static string? GetAttributes(AvaloniaObject element) => element.GetValue(AttributesProperty);

    public static void SetAttributes(AvaloniaObject element, string? value) => element.SetValue(AttributesProperty, value);

    /// <summary>Snippets for every marked element under <paramref name="root"/>, in document order, without repeats.</summary>
    public static IReadOnlyList<string> Collect(object? root)
    {
        var snippets = new List<string>();

        if (root is not ILogical logical)
        {
            return snippets;
        }

        foreach (var node in logical.GetSelfAndLogicalDescendants())
        {
            if (node is AvaloniaObject element && GetShow(element))
            {
                var snippet = Build(element);

                if (!snippets.Contains(snippet))
                {
                    snippets.Add(snippet);
                }
            }
        }

        return snippets;
    }

    public static string Build(AvaloniaObject element)
    {
        var builder = new StringBuilder("<").Append(element.GetType().Name);

        foreach (var name in (GetAttributes(element) ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var value = element.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)?.GetValue(element);
            builder.Append(' ').Append(name).Append("=\"").Append(Format(value)).Append('"');
        }

        if (Tw.GetClass(element) is { Length: > 0 } classes)
        {
            builder.Append(" tw:Tw.Class=\"").Append(classes.Trim()).Append('"');
        }

        return builder.Append(" />").ToString();
    }

    private static string Format(object? value) => value switch
    {
        null => string.Empty,
        double number => number.ToString(CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty,
    };
}
