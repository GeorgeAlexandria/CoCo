using CoCo.Settings;
using CoCo.UI.Data;

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
    }
}