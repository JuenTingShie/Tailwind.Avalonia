using Avalonia;
using Avalonia.Data;

namespace Tailwind.Avalonia;

public partial class Tw : AvaloniaObject
{
    private const int MarginMask = 1;
    private const int PaddingMask = 2;
    private const int WidthMask = 32;
    private const int MinWidthMask = 64;
    private const int MaxWidthMask = 128;
    private const int HeightMask = 256;
    private const int MinHeightMask = 512;
    private const int MaxHeightMask = 1024;
    private const int FontSizeMask = 2048;
    private const string LogArea = "Tailwind.Avalonia";

    public static readonly AttachedProperty<string?> ClassProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, string?>(
            "Class",
            default,
            false,
            BindingMode.OneWay);

    private static readonly AttachedProperty<int> AppliedMaskProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, int>("AppliedMask");

    private static readonly AttachedProperty<bool> AttachHandlerRegisteredProperty =
        AvaloniaProperty.RegisterAttached<Tw, AvaloniaObject, bool>("AttachHandlerRegistered");

    static Tw()
    {
        ClassProperty.Changed.AddClassHandler<AvaloniaObject>(HandleClassChanged);
        Visual.FlowDirectionProperty.Changed.AddClassHandler<Visual>(HandleFlowDirectionChanged);
    }

    public static void SetClass(AvaloniaObject element, string? value) => element.SetValue(ClassProperty, value);

    public static string? GetClass(AvaloniaObject element) => element.GetValue(ClassProperty);
}
