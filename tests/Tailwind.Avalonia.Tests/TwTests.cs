using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace Tailwind.Avalonia.Tests;

public class TwTests
{
    [Fact]
    public void SetClass_Applies_Physical_Spacing_Utilities()
    {
        var border = new Border();

        Tw.SetClass(border, "p-4 px-2 pt-3 m-1 mx-2");

        Assert.Equal(new Thickness(8, 12, 8, 16), border.Padding);
        Assert.Equal(new Thickness(8, 4, 8, 4), border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Logical_Spacing_For_RightToLeft()
    {
        var border = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(border, "ps-6 pe-2 py-4");

        Assert.Equal(new Thickness(8, 16, 24, 16), border.Padding);
    }

    [Fact]
    public void SetClass_Reapplies_Logical_Spacing_When_FlowDirection_Changes()
    {
        var border = new Border();

        Tw.SetClass(border, "ps-6 pe-2 py-4");

        Assert.Equal(new Thickness(24, 16, 8, 16), border.Padding);

        border.FlowDirection = FlowDirection.RightToLeft;

        Assert.Equal(new Thickness(8, 16, 24, 16), border.Padding);
    }

    [Fact]
    public void SetClass_Arranges_Logical_Padding_On_Expected_Rtl_Side()
    {
        var psChild = new Canvas();
        var psBorder = new Border
        {
            Child = psChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(psBorder, "ps-6");
        psBorder.Measure(new Size(100, 20));
        psBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(0, 0, 76, 20), psChild.Bounds);

        var peChild = new Canvas();
        var peBorder = new Border
        {
            Child = peChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(peBorder, "pe-2");
        peBorder.Measure(new Size(100, 20));
        peBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(8, 0, 92, 20), peChild.Bounds);
    }

    [Fact]
    public void SetClass_Uses_Physical_Left_For_Visual_Start_Padding()
    {
        var ltrChild = new Canvas();
        var ltrBorder = new Border
        {
            Child = ltrChild,
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlChild = new Canvas();
        var rtlBorder = new Border
        {
            Child = rtlChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "psv-6");
        Tw.SetClass(rtlBorder, "psv-6");

        Assert.Equal(new Thickness(24, 0, 0, 0), ltrBorder.Padding);
        Assert.Equal(new Thickness(24, 0, 0, 0), rtlBorder.Padding);

        ltrBorder.Measure(new Size(100, 20));
        ltrBorder.Arrange(new Rect(0, 0, 100, 20));
        rtlBorder.Measure(new Size(100, 20));
        rtlBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(24, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(24, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Uses_Physical_Right_For_Visual_End_Padding()
    {
        var ltrChild = new Canvas();
        var ltrBorder = new Border
        {
            Child = ltrChild,
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlChild = new Canvas();
        var rtlBorder = new Border
        {
            Child = rtlChild,
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "pev-6");
        Tw.SetClass(rtlBorder, "pev-6");

        Assert.Equal(new Thickness(0, 0, 24, 0), ltrBorder.Padding);
        Assert.Equal(new Thickness(0, 0, 24, 0), rtlBorder.Padding);

        ltrBorder.Measure(new Size(100, 20));
        ltrBorder.Arrange(new Rect(0, 0, 100, 20));
        rtlBorder.Measure(new Size(100, 20));
        rtlBorder.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(0, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(0, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Applies_Block_Padding_Utilities_Independent_Of_FlowDirection()
    {
        var ltrBorder = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlBorder = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "pbs-6 pbe-2");
        Tw.SetClass(rtlBorder, "pbs-6 pbe-2");

        Assert.Equal(new Thickness(0, 24, 0, 8), ltrBorder.Padding);
        Assert.Equal(new Thickness(0, 24, 0, 8), rtlBorder.Padding);
    }

    [Fact]
    public void SetClass_Uses_Physical_Left_For_Visual_Start_Margin()
    {
        var ltrChild = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };
        var ltrHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { ltrChild },
        };

        var rtlChild = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };
        var rtlHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { rtlChild },
        };

        Tw.SetClass(ltrChild, "msv-6");
        Tw.SetClass(rtlChild, "msv-6");

        Assert.Equal(new Thickness(24, 0, 0, 0), ltrChild.Margin);
        Assert.Equal(new Thickness(24, 0, 0, 0), rtlChild.Margin);

        ltrHost.Measure(new Size(100, 20));
        ltrHost.Arrange(new Rect(0, 0, 100, 20));
        rtlHost.Measure(new Size(100, 20));
        rtlHost.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(24, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(24, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Uses_Physical_Right_For_Visual_End_Margin()
    {
        var ltrChild = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };
        var ltrHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { ltrChild },
        };

        var rtlChild = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };
        var rtlHost = new Grid
        {
            Width = 100,
            Height = 20,
            Children = { rtlChild },
        };

        Tw.SetClass(ltrChild, "mev-6");
        Tw.SetClass(rtlChild, "mev-6");

        Assert.Equal(new Thickness(0, 0, 24, 0), ltrChild.Margin);
        Assert.Equal(new Thickness(0, 0, 24, 0), rtlChild.Margin);

        ltrHost.Measure(new Size(100, 20));
        ltrHost.Arrange(new Rect(0, 0, 100, 20));
        rtlHost.Measure(new Size(100, 20));
        rtlHost.Arrange(new Rect(0, 0, 100, 20));

        Assert.Equal(new Rect(0, 0, 76, 20), ltrChild.Bounds);
        Assert.Equal(new Rect(0, 0, 76, 20), rtlChild.Bounds);
    }

    [Fact]
    public void SetClass_Applies_Block_Margin_Utilities_Independent_Of_FlowDirection()
    {
        var ltrBorder = new Border
        {
            FlowDirection = FlowDirection.LeftToRight,
        };

        var rtlBorder = new Border
        {
            FlowDirection = FlowDirection.RightToLeft,
        };

        Tw.SetClass(ltrBorder, "mbs-6 mbe-2");
        Tw.SetClass(rtlBorder, "mbs-6 mbe-2");

        Assert.Equal(new Thickness(0, 24, 0, 8), ltrBorder.Margin);
        Assert.Equal(new Thickness(0, 24, 0, 8), rtlBorder.Margin);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Spacing_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "p-4 m-2");
        Tw.SetClass(border, null);

        Assert.Equal(default, border.Padding);
        Assert.Equal(default, border.Margin);
    }

    [Fact]
    public void SetClass_Applies_Color_Utilities()
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));
        Assert.True(TailwindColorPalette.TryGetColor("green-800", out var green800));
        Assert.True(TailwindColorPalette.TryGetColor("orange-800", out var orange800));

        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(textBlock, "text-orange-800");

        border.ApplyStyling();
        textBlock.ApplyStyling();

        Assert.Equal(blue700, Assert.IsType<SolidColorBrush>(border.Background).Color);
        Assert.Equal(green800, Assert.IsType<SolidColorBrush>(border.BorderBrush).Color);
        Assert.Equal(orange800, Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }

    [Fact]
    public void SetClass_Does_Not_Leak_Style_To_Descendants_Of_Same_Type()
    {
        Assert.True(TailwindColorPalette.TryGetColor("red-500", out var red500));

        var outer = new Border();
        var inner = new Border();
        outer.Child = inner;

        Tw.SetClass(outer, "bg-red-500");

        outer.ApplyStyling();
        inner.ApplyStyling();

        Assert.Equal(red500, Assert.IsType<SolidColorBrush>(outer.Background).Color);
        Assert.Null(inner.Background);
    }

    [Fact]
    public void SetClass_Applies_Sizing_Utilities()
    {
        var border = new Border();

        Tw.SetClass(border, "w-24 min-w-16 max-w-32 h-12 min-h-8 max-h-20");

        Assert.Equal(96d, border.Width);
        Assert.Equal(64d, border.MinWidth);
        Assert.Equal(128d, border.MaxWidth);
        Assert.Equal(48d, border.Height);
        Assert.Equal(32d, border.MinHeight);
        Assert.Equal(80d, border.MaxHeight);
    }

    [Fact]
    public void SetClass_Applies_Opacity_Utility()
    {
        var border = new Border();

        Tw.SetClass(border, "opacity-50");
        border.ApplyStyling();

        Assert.Equal(0.5d, border.Opacity);
    }

    [Fact]
    public void SetClass_Uses_Last_Opacity_Utility()
    {
        var border = new Border();

        Tw.SetClass(border, "opacity-80 opacity-20");
        border.ApplyStyling();

        Assert.Equal(0.2d, border.Opacity);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Opacity_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "opacity-50");
        Tw.SetClass(border, null);
        border.ApplyStyling();

        Assert.Equal(1d, border.Opacity);
    }

    [Fact]
    public void SetClass_Uses_Last_Sizing_Utility_For_Each_Property()
    {
        var border = new Border();

        Tw.SetClass(border, "w-16 w-24 min-w-8 min-w-12 max-w-40 max-w-32 h-10 h-12 min-h-4 min-h-6 max-h-24 max-h-20");

        Assert.Equal(96d, border.Width);
        Assert.Equal(48d, border.MinWidth);
        Assert.Equal(128d, border.MaxWidth);
        Assert.Equal(48d, border.Height);
        Assert.Equal(24d, border.MinHeight);
        Assert.Equal(80d, border.MaxHeight);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Sizing_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "w-24 min-w-16 max-w-32 h-12 min-h-8 max-h-20");
        Tw.SetClass(border, null);

        Assert.True(double.IsNaN(border.Width));
        Assert.Equal(0d, border.MinWidth);
        Assert.True(double.IsPositiveInfinity(border.MaxWidth));
        Assert.True(double.IsNaN(border.Height));
        Assert.Equal(0d, border.MinHeight);
        Assert.True(double.IsPositiveInfinity(border.MaxHeight));
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Color_Utilities_When_Class_Removed()
    {
        var border = new Border();

        Tw.SetClass(border, "bg-blue-700 border-green-800");
        Tw.SetClass(border, null);

        border.ApplyStyling();

        Assert.Null(border.Background);
        Assert.Null(border.BorderBrush);
    }

    [Fact]
    public void SetClass_Applies_Transparent_And_Opacity_Color_Utilities()
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));
        Assert.True(TailwindColorPalette.TryGetColor("orange-800", out var orange800));

        var border = new Border();
        var textBlock = new TextBlock();

        Tw.SetClass(border, "bg-blue-700/50 border-transparent");
        Tw.SetClass(textBlock, "text-orange-800/25");

        border.ApplyStyling();
        textBlock.ApplyStyling();

        var background = Assert.IsType<SolidColorBrush>(border.Background).Color;
        var borderBrush = Assert.IsType<SolidColorBrush>(border.BorderBrush).Color;
        var foreground = Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color;

        Assert.Equal((byte)128, background.A);
        Assert.Equal(blue700.R, background.R);
        Assert.Equal(blue700.G, background.G);
        Assert.Equal(blue700.B, background.B);
        Assert.Equal(Colors.Transparent, borderBrush);
        Assert.Equal((byte)64, foreground.A);
        Assert.Equal(orange800.R, foreground.R);
        Assert.Equal(orange800.G, foreground.G);
        Assert.Equal(orange800.B, foreground.B);
    }

    [Theory]
    [InlineData("bg-red-500/NaN")]
    [InlineData("bg-red-500/Infinity")]
    public void SetClass_Ignores_Color_Utilities_With_NonFinite_Opacity(string className)
    {
        var border = new Border();

        Tw.SetClass(border, className);

        Assert.Null(border.Background);
    }

    [Fact]
    public void SetClass_Applies_Font_Size_Utilities_And_Preserves_Text_Color_Parsing()
    {
        Assert.True(TailwindColorPalette.TryGetColor("sky-300", out var sky300));
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-base text-sky-300");

        textBlock.ApplyStyling();

        Assert.Equal(16d, textBlock.FontSize);
        Assert.Equal(sky300, Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }

    [Fact]
    public void SetClass_Applies_Font_Size_Without_Changing_Foreground_When_No_Color_Utility_Is_Present()
    {
        var textBlock = new TextBlock();
        var originalForeground = textBlock.Foreground;

        Tw.SetClass(textBlock, "text-4xl");

        Assert.Equal(36d, textBlock.FontSize);
        Assert.Equal(originalForeground, textBlock.Foreground);
    }

    [Fact]
    public void SetClass_Uses_Last_Font_Size_Utility()
    {
        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-sm text-2xl text-lg");

        Assert.Equal(18d, textBlock.FontSize);
    }

    [Fact]
    public void SetClass_Clears_Previously_Applied_Font_Size_When_Class_Removed()
    {
        var textBlock = new TextBlock();
        var defaultFontSize = textBlock.FontSize;

        Tw.SetClass(textBlock, "text-7xl");
        Tw.SetClass(textBlock, null);

        Assert.Equal(defaultFontSize, textBlock.FontSize);
    }

    [Fact]
    public void SetClass_Logs_Warning_For_Unrecognized_Token()
    {
        var border = new Border();
        var sink = new CapturingLogSink(border);
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            Tw.SetClass(border, "not-a-real-utility");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("not-a-real-utility", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    [Fact]
    public void SetClass_Logs_Warning_For_Unsupported_Border_Width_Token()
    {
        var border = new Border();
        var sink = new CapturingLogSink(border);
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            Tw.SetClass(border, "border-2");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("border-2", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    [Fact]
    public void SetClass_Logs_Warning_When_Padding_Property_Is_Missing()
    {
        var rectangle = new Rectangle();
        var sink = new CapturingLogSink(rectangle);
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            Tw.SetClass(rectangle, "p-4");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("Padding", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    [Fact]
    public void SetClass_Logs_Warning_When_FontSize_Property_Is_Missing()
    {
        var rectangle = new Rectangle();
        var sink = new CapturingLogSink(rectangle);
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            Tw.SetClass(rectangle, "text-lg");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("FontSize", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    [Fact]
    public void SetClass_Logs_Warning_When_Background_Property_Is_Missing()
    {
        var rectangle = new Rectangle();
        var sink = new CapturingLogSink(rectangle);
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            Tw.SetClass(rectangle, "bg-red-500");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("Background", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    [Fact]
    public void SetClass_Applies_Hover_Variant_For_Background()
    {
        Assert.True(TailwindColorPalette.TryGetColor("blue-500", out var blue500));
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));

        var border = new Border();

        Tw.SetClass(border, "bg-blue-500 hover:bg-blue-700");

        border.ApplyStyling();
        Assert.Equal(blue500, Assert.IsType<SolidColorBrush>(border.Background).Color);

        ((IPseudoClasses)border.Classes).Add(":pointerover");
        border.ApplyStyling();
        Assert.Equal(blue700, Assert.IsType<SolidColorBrush>(border.Background).Color);

        ((IPseudoClasses)border.Classes).Remove(":pointerover");
        border.ApplyStyling();
        Assert.Equal(blue500, Assert.IsType<SolidColorBrush>(border.Background).Color);
    }

    [Fact]
    public void SetClass_Applies_Pressed_Variant_For_Opacity()
    {
        var border = new Border();

        Tw.SetClass(border, "opacity-100 pressed:opacity-50");

        border.ApplyStyling();
        Assert.Equal(1d, border.Opacity);

        ((IPseudoClasses)border.Classes).Add(":pressed");
        border.ApplyStyling();
        Assert.Equal(0.5d, border.Opacity);
    }

    [Fact]
    public void SetClass_Applies_Focus_Variant_For_Foreground()
    {
        Assert.True(TailwindColorPalette.TryGetColor("sky-500", out var sky500));

        var textBlock = new TextBlock();

        Tw.SetClass(textBlock, "text-gray-500 focus:text-sky-500");

        ((IPseudoClasses)textBlock.Classes).Add(":focus");
        textBlock.ApplyStyling();

        Assert.Equal(sky500, Assert.IsType<SolidColorBrush>(textBlock.Foreground).Color);
    }

    [Fact]
    public void SetClass_Prefers_Later_Declared_Variant_When_Multiple_PseudoClasses_Are_Active()
    {
        Assert.True(TailwindColorPalette.TryGetColor("green-500", out var green500));

        var border = new Border();

        Tw.SetClass(border, "bg-blue-500 hover:bg-red-500 pressed:bg-green-500");

        ((IPseudoClasses)border.Classes).Add(":pointerover");
        ((IPseudoClasses)border.Classes).Add(":pressed");
        border.ApplyStyling();

        // Pressed is declared after Hover in VariantKind, so its Style is added
        // later and wins while both pseudo-classes are simultaneously active.
        Assert.Equal(green500, Assert.IsType<SolidColorBrush>(border.Background).Color);
    }

    [Fact]
    public void SetClass_Applies_Hover_Variant_On_Templated_ContentPresenter_Part()
    {
        // Reproduces FluentTheme's real Button ControlTheme: it sets Background on the
        // template's ContentPresenter#PART_ContentPresenter directly for :pointerover,
        // rather than relying on the TemplateBinding from Button.Background. A variant
        // style that only targets Button.Background never has any visible effect,
        // because the theme's part-level Setter always wins over the TemplateBinding.
        Assert.True(TailwindColorPalette.TryGetColor("blue-700", out var blue700));

        var button = new Button
        {
            Template = new FuncControlTemplate<Button>((owner, scope) =>
            {
                var presenter = new ContentPresenter { Name = "PART_ContentPresenter" };
                presenter.Bind(ContentPresenter.BackgroundProperty, new TemplateBinding(Button.BackgroundProperty));
                return presenter.RegisterInNameScope(scope);
            }),
        };

        // Stand-in for FluentTheme's own `^:pointerover /template/ ContentPresenter#PART_ContentPresenter` style.
        button.Styles.Add(new Style(x => x.OfType<Button>().Class(":pointerover").Template().OfType<ContentPresenter>().Name("PART_ContentPresenter"))
        {
            Setters = { new Setter(ContentPresenter.BackgroundProperty, Brushes.Gray) },
        });

        Tw.SetClass(button, "bg-blue-500 hover:bg-blue-700");

        button.ApplyTemplate();
        var contentPresenter = Assert.IsType<ContentPresenter>(button.Presenter);

        ((IPseudoClasses)button.Classes).Add(":pointerover");
        button.ApplyStyling();
        contentPresenter.ApplyStyling();

        Assert.Equal(blue700, Assert.IsAssignableFrom<ISolidColorBrush>(contentPresenter.Background).Color);
    }

    [Fact]
    public void SetClass_Applies_Focus_Variant_On_Templated_BorderElement_Part()
    {
        // Reproduces FluentTheme's real TextBox ControlTheme: it sets BorderBrush on the
        // template's Border#PART_BorderElement directly for :focus, rather than relying on
        // the TemplateBinding from TextBox.BorderBrush.
        Assert.True(TailwindColorPalette.TryGetColor("sky-400", out var sky400));

        var textBox = new TemplatedControl
        {
            Template = new FuncControlTemplate<TemplatedControl>((owner, scope) =>
            {
                var border = new Border { Name = "PART_BorderElement" };
                border.Bind(Border.BorderBrushProperty, new TemplateBinding(TemplatedControl.BorderBrushProperty));
                return border.RegisterInNameScope(scope);
            }),
        };

        // Stand-in for FluentTheme's own `^:focus /template/ Border#PART_BorderElement` style.
        textBox.Styles.Add(new Style(x => x.OfType<TemplatedControl>().Class(":focus").Template().OfType<Border>().Name("PART_BorderElement"))
        {
            Setters = { new Setter(Border.BorderBrushProperty, Brushes.Gray) },
        });

        Tw.SetClass(textBox, "border-slate-700 focus:border-sky-400");

        textBox.ApplyTemplate();
        var border = Assert.IsType<Border>(textBox.GetVisualChildren().Single());

        ((IPseudoClasses)textBox.Classes).Add(":focus");
        textBox.ApplyStyling();
        border.ApplyStyling();

        Assert.Equal(sky400, Assert.IsAssignableFrom<ISolidColorBrush>(border.BorderBrush).Color);
    }

    [Fact]
    public void SetClass_Logs_Warning_For_Variant_Token_With_Unsupported_Utility()
    {
        var border = new Border();
        var sink = new CapturingLogSink(border);
        var originalSink = Logger.Sink;
        Logger.Sink = sink;

        try
        {
            Tw.SetClass(border, "hover:p-4");

            var entry = Assert.Single(sink.Entries);
            Assert.Equal(LogEventLevel.Warning, entry.Level);
            Assert.Equal("hover:p-4", entry.PropertyValues[0]);
        }
        finally
        {
            Logger.Sink = originalSink;
        }
    }

    private sealed class CapturingLogSink : ILogSink
    {
        private readonly object _expectedSource;
        private readonly List<(LogEventLevel Level, string Area, string MessageTemplate, object?[] PropertyValues)> _entries = new();

        public CapturingLogSink(object expectedSource)
        {
            _expectedSource = expectedSource;
        }

        public IReadOnlyList<(LogEventLevel Level, string Area, string MessageTemplate, object?[] PropertyValues)> Entries
        {
            get
            {
                lock (_entries)
                {
                    return _entries.ToList();
                }
            }
        }

        public bool IsEnabled(LogEventLevel level, string area) => area == "Tailwind.Avalonia";

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
        {
            if (!ReferenceEquals(source, _expectedSource))
            {
                return;
            }

            lock (_entries)
            {
                _entries.Add((level, area, messageTemplate, Array.Empty<object?>()));
            }
        }

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate, object?[] propertyValues)
        {
            if (!ReferenceEquals(source, _expectedSource))
            {
                return;
            }

            lock (_entries)
            {
                _entries.Add((level, area, messageTemplate, propertyValues));
            }
        }
    }
}