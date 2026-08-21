using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using AvaloniaEdit.Highlighting;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Displays a single read-only AXAML snippet in a syntax-highlighted code editor.
/// </summary>
public partial class AxamlCodeBlock : UserControl
{
    public static readonly StyledProperty<string?> CodeProperty =
        AvaloniaProperty.Register<AxamlCodeBlock, string?>(nameof(Code));

    static AxamlCodeBlock()
    {
        CodeProperty.Changed.AddClassHandler<AxamlCodeBlock>((control, _) => control.ApplyCode());
    }

    public AxamlCodeBlock()
    {
        InitializeComponent();
        Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
    }

    [Content]
    public string? Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    private void ApplyCode()
    {
        Editor.Text = AxamlSnippetFormatter.Format(Code ?? string.Empty);
    }
}
