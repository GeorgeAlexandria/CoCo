using System;
using System.Collections.Generic;
using System.Windows;
using CoCo.Analyser;
using CoCo.Analyser.Classifications;
using CoCo.Editor;
using CoCo.Settings;
using CoCo.UI;
using CoCo.UI.Data;
using CoCo.Utils;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class OptionService
    {
        /// <summary>
        /// Converts <paramref name="option"/> to <see cref="EditorSettings"/>
        /// </summary>
        public static EditorSettings ToSettings(UI.Data.ClassificationData option)
        {
            List<ClassificationSettings> ToSettings(ICollection<Classification> classifications)
            {
                var classificationSetings = new List<ClassificationSettings>(classifications.Count);
                foreach (var classification in classifications)
                {
                    classificationSetings.Add(OptionService.ToSettings(classification));
                }
                return classificationSetings;
            }

            var languagesSettings = new List<EditorLanguageSettings>(option.Languages.Count);
            foreach (var language in option.Languages)
            {
                var classificationsSettings = ToSettings(language.Classifications);

                if (!PresetService.GetDefaultPresets().TryGetValue(language.Name, out var defaultPresets))
                {
                    defaultPresets = null;
                }

                var presetsSettings = new List<PresetSettings>(language.Presets.Count);
                foreach (var preset in language.Presets)
                {
                    if (!(defaultPresets is null) && defaultPresets.ContainsKey(preset.Name)) continue;

                    presetsSettings.Add(new PresetSettings
                    {
                        Name = preset.Name,
                        Classifications = ToSettings(preset.Classifications)
                    });
                }

                languagesSettings.Add(new EditorLanguageSettings
                {
                    Name = language.Name,
                    CurrentClassifications = classificationsSettings,
                    Presets = presetsSettings
                });
            }
            return new EditorSettings { Languages = languagesSettings };
        }

        /// <summary>
        /// Converts <paramref name="option"/> to <see cref="GeneralSettings"/>
        /// </summary>
        public static GeneralSettings ToSettings(GeneralOption option)
        {
            var languageSettings = new List<GeneralLanguageSettings>(option.Languages.Count);
            foreach (var item in option.Languages)
            {
                languageSettings.Add(new GeneralLanguageSettings
                {
                    Name = item.Name,
                    QuickInfoState = item.QuickInfoState,
                    EditorState = item.EditorState,
                });
            }
            return new GeneralSettings { Languages = languageSettings };
        }

        /// <summary>
        /// Converts <paramref name="settings"/> to <see cref="UI.Data.ClassificationData"/> using a default values
        /// </summary>
        public static UI.Data.ClassificationData ToOption(EditorSettings settings)
        {
            var defaultPresets = PresetService.GetDefaultPresets();

            var option = new UI.Data.ClassificationData();
            foreach (var (languageName, classifications) in ClassificationManager.GetClassifications())
            {
                var language = new ClassificationLanguage(languageName);
                var languageClassifications = new List<(string, string)>(17);
                foreach (var item in classifications)
                {
                    languageClassifications.Add((item.Classification, GetDisplayName(item.Classification, languageName)));
                }

                if (!defaultPresets.TryGetValue(language.Name, out var defaultLanguagePresets))
                {
                    defaultLanguagePresets = null;
                }

                var isLanguageExists = false;
                foreach (var languageSettings in settings.Languages)
                {
                    // NOTE: pacth existings languages presets and classifications using default values
                    if (languageSettings.Name.Equals(language.Name))
                    {
                        isLanguageExists = true;
                        FillClassifications(languageClassifications, languageSettings.CurrentClassifications, language.Classifications);

                        foreach (var presetSettings in languageSettings.Presets)
                        {
                            // NOTE: skip CoCo default presets, they will be added below
                            if (!(defaultLanguagePresets is null) && defaultLanguagePresets.ContainsKey(presetSettings.Name)) continue;

                            var preset = new Preset(presetSettings.Name);
                            FillClassifications(languageClassifications, presetSettings.Classifications, preset.Classifications);
                            language.Presets.Add(preset);
                        }
                        break;
                    }
                }

                // NOTE: add CoCo default presets
                foreach (var defaultPreset in defaultLanguagePresets.Values)
                {
                    var preset = new Preset(defaultPreset.Name);
                    FillClassifications(languageClassifications, defaultPreset.Classifications, preset.Classifications);
                    language.Presets.Add(preset);
                }
                // NOTE: add default classifications
                if (!isLanguageExists)
                {
                    FillClassifications(languageClassifications, Array.Empty<ClassificationSettings>(), language.Classifications);
                }
                option.Languages.Add(language);
            }

            return option;
        }

        /// <summary>
        /// Converts <paramref name="settings"/> to <see cref="GeneralOption"/> using a default values
        /// </summary>
        public static GeneralOption ToOption(GeneralSettings settings)
        {
            var option = new GeneralOption();

            foreach (var language in new[] { Languages.CSharp, Languages.VisualBasic })
            {
                var generalLanguage = new GeneralLanguage(language);

                var languageExists = false;
                foreach (var item in settings.Languages)
                {
                    if (item.Name.Equals(language))
                    {
                        languageExists = true;

                        generalLanguage.QuickInfoState = item.QuickInfoState.HasValue &&
                            QuickInfoStateService.SupportedState.ContainsKey(item.QuickInfoState.Value)
                                ? item.QuickInfoState.Value
                                : (int)GeneralChangingService.Instance.GetDefaultQuickInfoState();

                        generalLanguage.EditorState = item.EditorState.HasValue &&
                            EditorStateService.SupportedState.ContainsKey(item.EditorState.Value)
                                ? item.EditorState.Value
                                : (int)GeneralChangingService.Instance.GetDefaultEditorState();

                        option.Languages.Add(generalLanguage);
                    }
                }

                if (!languageExists)
                {
                    generalLanguage.QuickInfoState = (int)GeneralChangingService.Instance.GetDefaultQuickInfoState();
                    generalLanguage.EditorState = (int)GeneralChangingService.Instance.GetDefaultEditorState();
                    option.Languages.Add(generalLanguage);
                }
            }

            return option;
        }

        /// <summary>
        /// Converts an existing <paramref name="classificationsSettings"/> to <see cref="Classification"/> and appends them to
        /// <paramref name="classifications"/>, also create <see cref="Classification"/> from non existing classifications in
        /// <paramref name="classificationsSettings"/> set the default values of fields
        /// </summary>
        /// <param name="classificationNames">string * string -> name * display name</param>
        private static void FillClassifications(
            IEnumerable<(string, string)> classificationNames,
            ICollection<ClassificationSettings> classificationsSettings,
            ICollection<Classification> classifications)
        {
            var defaultIdentifierFormatting = FormattingService.GetDefaultIdentifierFormatting();
            foreach (var (name, displayName) in classificationNames)
            {
                var defaultFormatting = defaultIdentifierFormatting;
                if (ClassificationManager.TryGetDefaultNonIdentifierClassification(name, out var classification))
                {
                    defaultFormatting = FormattingService.GetDefaultFormatting(classification);
                }

                var isClassificationExists = false;
                foreach (var classificationSettings in classificationsSettings)
                {
                    if (classificationSettings.Name.Equals(name))
                    {
                        isClassificationExists = true;
                        classifications.Add(ToClassification(classificationSettings, defaultFormatting, displayName));
                        break;
                    }
                }
                if (isClassificationExists) continue;

                /// NOTE: Don't set classifications settings field that can be reset,
                /// because they will be handled correctly in the <see cref="ToClassification"/>
                var settings = defaultFormatting.ToDefaultSettings(name);
                classifications.Add(ToClassification(settings, defaultFormatting, displayName));
            }
        }

        /// <summary>
        /// Converts <paramref name="classificationSettings"/> to <see cref="Classification"/> setting the default values
        /// of non exist classifications fields using <paramref name="defaultFormatting"/>
        /// </summary>
        private static Classification ToClassification(
            in ClassificationSettings classificationSettings, TextFormattingRunProperties defaultFormatting, string displayName)
        {
            var classification = new Classification(classificationSettings.Name, displayName);

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

            if (string.IsNullOrWhiteSpace(classificationSettings.FontFamily) ||
                !FontFamilyService.SupportedFamilies.ContainsKey(classificationSettings.FontFamily))
            {
                classification.FontFamily = defaultFormatting.GetFontFamily();
            }
            else
            {
                classification.FontFamily = classificationSettings.FontFamily;
            }

            classification.IsBold = classificationSettings.IsBold ?? defaultFormatting.Bold;

            if (string.IsNullOrWhiteSpace(classificationSettings.FontStyle) ||
                !FontStyleService.SupportedStyleByNames.ContainsKey(classificationSettings.FontStyle))
            {
                classification.FontStyle = defaultFormatting.GetFontStyleName();
            }
            else
            {
                classification.FontStyle = classificationSettings.FontStyle;
            }

            if (!classificationSettings.FontStretch.HasValue ||
                !FontStretchService.SupportedStretches.ContainsKey(classificationSettings.FontStretch.Value))
            {
                classification.FontStretch = defaultFormatting.GetFontStretch();
            }
            else
            {
                classification.FontStretch = classificationSettings.FontStretch.Value;
            }

            classification.IsOverline = classificationSettings.IsOverline ??
                defaultFormatting.TextDecorations.Contains(TextDecorations.OverLine[0]);
            classification.IsUnderline = classificationSettings.IsUnderline ??
                defaultFormatting.TextDecorations.Contains(TextDecorations.Underline[0]);
            classification.IsStrikethrough = classificationSettings.IsStrikethrough ??
                defaultFormatting.TextDecorations.Contains(TextDecorations.Strikethrough[0]);
            classification.IsBaseline = classificationSettings.IsBaseline ??
                defaultFormatting.TextDecorations.Contains(TextDecorations.Baseline[0]);

            var defaultOption = ClassificationService.GetDefaultOption(classification.Name);

            classification.IsDisabled = classificationSettings.IsDisabled ?? defaultOption.IsDisabled;
            classification.IsDisabledInXml = classificationSettings.IsDisabledInXml ?? defaultOption.IsDisabledInXml;
            classification.IsDisabledInEditor = classificationSettings.IsDisabledInEditor ?? defaultOption.IsDisabledInEditor;
            classification.IsDisabledInQuickInfo = classificationSettings.IsDisabledInQuickInfo ?? defaultOption.IsDisabledInQuickInfo;

            return classification;
        }

        /// <summary>
        /// Converts the input <paramref name="name"/> to corresponding display name using <paramref name="language"/>
        /// </summary>
        private static string GetDisplayName(this string name, string language)
        {
            // NOTE: "CoCo language " - 5 + |language| + 1
            var preffixLength = 5 + 1 +
                (language.Equals(Languages.CSharp) ? 6 :
                language.Equals(Languages.VisualBasic) ? 12 :
                language.Length);

            if (name.Length < preffixLength + 5) throw new ArgumentException("Name must contains more than 11 characters");

            var builder = StringBuilderCache.Acquire();

            // NOTE: skip "CoCo language " prefix and upper the first char
            builder.Append(char.ToUpper(name[preffixLength]));

            // NOTE: append all remaining characters excluding the " name" suffix
            for (int i = preffixLength + 1; i < name.Length - 5; ++i)
            {
                builder.Append(name[i]);
            }
            return StringBuilderCache.Release(builder);
        }

        private static ClassificationSettings ToSettings(Classification classification)
        {
            var settings = new ClassificationSettings
            {
                Name = classification.Name,
                FontFamily = classification.FontFamily,
                IsBold = classification.IsBold,
                FontStyle = classification.FontStyle,
                FontStretch = classification.FontStretch,
                IsOverline = classification.IsOverline,
                IsUnderline = classification.IsUnderline,
                IsStrikethrough = classification.IsStrikethrough,
                IsBaseline = classification.IsBaseline,
                IsDisabled = classification.IsDisabled,
                IsDisabledInXml = classification.IsDisabledInXml,
                IsDisabledInEditor = classification.IsDisabledInEditor,
                IsDisabledInQuickInfo = classification.IsDisabledInQuickInfo,
            };

            if (!classification.BackgroundWasReset)
            {
                settings.Background = classification.Background;
            }
            if (!classification.ForegroundWasReset)
            {
                settings.Foreground = classification.Foreground;
            }
            if (!classification.FontRenderingSizeWasReset)
            {
                settings.FontRenderingSize = classification.FontRenderingSize;
            }
            return settings;
        }
    }
}