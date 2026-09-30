using Avalonia.Controls;
using Avalonia.Media;

namespace Tailwind.Avalonia.Tests;

public class TwStructuralVariantTests
{
    private static Color? ColorOf(Border border) => (border.Background as SolidColorBrush)?.Color;

    private static Border[] BuildRows(string classes)
    {
        var panel = new StackPanel();
        var rows = new Border[4];

        for (var i = 0; i < rows.Length; i++)
        {
            rows[i] = new Border();
            Tw.SetClass(rows[i], classes);
            panel.Children.Add(rows[i]);
        }

        foreach (var row in rows)
        {
            row.ApplyStyling();
        }

        return rows;
    }

    private static Color Color(string name)
    {
        Assert.True(TailwindColorPalette.TryGetColor(name, out var color));
        return color;
    }

    [Fact]
    public void Odd_And_Even_Variants_Alternate()
    {
        var rows = BuildRows("bg-white odd:bg-red-500 even:bg-blue-500");

        Assert.Equal(Color("red-500"), ColorOf(rows[0]));
        Assert.Equal(Color("blue-500"), ColorOf(rows[1]));
        Assert.Equal(Color("red-500"), ColorOf(rows[2]));
        Assert.Equal(Color("blue-500"), ColorOf(rows[3]));
    }

    [Fact]
    public void First_And_Last_Variants_Match_Only_The_Ends()
    {
        var rows = BuildRows("bg-white first:bg-red-500 last:bg-blue-500");

        Assert.Equal(Color("red-500"), ColorOf(rows[0]));
        Assert.Equal(Color("white"), ColorOf(rows[1]));
        Assert.Equal(Color("white"), ColorOf(rows[2]));
        Assert.Equal(Color("blue-500"), ColorOf(rows[3]));
    }
}
