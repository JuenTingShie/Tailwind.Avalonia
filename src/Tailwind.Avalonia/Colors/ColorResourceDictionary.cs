using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia;

public sealed class ColorResourceDictionary : ResourceDictionary
{
    public ColorResourceDictionary()
    {
        foreach (var token in TailwindColorPalette.Tokens)
        {
            Add($"Color{token.ResourceSuffix}", token.Color);
            Add($"Brush{token.ResourceSuffix}", new SolidColorBrush(token.Color));
        }
    }
}