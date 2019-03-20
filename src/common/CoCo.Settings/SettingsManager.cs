using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using CoCo.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoCo.Settings
{
    /// <summary>
    /// Is responsible to loading and saving settings
    /// </summary>
    public static class SettingsManager
    {
        private static readonly IReadOnlyDictionary<string, object> _emptyProperties = new Dictionary<string, object>();

        private const string CurrentClassificationsName = "current";

        public static void SaveSettings(QuickInfoSettings settings, string path)
        {
            var jSettings = new JObject();
            foreach (var language in settings.Languages)
            {
                var jLanguage = new JObject();
                if (language.State.HasValue)
                {
                    jLanguage.Add(nameof(QuickInfoLanguageSettings.State), new JValue(language.State));
                }
                jSettings.Add(language.Name, jLanguage);
            }

            WriteToFile(jSettings, path);
        }

        public static void SaveSettings(EditorSettings settings, string path)
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
                jSettings.Add(language.Name, jLanguage);
            }

            WriteToFile(jSettings, path);
        }

        public static QuickInfoSettings LoadQuickInfoSettings(string path)
        {
            if (!File.Exists(path))
            {
                return new QuickInfoSettings { Languages = new List<QuickInfoLanguageSettings>() };
            }

            JObject jSettings;
            using (var reader = File.OpenText(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                try
                {
                    jSettings = (JObject)JToken.ReadFrom(jsonReader);
                }
                catch (JsonReaderException)
                {
                    return new QuickInfoSettings { Languages = new List<QuickInfoLanguageSettings>() };
                }
            }

            var languages = new List<QuickInfoLanguageSettings>();
            foreach (var jSetting in jSettings)
            {
                if (!(jSetting.Value is JObject jLanguageSettings)) continue;

                var language = new QuickInfoLanguageSettings
                {
                    Name = jSetting.Key,
                };

                if (jLanguageSettings[nameof(QuickInfoLanguageSettings.State)] is JValue jValue && jValue.Value is long state)
                {
                    language.State = (int)state;
                }
                languages.Add(language);
            }

            return new QuickInfoSettings { Languages = languages };
        }

        public static EditorSettings LoadEditorSettings(string path, IMigrationService service = null)
        {
            if (!File.Exists(path))
            {
                return new EditorSettings { Languages = new List<EditorLanguageSettings>() };
            }

            JObject jSettings;
            using (var reader = File.OpenText(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                try
                {
                    jSettings = (JObject)JToken.ReadFrom(jsonReader);
                }
                catch (JsonReaderException)
                {
                    return new EditorSettings { Languages = new List<EditorLanguageSettings>() };
                }
            }

            var languages = new List<EditorLanguageSettings>();
            foreach (var jSetting in jSettings)
            {
                if (!(jSetting.Value is JObject jLanguageSettings)) continue;

                var language = new EditorLanguageSettings
                {
                    Name = jSetting.Key,
                    CurrentClassifications = new List<ClassificationSettings>(),
                    Presets = new List<PresetSettings>()
                };

                foreach (var (presetName, value) in jLanguageSettings)
                {
                    var classifications = new List<ClassificationSettings>();
                    if (value is JArray jClassifications)
                    {
                        foreach (var item in jClassifications)
                        {
                            if (item is JObject jClassification &&
                                TryParseClassification(jClassification, out var classification, out var properties))
                            {
                                if (!(service is null))
                                {
                                    service.MigrateClassification(properties, ref classification);
                                }
                                classifications.Add(classification);
                            }
                        }
                    }

                    var migratedClassifications = service is null
                        ? classifications
                        : service.MigrateClassifications(language.Name, classifications);

                    if (presetName == CurrentClassificationsName)
                    {
                        language.CurrentClassifications = migratedClassifications;
                    }
                    else
                    {
                        language.Presets.Add(new PresetSettings
                        {
                            Name = presetName,
                            Classifications = migratedClassifications
                        });
                    }
                }

                languages.Add(language);
            }
            return new EditorSettings { Languages = languages };
        }

        private static void WriteToFile(JObject jSettings, string path)
        {
            var info = new FileInfo(path);
            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }

            using (var writer = !info.Exists ? info.CreateText() : new StreamWriter(path))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jSettings.WriteTo(jsonWriter);
            }
        }

        private static bool TryParseClassification(
            JObject jObject, out ClassificationSettings classification, out IReadOnlyDictionary<string, object> properties)
        {
            if (!(jObject[nameof(ClassificationSettings.Name)] is JValue jValue) || !(jValue.Value is string name))
            {
                classification = default;
                properties = _emptyProperties;
                return false;
            }

            classification = new ClassificationSettings { Name = name };

            if (jObject[nameof(ClassificationSettings.Background)] is JValue jBackground &&
                jBackground.Value is string background && TryParseColor(background, out Color color))
            {
                classification.Background = color;
            }
            if (jObject[nameof(ClassificationSettings.Foreground)] is JValue jForeground &&
                jForeground.Value is string foreground && TryParseColor(foreground, out color))
            {
                classification.Foreground = color;
            }
            if (jObject[nameof(ClassificationSettings.FontFamily)] is JValue jFontFamily &&
                jFontFamily.Value is string fontFamily)
            {
                classification.FontFamily = fontFamily;
            }
            if (jObject[nameof(ClassificationSettings.IsBold)] is JValue jBold &&
                jBold.Value is bool isBold)
            {
                classification.IsBold = isBold;
            }
            if (jObject[nameof(ClassificationSettings.FontStyle)] is JValue jFontStyle &&
                jFontStyle.Value is string fontStyle)
            {
                classification.FontStyle = fontStyle;
            }
            if (jObject[nameof(ClassificationSettings.FontStretch)] is JValue jFontStretch &&
                jFontStretch.Value is long fontStretch && fontStretch < 10)
            {
                classification.FontStretch = (int)fontStretch;
            }
            if (jObject[nameof(ClassificationSettings.IsOverline)] is JValue jOverline &&
                jOverline.Value is bool isOverline)
            {
                classification.IsOverline = isOverline;
            }
            if (jObject[nameof(ClassificationSettings.IsUnderline)] is JValue jUnderline &&
                jUnderline.Value is bool isUnderline)
            {
                classification.IsUnderline = isUnderline;
            }
            if (jObject[nameof(ClassificationSettings.IsStrikethrough)] is JValue jStrikethrough &&
                jStrikethrough.Value is bool isStrikethrough)
            {
                classification.IsStrikethrough = isStrikethrough;
            }
            if (jObject[nameof(ClassificationSettings.IsBaseline)] is JValue jBaseline &&
                jBaseline.Value is bool isBaseline)
            {
                classification.IsBaseline = isBaseline;
            }
            if (jObject[nameof(ClassificationSettings.FontRenderingSize)] is JValue jRenderingSize &&
                jRenderingSize.Value is long renderingSize && renderingSize < 512)
            {
                classification.FontRenderingSize = (int)renderingSize;
            }
            if (jObject[nameof(ClassificationSettings.IsDisabled)] is JValue jDisabled &&
                jDisabled.Value is bool isDisabled)
            {
                classification.IsDisabled = isDisabled;
            }
            if (jObject[nameof(ClassificationSettings.IsDisabledInXml)] is JValue jDisabledInXml &&
                jDisabledInXml.Value is bool isDisabledInXml)
            {
                classification.IsDisabledInXml = isDisabledInXml;
            }

            var classificationProperties = new Dictionary<string, object>(jObject.Count);
            foreach (var (propertyName, value) in jObject)
            {
                if (value is JValue jProperty)
                {
                    classificationProperties.Add(propertyName, jProperty.Value);
                }
            }

            properties = classificationProperties;
            return true;
        }

        private static bool TryParseColor(string value, out Color color)
        {
            // NOTE: #ARGB – 9 chars
            if (value.Length == 9)
            {
                return ColorHelpers.TryParseColor(value.Substring(1), out color);
            }

            color = new Color();
            return false;
        }

        private static JObject ToJObject(ClassificationSettings classification)
        {
            var jClassification = new JObject();
            jClassification.Add(nameof(classification.Name), new JValue(classification.Name));
            if (classification.Background.HasValue)
            {
                jClassification.Add(nameof(classification.Background), new JValue(classification.Background.Value.ToString()));
            }
            if (classification.Foreground.HasValue)
            {
                jClassification.Add(nameof(classification.Foreground), new JValue(classification.Foreground.Value.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(classification.FontFamily))
            {
                jClassification.Add(nameof(classification.FontFamily), new JValue(classification.FontFamily));
            }
            if (classification.IsBold.HasValue)
            {
                jClassification.Add(nameof(classification.IsBold), new JValue(classification.IsBold.Value));
            }
            if (!string.IsNullOrWhiteSpace(classification.FontStyle))
            {
                jClassification.Add(nameof(classification.FontStyle), new JValue(classification.FontStyle));
            }
            if (classification.FontStretch.HasValue)
            {
                jClassification.Add(nameof(classification.FontStretch), new JValue(classification.FontStretch.Value));
            }
            if (classification.IsOverline.HasValue)
            {
                jClassification.Add(nameof(classification.IsOverline), new JValue(classification.IsOverline.Value));
            }
            if (classification.IsUnderline.HasValue)
            {
                jClassification.Add(nameof(classification.IsUnderline), new JValue(classification.IsUnderline.Value));
            }
            if (classification.IsStrikethrough.HasValue)
            {
                jClassification.Add(nameof(classification.IsStrikethrough), new JValue(classification.IsStrikethrough.Value));
            }
            if (classification.IsBaseline.HasValue)
            {
                jClassification.Add(nameof(classification.IsBaseline), new JValue(classification.IsBaseline.Value));
            }
            if (classification.FontRenderingSize.HasValue)
            {
                jClassification.Add(nameof(classification.FontRenderingSize), new JValue(classification.FontRenderingSize.Value));
            }
            if (classification.IsDisabled.HasValue)
            {
                jClassification.Add(nameof(classification.IsDisabled), new JValue(classification.IsDisabled.Value));
            }
            if (classification.IsDisabledInXml.HasValue)
            {
                jClassification.Add(nameof(classification.IsDisabledInXml), new JValue(classification.IsDisabledInXml.Value));
            }
            return jClassification;
        }
    }
}