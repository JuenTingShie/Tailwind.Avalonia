using Avalonia;
using Avalonia.Headless;

[assembly: AssemblyFixture(typeof(Tailwind.Avalonia.Tests.HeadlessPlatformFixture))]

namespace Tailwind.Avalonia.Tests;

/// <summary>
/// Registers Avalonia's headless platform once, before any test runs, so every test sees the same platform
/// services (such as the cursor factory) and no test races another while they are being registered.
/// </summary>
public sealed class HeadlessPlatformFixture
{
    public HeadlessPlatformFixture()
    {
        AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions()).SetupWithoutStarting();
    }
}
