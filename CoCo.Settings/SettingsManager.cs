using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoCo.Settings
{
    /// <summary>
    /// Is responsible at loading and saving settings
    /// </summary>
    public static class SettingsManager
    {
        private const string CurrentClassificationsName = "current";

        public static void SaveSettings(Settings settings, string path)
        {
            JArray ToJArray(ICollection<ClassificationSettings> classificationSettings)
            {
                var jClassifications = new JArray();
                foreach (var classification in classificationSettings)
                {
                    jClassifications.Add(ToJObject(classification));
                }
                return jClassifications;
            }

            var jSettings = new JObject();
            foreach (var language in settings.Languages)
            {
                var jLanguage = new JObject();
                jLanguage.Add(CurrentClassificationsName, ToJArray(language.CurrentClassifications));
                foreach (var preset in language.Presets)
                {
                    jLanguage.Add(preset.Name, ToJArray(preset.Classifications));
                }
                jSettings.Add(language.LanguageName, jLanguage);
            }

            // TODO: handle a couple of exception
            using (var writer = new StreamWriter(path))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jSettings.WriteTo(jsonWriter);
            }
        }

        public static Settings LoadSettings(string path)
        {
            // TODO: handle a couple of exception
            JObject jSettings;
            using (var reader = File.OpenText(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                jSettings = (JObject)JToken.ReadFrom(jsonReader);
            }

            var languages = new List<LanguageSettings>();
            foreach (var jSetting in jSettings)
            {
                var language = new LanguageSettings
                {
                    LanguageName = jSetting.Key,
                    CurrentClassifications = new List<ClassificationSettings>(),
                    Presets = new List<PresetSettings>()
                };

                var jLanguageSettings = jSetting.Value as JObject;
                foreach (var languagePair in jLanguageSettings)
                {
                    var classifications = new List<ClassificationSettings>();
                    if (languagePair.Value is JArray jClassifications)
                    {
                        foreach (var item in jClassifications)
                        {
                            if (item is JObject jClassification)
                            {
                                classifications.Add(ParseClassification(jClassification));
                            }
                        }
                    }

                    if (languagePair.Key == CurrentClassificationsName)
                    {
                        language.CurrentClassifications = classifications;
                    }
                    else
                    {
                        language.Presets.Add(new PresetSettings
                        {
                            Name = languagePair.Key,
                            Classifications = classifications
                        });
                    }
                }

                languages.Add(language);
            }
            return new Settings { Languages = languages };
        }

        private static ClassificationSettings ParseClassification(JObject jClassification)
        {
            // TODO: if something would not be presented – set the default value
            Color color;
            var classification = new ClassificationSettings();
            if (jClassification[nameof(ClassificationSettings.Name)] is JValue jName &&
                jName.Value is string name)
            {
                classification.Name = name;
            }
            if (jClassification[nameof(ClassificationSettings.Name)] is JValue jDisplayName &&
                jDisplayName.Value is string displayName)
            {
                classification.DisplayName = displayName;
            }
            if (jClassification[nameof(ClassificationSettings.Background)] is JArray background &&
                TryParseColor(background, out color))
            {
                classification.Background = color;
            }
            if (jClassification[nameof(ClassificationSettings.Foreground)] is JArray foreground &&
                TryParseColor(foreground, out color))
            {
                classification.Foreground = color;
            }
            if (jClassification[nameof(ClassificationSettings.IsBold)] is JValue jBold &&
                jBold.Value is bool isBold)
            {
                classification.IsBold = isBold;
            }
            if (jClassification[nameof(ClassificationSettings.IsItalic)] is JValue jItalic &&
                jItalic.Value is bool isItalic)
            {
                classification.IsItalic = isItalic;
            }
            if (jClassification[nameof(ClassificationSettings.FontRenderingSize)] is JValue jRenderingSize &&
                jRenderingSize.Value is long renderingSize && renderingSize < 512)
            {
                classification.FontRenderingSize = (int)renderingSize;
            }
            if (jClassification[nameof(ClassificationSettings.IsEnabled)] is JValue jEnabled &&
                jEnabled.Value is bool isEnabled)
            {
                classification.IsEnabled = isEnabled;
            }
            return classification;
        }

        private static bool TryParseColor(JArray jArray, out Color color)
        {
            color = new Color();
            // TODO: count is less 3
            if (jArray.Count > 3) return false;

            var rgb = new List<byte>(3);
            foreach (var item in jArray)
            {
                if (item is JValue jvalue)
                {
                    // HACK: all numerics data store as long in the newtonsoft json
                    if (!(jvalue.Value is long value) || value > byte.MaxValue) return false;
                    rgb.Add((byte)value);
                }
            }
            color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
            return true;
        }

        private static JObject ToJObject(ClassificationSettings classification)
        {
            JToken ToJObject(Color color) => new JArray(color.R, color.G, color.B);

            return new JObject
            {
                { nameof(classification.Name), new JValue(classification.Name) },
                { nameof(classification.DisplayName), new JValue(classification.DisplayName) },
                { nameof(classification.Background), ToJObject(classification.Background) },
                { nameof(classification.Foreground), ToJObject(classification.Foreground) },
                { nameof(classification.IsBold), new JValue(classification.IsBold) },
                { nameof(classification.IsItalic), new JValue(classification.IsItalic) },
                { nameof(classification.FontRenderingSize), new JValue(classification.FontRenderingSize) },
                { nameof(classification.IsEnabled), new JValue(classification.IsEnabled) }
            };
        }
    }
}