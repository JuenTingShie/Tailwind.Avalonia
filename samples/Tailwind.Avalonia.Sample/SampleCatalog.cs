using System;
using System.Collections.Generic;
using System.Linq;

using Avalonia.Controls;

namespace Tailwind.Avalonia.Sample;

/// <summary>
/// Every docs page in navigation order. To add a page, add one line here: sections appear in the order their
/// first page is listed, and pages keep their order within a section.
/// </summary>
internal static class SampleCatalog
{
    public static readonly IReadOnlyList<SamplePage> Pages =
    [
        Page("Spacing",       "Padding",                   static () => new Spacing.Padding()),
        Page("Spacing",       "Margin",                    static () => new Spacing.Margin()),
        Page("Sizing",        "Width",                     static () => new Sizing.Width()),
        Page("Sizing",        "Height",                    static () => new Sizing.Height()),
        Page("Sizing",        "Size keywords",             static () => new Sizing.SizeKeywords()),
        Page("Backgrounds",   "Gradients",                 static () => new Backgrounds.Gradients()),
        Page("Backgrounds",   "Background image",          static () => new Backgrounds.BackgroundImage()),
        Page("Borders",       "Radius",                    static () => new Borders.Radius()),
        Page("Borders",       "Width",                     static () => new Borders.Width()),
        Page("Typography",    "Font size",                 static () => new Typography.FontSize()),
        Page("Typography",    "Font weight",               static () => new Typography.FontWeight()),
        Page("Typography",    "Font style",                static () => new Typography.FontStyle()),
        Page("Typography",    "Font family",               static () => new Typography.FontFamily()),
        Page("Typography",    "Font variant numeric",      static () => new Typography.FontVariantNumeric()),
        Page("Typography",    "Font stretch",              static () => new Typography.FontStretch()),
        Page("Typography",    "Text decoration",           static () => new Typography.TextDecoration()),
        Page("Typography",    "Line clamp",                static () => new Typography.LineClamp()),
        Page("Typography",    "Text overflow",             static () => new Typography.TextOverflow()),
        Page("Typography",    "Tracking and leading",      static () => new Typography.LetterSpacingLineHeight()),
        Page("Typography",    "Text align",                static () => new Typography.TextAlign()),
        Page("Typography",    "Colors",                    static () => new Typography.ColorUtilities()),
        Page("Layout",        "Display",                   static () => new Layout.Display()),
        Page("Layout",        "Overflow",                  static () => new Layout.Overflow()),
        Page("Layout",        "Scroll snap",               static () => new Layout.ScrollSnap()),
        Page("Layout",        "Self alignment",            static () => new Layout.SelfAlignment()),
        Page("Layout",        "Direction and gap",         static () => new Layout.FlexDirectionGap()),
        Page("Layout",        "Position",                  static () => new Layout.Position()),
        Page("Layout",        "Grid",                      static () => new Layout.Grid()),
        Page("Layout",        "Object fit",                static () => new Layout.ObjectFit()),
        Page("Layout",        "Z-index",                   static () => new Layout.ZIndex()),
        Page("Transforms",    "Rotate",                    static () => new Transforms.Rotate()),
        Page("Transforms",    "Scale",                     static () => new Transforms.Scale()),
        Page("Transforms",    "Translate",                 static () => new Transforms.Translate()),
        Page("Transforms",    "Skew",                      static () => new Transforms.Skew()),
        Page("Interactivity", "Pseudo-class variants",     static () => new Interactivity.PseudoClassVariants()),
        Page("Interactivity", "More variants",             static () => new Interactivity.MoreVariants()),
        Page("Interactivity", "Structural variants",       static () => new Interactivity.StructuralVariants()),
        Page("Interactivity", "Cursor",                    static () => new Interactivity.Cursor()),
        Page("Interactivity", "Pointer events",            static () => new Interactivity.PointerEvents()),
        Page("Interactivity", "Caret and selection color", static () => new Interactivity.CaretSelectionColor()),
        Page("Interactivity", "Placeholder color",         static () => new Interactivity.PlaceholderColor()),
        Page("Interactivity", "Color scheme",              static () => new Interactivity.ColorScheme()),
        Page("Effects",       "Opacity",                   static () => new Effects.Opacity()),
        Page("Effects",       "Box shadow",                static () => new Effects.BoxShadow()),
        Page("Effects",       "Ring",                      static () => new Effects.Ring()),
        Page("Effects",       "Blur",                      static () => new Effects.Blur()),
        Page("Effects",       "Drop shadow",               static () => new Effects.DropShadow()),
        Page("Effects",       "Transitions",               static () => new Effects.Transitions()),
        Page("Effects",       "Animation",                 static () => new Effects.Animation()),
        Page("SVG",           "Fill and stroke",           static () => new Svg.FillStroke()),
    ];

    /// <summary>Groups <see cref="Pages"/> into navigation sections.</summary>
    public static SampleShellSectionDescriptor[] CreateSections() =>
        Pages
            .GroupBy(page => page.Section)
            .Select(group => new SampleShellSectionDescriptor(
                group.Key,
                group.Select(page => new SampleShellPageDescriptor(page.Title, page.Create)).ToArray()))
            .ToArray();

    private static SamplePage Page(string section, string title, Func<Control> create) => new(section, title, create);
}

/// <param name="Section">Navigation section, in sentence case like the page eyebrow.</param>
/// <param name="Title">Page title shown in the navigation list.</param>
/// <param name="Create">Builds the page; called only when the page is first visited.</param>
internal sealed record SamplePage(string Section, string Title, Func<Control> Create);
