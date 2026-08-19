using Avalonia;
using Avalonia.Controls;

namespace Tailwind.Avalonia;

public sealed class SpacingResourceDictionary : ResourceDictionary
{
    public SpacingResourceDictionary()
    {
        Add("SpacingBase", SpacingScale.BaseUnit);

        foreach (var (token, pixels) in SpacingScale.OrderedValues)
        {
            var suffix = SpacingScale.ToResourceSuffix(token);

            AddThicknessResources("Padding", suffix, pixels);
            AddThicknessResources("Margin", suffix, pixels);
            AddSizingResources(suffix, pixels);

            if (pixels > 0)
            {
                AddThicknessResources("NegativeMargin", suffix, -pixels);
            }
        }
    }

    private void AddThicknessResources(string prefix, string suffix, double pixels)
    {
        Add($"{prefix}{suffix}", new Thickness(pixels));
        Add($"{prefix}X{suffix}", new Thickness(pixels, 0, pixels, 0));
        Add($"{prefix}Y{suffix}", new Thickness(0, pixels, 0, pixels));
        Add($"{prefix}Top{suffix}", new Thickness(0, pixels, 0, 0));
        Add($"{prefix}Right{suffix}", new Thickness(0, 0, pixels, 0));
        Add($"{prefix}Bottom{suffix}", new Thickness(0, 0, 0, pixels));
        Add($"{prefix}Left{suffix}", new Thickness(pixels, 0, 0, 0));
    }

    private void AddSizingResources(string suffix, double pixels)
    {
        Add($"Width{suffix}", pixels);
        Add($"MinWidth{suffix}", pixels);
        Add($"MaxWidth{suffix}", pixels);
        Add($"Height{suffix}", pixels);
        Add($"MinHeight{suffix}", pixels);
        Add($"MaxHeight{suffix}", pixels);
    }
}