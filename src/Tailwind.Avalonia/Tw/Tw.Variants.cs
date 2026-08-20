using Avalonia;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Styling;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private enum VariantKind
    {
        Hover,
        Pressed,
        Focus,
    }

    private const int VariantCount = 3;

    private static readonly (string Prefix, VariantKind Kind)[] VariantPrefixes =
    {
        ("hover:", VariantKind.Hover),
        ("pressed:", VariantKind.Pressed),
        ("focus:", VariantKind.Focus),
    };

    private static string PseudoClassFor(VariantKind kind) => kind switch
    {
        VariantKind.Hover => ":pointerover",
        VariantKind.Pressed => ":pressed",
        VariantKind.Focus => ":focus",
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    private static bool TryParseVariantToken(string token, out VariantKind kind, out string remainder)
    {
        foreach (var (prefix, variantKind) in VariantPrefixes)
        {
            if (token.StartsWith(prefix, StringComparison.Ordinal))
            {
                kind = variantKind;
                remainder = token[prefix.Length..];
                return remainder.Length > 0;
            }
        }

        kind = default;
        remainder = string.Empty;
        return false;
    }

    private readonly record struct OpacityCategoryState(bool HasBase, double Base, double?[] Variants);
    private readonly record struct BrushCategoryState(bool HasBase, IBrush? Base, IBrush?[] Variants);

    private static readonly AttachedProperty<List<Style>?> AppliedVariantStylesProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, List<Style>?>("AppliedVariantStyles");

    private static void ApplyVariantStyles(
        AvaloniaObject element,
        BrushCategoryState background,
        BrushCategoryState foreground,
        BrushCategoryState borderBrush,
        OpacityCategoryState opacity)
    {
        if (element is not StyledElement styled)
        {
            return;
        }

        var previous = element.GetValue(AppliedVariantStylesProperty);

        if (previous is not null)
        {
            foreach (var style in previous)
            {
                styled.Styles.Remove(style);
            }
        }

        var newStyles = new List<Style>();

        AddBrushStyles(styled, "Background", background, newStyles);
        AddBrushStyles(styled, "Foreground", foreground, newStyles);
        AddBrushStyles(styled, "BorderBrush", borderBrush, newStyles);
        AddOpacityStyles(styled, opacity, newStyles);

        element.SetValue(AppliedVariantStylesProperty, newStyles.Count > 0 ? newStyles : null);
    }

    private static void AddBrushStyles(StyledElement element, string propertyName, BrushCategoryState state, List<Style> target)
    {
        if (!state.HasBase && Array.TrueForAll(state.Variants, v => v is null))
        {
            return;
        }

        var property = FindBrushProperty(element.GetType(), propertyName);

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' brush property on {ElementType}; the utility was ignored.",
                propertyName,
                element.GetType());
            return;
        }

        var elementType = element.GetType();

        if (state.HasBase)
        {
            var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
            element.Styles.Add(style);
            target.Add(style);
        }

        for (var i = 0; i < state.Variants.Length; i++)
        {
            if (state.Variants[i] is not { } variantBrush)
            {
                continue;
            }

            var pseudoClass = PseudoClassFor((VariantKind)i);
            var variantStyle = new Style(x => x.Is(elementType).Class(pseudoClass)) { Setters = { new Setter(property, variantBrush) } };
            element.Styles.Add(variantStyle);
            target.Add(variantStyle);
        }
    }

    private static void AddOpacityStyles(StyledElement element, OpacityCategoryState state, List<Style> target)
    {
        if (!state.HasBase && Array.TrueForAll(state.Variants, v => v is null))
        {
            return;
        }

        var property = FindDoubleProperty(element.GetType(), "Opacity");

        if (property is null)
        {
            Logger.TryGet(LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not find a '{PropertyName}' numeric property on {ElementType}; the utility was ignored.",
                "Opacity",
                element.GetType());
            return;
        }

        var elementType = element.GetType();

        if (state.HasBase)
        {
            var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
            element.Styles.Add(style);
            target.Add(style);
        }

        for (var i = 0; i < state.Variants.Length; i++)
        {
            if (state.Variants[i] is not { } variantOpacity)
            {
                continue;
            }

            var pseudoClass = PseudoClassFor((VariantKind)i);
            var variantStyle = new Style(x => x.Is(elementType).Class(pseudoClass)) { Setters = { new Setter(property, variantOpacity) } };
            element.Styles.Add(variantStyle);
            target.Add(variantStyle);
        }
    }
}
