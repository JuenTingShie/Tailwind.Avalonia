using System;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// A preview element that should print as a plain Avalonia control in generated code
/// (the helper type is a docs convenience, not something a reader would write).
/// </summary>
public interface IDocsSnippetElement
{
    string SnippetTypeName { get; }
}

/// <summary>
/// The small centered label used inside preview boxes. <see cref="OnColor"/> switches to the
/// dark-on-light variant for labels that sit on a saturated fill.
/// </summary>
public class DocsChip : TextBlock
{
    public static readonly StyledProperty<bool> OnColorProperty =
        AvaloniaProperty.Register<DocsChip, bool>(nameof(OnColor));

    public DocsChip()
    {
        Classes.Add("docs-chip");
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
    }

    public bool OnColor
    {
        get => GetValue(OnColorProperty);
        set => SetValue(OnColorProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(TextBlock);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OnColorProperty)
        {
            Classes.Set("on-color", OnColor);
        }
    }
}

/// <summary>
/// Padding diagram: a striped shell whose tw:Tw.Class sets the padding, around a violet core labelled
/// with that class. <see cref="Mirrored"/> cancels Avalonia's RTL mirror so the physical side is visible.
/// </summary>
public class PaddingSpecimen : Border, IDocsSnippetElement
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<PaddingSpecimen, string?>(nameof(Label));

    public static readonly StyledProperty<string> CoreClassProperty =
        AvaloniaProperty.Register<PaddingSpecimen, string>(nameof(CoreClass), "min-w-[88px] min-h-12");

    public static readonly StyledProperty<bool> MirroredProperty =
        AvaloniaProperty.Register<PaddingSpecimen, bool>(nameof(Mirrored));

    public PaddingSpecimen()
    {
        Classes.Add("docs-paddingShell");
    }

    /// <summary>Chip text; defaults to the specimen's own tw:Tw.Class.</summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>Utilities that size the core so the padding around it reads clearly.</summary>
    public string CoreClass
    {
        get => GetValue(CoreClassProperty);
        set => SetValue(CoreClassProperty, value);
    }

    public bool Mirrored
    {
        get => GetValue(MirroredProperty);
        set => SetValue(MirroredProperty, value);
    }

    public string SnippetTypeName => "Border";

    protected override Type StyleKeyOverride => typeof(Border);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Mirrored)
        {
            RenderTransformOrigin = RelativePoint.Center;
            RenderTransform = new ScaleTransform(-1, 1);
        }

        var core = new Grid { Children = { new DocsChip { Text = Label ?? Tw.GetClass(this) } } };
        Tw.SetClass(core, CoreClass);
        Child = new Border { Classes = { "docs-paddingCore" }, Child = core };
    }
}

/// <summary>
/// One row of a direction diagram: a small label on each side (for example "start -&gt;" and "&lt;- end")
/// around the specimen, so the reader can see which physical side a logical utility landed on.
/// </summary>
public class DirectionRow : Grid
{
    public static readonly StyledProperty<string?> StartProperty =
        AvaloniaProperty.Register<DirectionRow, string?>(nameof(Start));

    public static readonly StyledProperty<string?> EndProperty =
        AvaloniaProperty.Register<DirectionRow, string?>(nameof(End));

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<DirectionRow, Orientation>(nameof(Orientation));

    /// <summary>Vertical stacks the labels above and below, for block-axis utilities.</summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public string? Start
    {
        get => GetValue(StartProperty);
        set => SetValue(StartProperty, value);
    }

    public string? End
    {
        get => GetValue(EndProperty);
        set => SetValue(EndProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(Grid);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var vertical = Orientation == Orientation.Vertical;

        if (vertical)
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto");
            RowSpacing = 10;
        }
        else
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto");
            ColumnSpacing = 10;
        }

        foreach (var child in Children)
        {
            Place(child, vertical, 1);
        }

        Children.Insert(0, Label(Start, vertical, 0));
        Children.Add(Label(End, vertical, 2));
    }

    private static void Place(Control control, bool vertical, int index)
    {
        if (vertical)
        {
            SetRow(control, index);
        }
        else
        {
            SetColumn(control, index);
        }
    }

    private static TextBlock Label(string? text, bool vertical, int index)
    {
        var label = new TextBlock { Classes = { "docs-miniTitle" }, Text = text };

        if (vertical)
        {
            label.HorizontalAlignment = HorizontalAlignment.Center;
        }
        else
        {
            label.VerticalAlignment = VerticalAlignment.Center;
        }

        Place(label, vertical, index);
        return label;
    }
}

