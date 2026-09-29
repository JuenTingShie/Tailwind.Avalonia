using Avalonia.Controls;

namespace Tailwind.Avalonia.Tests;

public class TwPointerEventsTests
{
    [Fact]
    public void SetClass_PointerEventsNone_Disables_Hit_Testing()
    {
        var border = new Border();

        Tw.SetClass(border, "pointer-events-none");

        Assert.False(border.IsHitTestVisible);
    }

    [Fact]
    public void SetClass_PointerEventsAuto_Enables_Hit_Testing()
    {
        var border = new Border { IsHitTestVisible = false };

        Tw.SetClass(border, "pointer-events-auto");

        Assert.True(border.IsHitTestVisible);
    }

    [Fact]
    public void SetClass_Removing_PointerEventsNone_Restores_Hit_Testing()
    {
        var border = new Border();

        Tw.SetClass(border, "pointer-events-none");
        Tw.SetClass(border, null);

        Assert.True(border.IsHitTestVisible);
    }

    [Fact]
    public void SetClass_Last_PointerEvents_Wins()
    {
        var button = new Button();

        Tw.SetClass(button, "pointer-events-none pointer-events-auto");

        Assert.True(button.IsHitTestVisible);
    }
}
