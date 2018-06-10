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

        public static ClassificationSettings ToSettings(this Classification data) =>
            new ClassificationSettings
            {
                Name = data.Name,
                Background = data.Background,
                DisplayName = data.DisplayName,
                FontRenderingSize = data.FontRenderingSize,
                Foreground = data.Foreground,
                IsBold = data.IsBold,
                IsEnabled = data.IsEnabled,
                IsItalic = data.IsItalic
            };
    }
}