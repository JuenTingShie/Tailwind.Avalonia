using Avalonia.Controls;

namespace Tailwind.Avalonia.Tests;

public class TwLazyKeywordValueTests
{
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
}
