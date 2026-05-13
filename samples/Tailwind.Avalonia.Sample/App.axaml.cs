using System;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Tailwind.Avalonia.Sample;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activityApplicationLifetime)
        {
            activityApplicationLifetime.MainViewFactory = CreateSampleShell;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = CreateSampleShell();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static SampleShell CreateSampleShell()
    {
        try
        {
            return new SampleShell();
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to initialize {nameof(SampleShell)}.{Environment.NewLine}{exception}", exception);
        }
    }
}