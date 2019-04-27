using System.Collections.Generic;
using System.Linq;
using CoCo.Analyser;
using CoCo.Analyser.Classifications.CSharp;
using CoCo.Settings;
using CoCo.UI;
using CoCo.Utils;

namespace CoCo
{
    public sealed partial class MigrationService : IMigrationService
    {
        private MigrationService()
        {
        }

        public static MigrationService Instance = new MigrationService();

        public void MigrateClassification(IReadOnlyDictionary<string, object> properties, ref ClassificationSettings classification)
        {
            //NOTE: -> 2.5.0: Migrates a font style
            if (string.IsNullOrWhiteSpace(classification.FontStyle) &&
                properties.TryGetValue("IsItalic", out var value) && 
                value is bool isItalic)
            {
                classification.FontStyle = isItalic ? FontStyleService.Italic : FontStyleService.Normal;
            }
        }

        public ICollection<ClassificationSettings> MigrateClassifications(
            string language, ICollection<ClassificationSettings> classificationSettings)
        {
            if (!language.EqualsNoCase(Languages.CSharp)) return classificationSettings;

            // NOTE: -> 2.3.0: Migrates a csharp classifications name from "{Some name}" to "CoCo csharp {some name}"

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
    }
}