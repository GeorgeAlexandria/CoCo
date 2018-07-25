using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using CoCo.Utils;
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
                jSettings.Add(language.Name, jLanguage);
            }

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

        public static Settings LoadSettings(string path)
        {
            if (!File.Exists(path))
            {
                return new Settings { Languages = new List<LanguageSettings>() };
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
                    return new Settings { Languages = new List<LanguageSettings>() };
                }
            }

            var languages = new List<LanguageSettings>();
            foreach (var jSetting in jSettings)
            {
                if (!(jSetting.Value is JObject jLanguageSettings)) continue;

                var language = new LanguageSettings
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
                            if (item is JObject jClassification && TryParseClassification(jClassification, out var classification))
                            {
                                classifications.Add(classification);
                            }
                        }
                    }

                    if (presetName == CurrentClassificationsName)
                    {
                        language.CurrentClassifications = classifications;
                    }
                    else
                    {
                        language.Presets.Add(new PresetSettings
                        {
                            Name = presetName,
                            Classifications = classifications
                        });
                    }
                }

                languages.Add(language);
            }
            return new Settings { Languages = languages };
        }

        private static bool TryParseClassification(JObject jClassification, out ClassificationSettings classification)
        {
            if (!(jClassification[nameof(ClassificationSettings.Name)] is JValue jValue) ||
                !(jValue.Value is string name))
            {
                classification = default;
                return false;
            }

            classification = new ClassificationSettings { Name = name };

            Color color;
            if (jClassification[nameof(ClassificationSettings.Background)] is JValue jBackground &&
                jBackground.Value is string background && TryParseColor(background, out color))
            {
                classification.Background = color;
            }
            if (jClassification[nameof(ClassificationSettings.Foreground)] is JValue jForeground &&
                jForeground.Value is string foreground && TryParseColor(foreground, out color))
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
            return true;
        }

        private static bool TryParseColor(string value, out Color color)
        {
            byte ToByte(int integer, int offset) => (byte)(integer >> offset & 0xFF);

            // NOTE: #ARGB – 9 chars
            if (value.Length == 9)
            {
                value = value.Substring(1);
                if (int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var res))
                {
                    color = Color.FromArgb(ToByte(res, 24), ToByte(res, 16), ToByte(res, 8), ToByte(res, 0));
                    return true;
                }
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
            if (classification.IsBold.HasValue)
            {
                jClassification.Add(nameof(classification.IsBold), new JValue(classification.IsBold.Value));
            }
            if (classification.IsItalic.HasValue)
            {
                jClassification.Add(nameof(classification.IsItalic), new JValue(classification.IsItalic.Value));
            }
            if (classification.FontRenderingSize.HasValue)
            {
                jClassification.Add(nameof(classification.FontRenderingSize), new JValue(classification.FontRenderingSize.Value));
            }
            if (classification.IsEnabled.HasValue)
            {
                jClassification.Add(nameof(classification.IsEnabled), new JValue(classification.IsEnabled.Value));
            }
            return jClassification;
        }
    }
}