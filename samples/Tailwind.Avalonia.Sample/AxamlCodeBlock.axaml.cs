using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Metadata;

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
        AxamlHighlighting.Install(Editor);
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

    private async void OnCopyButtonClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

        if (clipboard is null)
        {
            return;
        }

        await clipboard.SetTextAsync(Editor.Text);

        CopyButton.Content = "Copied";
        await Task.Delay(TimeSpan.FromSeconds(1.5));
        CopyButton.Content = "Copy";
    }
}
