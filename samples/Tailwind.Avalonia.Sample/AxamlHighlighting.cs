using System;
using AvaloniaEdit;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Applies XML syntax highlighting to AxamlCodeBlock editors via a synchronous, one-time
/// tokenize pass (see AxamlTextMateColorizer) rather than AvaloniaEdit.TextMate's
/// Installation/TMModel, which drives tokenization on a background thread that deadlocks
/// under WASM. The grammar itself is compiled once and shared across every editor, since
/// TextMateSharp.Registry.Registry.LoadGrammar is the expensive (Oniguruma pattern
/// compilation) step and doesn't need to be repeated per instance.
/// </summary>
internal static class AxamlHighlighting
{
    private static readonly RegistryOptions RegistryOptions = new(ThemeName.DarkPlus);
    private static readonly string XmlScope = RegistryOptions.GetScopeByExtension(".xml");
    private static readonly AxamlHighlightColorizer Colorizer = new();

    private static readonly Registry SharedRegistry = new(RegistryOptions);
    private static readonly Lazy<IGrammar> SharedGrammar = new(() => SharedRegistry.LoadGrammar(XmlScope));
    private static readonly Lazy<Theme> SharedTheme = new(() =>
    {
        SharedRegistry.SetTheme(RegistryOptions.GetDefaultTheme());
        return SharedRegistry.GetTheme();
    });

    public static void Install(TextEditor editor)
    {
        var textMateColorizer = new AxamlTextMateColorizer(SharedTheme.Value);
        textMateColorizer.Tokenize(SharedGrammar.Value, editor.Document);

        editor.TextArea.TextView.LineTransformers.Add(textMateColorizer);
        editor.TextArea.TextView.LineTransformers.Add(Colorizer);
    }
}
