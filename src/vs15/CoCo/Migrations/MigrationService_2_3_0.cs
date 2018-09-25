using System.Collections.Generic;
using System.Linq;
using CoCo.Analyser;
using CoCo.Analyser.CSharp;
using CoCo.Utils;

namespace CoCo
{
    public static partial class MigrationService
    {
        /// <summary>
        /// Migrates the csharp classifications that look like "CoCo some name" to "CoCo csharp some name"
        /// </summary>
        public static Settings.Settings MigrateSettingsTo_2_3_0(Settings.Settings settings)
        {
            ICollection<Settings.ClassificationSettings> MigrateClassifications(
                ICollection<Settings.ClassificationSettings> classificationSettings)
            {
                var classifications = new Dictionary<string, Settings.ClassificationSettings>();
                foreach (var item in classificationSettings)
                {
                    classifications[item.Name] = item;
                }

                foreach (var name in CSharpNames.All)
                {
                    var oldName = name.Length < 13 ? name : name.Remove(5, 7);
                    if (!classifications.ContainsKey(name) &&
                        classifications.TryGetValue(oldName, out var classification))
                    {
                        classification.Name = name;
                        classifications[oldName] = classification;
                    }
                }
                return classifications.Values.ToList();
            }

            var languages = new List<Settings.LanguageSettings>(settings.Languages.Count);
            foreach (var languageSettings in settings.Languages)
            {
                if (!languageSettings.Name.EqualsNoCase(Languages.CSharp))
                {
                    languages.Add(languageSettings);
                    continue;
                }

                var language = languageSettings;
                language.CurrentClassifications = MigrateClassifications(language.CurrentClassifications);

                var presets = new List<Settings.PresetSettings>(language.Presets.Count);
                foreach (var presetSettings in language.Presets)
                {
                    var preset = presetSettings;
                    preset.Classifications = MigrateClassifications(preset.Classifications);
                    presets.Add(preset);
                }
                language.Presets = presets;

                languages.Add(language);
            }
            return new Settings.Settings { Languages = languages };
        }
    }
}