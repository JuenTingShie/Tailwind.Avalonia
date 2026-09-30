using Avalonia.Controls;
using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Tests;

public class DocsCodeTests
{
    [Fact]
    public void Build_Prints_Listed_Attributes_Then_The_Class()
    {
        SampleHeadless.Run(() =>
        {
            var border = new Border { Width = 96, Height = 40 };
            Tw.SetClass(border, "bg-sky-500 min-h-[60px]");
            DocsCode.SetAttributes(border, "Width, Height");

            Assert.Equal("<Border Width=\"96\" Height=\"40\" tw:Tw.Class=\"bg-sky-500 min-h-[60px]\" />", DocsCode.Build(border));
        });
    }

    [Fact]
    public void Example_Appends_Generated_Snippets_Once_In_Document_Order()
    {
        SampleHeadless.Run(() =>
        {
            var first = new Border();
            var second = new Border();
            var repeat = new Border();
            Tw.SetClass(first, "p-4");
            Tw.SetClass(second, "p-8");
            Tw.SetClass(repeat, "p-4");
            DocsCode.SetShow(first, true);
            DocsCode.SetShow(second, true);
            DocsCode.SetShow(repeat, true);

            var example = new DocsExample { Code = "<Manual />", Content = new StackPanel { Children = { first, second, repeat } } };
            var window = new Window { Content = example };
            window.Show();

            Assert.Equal(["<Manual />", "<Border tw:Tw.Class=\"p-4\" />", "<Border tw:Tw.Class=\"p-8\" />"], example.Snippets);
        });
    }

    [Fact]
    public void ShowChildren_Nests_Direct_Children_That_Have_Classes()
    {
        SampleHeadless.Run(() =>
        {
            var inner = new Grid();
            Tw.SetClass(inner, "animate-spin");
            inner.Children.Add(new Border());
            var wrapper = new Border { Child = inner };
            Tw.SetClass(wrapper, "-scale-x-100");
            DocsCode.SetShowChildren(wrapper, true);

            Assert.Equal("<Border tw:Tw.Class=\"-scale-x-100\">\n    <Grid tw:Tw.Class=\"animate-spin\" />\n</Border>", DocsCode.Build(wrapper));
        });
    }
}
