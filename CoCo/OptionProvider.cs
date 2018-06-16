using System.Collections.Generic;
using CoCo.Settings;
using CoCo.UI.Data;

namespace CoCo
{
    public class OptionProvider : IOptionProvider
    {
        private Option _model;

        public Option ReceiveOption()
        {
            if (_model != null) return _model;

            var settings = SettingsManager.LoadSettings(@"C:\temp\555.config");

            FormattingService.SetFormatting(settings);

            void Append(IEnumerable<ClassificationSettings> classifications, ICollection<Classification> appendedItems)
            {
                foreach (var classification in classifications)
                {
                    appendedItems.Add(classification.ToData());
                }
            }

            _model = new Option();
            foreach (var language in settings.Languages)
            {
                var languageModel = new Language(language.LanguageName);
                Append(language.CurrentSettings, languageModel.Classifications);

                foreach (var preset in language.Presettings)
                {
                    var presetData = new Preset(preset.Name);
                    Append(preset.Classifications, presetData.Classifications);
                    languageModel.Presets.Add(presetData);
                }

                _model.Languages.Add(languageModel);
            }

            return _model;
        }

        public void ReleaseOption(Option option)
        {
            List<ClassificationSettings> ConvertClassifications(ICollection<Classification> classi)
            {
                var classificationSetings = new List<ClassificationSettings>();
                foreach (var classification in classi)
                {
                    classificationSetings.Add(classification.ToSettings());
                }
                return classificationSetings;
            }

            FormattingService.SetFormatting(option);

            var languages = new List<LanguageSettings>();
            foreach (var language in option.Languages)
            {
                var classifications = ConvertClassifications(language.Classifications);

                var presets = new List<PresetSettings>();
                foreach (var preset in language.Presets)
                {
                    presets.Add(new PresetSettings
                    {
                        Name = preset.Name,
                        Classifications = ConvertClassifications(preset.Classifications)
                    });
                }

                languages.Add(new LanguageSettings
                {
                    LanguageName = language.Name,
                    CurrentSettings = classifications,
                    Presettings = presets
                });
            }
            var settings = new Settings.Settings
            {
                Languages = languages
            };

            SettingsManager.SaveSettings(settings, @"C:\temp\555.config");
        }
    }
}