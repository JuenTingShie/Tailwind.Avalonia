using Avalonia;
using Avalonia.Headless;
using Avalonia.Logging;

[assembly: AssemblyFixture(typeof(Tailwind.Avalonia.Sample.Tests.SampleHeadless))]

namespace Tailwind.Avalonia.Sample.Tests;

/// <summary>
/// Boots the real sample <see cref="App"/> (Fluent theme, AvaloniaEdit and the docs styles) on the headless
/// platform once, and records every warning Tailwind.Avalonia logs so a test can assert a page is clean.
/// </summary>
public sealed class SampleHeadless : IDisposable
{
    private static readonly object RunLock = new();
    private static readonly HeadlessUnitTestSession Session = HeadlessUnitTestSession.StartNew(typeof(SampleAppEntry));

    internal static readonly TwWarningSink Warnings = new();

    public SampleHeadless()
    {
        Run(static () => Logger.Sink = Warnings);
    }

    public static void Run(Action action)
    {
        lock (RunLock)
        {
            Session.Dispatch(action, CancellationToken.None).GetAwaiter().GetResult();
        }
    }

    public void Dispose() => Session.Dispose();
}

public static class SampleAppEntry
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

internal sealed class TwWarningSink : ILogSink
{
    private readonly List<string> messages = [];

    public IReadOnlyList<string> Messages => messages;

    public void Clear() => messages.Clear();

    public bool IsEnabled(LogEventLevel level, string area) => level >= LogEventLevel.Warning && area == "Tailwind.Avalonia";

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
        if (IsEnabled(level, area))
        {
            messages.Add(messageTemplate);
        }
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
    {
        if (IsEnabled(level, area))
        {
            messages.Add($"{messageTemplate} [{string.Join(", ", propertyValues)}]");
        }
    }
}
