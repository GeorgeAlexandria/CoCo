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

            _model = new Option();
            foreach (var language in settings.Languages)
            {
                var languageModel = new Language(language.LanguageName);
                foreach (var classification in language.CurrentSettings)
                {
                    languageModel.Classifications.Add(classification.ToData());
                }
                _model.Languages.Add(languageModel);
            }

            return _model;
        }

        public void ReleaseOption(Option option)
        {
            FormattingService.SetFormatting(option);

            var languages = new List<LanguageSettings>();
            foreach (var language in option.Languages)
            {
                var classifications = new List<ClassificationSettings>();
                foreach (var classification in language.Classifications)
                {
                    classifications.Add(classification.ToSettings());
                }

                languages.Add(new LanguageSettings
                {
                    LanguageName = language.Name,
                    CurrentSettings = classifications,
                    Presettings = new List<PresetSettings>()
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