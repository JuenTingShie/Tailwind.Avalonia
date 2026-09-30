using System.Linq;
using System;
using System.Collections.Generic;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Hosts the sample docs navigation and lazily loads heavy demo pages on demand.
/// </summary>
public partial class SampleShell : UserControl
{

    private readonly Dictionary<SampleShellPageDescriptor, Control> pageCache = new();
    private readonly SampleShellSectionDescriptor[] sections;
    private bool isSynchronizingSelection;
    private SampleLayout layout;
    private bool? lastNarrowLayout;
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
        NavigationSplitView.PropertyChanged += NavigationSplitViewPropertyChanged;
        SectionTabStrip.SelectionChanged += SectionSelectionChanged;
        PageTabStrip.SelectionChanged += PageSelectionChanged;
        PageTabStrip.Tapped += PageTabStripTapped;
        SizeChanged += SampleShellSizeChanged;

        sections = CreateSections();
        SectionTabStrip.ItemsSource = sections;

        if (sections.Length == 0)
        {
            return;
        }

        AttachedToVisualTree += SampleShellAttachedToVisualTree;
    }

    // Navigation comes from SampleCatalog; kept here so tests and callers have one entry point.
    internal static SampleShellSectionDescriptor[] CreateSections() => SampleCatalog.CreateSections();

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

    // Fallback for tapping a page TabStripItem that Avalonia had already auto-selected
    // (e.g. the sole page in a section): no value change means SelectionChanged never
    // fires, so PageSelectionChanged alone would silently swallow the click.
    private void PageTabStripTapped(object? sender, TappedEventArgs e)
    {
        if (isSynchronizingSelection || selectedSection is null)
        {
            return;
        }

        if (e.Source is not Control { DataContext: SampleShellPageDescriptor page } || ReferenceEquals(page, shownPage))
        {
            return;
        }

        selectedSection.SelectedPageIndex = FindPageIndex(selectedSection, page);
        ShowPage(selectedSection, page);
    }

    // Keep each page alive after first load so repeat tab switches only toggle visibility.
    /// <summary>Shows a page as if it had been picked in the navigation, and returns the hosted page control.</summary>
    internal Control Navigate(string sectionHeader, string pageHeader)
    {
        var section = sections.First(s => s.Header == sectionHeader);
        var page = section.Pages.First(p => p.Header == pageHeader);
        PreviewSection(section);
        section.SelectedPageIndex = FindPageIndex(section, page);
        ShowPage(section, page);
        SynchronizeNavigationSelection(section);
        return pageCache[page];
    }

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
        UpdateEmptyState();

        // Auto-close only while the pane is a modal overlay. On wide layouts it is
        // pinned inline beside the content, so navigating there must not dismiss the
        // navigation the user is still reading.
        if (layout.IsNarrow)
        {
            SetPaneOpen(false);
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

        UpdateEmptyState();
    }

    // Derive the empty state from what the host is actually showing rather than from a
    // second flag, so ShowPage and HideAllPages stay the only places that decide it.
    private void UpdateEmptyState()
    {
        var hasVisiblePage = false;

        foreach (var hostedPage in pageCache.Values)
        {
            if (hostedPage.IsVisible)
            {
                hasVisiblePage = true;
                break;
            }
        }

        PageEmptyState.IsVisible = !hasVisiblePage;
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

                // TabStrip re-selects the sole item once its container is realized
                // for a single-item source; clear again after that layout pass.
                if (section.Pages.Count == 1)
                {
                    Dispatcher.UIThread.Post(() => PageTabStrip.SelectedIndex = -1, DispatcherPriority.Loaded);
                }
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

    // Keep button chrome in sync however the pane closes, including Overlay light-dismiss
    // (tapping outside the pane), which flips IsPaneOpen without going through SetPaneOpen.
    private void NavigationSplitViewPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == SplitView.IsPaneOpenProperty)
        {
            UpdateNavigationChrome();
        }
    }

    // Switch between inline and overlay navigation so narrow screens keep the page readable.
    private void SampleShellSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateResponsiveLayout(e.NewSize.Width, e.NewSize.Height);
    }

    // Apply the current pane mode, widths, and shell spacing based on available width.
    private void UpdateResponsiveLayout(double width, double height)
    {
        layout = SampleLayout.For(width, height);
        var useNarrowLayout = layout.IsNarrow;
        NavigationSplitView.DisplayMode = useNarrowLayout ? SplitViewDisplayMode.Overlay : SplitViewDisplayMode.CompactInline;
        NavigationSplitView.OpenPaneLength = layout.PaneLength;
        ShellHeader.Padding = layout.HeaderPadding;
        PageContentChrome.Padding = layout.ContentPadding;
        RefreshPageLayoutClasses();

        // Pin the pane open on wide layouts, closed on narrow entry. Only touch
        // IsPaneOpen when the breakpoint actually crosses, so a manual toggle
        // inside the same band survives an unrelated resize event.
        if (lastNarrowLayout != useNarrowLayout)
        {
            SetPaneOpen(!useNarrowLayout);
        }

        lastNarrowLayout = useNarrowLayout;

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

    private void ApplyMobileDocsClass(Control control) => layout.ApplyTo(control);

    // Keep the shell buttons aligned with the current open/closed pane state.
    private void UpdateNavigationChrome()
    {
        var isPaneOpen = NavigationSplitView.IsPaneOpen;
        PaneToggleButton.IsVisible = !isPaneOpen;
        PaneCloseButton.IsVisible = isPaneOpen;
        ToolTip.SetTip(PaneToggleButton, layout.IsNarrow ? "Open navigation" : "Show navigation");
        ToolTip.SetTip(PaneCloseButton, layout.IsNarrow ? "Close navigation" : "Hide navigation");
    }

    // Chrome resync happens centrally in NavigationSplitViewPropertyChanged.
    private void SetPaneOpen(bool isOpen)
    {
        NavigationSplitView.IsPaneOpen = isOpen;
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
