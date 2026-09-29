using Avalonia.Controls;
using Avalonia.Input;

namespace Tailwind.Avalonia.Tests;

// The headless platform (and its cursor factory) is registered once by HeadlessPlatformFixture.
public class TwCursorTests
{
    [Theory]
    [InlineData("cursor-auto")]
    [InlineData("cursor-default")]
    [InlineData("cursor-pointer")]
    [InlineData("cursor-text")]
    [InlineData("cursor-wait")]
    [InlineData("cursor-progress")]
    [InlineData("cursor-crosshair")]
    [InlineData("cursor-move")]
    [InlineData("cursor-help")]
    [InlineData("cursor-not-allowed")]
    [InlineData("cursor-none")]
    [InlineData("cursor-ew-resize")]
    [InlineData("cursor-ns-resize")]
    public void SetClass_Applies_Cursor_When_Platform_Provides_Factory(string className)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.NotNull(border.Cursor);
    }

    [Fact]
    public void SetClass_Clears_Cursor_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "cursor-pointer");
        Tw.SetClass(border, null);

        Assert.Null(border.Cursor);
    }

    [Fact]
    public void Lazy_Keyword_Value_Is_Ignored_When_Platform_Service_Is_Missing()
    {
        var border = new Border();

        var resolved = Tw.TryResolveKeywordValue(
            border,
            "Cursor",
            (Func<object>)(() => throw new InvalidOperationException("Unable to locate 'Avalonia.Platform.ICursorFactory'.")),
            out _);

        Assert.False(resolved);
    }

    [Fact]
    public void SetClass_Ignores_Unknown_Cursor()
    {
        var border = new Border();

        Tw.SetClass(border, "cursor-banana");

        Assert.Null(border.Cursor);
    }
}
