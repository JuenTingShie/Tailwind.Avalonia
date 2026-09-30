using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // animate-spin / ping / pulse / bounce run Tailwind's built-in keyframes with Avalonia's Animation. The animation
    // starts once the element is in the visual tree, restarts when it is re-attached, and stops when it is detached
    // or the class is removed (Avalonia then restores the property values the animation was overriding).
    private static readonly AttachedProperty<AnimationRunner?> AnimationRunnerProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, AnimationRunner?>("AnimationRunner");

    internal static string? GetActiveAnimation(AvaloniaObject element) => element.GetValue(AnimationRunnerProperty)?.Name;

    internal static Exception? GetAnimationError(AvaloniaObject element) => element.GetValue(AnimationRunnerProperty)?.LastError;

    internal static bool IsAnimationRunning(AvaloniaObject element) => element.GetValue(AnimationRunnerProperty)?.IsRunning == true;

    // Returns true for animate-* tokens; the name is null for animate-none.
    private static bool TryParseAnimationUtility(string token, out string? name)
    {
        name = null;

        switch (token)
        {
            case "animate-none":
                return true;
            case "animate-spin":
            case "animate-ping":
            case "animate-pulse":
            case "animate-bounce":
                name = token["animate-".Length..];
                return true;
            default:
                return false;
        }
    }

    private static void SyncAnimation(AvaloniaObject element, string? name)
    {
        var current = element.GetValue(AnimationRunnerProperty);

        if (current?.Name == name)
        {
            return;
        }

        current?.Dispose();
        element.SetValue(AnimationRunnerProperty, null);

        if (name is not null && element is Visual visual)
        {
            element.SetValue(AnimationRunnerProperty, new AnimationRunner(visual, name));
        }
    }

    private sealed class AnimationRunner : IDisposable
    {
        private readonly Visual visual;
        private CancellationTokenSource? cancellation;
        private bool ownsTransform;

        public AnimationRunner(Visual visual, string name)
        {
            this.visual = visual;
            Name = name;
            visual.AttachedToVisualTree += OnAttached;
            visual.DetachedFromVisualTree += OnDetached;

            if (visual.IsAttachedToVisualTree())
            {
                Start();
            }
        }

        public string Name { get; }

        public bool IsRunning { get; private set; }

        public Exception? LastError { get; private set; }

        public void Dispose()
        {
            visual.AttachedToVisualTree -= OnAttached;
            visual.DetachedFromVisualTree -= OnDetached;
            Stop();
        }

        private void OnAttached(object? sender, VisualTreeAttachmentEventArgs e) => Start();

        private void OnDetached(object? sender, VisualTreeAttachmentEventArgs e) => Stop();

        private void Start()
        {
            Stop();
            var source = cancellation = new CancellationTokenSource();

            // Wait for layout so bounce can use the element's real height.
            Dispatcher.UIThread.Post(
                () =>
                {
                    if (source.IsCancellationRequested)
                    {
                        return;
                    }

                    ownsTransform = !visual.IsSet(Visual.RenderTransformProperty);
                    IsRunning = true;
                    _ = LoopAsync(source.Token);
                },
                DispatcherPriority.Loaded);
        }

        private async Task LoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await BuildAnimation(Name, visual).RunAsync(visual, token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                LastError = exception;
            }
        }

        private void Stop()
        {
            IsRunning = false;
            cancellation?.Cancel();
            cancellation?.Dispose();
            cancellation = null;

            // The transform animator leaves the transform it created behind; remove it if the element had none before.
            if (ownsTransform)
            {
                visual.ClearValue(Visual.RenderTransformProperty);
                ownsTransform = false;
            }
        }
    }

    // RunAsync rejects IterationCount.Infinite, so the animations use a count so large it never ends in practice
    // (about 11 days for a one second cycle); LoopAsync restarts the animation if it ever does.
    private static readonly IterationCount LongRunning = new(1_000_000);

    private static Animation BuildAnimation(string name, Visual visual)
    {
        static KeyFrame Frame(double cue, params Setter[] setters)
        {
            var frame = new KeyFrame { Cue = new Cue(cue) };

            foreach (var setter in setters)
            {
                frame.Setters.Add(setter);
            }

            return frame;
        }

        // Transform sub-properties (RotateTransform.Angle, ScaleTransform.ScaleX, ...) are animated by Avalonia's
        // TransformAnimator, which creates the matching transform on RenderTransform while the animation runs.
        static Setter Rotate(double angle) => new(RotateTransform.AngleProperty, angle);
        static Setter ScaleX(double value) => new(ScaleTransform.ScaleXProperty, value);
        static Setter ScaleY(double value) => new(ScaleTransform.ScaleYProperty, value);
        static Setter TranslateY(double value) => new(TranslateTransform.YProperty, value);
        static Setter Opacity(double value) => new(Visual.OpacityProperty, value);

        switch (name)
        {
            case "spin":
                return new Animation
                {
                    IterationCount = LongRunning,
                    Duration = TimeSpan.FromSeconds(1),
                    
                    Easing = new LinearEasing(),
                    Children = { Frame(0, Rotate(0)), Frame(1, Rotate(360)) },
                };

            case "ping":
                return new Animation
                {
                    IterationCount = LongRunning,
                    Duration = TimeSpan.FromSeconds(1),
                    
                    Easing = new SplineEasing(0, 0, 0.2, 1),
                    Children =
                    {
                        Frame(0, ScaleX(1), ScaleY(1), Opacity(1)),
                        Frame(0.75, ScaleX(2), ScaleY(2), Opacity(0)),
                        Frame(1, ScaleX(2), ScaleY(2), Opacity(0)),
                    },
                };

            case "pulse":
                return new Animation
                {
                    IterationCount = LongRunning,
                    Duration = TimeSpan.FromSeconds(2),
                    
                    Easing = new SplineEasing(0.4, 0, 0.6, 1),
                    Children = { Frame(0, Opacity(1)), Frame(0.5, Opacity(0.5)), Frame(1, Opacity(1)) },
                };

            default:
                // bounce: translateY(-25%) is resolved to pixels from the element's height when the animation starts.
                var lift = visual.Bounds.Height > 0 ? visual.Bounds.Height * 0.25 : 6;
                return new Animation
                {
                    IterationCount = LongRunning,
                    Duration = TimeSpan.FromSeconds(1),
                    
                    Easing = new SplineEasing(0.4, 0, 0.6, 1),
                    Children = { Frame(0, TranslateY(-lift)), Frame(0.5, TranslateY(0)), Frame(1, TranslateY(-lift)) },
                };
        }
    }
}
