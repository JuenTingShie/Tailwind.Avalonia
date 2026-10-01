using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Tailwind.Avalonia.Sample.Tests;

public class SampleShellNavigationTests
{
    private static (Window Window, SampleShell Shell) Open()
    {
        var shell = new SampleShell();
        var window = new Window { Width = 1280, Height = 900, Content = shell };
        window.Show();
        window.UpdateLayout();
        return (window, shell);
    }

    private static List<string> PageEntries(SampleShell shell) =>
        shell.NavigationList.Items.OfType<object>()
            .Where(i => i is not SampleNavigationHeading)
            .Select(i => (string)i.GetType().GetProperty("Header")!.GetValue(i)!)
            .ToList();

    [Fact]
    public void Search_Filters_The_List_And_Enter_Opens_The_First_Match()
    {
        SampleHeadless.Run(() =>
        {
            var (window, shell) = Open();
            Assert.Equal(SampleCatalog.Pages.Count, PageEntries(shell).Count);

            shell.NavigationSearch.Text = "ring";
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.Equal(["Ring"], PageEntries(shell));
            Assert.Contains(shell.NavigationList.Items.OfType<object>(), i => i is SampleNavigationHeading { Header: "Effects" });

            shell.NavigationSearch.Focus();
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            window.UpdateLayout();
            Assert.Equal("Ring", shell.CurrentPageText.Text);

            window.Close();
        });
    }

    [Fact]
    public void Ctrl_K_Focuses_Search_Even_Without_Focus_In_The_Shell()
    {
        SampleHeadless.Run(() =>
        {
            var (window, shell) = Open();
            window.Focus();
            window.KeyPressQwerty(PhysicalKey.K, RawInputModifiers.Control);
            Assert.True(shell.NavigationSearch.IsFocused);
            window.Close();
        });
    }

    [Fact]
    public void Pages_Have_Previous_And_Next_Links()
    {
        SampleHeadless.Run(() =>
        {
            var (window, shell) = Open();
            var first = SampleCatalog.Pages[0];
            shell.Navigate(first.Section, first.Title);
            window.UpdateLayout();

            var links = window.GetVisualDescendants().OfType<Button>().Where(b => b.Classes.Contains("docs-pagerLink")).ToList();
            Assert.Single(links);
            links[0].RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
            window.UpdateLayout();

            Assert.Equal(SampleCatalog.Pages[1].Title, shell.CurrentPageText.Text);
            window.Close();
        });
    }
}
