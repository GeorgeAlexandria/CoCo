using System.Collections.Generic;
using CoCo.Settings;
using CoCo.UI.Data;

namespace CoCo
{
    public class OptionProvider : IOptionProvider
    {
        private Option _option;

        // TODO: It must be a path to CoCo folder at %AppLocal%
        private const string settingsPath = @"C:\temp\555.config";

        public Option ReceiveOption()
        {
            if (_option != null) return _option;

            var settings = SettingsManager.LoadSettings(settingsPath);
            FormattingService.SetFormatting(settings);

            void Append(ICollection<ClassificationSettings> classificationsSettings, ICollection<Classification> classifications)
            {
                foreach (var classificationSettings in classificationsSettings)
                {
                    classifications.Add(classificationSettings.ToData());
                }
            }

            _option = new Option();
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

                _option.Languages.Add(language);
            }

            return _option;
        }

        public void ReleaseOption(Option option)
        {
            List<ClassificationSettings> ToSettings(ICollection<Classification> classifications)
            {
                var classificationSetings = new List<ClassificationSettings>(classifications.Count);
                foreach (var classification in classifications)
                {
                    classificationSetings.Add(classification.ToSettings());
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
    }
}