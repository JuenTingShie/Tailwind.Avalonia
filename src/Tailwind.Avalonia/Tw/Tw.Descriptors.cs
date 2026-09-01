using Avalonia.Media;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private readonly record struct SpacingUtility(SpacingTarget Target, SpacingEdge Edge, double Pixels);
    private readonly record struct BrushUtility(BrushTarget Target, IBrush Brush);
    private readonly record struct SizingUtility(SizingTarget Target, double Pixels);
    private readonly record struct FontSizeUtility(double Pixels);

    private readonly record struct UtilityDescriptor(string Prefix, SpacingTarget Target, SpacingEdge Edge);
    private readonly record struct BrushUtilityDescriptor(string Prefix, BrushTarget Target);
    private readonly record struct SizingUtilityDescriptor(string Prefix, SizingTarget Target);
    private readonly record struct CornerRadiusUtility(CornerRadiusEdge Edge, double Pixels);
    private readonly record struct CornerRadiusUtilityDescriptor(string Prefix, CornerRadiusEdge Edge);
    private readonly record struct BorderWidthUtilityDescriptor(string Prefix, SpacingEdge Edge);

    private enum SpacingTarget
    {
        Margin,
        Padding,
        BorderWidth,
    }

    private enum BrushTarget
    {
        Background,
        Foreground,
        BorderBrush,
    }

    private enum SizingTarget
    {
        Width,
        MinWidth,
        MaxWidth,
        Height,
        MinHeight,
        MaxHeight,
    }

    private enum SpacingEdge
    {
        All,
        X,
        Y,
        Top,
        Right,
        Bottom,
        Left,
        Start,
        End,
        BlockStart,
        BlockEnd,
    }

    private enum CornerRadiusEdge
    {
        All,
        Top,
        Right,
        Bottom,
        Left,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
    }

    private static class UtilityDescriptors
    {
        public static readonly UtilityDescriptor[] All =
        {
            new("mbs-", SpacingTarget.Margin, SpacingEdge.BlockStart),
            new("mbe-", SpacingTarget.Margin, SpacingEdge.BlockEnd),
            new("pbs-", SpacingTarget.Padding, SpacingEdge.BlockStart),
            new("pbe-", SpacingTarget.Padding, SpacingEdge.BlockEnd),
            new("mx-", SpacingTarget.Margin, SpacingEdge.X),
            new("my-", SpacingTarget.Margin, SpacingEdge.Y),
            new("msv-", SpacingTarget.Margin, SpacingEdge.Left),
            new("mev-", SpacingTarget.Margin, SpacingEdge.Right),
            new("ms-", SpacingTarget.Margin, SpacingEdge.Start),
            new("me-", SpacingTarget.Margin, SpacingEdge.End),
            new("mt-", SpacingTarget.Margin, SpacingEdge.Top),
            new("mr-", SpacingTarget.Margin, SpacingEdge.Right),
            new("mb-", SpacingTarget.Margin, SpacingEdge.Bottom),
            new("ml-", SpacingTarget.Margin, SpacingEdge.Left),
            new("px-", SpacingTarget.Padding, SpacingEdge.X),
            new("py-", SpacingTarget.Padding, SpacingEdge.Y),
            new("psv-", SpacingTarget.Padding, SpacingEdge.Left),
            new("pev-", SpacingTarget.Padding, SpacingEdge.Right),
            new("ps-", SpacingTarget.Padding, SpacingEdge.Start),
            new("pe-", SpacingTarget.Padding, SpacingEdge.End),
            new("pt-", SpacingTarget.Padding, SpacingEdge.Top),
            new("pr-", SpacingTarget.Padding, SpacingEdge.Right),
            new("pb-", SpacingTarget.Padding, SpacingEdge.Bottom),
            new("pl-", SpacingTarget.Padding, SpacingEdge.Left),
            new("m-", SpacingTarget.Margin, SpacingEdge.All),
            new("p-", SpacingTarget.Padding, SpacingEdge.All),
        };
    }

    private static class BrushUtilityDescriptors
    {
        public static readonly BrushUtilityDescriptor[] All =
        {
            new("bg-", BrushTarget.Background),
            new("text-", BrushTarget.Foreground),
            new("border-", BrushTarget.BorderBrush),
        };
    }

    private static class SizingUtilityDescriptors
    {
        public static readonly SizingUtilityDescriptor[] All =
        {
            new("min-w-", SizingTarget.MinWidth),
            new("max-w-", SizingTarget.MaxWidth),
            new("min-h-", SizingTarget.MinHeight),
            new("max-h-", SizingTarget.MaxHeight),
            new("w-", SizingTarget.Width),
            new("h-", SizingTarget.Height),
        };
    }

    private static class CornerRadiusUtilityDescriptors
    {
        public static readonly CornerRadiusUtilityDescriptor[] All =
        {
            new("rounded-tl-", CornerRadiusEdge.TopLeft),
            new("rounded-tr-", CornerRadiusEdge.TopRight),
            new("rounded-br-", CornerRadiusEdge.BottomRight),
            new("rounded-bl-", CornerRadiusEdge.BottomLeft),
            new("rounded-t-", CornerRadiusEdge.Top),
            new("rounded-r-", CornerRadiusEdge.Right),
            new("rounded-b-", CornerRadiusEdge.Bottom),
            new("rounded-l-", CornerRadiusEdge.Left),
            new("rounded-", CornerRadiusEdge.All),
        };
    }

    private static class BorderWidthUtilityDescriptors
    {
        public static readonly BorderWidthUtilityDescriptor[] All =
        {
            new("border-bs", SpacingEdge.BlockStart),
            new("border-be", SpacingEdge.BlockEnd),
            new("border-x", SpacingEdge.X),
            new("border-y", SpacingEdge.Y),
            new("border-s", SpacingEdge.Start),
            new("border-e", SpacingEdge.End),
            new("border-t", SpacingEdge.Top),
            new("border-r", SpacingEdge.Right),
            new("border-b", SpacingEdge.Bottom),
            new("border-l", SpacingEdge.Left),
            new("border", SpacingEdge.All),
        };
    }
}
