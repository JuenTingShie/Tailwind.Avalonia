namespace Tailwind.Avalonia.Sample.Docs;

/// <summary>
/// One row of a docs utility reference table: the Tailwind class and the AXAML it expands to.
/// Declared in AXAML as a child of <see cref="DocsUtilityTable"/>.
/// </summary>
public sealed class UtilityReferenceRow
{
    public UtilityReferenceRow()
    {
    }

    public UtilityReferenceRow(string className, string axamlStyle)
    {
        ClassName = className;
        AxamlStyle = axamlStyle;
    }

    /// <summary>The utility class, with placeholders such as &lt;number&gt; or [&lt;value&gt;].</summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>The AXAML the class is equivalent to.</summary>
    public string AxamlStyle { get; set; } = string.Empty;
}
