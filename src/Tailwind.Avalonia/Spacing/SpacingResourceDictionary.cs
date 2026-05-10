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

            AddPaddingResources(suffix, pixels);
            AddMarginResources(suffix, pixels);
            AddSizingResources(suffix, pixels);

            if (pixels > 0)
            {
                AddNegativeMarginResources(suffix, pixels);
            }
        }
    }

    private void AddPaddingResources(string suffix, double pixels)
    {
        Add($"Padding{suffix}", new Thickness(pixels));
        Add($"PaddingX{suffix}", new Thickness(pixels, 0, pixels, 0));
        Add($"PaddingY{suffix}", new Thickness(0, pixels, 0, pixels));
        Add($"PaddingTop{suffix}", new Thickness(0, pixels, 0, 0));
        Add($"PaddingRight{suffix}", new Thickness(0, 0, pixels, 0));
        Add($"PaddingBottom{suffix}", new Thickness(0, 0, 0, pixels));
        Add($"PaddingLeft{suffix}", new Thickness(pixels, 0, 0, 0));
    }

    private void AddMarginResources(string suffix, double pixels)
    {
        Add($"Margin{suffix}", new Thickness(pixels));
        Add($"MarginX{suffix}", new Thickness(pixels, 0, pixels, 0));
        Add($"MarginY{suffix}", new Thickness(0, pixels, 0, pixels));
        Add($"MarginTop{suffix}", new Thickness(0, pixels, 0, 0));
        Add($"MarginRight{suffix}", new Thickness(0, 0, pixels, 0));
        Add($"MarginBottom{suffix}", new Thickness(0, 0, 0, pixels));
        Add($"MarginLeft{suffix}", new Thickness(pixels, 0, 0, 0));
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

    private void AddNegativeMarginResources(string suffix, double pixels)
    {
        Add($"NegativeMargin{suffix}", new Thickness(-pixels));
        Add($"NegativeMarginX{suffix}", new Thickness(-pixels, 0, -pixels, 0));
        Add($"NegativeMarginY{suffix}", new Thickness(0, -pixels, 0, -pixels));
        Add($"NegativeMarginTop{suffix}", new Thickness(0, -pixels, 0, 0));
        Add($"NegativeMarginRight{suffix}", new Thickness(0, 0, -pixels, 0));
        Add($"NegativeMarginBottom{suffix}", new Thickness(0, 0, 0, -pixels));
        Add($"NegativeMarginLeft{suffix}", new Thickness(-pixels, 0, 0, 0));
    }
}