using Avalonia;
using Avalonia.Controls;

namespace Tailwind.Avalonia;

public partial class Tw
{
    private const string GridColumnDefinitions = "Grid.ColumnDefinitions";
    private const string GridRowDefinitions = "Grid.RowDefinitions";

    // col-span-N / row-span-N / col-start-N / row-start-N set Grid's attached properties (1-based like CSS);
    // grid-cols-N / grid-rows-N create N equal star tracks on a Grid.
    private static bool TryParseGridUtility(string token, out KeywordAssignment[] assignments)
    {
        assignments = [];

        if (token.Contains(':') || token.Contains('('))
        {
            return false;
        }

        if (token == "col-span-full" || token == "row-span-full")
        {
            // Spans every track that exists on the grid; 1000 is clamped by Grid to the track count.
            assignments = [new KeywordAssignment(token.StartsWith("col", StringComparison.Ordinal) ? "Grid.ColumnSpan" : "Grid.RowSpan", 1000)];
            return true;
        }

        foreach (var (prefix, property, offset, min) in GridPrefixes)
        {
            if (!token.StartsWith(prefix, StringComparison.Ordinal))
            {
                continue;
            }

            var text = token[prefix.Length..];

            if (!int.TryParse(text, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var number) ||
                number < min || number > 1000)
            {
                return false;
            }

            assignments = [new KeywordAssignment(property, number + offset)];
            return true;
        }

        return false;
    }

    private static readonly (string Prefix, string Property, int Offset, int Min)[] GridPrefixes =
    [
        ("col-span-", "Grid.ColumnSpan", 0, 1),
        ("row-span-", "Grid.RowSpan", 0, 1),
        ("col-start-", "Grid.Column", -1, 1),
        ("row-start-", "Grid.Row", -1, 1),
        ("grid-cols-", GridColumnDefinitions, 0, 1),
        ("grid-rows-", GridRowDefinitions, 0, 1),
    ];

    private static bool TryApplyGridDefinitions(AvaloniaObject element, string propertyName, object value)
    {
        if (propertyName != GridColumnDefinitions && propertyName != GridRowDefinitions)
        {
            return false;
        }

        if (element is Grid grid && value is int count)
        {
            if (propertyName == GridColumnDefinitions)
            {
                grid.ColumnDefinitions.Clear();
                for (var i = 0; i < count; i++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
                }
            }
            else
            {
                grid.RowDefinitions.Clear();
                for (var i = 0; i < count; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
                }
            }
        }

        // Non-Grid elements ignore the token silently, consistent with other layout-only utilities.
        return true;
    }

    private static bool ClearGridDefinitions(AvaloniaObject element, string propertyName)
    {
        if (propertyName != GridColumnDefinitions && propertyName != GridRowDefinitions)
        {
            return false;
        }

        if (element is Grid grid)
        {
            (propertyName == GridColumnDefinitions ? (System.Collections.IList)grid.ColumnDefinitions : grid.RowDefinitions).Clear();
        }

        return true;
    }
}
