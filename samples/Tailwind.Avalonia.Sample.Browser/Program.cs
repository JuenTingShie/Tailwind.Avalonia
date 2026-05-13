using System.Threading.Tasks;

using Avalonia;
using Avalonia.Browser;

using Tailwind.Avalonia.Sample;

namespace Tailwind.Avalonia.Sample.Browser;

internal static class Program
{
        private static Task Main(string[] args) => BuildAvaloniaApp()
            .StartBrowserAppAsync("out");

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>();
}