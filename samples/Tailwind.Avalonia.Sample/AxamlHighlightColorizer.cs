using System.Text.RegularExpressions;

using Avalonia.Media;

using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Overlays coloring for Avalonia markup extensions and tw:Tw.Class utility tokens on top
/// of the baseline TextMate XML highlighting installed by <see cref="AxamlHighlighting"/>.
/// </summary>
internal sealed class AxamlHighlightColorizer : DocumentColorizingTransformer
{
    private static readonly Regex MarkupExtensionPattern = new(@"\{[^{}]*\}", RegexOptions.Compiled);
    private static readonly Regex TwClassAttributePattern = new(@"tw:Tw\.Class=""([^""]*)""", RegexOptions.Compiled);
    private static readonly Regex TokenPattern = new(@"\S+", RegexOptions.Compiled);

    private static readonly IBrush MarkupExtensionBrush = CreateBrush("violet-300");
    private static readonly IBrush UtilityClassBrush = CreateBrush("sky-400");

    protected override void ColorizeLine(DocumentLine line)
    {
        var text = CurrentContext.Document.GetText(line);

        foreach (Match match in MarkupExtensionPattern.Matches(text))
        {
            var start = line.Offset + match.Index;
            var end = start + match.Length;
            ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(MarkupExtensionBrush));
        }

        foreach (Match attribute in TwClassAttributePattern.Matches(text))
        {
            var value = attribute.Groups[1];

            foreach (Match token in TokenPattern.Matches(value.Value))
            {
                var start = line.Offset + value.Index + token.Index;
                var end = start + token.Length;
                ChangeLinePart(start, end, element => element.TextRunProperties.SetForegroundBrush(UtilityClassBrush));
            }
        }
    }

    private static IBrush CreateBrush(string tailwindToken)
    {
        return TailwindColorPalette.TryGetColor(tailwindToken, out var color)
            ? new SolidColorBrush(color)
            : Brushes.White;
    }
}
