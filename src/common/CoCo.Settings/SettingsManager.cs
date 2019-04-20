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

        public static void SaveSettings(GeneralSettings settings, string path)
        {
            var jSettings = new JObject();
            foreach (var language in settings.Languages)
            {
                var jLanguage = new JObject();
                if (language.QuickInfoState.HasValue)
                {
                    jLanguage.Add(nameof(GeneralLanguageSettings.QuickInfoState), language.QuickInfoState.Value);
                }
                if (language.EditorState.HasValue)
                {
                    jLanguage.Add(nameof(GeneralLanguageSettings.EditorState), language.EditorState.Value);
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

        public static GeneralSettings LoadGeneralSettings(string path, IMigrationService service = null)
        {
            if (!File.Exists(path))
            {
                return new GeneralSettings { Languages = new List<GeneralLanguageSettings>() };
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
                    return new GeneralSettings { Languages = new List<GeneralLanguageSettings>() };
                }
            }

            var languages = new List<GeneralLanguageSettings>();
            foreach (var jSetting in jSettings)
            {
                if (!(jSetting.Value is JObject jLanguageSettings)) continue;

                var language = new GeneralLanguageSettings
                {
                    Name = jSetting.Key,
                };

                ParseGeneralSettings(jLanguageSettings, ref language, out var properties);
                if (!(service is null))
                {
                    service.MigrateGeneral(properties, ref language);
                }
                languages.Add(language);
            }

            return new GeneralSettings { Languages = languages };
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

        private static void ParseGeneralSettings(
           JObject jObject, ref GeneralLanguageSettings generalLanguage, out Dictionary<string, object> properties)
        {
            if (jObject.TryGetProperty(nameof(GeneralLanguageSettings.QuickInfoState), out long quickInfoState))
            {
                generalLanguage.QuickInfoState = (int)quickInfoState;
            }
            if (jObject.TryGetProperty(nameof(GeneralLanguageSettings.EditorState), out long state))
            {
                generalLanguage.EditorState = (int)state;
            }

            properties = jObject.GetProperties();
        }

        private static bool TryParseClassification(
            JObject jObject, out ClassificationSettings classification, out IReadOnlyDictionary<string, object> properties)
        {
            if (!jObject.TryGetProperty(nameof(ClassificationSettings.Name), out string name))
            {
                classification = default;
                properties = _emptyProperties;
                return false;
            }

            classification = new ClassificationSettings { Name = name };

            if (jObject.TryGetProperty(nameof(classification.Background), out string background) &&
                background.TryParseColor(out Color color))
            {
                classification.Background = color;
            }
            if (jObject.TryGetProperty(nameof(classification.Foreground), out string foreground) &&
                foreground.TryParseColor(out color))
            {
                classification.Foreground = color;
            }
            if (jObject.TryGetProperty(nameof(classification.FontFamily), out string fontFamily))
            {
                classification.FontFamily = fontFamily;
            }
            if (jObject.TryGetProperty(nameof(classification.FontStyle), out string fontStyle))
            {
                classification.FontStyle = fontStyle;
            }
            if (jObject.TryGetProperty(nameof(classification.FontStretch), out long fontStretch) && fontStretch < 10)
            {
                classification.FontStretch = (int)fontStretch;
            }
            if (jObject.TryGetProperty(nameof(classification.FontRenderingSize), out long renderingSize) && renderingSize < 512)
            {
                classification.FontRenderingSize = (int)renderingSize;
            }

            classification.IsBold = jObject.GetPropertyValue<bool>(nameof(classification.IsBold));
            classification.IsOverline = jObject.GetPropertyValue<bool>(nameof(classification.IsOverline));
            classification.IsUnderline = jObject.GetPropertyValue<bool>(nameof(classification.IsUnderline));
            classification.IsStrikethrough = jObject.GetPropertyValue<bool>(nameof(classification.IsStrikethrough));
            classification.IsBaseline = jObject.GetPropertyValue<bool>(nameof(classification.IsBaseline));
            classification.IsDisabled = jObject.GetPropertyValue<bool>(nameof(classification.IsDisabled));
            classification.IsDisabledInEditor = jObject.GetPropertyValue<bool>(nameof(classification.IsDisabledInEditor));
            classification.IsDisabledInQuickInfo = jObject.GetPropertyValue<bool>(nameof(classification.IsDisabledInQuickInfo));
            classification.IsDisabledInXml = jObject.GetPropertyValue<bool>(nameof(classification.IsDisabledInXml));

            properties = jObject.GetProperties();
            return true;
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
            if (!string.IsNullOrWhiteSpace(classification.FontStyle))
            {
                jClassification.Add(nameof(classification.FontStyle), new JValue(classification.FontStyle));
            }

            jClassification
                .AppendProperty(nameof(classification.IsBold), classification.IsBold)
                .AppendProperty(nameof(classification.FontStretch), classification.FontStretch)
                .AppendProperty(nameof(classification.IsOverline), classification.IsOverline)
                .AppendProperty(nameof(classification.IsUnderline), classification.IsUnderline)
                .AppendProperty(nameof(classification.IsStrikethrough), classification.IsStrikethrough)
                .AppendProperty(nameof(classification.IsBaseline), classification.IsBaseline)
                .AppendProperty(nameof(classification.FontRenderingSize), classification.FontRenderingSize)
                .AppendProperty(nameof(classification.IsDisabled), classification.IsDisabled)
                .AppendProperty(nameof(classification.IsDisabledInEditor), classification.IsDisabledInEditor)
                .AppendProperty(nameof(classification.IsDisabledInQuickInfo), classification.IsDisabledInQuickInfo)
                .AppendProperty(nameof(classification.IsDisabledInXml), classification.IsDisabledInXml);

            return jClassification;
        }

        private static bool TryParseColor(this string value, out Color color)
        {
            // NOTE: #ARGB – 9 chars
            if (value.Length == 9)
            {
                return ColorHelpers.TryParseColor(value.Substring(1), out color);
            }

            color = new Color();
            return false;
        }

        /// <summary>
        /// Try to retrieves value for <paramref name="name"/> property under <paramref name="jObject"/> to <paramref name="value"/>
        /// </summary>
        private static bool TryGetProperty<T>(this JObject jObject, string name, out T value)
        {
            if (jObject[name] is JValue jValue && jValue.Value is T receivedValue)
            {
                value = receivedValue;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Retrieves value for <paramref name="name"/> property under <paramref name="jObject"/>
        /// </summary>
        /// <returns></returns>
        private static T? GetPropertyValue<T>(this JObject jObject, string name) where T : struct =>
            jObject[name] is JValue jValue && jValue.Value is T receivedValue
                ? receivedValue
                : default;

        /// <summary>
        /// Appends property to <paramref name="jObject"/> if <paramref name="value"/> has value
        /// </summary>
        private static JObject AppendProperty<T>(this JObject jObject, string name, T? value) where T : struct
        {
            if (value.HasValue)
            {
                jObject.Add(name, new JValue(value.Value));
            }
            return jObject;
        }

        /// <summary>
        /// Retrieves properties and their values
        /// </summary>
        private static Dictionary<string, object> GetProperties(this JObject jObject)
        {
            var properties = new Dictionary<string, object>(jObject.Count);
            foreach (var (propertyName, value) in jObject)
            {
                if (value is JValue jProperty)
                {
                    properties.Add(propertyName, jProperty.Value);
                }
            }
            return properties;
        }
    }
}