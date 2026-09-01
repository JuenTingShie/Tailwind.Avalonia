using Avalonia.Controls;

using Tailwind.Avalonia.Sample.Docs;

namespace Tailwind.Avalonia.Sample.Borders;

public partial class Radius : UserControl
{
    private static readonly UtilityReferenceRow[] UtilityRows =
    [
        new("rounded", "<Border CornerRadius=\"4\" />"),
        new("rounded-none", "<Border CornerRadius=\"0\" />"),
        new("rounded-xs", "<Border CornerRadius=\"2\" />"),
        new("rounded-sm", "<Border CornerRadius=\"4\" />"),
        new("rounded-md", "<Border CornerRadius=\"6\" />"),
        new("rounded-lg", "<Border CornerRadius=\"8\" />"),
        new("rounded-xl", "<Border CornerRadius=\"12\" />"),
        new("rounded-2xl", "<Border CornerRadius=\"16\" />"),
        new("rounded-3xl", "<Border CornerRadius=\"24\" />"),
        new("rounded-4xl", "<Border CornerRadius=\"32\" />"),
        new("rounded-full", "<Border CornerRadius=\"9999\" />"),
        new("rounded-[<value>]", "<Border CornerRadius=\"<parsed value>\" />"),
        new("rounded-t-<name>", "<Border CornerRadius=\"<v>,<v>,0,0\" />"),
        new("rounded-r-<name>", "<Border CornerRadius=\"0,<v>,<v>,0\" />"),
        new("rounded-b-<name>", "<Border CornerRadius=\"0,0,<v>,<v>\" />"),
        new("rounded-l-<name>", "<Border CornerRadius=\"<v>,0,0,<v>\" />"),
        new("rounded-tl-<name>", "<Border CornerRadius=\"<v>,0,0,0\" />"),
        new("rounded-tr-<name>", "<Border CornerRadius=\"0,<v>,0,0\" />"),
        new("rounded-br-<name>", "<Border CornerRadius=\"0,0,<v>,0\" />"),
        new("rounded-bl-<name>", "<Border CornerRadius=\"0,0,0,<v>\" />"),
    ];

    /// <summary>
    /// Initializes the border-radius docs page and seeds the utility reference table.
    /// </summary>
    public Radius()
    {
        InitializeComponent();
        UtilityTable.Rows = UtilityRows;
    }
}
