using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;

namespace Tailwind.Avalonia.Sample.Tests;

/// <summary>Asks the real parser whether a single class token is understood by at least one kind of control.</summary>
internal static class ParserProbe
{
    private static readonly Func<AvaloniaObject>[] Targets =
    [
        () => new Border(), () => new TextBlock(), () => new SelectableTextBlock(), () => new TextBox(), () => new Button(),
        () => new ContentControl(), () => new StackPanel(), () => new WrapPanel(), () => new Grid(), () => new Canvas(),
        () => new ScrollViewer(), () => new Image(), () => new global::Avalonia.Controls.Shapes.Path(), () => new Rectangle(),
        () => new ThemeVariantScope(),
    ];

    /// <summary>Must run on the UI thread.</summary>
    public static bool IsRecognized(string token)
    {
        foreach (var create in Targets)
        {
            SampleHeadless.Warnings.Clear();
            Tw.SetClass(create(), token);

            if (SampleHeadless.Warnings.Messages.Count == 0)
            {
                return true;
            }
        }

        return false;
    }
}
