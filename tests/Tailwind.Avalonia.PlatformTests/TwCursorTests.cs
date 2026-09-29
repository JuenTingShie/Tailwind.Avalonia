using Avalonia.Controls;
using Avalonia.Input;

namespace Tailwind.Avalonia.PlatformTests;

// The headless platform (and its cursor factory) is provided by HeadlessPlatformFixture; the tests that create a
// Cursor run on its UI thread, where the platform services are available.
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
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, className);

            Assert.NotNull(border.Cursor);
        });
    }

    [Fact]
    public void SetClass_Clears_Cursor_When_Class_Removed()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "cursor-pointer");
            Tw.SetClass(border, null);

            Assert.Null(border.Cursor);
        });
    }

    [Fact]
    public void SetClass_Ignores_Unknown_Cursor()
    {
        HeadlessPlatformFixture.Run(() =>
        {
            var border = new Border();

            Tw.SetClass(border, "cursor-banana");

            Assert.Null(border.Cursor);
        });
    }
}
