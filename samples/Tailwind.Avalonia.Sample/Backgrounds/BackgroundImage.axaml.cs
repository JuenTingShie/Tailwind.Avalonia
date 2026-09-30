using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Backgrounds;

public partial class BackgroundImage : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("bg-[url(avares://...)]", "ImageBrush.Source (avares:// resources only)"),
        new("bg-cover", "Stretch: UniformToFill"),
        new("bg-contain", "Stretch: Uniform"),
        new("bg-auto", "Stretch: None (natural size)"),
        new("bg-repeat", "TileMode: Tile at natural size (default)"),
        new("bg-no-repeat", "TileMode: None"),
        new("bg-center / top / bottom / left / right", "AlignmentX / AlignmentY"),
        new("bg-top-left / top-right / bottom-left / bottom-right", "AlignmentX + AlignmentY"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public BackgroundImage()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
