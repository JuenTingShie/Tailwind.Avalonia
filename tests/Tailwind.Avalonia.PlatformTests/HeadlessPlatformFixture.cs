using Avalonia;
using Avalonia.Headless;

[assembly: AssemblyFixture(typeof(Tailwind.Avalonia.PlatformTests.HeadlessPlatformFixture))]

namespace Tailwind.Avalonia.PlatformTests;

/// <summary>
/// Starts Avalonia's headless platform once, before any test runs, so every test sees the same platform services
/// (such as the cursor factory) and no test races another while they are being registered. The session also owns
/// the UI thread: tests that touch thread-affine objects such as transitions run their body on it through
/// <see cref="Run"/>.
/// </summary>
public sealed class HeadlessPlatformFixture : IDisposable
{
    private static readonly object RunLock = new();

    private static readonly HeadlessUnitTestSession Session = HeadlessUnitTestSession.StartNew(typeof(HeadlessAppEntry));

    public HeadlessPlatformFixture()
    {
        // Touch the session so the platform is registered before the first test, not lazily inside one.
        Run(static () => { });
    }

    /// <summary>Runs an action on the headless UI thread and rethrows any failure.</summary>
    public static void Run(Action action)
    {
        // Tests run in parallel; the session hands work to one UI thread, so callers take turns.
        lock (RunLock)
        {
            Session.Dispatch(action, CancellationToken.None).GetAwaiter().GetResult();
        }
    }

    public void Dispose() => Session.Dispose();
}

public static class HeadlessAppEntry
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
