using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Layout;

public partial class ObjectFit : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("object-contain", "Stretch: Uniform"),
        new("object-cover", "Stretch: UniformToFill"),
        new("object-fill", "Stretch: Fill"),
        new("object-none", "Stretch: None"),
        new("object-scale-down", "Stretch: Uniform, StretchDirection: DownOnly"),
    ];

    /// <summary>
    /// Initializes the docs page and seeds the utility reference table.
    /// </summary>
    public ObjectFit()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
