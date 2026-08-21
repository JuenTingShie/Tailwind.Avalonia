using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Shared TextMate registry so every AxamlCodeBlock reuses the same loaded grammar/theme
/// instead of re-parsing them per instance.
/// </summary>
internal static class AxamlHighlighting
{
    private static readonly RegistryOptions RegistryOptions = new(ThemeName.DarkPlus);
    private static readonly string XmlScope = RegistryOptions.GetScopeByExtension(".xml");

    public static void Install(TextEditor editor)
    {
        var installation = editor.InstallTextMate(RegistryOptions);
        installation.SetGrammar(XmlScope);
    }
}