/// <summary>
/// Left-to-right and right-to-left versions of the same diagram side by side, each in an inset panel.
/// </summary>
public class DirectionCompare : Grid
{
    public static readonly StyledProperty<Control?> LeftToRightProperty =
        AvaloniaProperty.Register<DirectionCompare, Control?>(nameof(LeftToRight));

    public static readonly StyledProperty<Control?> RightToLeftProperty =
        AvaloniaProperty.Register<DirectionCompare, Control?>(nameof(RightToLeft));

    public Control? LeftToRight
    {
        get => GetValue(LeftToRightProperty);
        set => SetValue(LeftToRightProperty, value);
    }

    public Control? RightToLeft
    {
        get => GetValue(RightToLeftProperty);
        set => SetValue(RightToLeftProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(Grid);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ColumnDefinitions = new ColumnDefinitions("*,*");
        ColumnSpacing = 20;
        Children.Add(Panel("Left-to-right", LeftToRight, 0));
        Children.Add(Panel("Right-to-left", RightToLeft, 1));
    }

    private static Border Panel(string title, Control? content, int column)
    {
        var stack = new StackPanel { Spacing = 12, Children = { new TextBlock { Classes = { "docs-miniTitle" }, Text = title } } };

        if (content is not null)
        {
            stack.Children.Add(content);
        }

        var panel = new Border { Classes = { "docs-inset" }, Child = stack };
        SetColumn(panel, column);
        return panel;
    }
}

/// <summary>
/// Margin diagram: a striped shell (optionally sized by <see cref="Frame"/>) holding a coloured box whose
/// tw:Tw.Class is <see cref="Box"/>, so the stripes show the margin around it. The box, not the shell, is
/// what generated code prints when <see cref="ShowCode"/> is set.
/// </summary>
public class MarginSpecimen : Border
{
    public static readonly StyledProperty<string?> BoxProperty =
        AvaloniaProperty.Register<MarginSpecimen, string?>(nameof(Box));

    public static readonly StyledProperty<string?> FrameProperty =
        AvaloniaProperty.Register<MarginSpecimen, string?>(nameof(Frame));

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<MarginSpecimen, string?>(nameof(Label));

    public static readonly StyledProperty<bool> OnColorProperty =
        AvaloniaProperty.Register<MarginSpecimen, bool>(nameof(OnColor));

    public static readonly StyledProperty<bool> ShowCodeProperty =
        AvaloniaProperty.Register<MarginSpecimen, bool>(nameof(ShowCode));

    public static readonly StyledProperty<string?> CodeAttributesProperty =
        AvaloniaProperty.Register<MarginSpecimen, string?>(nameof(CodeAttributes));

    public MarginSpecimen()
    {
        Classes.Add("docs-marginShell");
    }

    /// <summary>Utilities on the inner box, for example "bg-sky-500 m-8".</summary>
    public string? Box
    {
        get => GetValue(BoxProperty);
        set => SetValue(BoxProperty, value);
    }

    /// <summary>Utilities that size the space the box sits in, for example "h-20".</summary>
    public string? Frame
    {
        get => GetValue(FrameProperty);
        set => SetValue(FrameProperty, value);
    }

    /// <summary>Chip text; defaults to the last class in <see cref="Box"/>.</summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public bool OnColor
    {
        get => GetValue(OnColorProperty);
        set => SetValue(OnColorProperty, value);
    }

    public bool ShowCode
    {
        get => GetValue(ShowCodeProperty);
        set => SetValue(ShowCodeProperty, value);
    }

    /// <summary>Plain properties of the box to print in generated code, as for <see cref="DocsCode"/>.</summary>
    public string? CodeAttributes
    {
        get => GetValue(CodeAttributesProperty);
        set => SetValue(CodeAttributesProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(Border);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var label = Label ?? Box?.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        var box = new Border
        {
            CornerRadius = new CornerRadius(12),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Child = new DocsChip { OnColor = OnColor, Text = label },
        };
        Tw.SetClass(box, Box);
        DocsCode.SetShow(box, ShowCode);
        DocsCode.SetAttributes(box, CodeAttributes);

        if (Frame is null)
        {
            Child = box;
            return;
        }

        var frame = new Grid { Children = { box } };
        Tw.SetClass(frame, Frame);
        Child = frame;
    }
}
