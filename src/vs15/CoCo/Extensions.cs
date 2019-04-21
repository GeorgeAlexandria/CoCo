using System.Linq;
using System.Windows;
using System.Windows.Media;
using CoCo.Analyser.Classifications;
using CoCo.Settings;
using CoCo.UI;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    using DrawingColor = System.Drawing.Color;
    using MediaColor = System.Windows.Media.Color;

    public static class Extensions
    {
        /// <summary>
        /// Creates the default classification from <paramref name="formatting"/>
        /// which doesn't set the default values for properties that can be reset
        /// </summary>
        public static ClassificationSettings ToDefaultSettings(
           this TextFormattingRunProperties formatting, string classificationName)
        {
            var defaultOption = ClassificationService.GetDefaultOption(classificationName);
            return new ClassificationSettings
            {
                Name = classificationName,
                FontFamily = formatting.GetFontFamily(),
                IsBold = formatting.Bold,
                FontStyle = formatting.GetFontStyleName(),
                IsOverline = formatting.TextDecorations.Contains(TextDecorations.OverLine[0]),
                IsUnderline = formatting.TextDecorations.Contains(TextDecorations.Underline[0]),
                IsStrikethrough = formatting.TextDecorations.Contains(TextDecorations.Strikethrough[0]),
                IsBaseline = formatting.TextDecorations.Contains(TextDecorations.Baseline[0]),
                IsDisabled = defaultOption.IsDisabled,
                IsDisabledInXml = defaultOption.IsDisabledInXml,
                IsDisabledInEditor = defaultOption.IsDisabledInEditor,
                IsDisabledInQuickInfo = defaultOption.IsDisabledInQuickInfo,
            };
        }

        /// <summary>
        /// Returns the relevant font style name for <paramref name="formatting"/> if it exists or the fallback name
        /// </summary>
        public static string GetFontStyleName(this TextFormattingRunProperties formatting)
        {
            if (formatting.Italic) return FontStyleService.Italic;
            if (formatting.TypefaceEmpty) return FontStyleService.Normal;

            var styleName = formatting.Typeface.Style.ToString();
            return FontStyleService.SupportedStyleByNames.ContainsKey(styleName) ? styleName : FontStyleService.Normal;
        }

        /// <summary>
        /// Returns the relevant font family name for <paramref name="formatting"/> if if exists or the fallback name
        /// </summary>
        public static string GetFontFamily(this TextFormattingRunProperties formatting)
        {
            string source;
            if (!formatting.TypefaceEmpty)
            {
                source = formatting.Typeface.FontFamily.Source;
                if (FontFamilyService.SupportedFamilies.ContainsKey(source)) return source;
            }

            source = "Consolas";
            return FontFamilyService.SupportedFamilies.ContainsKey(source)
                ? source
                : FontFamilyService.SupportedFamilies.Keys.First();
        }

        /// <summary>
        /// Returns the relevant font stretch for <paramref name="formatting"/> if if exists or the fallback value
        /// </summary>
        public static int GetFontStretch(this TextFormattingRunProperties formatting)
        {
            if (formatting.TypefaceEmpty) return FontStretchService.Normal;

            var stretch = formatting.Typeface.Stretch.ToOpenTypeStretch();
            return FontStretchService.SupportedStretches.ContainsKey(stretch) ? stretch : FontStretchService.Normal;
        }

        /// <summary>
        /// Gets <see cref="SolidColorBrush.Color"/> if <paramref name="brush"/> is <see cref="SolidColorBrush"/>
        /// or <see cref="Colors.Black"/>
        /// </summary>
        public static Color GetColor(this Brush brush) => brush is SolidColorBrush colorBrush ? colorBrush.Color : Colors.Black;

        /// <summary>
        /// Converts <see cref="DrawingColor"/> to <see cref="MediaColor"/>
        /// </summary>
        public static MediaColor DrawingToMedia(this DrawingColor color) => MediaColor.FromArgb(color.A, color.R, color.G, color.B);
    }
}