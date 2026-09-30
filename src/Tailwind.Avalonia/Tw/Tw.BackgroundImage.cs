using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Tailwind.Avalonia;

public partial class Tw
{
    // bg-[url(avares://...)] plus bg-cover / bg-contain / bg-auto, bg-repeat / bg-no-repeat and the bg-<position>
    // keywords collect into one ImageBrush that replaces the element's Background (Avalonia has a single Background).
    // Unset parts follow the CSS defaults: natural size, repeating, anchored top-left.
    private sealed class BackgroundImageState
    {
        public string? Url { get; set; }

        public Stretch Stretch { get; set; } = Stretch.None;

        public bool Repeat { get; set; } = true;

        public AlignmentX AlignmentX { get; set; } = AlignmentX.Left;

        public AlignmentY AlignmentY { get; set; } = AlignmentY.Top;

        public bool IsUsable => Url is not null;
    }

    private static bool TryApplyBackgroundImageToken(string token, BackgroundImageState state)
    {
        if (token.StartsWith("bg-[url(", StringComparison.Ordinal) && token.EndsWith(")]", StringComparison.Ordinal))
        {
            var url = token["bg-[url(".Length..^2].Trim('\'', '"');

            if (url.Length == 0)
            {
                return false;
            }

            state.Url = url;
            return true;
        }

        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        switch (token)
        {
            case "bg-auto": state.Stretch = Stretch.None; return true;
            case "bg-cover": state.Stretch = Stretch.UniformToFill; return true;
            case "bg-contain": state.Stretch = Stretch.Uniform; return true;
            case "bg-repeat": state.Repeat = true; return true;
            case "bg-no-repeat": state.Repeat = false; return true;
            case "bg-center": state.AlignmentX = AlignmentX.Center; state.AlignmentY = AlignmentY.Center; return true;
            case "bg-top": state.AlignmentX = AlignmentX.Center; state.AlignmentY = AlignmentY.Top; return true;
            case "bg-bottom": state.AlignmentX = AlignmentX.Center; state.AlignmentY = AlignmentY.Bottom; return true;
            case "bg-left": state.AlignmentX = AlignmentX.Left; state.AlignmentY = AlignmentY.Center; return true;
            case "bg-right": state.AlignmentX = AlignmentX.Right; state.AlignmentY = AlignmentY.Center; return true;
            case "bg-top-left": state.AlignmentX = AlignmentX.Left; state.AlignmentY = AlignmentY.Top; return true;
            case "bg-top-right": state.AlignmentX = AlignmentX.Right; state.AlignmentY = AlignmentY.Top; return true;
            case "bg-bottom-left": state.AlignmentX = AlignmentX.Left; state.AlignmentY = AlignmentY.Bottom; return true;
            case "bg-bottom-right": state.AlignmentX = AlignmentX.Right; state.AlignmentY = AlignmentY.Bottom; return true;
            default: return false;
        }
    }

    private static IBrush? TryBuildImageBrush(AvaloniaObject element, BackgroundImageState state)
    {
        try
        {
            if (!Uri.TryCreate(state.Url, UriKind.Absolute, out var uri) || uri.Scheme != "avares")
            {
                throw new InvalidOperationException($"'{state.Url}' is not an avares:// resource URI.");
            }

            var bitmap = new Bitmap(AssetLoader.Open(uri));
            var brush = new ImageBrush(bitmap)
            {
                Stretch = state.Stretch,
                AlignmentX = state.AlignmentX,
                AlignmentY = state.AlignmentY,
            };

            // Tiling only makes sense at the image's natural size; a stretched image already fills the element.
            if (state.Repeat && state.Stretch == Stretch.None)
            {
                brush.TileMode = TileMode.Tile;
                brush.DestinationRect = new RelativeRect(0, 0, bitmap.Size.Width, bitmap.Size.Height, RelativeUnit.Absolute);
            }

            return brush;
        }
        catch (Exception exception) when (exception is InvalidOperationException or IOException or ArgumentException or UriFormatException)
        {
            global::Avalonia.Logging.Logger.TryGet(global::Avalonia.Logging.LogEventLevel.Warning, LogArea)?.Log(
                element,
                "Tw.Class could not load the background image '{Url}' ({Reason}); the utility was ignored.",
                state.Url,
                exception.Message);
            return null;
        }
    }
}
