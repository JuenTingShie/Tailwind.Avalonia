using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Re-indents a docs AXAML snippet from its real element nesting depth instead of
/// trusting whatever leading whitespace the snippet happened to be authored with.
/// </summary>
public static class AxamlSnippetFormatter
{
    private const string WrapperOpen =
        "<__snippetRoot" +
        " xmlns=\"https://github.com/avaloniaui\"" +
        " xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"" +
        " xmlns:tw=\"using:Tailwind.Avalonia\"" +
        " xmlns:media=\"clr-namespace:Avalonia.Media;assembly=Avalonia.Base\">";

    private const string WrapperClose = "</__snippetRoot>";

    private static readonly string[] InjectedNamespaceValues =
    [
        "https://github.com/avaloniaui",
        "http://schemas.microsoft.com/winfx/2006/xaml",
        "using:Tailwind.Avalonia",
        "clr-namespace:Avalonia.Media;assembly=Avalonia.Base",
    ];

    /// <summary>
    /// Formats a single-root AXAML element snippet, falling back to a trimmed copy of
    /// the input when the snippet cannot be parsed as XML.
    /// </summary>
    public static string Format(string snippet)
    {
        var trimmed = snippet.Trim();

        if (trimmed.Length == 0)
        {
            return string.Empty;
        }

        XElement root;

        try
        {
            root = XElement.Parse(WrapperOpen + trimmed + WrapperClose, LoadOptions.PreserveWhitespace);
        }
        catch (XmlException)
        {
            return trimmed;
        }

        var element = root.Elements().FirstOrDefault();

        if (element is null)
        {
            return trimmed;
        }

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            OmitXmlDeclaration = true,
            NewLineChars = "\n",
            ConformanceLevel = ConformanceLevel.Fragment,
        };

        var builder = new StringBuilder();

        using (var writer = XmlWriter.Create(builder, settings))
        {
            element.WriteTo(writer);
        }

        var formatted = builder.ToString();

        foreach (var injectedNamespace in InjectedNamespaceValues)
        {
            formatted = Regex.Replace(
                formatted,
                @"\s+xmlns(:\w+)?=""" + Regex.Escape(injectedNamespace) + @"""",
                string.Empty);
        }

        return formatted;
    }
}
