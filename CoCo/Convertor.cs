using System.Windows.Media;
using CoCo.Settings;
using CoCo.UI.Data;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class Convertor
    {
        public static Classification ToData(this ClassificationSettings settings) =>
            new Classification(settings.Name, settings.DisplayName)
            {
                Background = settings.Background,
                Foreground = settings.Foreground,
                IsBold = settings.IsBold,
                IsItalic = settings.IsItalic,
                FontRenderingSize = settings.FontRenderingSize,
                IsEnabled = settings.IsEnabled,
            };

        public static ClassificationSettings ToSettings(this Classification classification) =>
            new ClassificationSettings
            {
                Name = classification.Name,
                DisplayName = classification.DisplayName,
                Background = classification.Background,
                Foreground = classification.Foreground,
                IsBold = classification.IsBold,
                IsItalic = classification.IsItalic,
                FontRenderingSize = classification.FontRenderingSize,
                IsEnabled = classification.IsEnabled,
            };

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