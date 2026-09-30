using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample.Tests;

public class SampleCatalogTests
{
    private static readonly string[] ShellTypes = ["SampleShell", "MainWindow", "AxamlCodeBlock"];

    [Fact]
    public void Every_Page_Control_Is_Registered_Exactly_Once()
    {
        var pageTypes = typeof(SampleCatalog).Assembly.GetTypes()
            .Where(t => typeof(UserControl).IsAssignableFrom(t) && !t.IsAbstract && !ShellTypes.Contains(t.Name))
            .ToHashSet();
        var registered = new List<Type>();

        SampleHeadless.Run(() => registered.AddRange(SampleCatalog.Pages.Select(p => p.Create().GetType())));

        Assert.Equal(registered.Count, registered.Distinct().Count());
        Assert.Empty(pageTypes.Except(registered).Select(t => t.FullName));
    }

    [Fact]
    public void Titles_Are_Unique_Within_A_Section()
    {
        var duplicates = SampleCatalog.Pages.GroupBy(p => (p.Section, p.Title)).Where(g => g.Count() > 1).Select(g => g.Key.ToString());

        Assert.Empty(duplicates);
    }

    [Fact]
    public void A_Section_Is_Listed_Contiguously()
    {
        var order = SampleCatalog.Pages.Select(p => p.Section).ToList();
        var seen = new HashSet<string>();

        for (var i = 0; i < order.Count; i++)
        {
            if (i > 0 && order[i] != order[i - 1])
            {
                Assert.True(seen.Add(order[i]), $"Section '{order[i]}' is split: keep its pages together in SampleCatalog.");
            }
            else if (i == 0)
            {
                seen.Add(order[i]);
            }
        }
    }
}
