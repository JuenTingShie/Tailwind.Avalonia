using Avalonia;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private static void HandleClassChanged(AvaloniaObject element, AvaloniaPropertyChangedEventArgs args)
    {
        if (element is Visual visual)
        {
            EnsureAttachHandlerRegistered(visual);
        }

        ApplyUtilities(element, args.GetNewValue<string?>());
    }

    private static void HandleFlowDirectionChanged(Visual visual, AvaloniaPropertyChangedEventArgs args)
    {
        ReapplyIfLogicalUtilitiesPresent(visual);
    }

    private static void HandleAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
    {
        if (sender is Visual visual)
        {
            ReapplyIfLogicalUtilitiesPresent(visual);
        }
    }

    private static void ReapplyIfLogicalUtilitiesPresent(Visual visual)
    {
        var classList = GetClass(visual);

        if (string.IsNullOrWhiteSpace(classList))
        {
            return;
        }

        var tokens = classList.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

        if (ContainsLogicalUtilities(tokens))
        {
            ApplyUtilities(visual, tokens);
        }
    }

    private static void EnsureAttachHandlerRegistered(Visual visual)
    {
        if (visual.GetValue(AttachHandlerRegisteredProperty))
        {
            return;
        }

        visual.AttachedToVisualTree += HandleAttachedToVisualTree;
        visual.SetValue(AttachHandlerRegisteredProperty, true);
    }

    private static readonly string[] LogicalUtilityPrefixes = UtilityDescriptors.All
        .Where(descriptor => descriptor.Edge is SpacingEdge.Start or SpacingEdge.End)
        .Select(descriptor => descriptor.Prefix)
        .ToArray();

    private static bool ContainsLogicalUtilities(string[] tokens)
    {
        foreach (var token in tokens)
        {
            var candidate = token.StartsWith("-", StringComparison.Ordinal) ? token[1..] : token;

            if (LogicalUtilityPrefixes.Any(prefix => candidate.StartsWith(prefix, StringComparison.Ordinal)))
            {
                return true;
            }
        }

        return false;
    }
}
