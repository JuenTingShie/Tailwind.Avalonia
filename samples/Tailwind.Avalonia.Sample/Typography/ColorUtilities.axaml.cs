using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample.Typography;

public partial class ColorUtilities : UserControl
{
    // These are C# string literals bound straight to TextBlock.Text, so the
    // placeholder brackets are written literally - XML entities would render
    // as "&lt;color&gt;" on screen.

    /// <summary>
    /// Initializes the color docs page.
    /// </summary>
    public ColorUtilities()
    {
        InitializeComponent();
    }
}
