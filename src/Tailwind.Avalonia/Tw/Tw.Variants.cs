using Avalonia;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Styling;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private readonly record struct OpacityCategoryState(bool HasBase, double Base);
    private readonly record struct BrushCategoryState(bool HasBase, IBrush? Base);

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
        if (!state.HasBase)
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
        var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
        element.Styles.Add(style);
        target.Add(style);
    }

    private static void AddOpacityStyles(StyledElement element, OpacityCategoryState state, List<Style> target)
    {
        if (!state.HasBase)
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
        var style = new Style(x => x.Is(elementType)) { Setters = { new Setter(property, state.Base) } };
        element.Styles.Add(style);
        target.Add(style);
    }
}
