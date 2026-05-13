using Avalonia.Controls;

namespace Tailwind.Avalonia;

/// <summary>
/// Exposes Tailwind font-size tokens as Avalonia-friendly StaticResource keys.
/// </summary>
public sealed class FontSizeResourceDictionary : ResourceDictionary
{
    public FontSizeResourceDictionary()
    {
        foreach (var (token, pixels) in FontSizeScale.OrderedValues)
        {
            Add($"FontSize{FontSizeScale.ToResourceSuffix(token)}", pixels);
        }
    }
}