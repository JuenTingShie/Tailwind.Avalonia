using System;
using Avalonia;
using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Normalizes multi-line docs code snippets so XAML source indentation does not leak into the rendered sample.
/// </summary>
public class DocsCodeTextBehavior : AvaloniaObject
{
    /// <summary>
    /// Enables indentation normalization for a docs code <see cref="TextBlock"/>.
    /// </summary>
    public static readonly AttachedProperty<bool> NormalizeIndentProperty =
        AvaloniaProperty.RegisterAttached<DocsCodeTextBehavior, TextBlock, bool>("NormalizeIndent");

    static DocsCodeTextBehavior()
    {
        NormalizeIndentProperty.Changed.AddClassHandler<TextBlock>(HandleNormalizeIndentChanged);
        TextBlock.TextProperty.Changed.AddClassHandler<TextBlock>(HandleTextChanged);
    }

    /// <summary>
    /// Sets whether indentation normalization is active for the target element.
    /// </summary>
    public static void SetNormalizeIndent(AvaloniaObject element, bool value)
    {
        element.SetValue(NormalizeIndentProperty, value);
    }

    /// <summary>
    /// Gets whether indentation normalization is active for the target element.
    /// </summary>
    public static bool GetNormalizeIndent(AvaloniaObject element)
    {
        return element.GetValue(NormalizeIndentProperty);
    }

    /// <summary>
    /// Applies normalization immediately when the behavior is turned on.
    /// </summary>
    private static void HandleNormalizeIndentChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is true)
        {
            NormalizeText(textBlock);
        }
    }

    /// <summary>
    /// Re-normalizes any later text updates while the behavior is enabled.
    /// </summary>
    private static void HandleTextChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs args)
    {
        if (GetNormalizeIndent(textBlock))
        {
            NormalizeText(textBlock);
        }
    }

    /// <summary>
    /// Rewrites the control text only when the normalized form differs.
    /// </summary>
    private static void NormalizeText(TextBlock textBlock)
    {
        var currentText = textBlock.Text;

        if (string.IsNullOrEmpty(currentText))
        {
            return;
        }

        var normalizedText = NormalizeMultilineText(currentText);

        if (!string.Equals(currentText, normalizedText, StringComparison.Ordinal))
        {
            textBlock.Text = normalizedText;
        }
    }

    /// <summary>
    /// Removes shared XAML indentation while keeping the snippet's own relative indent.
    /// </summary>
    private static string NormalizeMultilineText(string text)
    {
        var lines = text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var start = 0;
        var end = lines.Length - 1;

        while (start <= end && string.IsNullOrWhiteSpace(lines[start]))
        {
            start++;
        }

        while (end >= start && string.IsNullOrWhiteSpace(lines[end]))
        {
            end--;
        }

        if (start > end)
        {
            return string.Empty;
        }

        var trimmedLines = lines[start..(end + 1)];

        RemoveSharedIndent(trimmedLines, startIndex: 0);

        if (trimmedLines.Length > 1 && CountLeadingWhitespace(trimmedLines[0]) == 0)
        {
            RemoveSharedIndent(trimmedLines, startIndex: 1);
        }

        for (var index = 0; index < trimmedLines.Length; index++)
        {
            trimmedLines[index] = trimmedLines[index].TrimEnd();
        }

        if (TryCollapseSimpleSelfClosingElement(trimmedLines, out var collapsedText))
        {
            return collapsedText;
        }

        return string.Join(Environment.NewLine, trimmedLines);
    }

    /// <summary>
    /// Collapses escaped self-closing element snippets into a single readable line.
    /// </summary>
    private static bool TryCollapseSimpleSelfClosingElement(string[] lines, out string collapsedText)
    {
        collapsedText = string.Empty;

        if (lines.Length < 2)
        {
            return false;
        }

        var firstLine = lines[0].Trim();

        if (!firstLine.StartsWith("&lt;", StringComparison.Ordinal)
            || firstLine.Contains("&gt;", StringComparison.Ordinal)
            || firstLine.Contains("/&gt;", StringComparison.Ordinal))
        {
            return false;
        }

        for (var index = 1; index < lines.Length; index++)
        {
            var currentLine = lines[index].Trim();

            if (string.IsNullOrWhiteSpace(currentLine))
            {
                return false;
            }

            if (index < lines.Length - 1)
            {
                if (currentLine.Contains("&lt;", StringComparison.Ordinal)
                    || currentLine.Contains("&gt;", StringComparison.Ordinal))
                {
                    return false;
                }

                continue;
            }

            if (currentLine.Contains("&lt;", StringComparison.Ordinal)
                || !currentLine.EndsWith("/&gt;", StringComparison.Ordinal))
            {
                return false;
            }
        }

        var collapsedParts = new string[lines.Length];

        for (var index = 0; index < lines.Length; index++)
        {
            collapsedParts[index] = lines[index].Trim();
        }

        collapsedText = string.Join(" ", collapsedParts);
        return true;
    }

    /// <summary>
    /// Removes the smallest shared left padding from the selected line range.
    /// </summary>
    private static void RemoveSharedIndent(string[] lines, int startIndex)
    {
        var sharedIndent = int.MaxValue;

        for (var index = startIndex; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
            {
                continue;
            }

            sharedIndent = Math.Min(sharedIndent, CountLeadingWhitespace(lines[index]));
        }

        if (sharedIndent is 0 or int.MaxValue)
        {
            return;
        }

        for (var index = startIndex; index < lines.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
            {
                continue;
            }

            lines[index] = lines[index][sharedIndent..];
        }
    }

    /// <summary>
    /// Counts contiguous leading whitespace characters for indentation calculations.
    /// </summary>
    private static int CountLeadingWhitespace(string value)
    {
        var count = 0;

        while (count < value.Length && char.IsWhiteSpace(value[count]))
        {
            count++;
        }

        return count;
    }
}