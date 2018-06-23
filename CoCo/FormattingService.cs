using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using CoCo.Settings;
using CoCo.UI.Data;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class FormattingService
    {
        /// <summary>
        /// Returns normalized settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Settings.Settings SetFormatting(Settings.Settings settings)
        {
            var classificationManager = ClassificationManager.Instance;

            var classifications = classificationManager.GetClassifications();
            var classificationFormatMap = classificationManager.FormatMapService.GetClassificationFormatMap(category: "text");
            var defaultFormatting = classificationFormatMap.GetExplicitTextProperties(classificationManager.DefaultClassification);
            var presets = PresetService.GetDefaultPresets(defaultFormatting);

            var classificationsMap = new Dictionary<string, IClassificationType>(23);
            var settingsCopy = new Settings.Settings { Languages = new List<LanguageSettings>() };
            foreach (var pair in classifications)
            {
                var language = new LanguageSettings { LanguageName = pair.Key, };

                foreach (var item in pair.Value)
                {
                    classificationsMap.Add(item.Classification, item);
                }

                if (!presets.TryGetValue(language.LanguageName, out var languagePresets))
                {
                    languagePresets = new List<PresetSettings>();
                }
                var presetNames = languagePresets.ToDictionary(x => x.Name);

                var isLanguageExists = false;
                foreach (var languageSettings in settings.Languages)
                {
                    if (languageSettings.LanguageName.Equals(language.LanguageName))
                    {
                        isLanguageExists = true;
                        language.CurrentClassifications =
                            PrepareClassifications(classificationsMap.Keys, languageSettings.CurrentClassifications, defaultFormatting);
                        language.Presets = new List<PresetSettings>();

                        foreach (var presetSettings in languageSettings.Presets)
                        {
                            if (presetNames.ContainsKey(presetSettings.Name)) continue;

                            var presetCopy = new PresetSettings
                            {
                                Name = presetSettings.Name,
                                Classifications = PrepareClassifications(classificationsMap.Keys, presetSettings.Classifications, defaultFormatting)
                            };
                            language.Presets.Add(presetCopy);
                        }
                        break;
                    }
                }
                foreach (var preset in languagePresets)
                {
                    language.Presets.Add(new PresetSettings
                    {
                        Name = preset.Name,
                        Classifications = PrepareClassifications(classificationsMap.Keys, preset.Classifications, defaultFormatting)
                    });
                }
                if (!isLanguageExists)
                {
                    language.CurrentClassifications = PrepareClassifications(classificationsMap.Keys, Array.Empty<ClassificationSettings>(), defaultFormatting);
                }
                settingsCopy.Languages.Add(language);
            }

            foreach (var language in settingsCopy.Languages)
            {
                // TODO: do need to write in a log if the classification after preparing still not exists?
                if (!classifications.TryGetValue(language.LanguageName, out var t)) continue;
                foreach (var classification in language.CurrentClassifications)
                {
                    if (classificationsMap.TryGetValue(classification.Name, out var classificationType))
                    {
                        var formatting = classificationFormatMap.GetExplicitTextProperties(classificationType);
                        formatting = Apply(formatting, defaultFormatting, classification);
                        classificationFormatMap.SetExplicitTextProperties(classificationType, formatting);
                    }
                }
            }

            return settingsCopy;
        }

        /// <summary>
        /// Apply the default values for <paramref name="defaultFormatting"/> to classifications fields
        /// if classification doesn't exist in <paramref name="classificationsSettings"/>  or if it values don't exist
        /// </summary>
        private static List<ClassificationSettings> PrepareClassifications(
            IEnumerable<string> classificationNames,
            ICollection<ClassificationSettings> classificationsSettings,
            TextFormattingRunProperties defaultFormatting)
        {
            var classifications = new List<ClassificationSettings>();
            foreach (var name in classificationNames)
            {
                var isClassificationExists = false;
                foreach (var classificationSettings in classificationsSettings)
                {
                    if (classificationSettings.Name.Equals(name))
                    {
                        isClassificationExists = true;
                        classifications.Add(PrepareClassification(classificationSettings, defaultFormatting));
                        break;
                    }
                }
                if (isClassificationExists) continue;

                classifications.Add(defaultFormatting.ToSettings(name));
            }
            return classifications;
        }

        /// <summary>
        /// Apply default values from <paramref name="defaultFormatting"/> to classification fields that don't exist.
        /// </summary>
        private static ClassificationSettings PrepareClassification(
            ClassificationSettings classificationSettings, TextFormattingRunProperties defaultFormatting)
        {
            if (!classificationSettings.Background.HasValue)
            {
                classificationSettings.Background = defaultFormatting.BackgroundBrush.GetColor();
            }
            if (!classificationSettings.Foreground.HasValue)
            {
                classificationSettings.Foreground = defaultFormatting.ForegroundBrush.GetColor();
            }
            if (!classificationSettings.IsBold.HasValue)
            {
                classificationSettings.IsBold = defaultFormatting.Bold;
            }
            if (!classificationSettings.IsItalic.HasValue)
            {
                classificationSettings.IsItalic = defaultFormatting.Italic;
            }
            if (!classificationSettings.FontRenderingSize.HasValue)
            {
                classificationSettings.FontRenderingSize = (int)defaultFormatting.FontRenderingEmSize;
            }
            if (!classificationSettings.IsEnabled.HasValue)
            {
                classificationSettings.IsEnabled = true;
            }
            return classificationSettings;
        }

        public static void SetFormatting(Option option)
        {
            var classifications = ClassificationManager.Instance.GetClassifications();
            var classificationFormatMap = ClassificationManager.Instance.FormatMapService.GetClassificationFormatMap(category: "text");

            var classificationsSettings = new Dictionary<string, Classification>(23);
            foreach (var language in option.Languages)
            {
                foreach (var classification in language.Classifications)
                {
                    classificationsSettings.Add(classification.Name, classification);
                }
            }

            foreach (var languageClassifications in classifications)
            {
                foreach (var classificationType in languageClassifications.Value)
                {
                    if (classificationsSettings.TryGetValue(classificationType.Classification, out var classificationSettings))
                    {
                        var formatting = classificationFormatMap.GetExplicitTextProperties(classificationType);
                        formatting = Apply(formatting, classificationSettings);
                        classificationFormatMap.SetExplicitTextProperties(classificationType, formatting);
                    }
                    else
                    {
                        // use default values
                    }
                }
            }
        }

        private static TextFormattingRunProperties Apply(
            TextFormattingRunProperties formatting, TextFormattingRunProperties defaultFormatting, ClassificationSettings settings)
        {
            // NOTE: avoid creating a new instance for fields that weren't changed
            var isItalic = settings.IsItalic ?? defaultFormatting.Italic;
            if (formatting.Italic != isItalic)
            {
                formatting = formatting.SetItalic(isItalic);
            }

            var isBold = settings.IsBold ?? defaultFormatting.Bold;
            if (formatting.Bold != isBold)
            {
                formatting = formatting.SetBold(isBold);
            }

            var fontRenderingSize = settings.FontRenderingSize ?? defaultFormatting.FontRenderingEmSize;
            if (Math.Abs(formatting.FontRenderingEmSize - fontRenderingSize) > 0.001)
            {
                formatting = formatting.SetFontRenderingEmSize(fontRenderingSize);
            }

            if (settings.Background.HasValue)
            {
                if (!(formatting.BackgroundBrush is SolidColorBrush backgroundBrush) ||
                    !backgroundBrush.Color.Equals(settings.Background.Value))
                {
                    formatting = formatting.SetBackgroundBrush(new SolidColorBrush(settings.Background.Value));
                }
            }
            else
            {
                formatting = formatting.SetBackgroundBrush(defaultFormatting.BackgroundBrush);
            }

            if (settings.Foreground.HasValue)
            {
                if (!(formatting.ForegroundBrush is SolidColorBrush foregroundBrush) ||
                    !foregroundBrush.Color.Equals(settings.Foreground.Value))
                {
                    formatting = formatting.SetForegroundBrush(new SolidColorBrush(settings.Foreground.Value));
                }
            }
            else
            {
                formatting = formatting.SetForegroundBrush(defaultFormatting.ForegroundBrush);
            }

            return formatting;
        }

        private static TextFormattingRunProperties Apply(TextFormattingRunProperties formatting, Classification classification)
        {
            // NOTE: avoid creating a new instance for fields that weren't changed
            if (formatting.Italic != classification.IsItalic)
            {
                formatting = formatting.SetItalic(classification.IsItalic);
            }
            if (formatting.Bold != classification.IsBold)
            {
                formatting = formatting.SetBold(classification.IsBold);
            }
            if (Math.Abs(formatting.FontRenderingEmSize - classification.FontRenderingSize) > 0.001)
            {
                formatting = formatting.SetFontRenderingEmSize(classification.FontRenderingSize);
            }
            if (!(formatting.BackgroundBrush is SolidColorBrush backgroundBrush) ||
                !backgroundBrush.Color.Equals(classification.Background))
            {
                formatting = formatting.SetBackgroundBrush(new SolidColorBrush(classification.Background));
            }
            if (!(formatting.ForegroundBrush is SolidColorBrush foregroundBrush) ||
                !foregroundBrush.Color.Equals(classification.Background))
            {
                formatting = formatting.SetForegroundBrush(new SolidColorBrush(classification.Foreground));
            }
            return formatting;
        }
    }
}