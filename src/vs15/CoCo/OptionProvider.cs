using System.Collections.Generic;
using CoCo.Settings;
using CoCo.UI.Data;

namespace CoCo
{
    public static class OptionProvider
    {
        public static Option ReceiveOption()
        {
            MigrationService.MigrateSettings();
            var settings = SettingsManager.LoadSettings(Paths.CoCoSettingsFile);
            return FormattingService.SetFormatting(settings);
        }

        public static void ReleaseOption(Option option)
        {
            List<ClassificationSettings> ToSettings(ICollection<Classification> classifications)
            {
                var classificationSetings = new List<ClassificationSettings>(classifications.Count);
                foreach (var classification in classifications)
                {
                    classificationSetings.Add(OptionProvider.ToSettings(classification));
                }
                return classificationSetings;
            }

            FormattingService.SetFormatting(option);

            var languagesSettings = new List<LanguageSettings>(option.Languages.Count);
            foreach (var language in option.Languages)
            {
                var classificationsSettings = ToSettings(language.Classifications);

                var presetsSettings = new List<PresetSettings>(language.Presets.Count);
                foreach (var preset in language.Presets)
                {
                    presetsSettings.Add(new PresetSettings
                    {
                        Name = preset.Name,
                        Classifications = ToSettings(preset.Classifications)
                    });
                }

                languagesSettings.Add(new LanguageSettings
                {
                    Name = language.Name,
                    CurrentClassifications = classificationsSettings,
                    Presets = presetsSettings
                });
            }
            var settings = new Settings.Settings
            {
                Languages = languagesSettings
            };

            SettingsManager.SaveSettings(settings, Paths.CoCoSettingsFile);
        }

        private static ClassificationSettings ToSettings(Classification classification)
        {
            var settings = new ClassificationSettings
            {
                Name = classification.Name,
                IsBold = classification.IsBold,
                IsItalic = classification.IsItalic,
                IsOverline = classification.IsOverline,
                IsUnderline = classification.IsUnderline,
                IsStrikethrough = classification.IsStrikethrough,
                IsBaseline = classification.IsBaseline,
                IsEnabled = classification.IsEnabled,
            };

            if (!classification.BackgroundWasReset)
            {
                settings.Background = classification.Background;
            }
            if (!classification.ForegroundWasReset)
            {
                settings.Foreground = classification.Foreground;
            }
            if (!classification.FontRenderingSizeWasReset)
            {
                settings.FontRenderingSize = classification.FontRenderingSize;
            }
            return settings;
        }
    }
}