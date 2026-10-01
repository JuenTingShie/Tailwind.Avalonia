using System.Linq;
using System;
using System.Collections.Generic;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Automation;
using Avalonia.Layout;
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

    /// <summary>
    /// Initializes the sample shell and shows the first page.
    /// </summary>
    public SampleShell()
    {
        InitializeComponent();

        // Wire events in code-behind so the designer runtime compiler doesn't need
        // to resolve string-based event handlers from XAML for this shared shell.
        PaneCloseButton.Click += NavigationToggleClicked;
        PaneToggleButton.Click += NavigationToggleClicked;
        NavigationSplitView.PropertyChanged += NavigationSplitViewPropertyChanged;
        NavigationList.SelectionChanged += NavigationSelectionChanged;
        NavigationList.ContainerPrepared += NavigationContainerPrepared;
        NavigationSearch.TextChanged += (_, _) => RefreshNavigationItems();
        NavigationSearch.KeyDown += NavigationSearchKeyDown;
        SizeChanged += SampleShellSizeChanged;

        sections = CreateSections();
        RefreshNavigationItems();

        if (sections.Length == 0)
        {
            return;
        }

        AttachedToVisualTree += SampleShellAttachedToVisualTree;
    }

    // Navigation comes from SampleCatalog; kept here so tests and callers have one entry point.
    internal static SampleShellSectionDescriptor[] CreateSections() => SampleCatalog.CreateSections();

    /// <summary>Every page in navigation order, used for the previous / next links.</summary>
    private IEnumerable<(SampleShellSectionDescriptor Section, SampleShellPageDescriptor Page)> AllPages() =>
        sections.SelectMany(section => section.Pages.Select(page => (section, page)));

    // One flat list: a heading item per section followed by its pages, filtered by the search text
    // (a section name match keeps all of its pages).
    private void RefreshNavigationItems()
    {
        var query = NavigationSearch.Text?.Trim() ?? string.Empty;
        var items = new List<object>();

        foreach (var section in sections)
        {
            var sectionMatches = query.Length == 0 || section.Header.Contains(query, StringComparison.OrdinalIgnoreCase);
            var pages = section.Pages.Where(page => sectionMatches || page.Header.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            if (pages.Count == 0)
            {
                continue;
            }

            items.Add(new SampleNavigationHeading(section.Header));
            items.AddRange(pages);
        }

        if (items.Count == 0)
        {
            items.Add(new SampleNavigationHeading("No matching pages"));
        }

        isSynchronizingSelection = true;

        try
        {
            NavigationList.ItemsSource = items;
            NavigationList.SelectedItem = shownPage is not null && items.Contains(shownPage) ? shownPage : null;
        }
        finally
        {
            isSynchronizingSelection = false;
        }
    }

    private static void NavigationContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        var isHeading = e.Container.DataContext is SampleNavigationHeading;
        e.Container.Classes.Set("nav-header", isHeading);
    }

    private void NavigationSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (isSynchronizingSelection || NavigationList.SelectedItem is not SampleShellPageDescriptor page)
        {
            return;
        }

        var section = sections.First(s => s.Pages.Contains(page));
        ShowPage(section, page);
    }

    // Enter in the search box opens the first match; Escape clears the search.
    private void NavigationSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && NavigationList.ItemsSource?.OfType<SampleShellPageDescriptor>().FirstOrDefault() is { } first)
        {
            NavigationList.SelectedItem = first;
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            NavigationSearch.Text = string.Empty;
            e.Handled = true;
        }
    }

    // Ctrl+K (Cmd+K) jumps to the page search from anywhere in the shell.
    private TopLevel? shortcutHost;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        // Listen on the TopLevel so Ctrl+K also works when nothing inside the shell has focus.
        shortcutHost = TopLevel.GetTopLevel(this);
        shortcutHost?.AddHandler(KeyDownEvent, ShellKeyDown, RoutingStrategies.Tunnel);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        shortcutHost?.RemoveHandler(KeyDownEvent, ShellKeyDown);
        shortcutHost = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void ShellKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.K && (e.KeyModifiers.HasFlag(KeyModifiers.Control) || e.KeyModifiers.HasFlag(KeyModifiers.Meta)))
        {
            SetPaneOpen(true);
            NavigationSearch.Focus();
            NavigationSearch.SelectAll();
            e.Handled = true;
        }
    }

    /// <summary>Shows a page as if it had been picked in the navigation, and returns the hosted page control.</summary>
    internal Control Navigate(string sectionHeader, string pageHeader)
    {
        var section = sections.First(s => s.Header == sectionHeader);
        var page = section.Pages.First(p => p.Header == pageHeader);
        ShowPage(section, page);
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

        isSynchronizingSelection = true;

        try
        {
            NavigationList.SelectedItem = NavigationList.ItemsSource?.OfType<object>().Contains(page) == true ? page : null;
        }
        finally
        {
            isSynchronizingSelection = false;
        }

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
        AttachPager(createdPage, page);
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
        Classes.Set(SampleLayout.NarrowShellClass, layout.IsNarrow);
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

    // Previous / next links under every page, in navigation order, so a reader can walk the docs without
    // reopening the navigation.
    private void AttachPager(Control pageView, SampleShellPageDescriptor page)
    {
        if (pageView is not ContentControl { Content: Docs.DocsPage docsPage })
        {
            return;
        }

        var all = AllPages().ToList();
        var index = all.FindIndex(entry => ReferenceEquals(entry.Page, page));
        var pager = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 12 };

        if (index > 0)
        {
            pager.Children.Add(PagerLink(all[index - 1], "Previous", HorizontalAlignment.Left, 0));
        }

        if (index >= 0 && index < all.Count - 1)
        {
            pager.Children.Add(PagerLink(all[index + 1], "Next", HorizontalAlignment.Right, 1));
        }

        docsPage.Footer = pager;
    }

    private Button PagerLink((SampleShellSectionDescriptor Section, SampleShellPageDescriptor Page) target, string label, HorizontalAlignment alignment, int column)
    {
        var button = new Button
        {
            Classes = { "docs-pagerLink" },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = alignment,
            Content = new StackPanel
            {
                Spacing = 2,
                Children =
                {
                    new TextBlock { Classes = { "docs-pagerLabel" }, Text = label, HorizontalAlignment = alignment },
                    new TextBlock { Classes = { "docs-pagerTitle" }, Text = target.Page.Header, HorizontalAlignment = alignment },
                },
            },
        };
        AutomationProperties.SetName(button, $"{label}: {target.Page.Header}");
        button.Click += (_, _) =>
        {
            ShowPage(target.Section, target.Page);

            if (pageCache[target.Page] is ContentControl { Content: Docs.DocsPage nextPage })
            {
                nextPage.ScrollToTop();
            }
        };
        Grid.SetColumn(button, column);
        return button;
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

        if (initialSection.Pages.Count > 0)
        {
            ShowPage(initialSection, initialSection.Pages[0]);
        }
    }
}

internal sealed record SampleNavigationHeading(string Header);

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
