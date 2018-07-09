using System.Windows.Media;
using CoCo.Settings;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class Extensions
    {
        /// <summary>
        /// Creates classification from <paramref name="formatting"/>
        /// </summary>
        public static ClassificationSettings ToSettings(
           this TextFormattingRunProperties formatting,
           string classificationName) => new ClassificationSettings
           {
               Name = classificationName,
               Background = formatting.BackgroundBrush.GetColor(),
               Foreground = formatting.ForegroundBrush.GetColor(),
               FontRenderingSize = (int)formatting.FontRenderingEmSize,
               IsBold = formatting.Bold,
               IsItalic = formatting.Italic,
               IsEnabled = true,
           };

        /// <summary>
        /// Gets <see cref="SolidColorBrush.Color"/> if <paramref name="brush"/> is <see cref="SolidColorBrush"/>
        /// or <see cref="Colors.Black"/>
        /// </summary>
        public static Color GetColor(this Brush brush) => brush is SolidColorBrush colorBrush ? colorBrush.Color : Colors.Black;
    }
}