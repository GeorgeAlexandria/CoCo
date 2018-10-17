using System.Windows;
using System.Windows.Media;
using CoCo.Settings;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class Extensions
    {
        /// <summary>
        /// Creates the default classification from <paramref name="formatting"/>
        /// which doesn't set the default values for properties that can be reset
        /// </summary>
        public static ClassificationSettings ToDefaultSettings(
           this TextFormattingRunProperties formatting, string classificationName) => new ClassificationSettings
           {
               Name = classificationName,
               IsBold = formatting.Bold,
               IsItalic = formatting.Italic,
               IsOverline = formatting.TextDecorations.Contains(TextDecorations.OverLine[0]),
               IsUnderline = formatting.TextDecorations.Contains(TextDecorations.Underline[0]),
               IsStrikethrough = formatting.TextDecorations.Contains(TextDecorations.Strikethrough[0]),
               IsBaseline = formatting.TextDecorations.Contains(TextDecorations.Baseline[0]),
               IsDisabled = false,
               IsDisabledInXml = false,
           };

        /// <summary>
        /// Gets <see cref="SolidColorBrush.Color"/> if <paramref name="brush"/> is <see cref="SolidColorBrush"/>
        /// or <see cref="Colors.Black"/>
        /// </summary>
        public static Color GetColor(this Brush brush) => brush is SolidColorBrush colorBrush ? colorBrush.Color : Colors.Black;
    }
}