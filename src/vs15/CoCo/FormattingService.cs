using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using CoCo.Settings;
using CoCo.UI.Data;
using CoCo.Utils;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class FormattingService
    {
        /// <summary>
        /// Converts <paramref name="settings"/> to <see cref="Option"/> using a default values and applies them to formattings
        /// </summary>
        /// <returns>Converted option</returns>
        public static Option SetFormatting(Settings.Settings settings)
        {
            var classificationTypes = ClassificationManager.Instance.GetClassifications();
            var defaultFormatting = GetDefaultFormatting();

            var defaultPresets = PresetService.GetDefaultPresets(defaultFormatting);

            var classificationsMap = new Dictionary<string, IClassificationType>(23);
            var option = new Option();
            foreach (var (languageName, classifications) in classificationTypes)
            {
                var language = new Language(languageName);
                foreach (var item in classifications)
                {
                    classificationsMap.Add(item.Classification, item);
                }

                if (!defaultPresets.TryGetValue(language.Name, out var defaultLanguagePresets))
                {
                    defaultLanguagePresets = new List<PresetSettings>();
                }
                var presetNames = defaultLanguagePresets.ToDictionary(x => x.Name);

                List<ClassificationSettings> patchedClassifications;
                var isLanguageExists = false;
                foreach (var languageSettings in settings.Languages)
                {
                    // NOTE: pacth existings languages presets and classifications using default values
                    if (languageSettings.Name.Equals(language.Name))
                    {
                        isLanguageExists = true;
                        FillClassifications(
                            classificationsMap.Keys, languageSettings.CurrentClassifications, language.Classifications, defaultFormatting);

                        foreach (var presetSettings in languageSettings.Presets)
                        {
                            // NOTE: skip CoCo default presets, they will be added below
                            if (presetNames.ContainsKey(presetSettings.Name)) continue;

                            var preset = new Preset(presetSettings.Name);
                            FillClassifications(
                                classificationsMap.Keys, presetSettings.Classifications, preset.Classifications, defaultFormatting);
                            language.Presets.Add(preset);
                        }
                        break;
                    }
                }

                // NOTE: add CoCo default presets
                foreach (var defaultPreset in defaultLanguagePresets)
                {
                    var preset = new Preset(defaultPreset.Name);
                    FillClassifications(classificationsMap.Keys, defaultPreset.Classifications, preset.Classifications, defaultFormatting);
                    language.Presets.Add(preset);
                }
                // NOTE: add default classifications
                if (!isLanguageExists)
                {
                    FillClassifications(
                        classificationsMap.Keys, Array.Empty<ClassificationSettings>(), language.Classifications, defaultFormatting);
                }
                option.Languages.Add(language);
            }

            SetFormatting(option, classificationsMap);
            return option;
        }

        /// NOTE: assume that <param name="option"/> is correct input data from options pages
        public static void SetFormatting(Option option) => SetFormatting(option, null);

        public static TextFormattingRunProperties GetDefaultFormatting()
        {
            var classificationFormatMap = ServicesProvider.Instance.FormatMapService.GetClassificationFormatMap(category: "text");
            return GetDefaultFormatting(classificationFormatMap, ClassificationManager.Instance.DefaultClassification);
        }

        private static void SetFormatting(Option option, Dictionary<string, IClassificationType> classificationsMap = null)
        {
            var classificationTypes = ClassificationManager.Instance.GetClassifications();
            var classificationFormatMap = ServicesProvider.Instance.FormatMapService.GetClassificationFormatMap(category: "text");
            var defaultFormatting = GetDefaultFormatting(classificationFormatMap, ClassificationManager.Instance.DefaultClassification);

            if (classificationsMap is null)
            {
                classificationsMap = new Dictionary<string, IClassificationType>(23);
                foreach (var pair in classificationTypes)
                {
                    foreach (var item in pair.Value)
                    {
                        classificationsMap.Add(item.Classification, item);
                    }
                }
            }

            foreach (var language in option.Languages)
            {
                // TODO: do need to write in a log if the classification after preparing still not exists?
                if (!classificationTypes.ContainsKey(language.Name)) continue;
                foreach (var classification in language.Classifications)
                {
                    if (classificationsMap.TryGetValue(classification.Name, out var classificationType))
                    {
                        var formatting = classificationFormatMap.GetExplicitTextProperties(classificationType);
                        formatting = Apply(formatting, classification, defaultFormatting);
                        classificationFormatMap.SetExplicitTextProperties(classificationType, formatting);
                    }
                }
            }
        }

        /// <summary>
        /// Converts an existing <paramref name="classificationsSettings"/> to <see cref="Classification"/> and appends them to
        /// <paramref name="classifications"/>, also create <see cref="Classification"/> from non existing classifications in
        /// <paramref name="classificationsSettings"/> set the default values of fields using <paramref name="defaultFormatting"/>
        /// </summary>
        private static void FillClassifications(
            IEnumerable<string> classificationNames,
            ICollection<ClassificationSettings> classificationsSettings,
            ICollection<Classification> classifications,
            TextFormattingRunProperties defaultFormatting)
        {
            foreach (var name in classificationNames)
            {
                var isClassificationExists = false;
                foreach (var classificationSettings in classificationsSettings)
                {
                    if (classificationSettings.Name.Equals(name))
                    {
                        isClassificationExists = true;
                        classifications.Add(ToClassification(classificationSettings, defaultFormatting));
                        break;
                    }
                }
                if (isClassificationExists) continue;

                /// NOTE: Don't set classifications settings field that can be reset,
                /// because they will be handled correctly in the <see cref="ToClassification"/>
                var settings = defaultFormatting.ToDefaultSettings(name);
                classifications.Add(ToClassification(settings, defaultFormatting));
            }
        }

        /// <summary>
        /// Converts <paramref name="classificationSettings"/> to <see cref="Classification"/> setting the default values
        /// of non exist classifications fields using <paramref name="defaultFormatting"/>
        /// </summary>
        private static Classification ToClassification(
            in ClassificationSettings classificationSettings, TextFormattingRunProperties defaultFormatting)
        {
            // TODO: temporary the display name will equals it name
            var classification = new Classification(classificationSettings.Name, classificationSettings.Name);

            if (!classificationSettings.Background.HasValue)
            {
                classification.Background = defaultFormatting.BackgroundBrush.GetColor();
                classification.BackgroundWasReset = true;
            }
            else
            {
                classification.Background = classificationSettings.Background.Value;
            }

            if (!classificationSettings.Foreground.HasValue)
            {
                classification.Foreground = defaultFormatting.ForegroundBrush.GetColor();
                classification.ForegroundWasReset = true;
            }
            else
            {
                classification.Foreground = classificationSettings.Foreground.Value;
            }

            if (!classificationSettings.FontRenderingSize.HasValue)
            {
                classification.FontRenderingSize = (int)defaultFormatting.FontRenderingEmSize;
                classification.FontRenderingSizeWasReset = true;
            }
            else
            {
                classification.FontRenderingSize = classificationSettings.FontRenderingSize.Value;
            }

            classification.IsBold = classificationSettings.IsBold ?? defaultFormatting.Bold;
            classification.IsItalic = classificationSettings.IsItalic ?? defaultFormatting.Italic;

            if (!classificationSettings.IsEnabled.HasValue)
            {
                classification.IsEnabled = true;
            }
            return classification;
        }

        private static TextFormattingRunProperties Apply(
            TextFormattingRunProperties formatting,
            Classification classification,
            TextFormattingRunProperties defaultFormatting)
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

            if (classification.FontRenderingSizeWasReset)
            {
                /// NOTE: we should not try to set property of <param name="defaultFormatting" /> if it's marked as empty
                /// to avoid set which will mark property of <param name="formatting" /> as non empty
                formatting = defaultFormatting.FontRenderingEmSizeEmpty
                    ? formatting.ClearFontRenderingEmSize()
                    : formatting.SetFontHintingEmSize(defaultFormatting.FontRenderingEmSize);
            }
            else if (Math.Abs(formatting.FontRenderingEmSize - classification.FontRenderingSize) > 0.001)
            {
                formatting = formatting.SetFontRenderingEmSize(classification.FontRenderingSize);
            }

            if (classification.BackgroundWasReset)
            {
                /// NOTE: we should not try to set a some of value from <param name="defaultFormatting" /> if it's marked as empty
                /// to avoid set that will mark value from <param name="formatting" /> as non empty
                formatting = defaultFormatting.BackgroundBrushEmpty
                    ? formatting.ClearBackgroundBrush()
                    : formatting.SetBackground(defaultFormatting.BackgroundBrush.GetColor());
            }
            else if (!(formatting.BackgroundBrush is SolidColorBrush backgroundBrush) ||
                !backgroundBrush.Color.Equals(classification.Background))
            {
                formatting = formatting.SetBackgroundBrush(new SolidColorBrush(classification.Background));
            }

            if (classification.ForegroundWasReset)
            {
                // NOTE: Foreground always is set, just look at
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.text.classification.iclassificationformatmap.defaulttextproperties?view=visualstudiosdk-2017#remarks
                formatting = formatting.SetForeground(defaultFormatting.ForegroundBrush.GetColor());
            }
            else if (!(formatting.ForegroundBrush is SolidColorBrush foregroundBrush) ||
                !foregroundBrush.Color.Equals(classification.Foreground))
            {
                formatting = formatting.SetForegroundBrush(new SolidColorBrush(classification.Foreground));
            }
            return formatting;
        }

        private static TextFormattingRunProperties GetDefaultFormatting(
            IClassificationFormatMap classificationFormatMap,
            IClassificationType defaultClassification)
        {
            var defaultFormatting = classificationFormatMap.GetExplicitTextProperties(defaultClassification);

            // NOTE: Should use the default colors of all formattings if default formatting doesn't have explicitly set values
            if (defaultFormatting.BackgroundBrushEmpty)
            {
                // NOTE: if default background is not set we just leave the empty background => vs will use the transparent background
                // for this classification
                if (!classificationFormatMap.DefaultTextProperties.BackgroundBrushEmpty)
                {
                    defaultFormatting = defaultFormatting.SetBackgroundBrush(classificationFormatMap.DefaultTextProperties.BackgroundBrush);
                }
            }
            if (defaultFormatting.ForegroundBrushEmpty)
            {
                // NOTE: Foreground always is set, just look at
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.text.classification.iclassificationformatmap.defaulttextproperties?view=visualstudiosdk-2017#remarks
                defaultFormatting = defaultFormatting.SetForegroundBrush(classificationFormatMap.DefaultTextProperties.ForegroundBrush);
            }
            return defaultFormatting;
        }
    }
}