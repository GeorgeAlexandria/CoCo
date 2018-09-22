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
        /// Migrates the csharp classifications that look like "Some name" to "CoCo csharp some name"
        /// </summary>
        public static Settings.Settings MigrateSettingsTo_2_3_0(Settings.Settings settings)
        {
            ICollection<Settings.ClassificationSettings> MigrateClassifications(
                ICollection<Settings.ClassificationSettings> classificationSettings)
            {
                string PatchName(string name)
                {
                    if (name.Length < 13) return name;

                    var builder = StringBuilderCache.Acquire();
                    builder.Append(char.ToUpper(name[12]));
                    for (int i = 13; i < name.Length; ++i)
                    {
                        builder.Append(name[i]);
                    }
                    return StringBuilderCache.Release(builder);
                }

                var classifications = new Dictionary<string, Settings.ClassificationSettings>();
                foreach (var item in classificationSettings)
                {
                    classifications[item.Name] = item;
                }

                foreach (var name in CSharpNames.All)
                {
                    var oldName = PatchName(name);
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
                if (!languageSettings.Name.EqualsNoCase(Languages.CSharp)) continue;

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