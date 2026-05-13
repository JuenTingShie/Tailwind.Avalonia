using System;
using System.Collections.Generic;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Hosts the sample docs navigation and lazily loads heavy demo pages on demand.
/// </summary>
public partial class SampleShell : UserControl
{
    private readonly Dictionary<SampleShellPageDescriptor, Control> pageCache = new();
    private readonly SampleShellSectionDescriptor[] sections;
    private SampleShellSectionDescriptor? selectedSection;

    /// <summary>
    /// Initializes the sample shell and selects the first docs section.
    /// </summary>
    public SampleShell()
    {
        InitializeComponent();

        // Wire events in code-behind so the designer runtime compiler doesn't need
        // to resolve string-based event handlers from XAML for this shared shell.
        SectionTabStrip.SelectionChanged += SectionSelectionChanged;
        PageTabStrip.SelectionChanged += PageSelectionChanged;

        sections = CreateSections();
        SectionTabStrip.ItemsSource = sections;

        if (sections.Length == 0)
        {
            return;
        }

        AttachedToVisualTree += SampleShellAttachedToVisualTree;
    }

    // Define the docs navigation tree so pages can be created only when first visited.
    private static SampleShellSectionDescriptor[] CreateSections()
    {
        return
        [
            new(
                "SPACING",
                new SampleShellPageDescriptor("Padding", static () => new Spacing.Padding()),
                new SampleShellPageDescriptor("Margin", static () => new Spacing.Margin())),
            new(
                "SIZING",
                new SampleShellPageDescriptor("Width", static () => new Sizing.Width()),
                new SampleShellPageDescriptor("Height", static () => new Sizing.Height())),
            new(
                "TYPOGRAPHY",
                new SampleShellPageDescriptor("Font size", static () => new Typography.FontSize()),
                new SampleShellPageDescriptor("Colors", static () => new Typography.ColorUtilities())),
        ];
    }

    // Sync the page strip whenever the active top-level section changes.
    private void SectionSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (SectionTabStrip is null || PageTabStrip is null || PageHost is null)
        {
            return;
        }

        if (sender is not TabStrip { SelectedItem: SampleShellSectionDescriptor section })
        {
            return;
        }

        SelectSection(section);
    }

    // Remember the active page per section and surface the cached page view.
    private void PageSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (PageTabStrip is null || PageHost is null)
        {
            return;
        }

        if (selectedSection is null || sender is not TabStrip { SelectedItem: SampleShellPageDescriptor page })
        {
            return;
        }

        selectedSection.SelectedPageIndex = PageTabStrip.SelectedIndex;
        ShowPage(page);
    }

    // Keep each page alive after first load so repeat tab switches only toggle visibility.
    private void ShowPage(SampleShellPageDescriptor page)
    {
        var targetPage = GetOrCreatePage(page);

        foreach (var hostedPage in pageCache.Values)
        {
            hostedPage.IsVisible = ReferenceEquals(hostedPage, targetPage);
        }
    }

    // Create a docs page lazily and keep it hosted for future visits.
    private Control GetOrCreatePage(SampleShellPageDescriptor page)
    {
        if (pageCache.TryGetValue(page, out var cachedPage))
        {
            return cachedPage;
        }

        var createdPage = page.CreateView();
        createdPage.IsVisible = false;
        PageHost.Children.Add(createdPage);
        pageCache.Add(page, createdPage);
        return createdPage;
    }

    // Collapse every hosted page when there is temporarily no valid selection.
    private void HideAllPages()
    {
        foreach (var hostedPage in pageCache.Values)
        {
            hostedPage.IsVisible = false;
        }
    }

    private void SelectSection(SampleShellSectionDescriptor section)
    {
        selectedSection = section;
        PageTabStrip.ItemsSource = section.Pages;

        if (!ReferenceEquals(SectionTabStrip.SelectedItem, section))
        {
            SectionTabStrip.SelectedItem = section;
        }

        if (section.Pages.Count == 0)
        {
            PageTabStrip.SelectedIndex = -1;
            HideAllPages();
            return;
        }

        var targetIndex = Math.Clamp(section.SelectedPageIndex, 0, section.Pages.Count - 1);
        var targetPage = section.Pages[targetIndex];

        if (!ReferenceEquals(PageTabStrip.SelectedItem, targetPage))
        {
            PageTabStrip.SelectedItem = targetPage;
        }

        ShowPage(targetPage);
    }

    private void SampleShellAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        AttachedToVisualTree -= SampleShellAttachedToVisualTree;
        SelectSection(sections[0]);
    }
}

internal sealed class SampleShellSectionDescriptor(string header, params SampleShellPageDescriptor[] pages)
{
    public string Header { get; } = header;

    public IReadOnlyList<SampleShellPageDescriptor> Pages { get; } = pages;

    public int SelectedPageIndex { get; set; }
}

internal sealed class SampleShellPageDescriptor(string header, Func<Control> createView)
{
    public string Header { get; } = header;

    // Build the page only when the user actually navigates to it.
    public Control CreateView()
    {
        return createView();
    }
}