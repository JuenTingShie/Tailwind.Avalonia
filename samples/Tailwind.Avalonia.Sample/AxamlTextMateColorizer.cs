using System;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using TextMateSharp.Grammars;
using TextMateSharp.Themes;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Applies TextMate grammar-based syntax highlighting to a static, read-only document by
/// tokenizing it once, synchronously, up front - instead of through
/// AvaloniaEdit.TextMate.TextMate.Installation, whose TMModel drives tokenization on a
/// background thread coordinated with a blocking WaitHandle. That model assumes a real second
/// thread; under WASM (single-threaded, no WasmEnableThreads) it deadlocks the browser's only
/// thread as soon as tokenization starts. See https://github.com/danipen/TextMateSharp/issues/57.
/// Since AxamlCodeBlock's editor is read-only and its text never changes after being set, we
/// don't need TMModel's incremental re-tokenization - a one-time synchronous pass is enough.
/// </summary>
internal sealed class AxamlTextMateColorizer : DocumentColorizingTransformer
{
    private static readonly TimeSpan TokenizeTimeLimit = TimeSpan.FromMilliseconds(3000);

    private readonly Theme _theme;
    private readonly Dictionary<int, IBrush> _brushes;
    private IToken[][] _lineTokens = [];

    public AxamlTextMateColorizer(Theme theme)
    {
        _theme = theme;
        _brushes = BuildBrushes(theme);
    }

    public void Tokenize(IGrammar grammar, TextDocument document)
    {
        var lineTokens = new IToken[document.LineCount][];
        IStateStack? ruleStack = null;

        foreach (var line in document.Lines)
        {
            var result = grammar.TokenizeLine(new LineText(document.GetText(line)), ruleStack, TokenizeTimeLimit);
            ruleStack = result.RuleStack;
            lineTokens[line.LineNumber - 1] = result.Tokens;
        }

        _lineTokens = lineTokens;
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        var lineIndex = line.LineNumber - 1;
        if (lineIndex >= _lineTokens.Length)
        {
            return;
        }

        var tokens = _lineTokens[lineIndex];
        var lineStartOffset = line.Offset;

        foreach (var token in tokens)
        {
            if (token.Scopes.Count == 0)
            {
                continue;
            }

            var start = lineStartOffset + token.StartIndex;
            var end = lineStartOffset + Math.Min(token.EndIndex, line.Length);

            if (start >= end)
            {
                continue;
            }

            var foreground = 0;
            var background = 0;

            foreach (var rule in _theme.Match(token.Scopes))
            {
                if (foreground == 0 && rule.foreground > 0)
                {
                    foreground = rule.foreground;
                }

                if (background == 0 && rule.background > 0)
                {
                    background = rule.background;
                }
            }

            if (foreground == 0 && background == 0)
            {
                continue;
            }

            ChangeLinePart(start, end, element =>
            {
                if (foreground != 0 && _brushes.TryGetValue(foreground, out var foregroundBrush))
                {
                    element.TextRunProperties.SetForegroundBrush(foregroundBrush);
                }

                if (background != 0 && _brushes.TryGetValue(background, out var backgroundBrush))
                {
                    element.TextRunProperties.SetBackgroundBrush(backgroundBrush);
                }
            });
        }
    }

    private static Dictionary<int, IBrush> BuildBrushes(Theme theme)
    {
        var brushes = new Dictionary<int, IBrush>();

        foreach (var color in theme.GetColorMap())
        {
            var id = theme.GetColorId(color);
            brushes[id] = new ImmutableSolidColorBrush(Color.Parse(NormalizeColor(color)));
        }

        return brushes;
    }

    private static string NormalizeColor(string color)
    {
        if (color.Length == 9)
        {
            Span<char> normalized = stackalloc char[] { '#', color[7], color[8], color[1], color[2], color[3], color[4], color[5], color[6] };
            return normalized.ToString();
        }

        return color;
    }
}
