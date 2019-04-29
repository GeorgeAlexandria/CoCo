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
            if (classification.FontStyle is null && properties.TryGetValue("IsItalic", out var value) && value is bool isItalic)
            {
                classification.FontStyle = isItalic ? FontStyleService.Italic : FontStyleService.Normal;
            }
        }

        public ICollection<ClassificationSettings> MigrateClassifications(
            string language, ICollection<ClassificationSettings> classificationSettings)
        {
            if (!language.EqualsNoCase(Languages.CSharp)) return classificationSettings;

            // NOTE: -> 2.3.0: Migrates a csharp classifications name from "CoCo {some name}" to "CoCo csharp {some name}"

            var classifications = new Dictionary<string, ClassificationSettings>();
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
    }
}