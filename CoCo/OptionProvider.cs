using System.Collections.Generic;
using CoCo.Settings;
using CoCo.UI.Data;

namespace CoCo
{
    public class OptionProvider
    {
        // TODO: It must be a path to CoCo folder at %AppLocal%
        private const string settingsPath = @"C:\temp\555.config";

        public static Option ReceiveOption()
        {
            var settings = SettingsManager.LoadSettings(settingsPath);
            settings = FormattingService.SetFormatting(settings);

            void Append(ICollection<ClassificationSettings> classificationsSettings, ICollection<Classification> classifications)
            {
                foreach (var classificationSettings in classificationsSettings)
                {
                    classifications.Add(ToClassification(classificationSettings));
                }
            }

            var option = new Option();
            foreach (var languageSettings in settings.Languages)
            {
                var language = new Language(languageSettings.LanguageName);
                Append(languageSettings.CurrentClassifications, language.Classifications);

                foreach (var presetSettings in languageSettings.Presets)
                {
                    var preset = new Preset(presetSettings.Name);
                    Append(presetSettings.Classifications, preset.Classifications);
                    language.Presets.Add(preset);
                }

                option.Languages.Add(language);
            }

            return option;
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
                    LanguageName = language.Name,
                    CurrentClassifications = classificationsSettings,
                    Presets = presetsSettings
                });
            }
            var settings = new Settings.Settings
            {
                Languages = languagesSettings
            };

            SettingsManager.SaveSettings(settings, settingsPath);
        }

        private static ClassificationSettings ToSettings(Classification classification) =>
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

        public static Classification ToClassification(ClassificationSettings settings) =>
            new Classification(settings.Name, settings.DisplayName)
            {
                Background = settings.Background.Value,
                Foreground = settings.Foreground.Value,
                IsBold = settings.IsBold.Value,
                IsItalic = settings.IsItalic.Value,
                FontRenderingSize = settings.FontRenderingSize.Value,
                IsEnabled = settings.IsEnabled.Value,
            };
    }
}