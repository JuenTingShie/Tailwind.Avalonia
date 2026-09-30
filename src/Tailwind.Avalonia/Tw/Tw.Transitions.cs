using System.Globalization;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // transition-*, duration-*, ease-* and delay-* collect into one Transitions collection on the element. Avalonia
    // animates a property when its value changes and a Transition exists for that exact property, so the property
    // list is resolved against the element type (Border.Background and TextBlock.Background are different properties).
    private sealed class TransitionState
    {
        public bool HasTransition { get; set; }

        public bool None { get; set; }

        public bool Colors { get; set; }

        public bool Opacity { get; set; }

        public bool Transform { get; set; }

        public bool Shadow { get; set; }

        public double DurationMs { get; set; } = 150;

        public double DelayMs { get; set; }

        public Easing Easing { get; set; } = EaseInOut;

        // Whether the class list set the value explicitly; animate-* only overrides its defaults when it did.
        public bool HasDuration { get; set; }

        public bool HasDelay { get; set; }

        public bool HasEasing { get; set; }
    }

    private static readonly Easing EaseLinear = new LinearEasing();
    private static readonly Easing EaseIn = new SplineEasing(0.4, 0, 1, 1);
    private static readonly Easing EaseOut = new SplineEasing(0, 0, 0.2, 1);
    private static readonly Easing EaseInOut = new SplineEasing(0.4, 0, 0.2, 1);

    private static bool TryApplyTransitionToken(string token, TransitionState state)
    {
        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        switch (token)
        {
            case "transition":
            case "transition-all":
                state.HasTransition = true;
                state.None = false;
                state.Colors = state.Opacity = state.Transform = state.Shadow = true;
                return true;
            case "transition-colors":
                state.HasTransition = true;
                state.None = false;
                state.Colors = true;
                return true;
            case "transition-opacity":
                state.HasTransition = true;
                state.None = false;
                state.Opacity = true;
                return true;
            case "transition-transform":
                state.HasTransition = true;
                state.None = false;
                state.Transform = true;
                return true;
            case "transition-shadow":
                state.HasTransition = true;
                state.None = false;
                state.Shadow = true;
                return true;
            case "transition-none":
                state.HasTransition = true;
                state.None = true;
                state.Colors = state.Opacity = state.Transform = state.Shadow = false;
                return true;
            case "ease-linear":
                state.Easing = EaseLinear;
                state.HasEasing = true;
                return true;
            case "ease-in":
                state.Easing = EaseIn;
                state.HasEasing = true;
                return true;
            case "ease-out":
                state.Easing = EaseOut;
                state.HasEasing = true;
                return true;
            case "ease-in-out":
                state.Easing = EaseInOut;
                state.HasEasing = true;
                return true;
        }

        if (token.StartsWith("duration-", StringComparison.Ordinal) && TryParseMilliseconds(token["duration-".Length..], out var duration))
        {
            state.DurationMs = duration;
            state.HasDuration = true;
            return true;
        }

        if (token.StartsWith("delay-", StringComparison.Ordinal) && TryParseMilliseconds(token["delay-".Length..], out var delay))
        {
            state.DelayMs = delay;
            state.HasDelay = true;
            return true;
        }

        return false;
    }

    // Milliseconds: a bare non-negative integer (duration-300) or an arbitrary value such as duration-[250ms] or duration-[0.5s].
    private static bool TryParseMilliseconds(string value, out double milliseconds)
    {
        milliseconds = default;

        if (value.Length > 2 && value[0] == '[' && value[^1] == ']')
        {
            var inner = value[1..^1];
            var factor = 1d;

            if (inner.EndsWith("ms", StringComparison.Ordinal))
            {
                inner = inner[..^2];
            }
            else if (inner.EndsWith('s'))
            {
                inner = inner[..^1];
                factor = 1000;
            }

            if (TryParseNumber(inner, out var number))
            {
                milliseconds = number * factor;
                return true;
            }

            return false;
        }

        if (value.Length > 0 && value.All(char.IsAsciiDigit) && double.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out milliseconds))
        {
            return true;
        }

        return false;
    }

    private static Transitions? BuildTransitions(AvaloniaObject element, TransitionState state)
    {
        if (state.None)
        {
            return new Transitions();
        }

        var transitions = new Transitions();
        var duration = TimeSpan.FromMilliseconds(state.DurationMs);
        var delay = TimeSpan.FromMilliseconds(state.DelayMs);
        var type = element.GetType();

        void Add(bool enabled, string propertyName)
        {
            if (!enabled || FindKeywordProperty(type, propertyName) is not { } property)
            {
                return;
            }

            ITransition? transition = null;
            var easing = state.Easing;

            if (typeof(IBrush).IsAssignableFrom(property.PropertyType))
            {
                transition = new BrushTransition { Property = property, Duration = duration, Delay = delay, Easing = easing };
            }
            else if (property.PropertyType == typeof(double))
            {
                transition = new DoubleTransition { Property = property, Duration = duration, Delay = delay, Easing = easing };
            }
            else if (property.PropertyType == typeof(ITransform))
            {
                transition = new TransformOperationsTransition { Property = property, Duration = duration, Delay = delay, Easing = easing };
            }
            else if (property.PropertyType == typeof(BoxShadows))
            {
                transition = new BoxShadowsTransition { Property = property, Duration = duration, Delay = delay, Easing = easing };
            }

            if (transition is not null)
            {
                transitions.Add(transition);
            }
        }

        Add(state.Colors, "Background");
        Add(state.Colors, "Foreground");
        Add(state.Colors, "BorderBrush");
        Add(state.Opacity, "Opacity");
        Add(state.Transform, "RenderTransform");
        Add(state.Shadow, "BoxShadow");
        return transitions;
    }
}
