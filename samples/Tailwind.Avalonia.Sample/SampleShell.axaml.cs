using System;
using System.Collections.Generic;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Hosts the sample docs navigation and lazily loads heavy demo pages on demand.
/// </summary>
public partial class SampleShell : UserControl
{
    private const string MobileDocsClass = "docs-mobile";
    private const double NarrowLayoutBreakpoint = 960;
    private const double CompactDocsHeightBreakpoint = 640;
    private const double NarrowPaneLength = 304;
    private const double WidePaneLength = 336;

    private readonly Dictionary<SampleShellPageDescriptor, Control> pageCache = new();
    private readonly SampleShellSectionDescriptor[] sections;
    private bool isSynchronizingSelection;
    private bool isNarrowLayout;
    private bool isCompactDocs;
    private SampleShellPageDescriptor? shownPage;
    private SampleShellSectionDescriptor? shownSection;
    private SampleShellSectionDescriptor? selectedSection;

    /// <summary>
    /// Initializes the sample shell and selects the first docs section.
    /// </summary>
    public SampleShell()
    {
        InitializeComponent();

        // Wire events in code-behind so the designer runtime compiler doesn't need
        // to resolve string-based event handlers from XAML for this shared shell.
        PaneCloseButton.Click += NavigationToggleClicked;
        PaneToggleButton.Click += NavigationToggleClicked;
        SectionTabStrip.SelectionChanged += SectionSelectionChanged;
        PageTabStrip.SelectionChanged += PageSelectionChanged;
        SizeChanged += SampleShellSizeChanged;

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
        if (isSynchronizingSelection)
        {
            return;
        }

        if (SectionTabStrip is null || PageTabStrip is null || PageHost is null)
        {
            return;
        }

        if (sender is not TabStrip { SelectedItem: SampleShellSectionDescriptor section })
        {
            return;
        }

        PreviewSection(section);
    }

    // Remember active page per section and surface cached page view only when page row is chosen.
    private void PageSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (isSynchronizingSelection)
        {
            return;
        }

        if (PageTabStrip is null || PageHost is null)
        {
            return;
        }

        if (selectedSection is null || sender is not TabStrip { SelectedItem: SampleShellPageDescriptor page })
        {
            return;
        }

        selectedSection.SelectedPageIndex = PageTabStrip.SelectedIndex;
        ShowPage(selectedSection, page);
    }

    // Keep each page alive after first load so repeat tab switches only toggle visibility.
    private void ShowPage(SampleShellSectionDescriptor section, SampleShellPageDescriptor page)
    {
        var targetPage = GetOrCreatePage(page);

        foreach (var hostedPage in pageCache.Values)
        {
            hostedPage.IsVisible = ReferenceEquals(hostedPage, targetPage);
        }

        shownSection = section;
        shownPage = page;
        CurrentSectionText.Text = section.Header;
        CurrentPageText.Text = page.Header;

        SetPaneOpen(false);
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
        ApplyMobileDocsClass(createdPage);
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

    // Change visible page list for chosen section, but do not force content switch.
    private void PreviewSection(SampleShellSectionDescriptor section)
    {
        selectedSection = section;
        SynchronizeNavigationSelection(section);
    }

    private void SynchronizeNavigationSelection(SampleShellSectionDescriptor section)
    {
        isSynchronizingSelection = true;

        try
        {
            PageTabStrip.ItemsSource = section.Pages;

            if (!ReferenceEquals(SectionTabStrip.SelectedItem, section))
            {
                SectionTabStrip.SelectedItem = section;
            }

            if (shownPage is not null && FindPageIndex(section, shownPage) >= 0)
            {
                PageTabStrip.SelectedItem = shownPage;
            }
            else
            {
                PageTabStrip.SelectedIndex = -1;
            }
        }
        finally
        {
            isSynchronizingSelection = false;
        }
    }

    private static int FindPageIndex(SampleShellSectionDescriptor section, SampleShellPageDescriptor page)
    {
        for (var index = 0; index < section.Pages.Count; index++)
        {
            if (ReferenceEquals(section.Pages[index], page))
            {
                return index;
            }
        }

        return -1;
    }

    // Toggle the navigation pane from either the content header or the pane itself.
    private void NavigationToggleClicked(object? sender, RoutedEventArgs e)
    {
        SetPaneOpen(!NavigationSplitView.IsPaneOpen);
    }

    // Switch between inline and overlay navigation so narrow screens keep the page readable.
    private void SampleShellSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateResponsiveLayout(e.NewSize.Width, e.NewSize.Height);
    }

    // Apply the current pane mode, widths, and shell spacing based on available width.
    private void UpdateResponsiveLayout(double width, double height)
    {
        var useNarrowLayout = width > 0 && width < NarrowLayoutBreakpoint;
        var useCompactDocs = useNarrowLayout || (height > 0 && height < CompactDocsHeightBreakpoint);

        isNarrowLayout = useNarrowLayout;
        isCompactDocs = useCompactDocs;
        NavigationSplitView.DisplayMode = SplitViewDisplayMode.Overlay;
        NavigationSplitView.OpenPaneLength = useNarrowLayout ? NarrowPaneLength : WidePaneLength;
        ShellHeader.Padding = useNarrowLayout ? new Thickness(10, 0) : new Thickness(12, 0);
        PageContentChrome.Padding = useCompactDocs ? new Thickness(10) : new Thickness(18);
        RefreshPageLayoutClasses();

        UpdateNavigationChrome();
    }

    // Keep all loaded sample pages in sync with current mobile/desktop docs style mode.
    private void RefreshPageLayoutClasses()
    {
        foreach (var cachedPage in pageCache.Values)
        {
            ApplyMobileDocsClass(cachedPage);
        }
    }

    private void ApplyMobileDocsClass(Control control)
    {
        if (isCompactDocs)
        {
            control.Classes.Add(MobileDocsClass);
        }
        else
        {
            control.Classes.Remove(MobileDocsClass);
        }
    }

    // Keep the shell buttons aligned with the current open/closed pane state.
    private void UpdateNavigationChrome()
    {
        var isPaneOpen = NavigationSplitView.IsPaneOpen;
        PaneToggleButton.IsVisible = !isPaneOpen;
        PaneCloseButton.IsVisible = isPaneOpen;
        ToolTip.SetTip(PaneToggleButton, isNarrowLayout ? "Open navigation" : "Show navigation");
        ToolTip.SetTip(PaneCloseButton, isNarrowLayout ? "Close navigation" : "Hide navigation");
    }

    // Centralize pane state changes so the button chrome stays in sync.
    private void SetPaneOpen(bool isOpen)
    {
        NavigationSplitView.IsPaneOpen = isOpen;

        UpdateNavigationChrome();
    }

    private void SampleShellAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        AttachedToVisualTree -= SampleShellAttachedToVisualTree;
        UpdateResponsiveLayout(Bounds.Width, Bounds.Height);

        if (sections.Length == 0)
        {
            return;
        }

        var initialSection = sections[0];
        PreviewSection(initialSection);

        if (initialSection.Pages.Count == 0)
        {
            return;
        }

        var initialPage = initialSection.Pages[0];
        initialSection.SelectedPageIndex = 0;
        ShowPage(initialSection, initialPage);
        SynchronizeNavigationSelection(initialSection);
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
