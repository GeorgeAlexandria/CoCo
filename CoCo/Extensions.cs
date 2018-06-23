using System.Windows.Media;
using CoCo.Settings;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class Extensions
    {
        /// <summary>
        /// Creates classification from <paramref name="defaultFormatting"/>
        /// </summary>
        public static ClassificationSettings ToSettings(
           this TextFormattingRunProperties defaultFormatting,
           string classificationName,
           string displayName = null) => new ClassificationSettings
           {
               Name = classificationName,
               DisplayName = displayName ?? classificationName,
               Background = defaultFormatting.BackgroundBrush.GetColor(),
               Foreground = defaultFormatting.ForegroundBrush.GetColor(),
               FontRenderingSize = (int)defaultFormatting.FontRenderingEmSize,
               IsBold = defaultFormatting.Bold,
               IsItalic = defaultFormatting.Italic,
               IsEnabled = true,
           };

        public static Color GetColor(this Brush brush) => brush is SolidColorBrush colorBrush ? colorBrush.Color : Colors.Black;
    }
}