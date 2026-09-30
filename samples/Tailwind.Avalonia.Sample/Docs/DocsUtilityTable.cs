using System.Collections.Generic;
using System.Linq;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Metadata;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// Class-to-AXAML reference table with built-in row collapsing. Every docs page
/// used to carry its own presenter plus a click handler for this; the control
/// now owns both.
/// </summary>
public class DocsUtilityTable : TemplatedControl
{
    public static readonly StyledProperty<int> CollapsedRowCountProperty =
        AvaloniaProperty.Register<DocsUtilityTable, int>(nameof(CollapsedRowCount), 5);

    public static readonly DirectProperty<DocsUtilityTable, IReadOnlyList<UtilityReferenceRow>> VisibleRowsProperty =
        AvaloniaProperty.RegisterDirect<DocsUtilityTable, IReadOnlyList<UtilityReferenceRow>>(
            nameof(VisibleRows),
            o => o.VisibleRows);

    public static readonly DirectProperty<DocsUtilityTable, string> ToggleLabelProperty =
        AvaloniaProperty.RegisterDirect<DocsUtilityTable, string>(nameof(ToggleLabel), o => o.ToggleLabel);

    public static readonly DirectProperty<DocsUtilityTable, bool> CanToggleProperty =
        AvaloniaProperty.RegisterDirect<DocsUtilityTable, bool>(nameof(CanToggle), o => o.CanToggle);

    private IReadOnlyList<UtilityReferenceRow> visibleRows = [];
    private string toggleLabel = string.Empty;
    private bool canToggle;
    private bool isExpanded;
    private Button? toggleButton;

    static DocsUtilityTable()
    {
        CollapsedRowCountProperty.Changed.AddClassHandler<DocsUtilityTable>((table, _) => table.Refresh());
    }

    /// <summary>
    /// Initializes the table and re-slices it whenever rows are added in AXAML.
    /// </summary>
    public DocsUtilityTable()
    {
        Rows.CollectionChanged += (_, _) => Refresh();
    }

    /// <summary>
    /// Full row set for the utility family, declared as AXAML children.
    /// </summary>
    [Content]
    public AvaloniaList<UtilityReferenceRow> Rows { get; } = [];

    /// <summary>
    /// How many rows stay visible before the reader opts into the rest.
    /// </summary>
    public int CollapsedRowCount
    {
        get => GetValue(CollapsedRowCountProperty);
        set => SetValue(CollapsedRowCountProperty, value);
    }

    /// <summary>
    /// Rows the template currently renders.
    /// </summary>
    public IReadOnlyList<UtilityReferenceRow> VisibleRows
    {
        get => visibleRows;
        private set => SetAndRaise(VisibleRowsProperty, ref visibleRows, value);
    }

    /// <summary>
    /// Text on the expand/collapse control, phrased with the real row count.
    /// </summary>
    public string ToggleLabel
    {
        get => toggleLabel;
        private set => SetAndRaise(ToggleLabelProperty, ref toggleLabel, value);
    }

    /// <summary>
    /// False when every row already fits, which hides the toggle entirely.
    /// </summary>
    public bool CanToggle
    {
        get => canToggle;
        private set => SetAndRaise(CanToggleProperty, ref canToggle, value);
    }

    /// <summary>Below this width the class and its styles stack instead of sitting in two columns.</summary>
    public const double StackBelowWidth = 560;

    protected override Size MeasureOverride(Size availableSize)
    {
        var stacked = availableSize.Width < StackBelowWidth;
        PseudoClasses.Set(":stacked", stacked);
        var size = base.MeasureOverride(availableSize);

        // DockPanel.Dock does not take a style setter reliably inside the template, so the
        // class cell is docked here: left beside its styles, or on top of them when stacked.
        var dock = stacked ? Dock.Top : Dock.Left;
        var changed = false;

        foreach (var cell in this.GetVisualDescendants().OfType<TextBlock>())
        {
            if ((cell.Classes.Contains("docs-tableClass") || cell.Classes.Contains("docs-referenceClassHeading")) && DockPanel.GetDock(cell) != dock)
            {
                DockPanel.SetDock(cell, dock);
                cell.Margin = stacked ? new Thickness(0, 0, 0, 4) : new Thickness(0, 0, 24, 0);
                changed = true;
            }
        }

        return changed ? base.MeasureOverride(availableSize) : size;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (toggleButton is not null)
        {
            toggleButton.Click -= ToggleClicked;
        }

        toggleButton = e.NameScope.Find<Button>("PART_ToggleButton");

        if (toggleButton is not null)
        {
            toggleButton.Click += ToggleClicked;
        }

        Refresh();
    }

    private void ToggleClicked(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        isExpanded = !isExpanded;
        Refresh();
    }

    // Recompute the rendered slice, the toggle label, and whether a toggle is warranted.
    private void Refresh()
    {
        IReadOnlyList<UtilityReferenceRow> allRows = Rows;
        var collapsedCount = CollapsedRowCount < 0 ? 0 : CollapsedRowCount;
        var isCollapsible = allRows.Count > collapsedCount;

        CanToggle = isCollapsible;

        if (!isCollapsible)
        {
            isExpanded = false;
        }

        VisibleRows = isExpanded || !isCollapsible
            ? allRows
            : allRows.Take(collapsedCount).ToArray();

        ToggleLabel = isExpanded
            ? "Show fewer"
            : $"Show all {allRows.Count} classes";

        PseudoClasses.Set(":expanded", isExpanded);
    }
}
