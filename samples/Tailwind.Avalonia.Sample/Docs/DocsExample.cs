using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// A live preview surface with the AXAML that produced it pinned underneath.
/// Replaces the hand-rolled surface + preview + code footer stack the docs
/// pages used to repeat for every example.
/// </summary>
public class DocsExample : ContentControl
{
    public static readonly StyledProperty<string?> CodeProperty =
        AvaloniaProperty.Register<DocsExample, string?>(nameof(Code));

    public static readonly StyledProperty<string> CodeLabelProperty =
        AvaloniaProperty.Register<DocsExample, string>(nameof(CodeLabel), "AXAML");

    public static readonly DirectProperty<DocsExample, bool> HasCodeProperty =
        AvaloniaProperty.RegisterDirect<DocsExample, bool>(nameof(HasCode), o => o.HasCode);

    // Registered so the control template can reach the list through a
    // TemplateBinding; the collection itself is never replaced.
    public static readonly DirectProperty<DocsExample, AvaloniaList<string>> SnippetsProperty =
        AvaloniaProperty.RegisterDirect<DocsExample, AvaloniaList<string>>(nameof(Snippets), o => o.Snippets);

    private bool hasCode;

    static DocsExample()
    {
        CodeProperty.Changed.AddClassHandler<DocsExample>((example, _) => example.ApplyCode());
    }

    /// <summary>
    /// Initializes the example and tracks snippet changes so the code footer
    /// only renders when there is something to show.
    /// </summary>
    public DocsExample()
    {
        Snippets.CollectionChanged += (_, _) => RefreshHasCode();
    }

    /// <summary>
    /// AXAML snippets rendered under the preview, one code block each.
    /// </summary>
    public AvaloniaList<string> Snippets { get; } = [];

    /// <summary>
    /// Shorthand for a single-snippet example. Setting it replaces <see cref="Snippets"/>.
    /// </summary>
    public string? Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    /// <summary>
    /// Label above the code footer. Defaults to "AXAML"; some pages use it for
    /// a variant like "Actual usage".
    /// </summary>
    public string CodeLabel
    {
        get => GetValue(CodeLabelProperty);
        set => SetValue(CodeLabelProperty, value);
    }

    /// <summary>
    /// True while at least one snippet is present, driving footer visibility.
    /// </summary>
    public bool HasCode
    {
        get => hasCode;
        private set => SetAndRaise(HasCodeProperty, ref hasCode, value);
    }

    private void ApplyCode()
    {
        Snippets.Clear();

        if (!string.IsNullOrEmpty(Code))
        {
            Snippets.Add(Code);
        }
    }

    private bool generated;

    /// <summary>
    /// Appends the snippets generated from preview elements marked with <see cref="DocsCode.ShowProperty"/>.
    /// This runs after the first measure of the preview, when every descendant exists, including helpers
    /// that build their children in OnInitialized. Until a marked element appears it keeps checking.
    /// </summary>
    protected override global::Avalonia.Size MeasureOverride(global::Avalonia.Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);

        if (!generated)
        {
            var found = DocsCode.Collect(Content);

            if (found.Count > 0)
            {
                generated = true;

                foreach (var snippet in found)
                {
                    if (!Snippets.Contains(snippet))
                    {
                        Snippets.Add(snippet);
                    }
                }
            }
        }

        return size;
    }

    private void RefreshHasCode()
    {
        HasCode = Snippets.Count > 0;
    }
}
