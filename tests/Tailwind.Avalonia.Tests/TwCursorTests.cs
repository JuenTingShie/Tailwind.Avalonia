using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Headless;

namespace Tailwind.Avalonia.Tests;

// AvaloniaLocator is process-wide, so tests that swap the cursor factory run one at a time.
[Collection("AvaloniaLocator")]
public class TwCursorTests
{
    // The headless platform supplies a real ICursorFactory without needing a window system.
    private static readonly object PlatformLock = new();
    private static bool platformReady;

    private static void EnsureHeadlessPlatform()
    {
        lock (PlatformLock)
        {
            if (platformReady)
            {
                return;
            }

            AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions()).SetupWithoutStarting();
            platformReady = true;
        }
    }

    private static void UseHeadlessPlatform() => EnsureHeadlessPlatform();

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
        UseHeadlessPlatform();
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.NotNull(border.Cursor);
    }

    [Fact]
    public void SetClass_Clears_Cursor_When_Class_Removed()
    {
        UseHeadlessPlatform();
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
        UseHeadlessPlatform();
        var border = new Border();

        Tw.SetClass(border, "cursor-banana");

        Assert.Null(border.Cursor);
    }
}
