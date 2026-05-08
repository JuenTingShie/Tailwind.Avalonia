using System;
using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample.Spacing;

public sealed record SpacingUtilityReferenceRow(string ClassName, string AxamlStyle);

internal sealed class SpacingUtilityReferenceTablePresenter
{
    private readonly ItemsControl itemsControl;
    private readonly Button toggleButton;
    private readonly SpacingUtilityReferenceRow[] allRows;
    private readonly int collapsedRowCount;
    private bool isExpanded;

    /// <summary>
    /// Initializes the presenter for a docs reference table and applies the initial collapsed state.
    /// </summary>
    public SpacingUtilityReferenceTablePresenter(
        ItemsControl itemsControl,
        Button toggleButton,
        SpacingUtilityReferenceRow[] allRows,
        int collapsedRowCount)
    {
        this.itemsControl = itemsControl;
        this.toggleButton = toggleButton;
        this.allRows = allRows;
        this.collapsedRowCount = collapsedRowCount;

        Apply();
    }

    /// <summary>
    /// Switches between collapsed and expanded row sets for the reference table.
    /// </summary>
    public void Toggle()
    {
        isExpanded = !isExpanded;
        Apply();
    }

    /// <summary>
    /// Pushes the visible rows, grid height, and toggle label into the view.
    /// </summary>
    private void Apply()
    {
        var visibleRowCount = isExpanded
            ? allRows.Length
            : Math.Min(collapsedRowCount, allRows.Length);

        var visibleRows = visibleRowCount == allRows.Length
            ? allRows
            : allRows[..visibleRowCount];

        itemsControl.ItemsSource = visibleRows;
        toggleButton.Content = isExpanded ? "CLOSE" : "SHOW MORE";
        toggleButton.IsVisible = allRows.Length > collapsedRowCount;
        toggleButton.IsEnabled = allRows.Length > collapsedRowCount;
    }
}