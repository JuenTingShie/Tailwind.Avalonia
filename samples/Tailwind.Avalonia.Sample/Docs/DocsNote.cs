using Avalonia;
using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// Callout box used for support notes. Tone comes from a style class:
/// no class for neutral, <c>supported</c> for the implemented-here banner,
/// <c>gap</c> for Tailwind rows this parser does not cover yet.
/// </summary>
public class DocsNote : ContentControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<DocsNote, string?>(nameof(Title));

    public static readonly StyledProperty<string?> BodyProperty =
        AvaloniaProperty.Register<DocsNote, string?>(nameof(Body));

    /// <summary>
    /// Headline for the callout.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Single paragraph shown under the title; hidden when empty. Richer
    /// callouts put their markup in <see cref="ContentControl.Content"/> instead.
    /// </summary>
    public string? Body
    {
        get => GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }
}
