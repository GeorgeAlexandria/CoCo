using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using CoCo.Providers;
using CoCo.UI.Data;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo.Services
{
    public static class FormattingService
    {
        public static TextFormattingRunProperties GetDefaultFormatting(string classificationName)
        {
            var classificationFormatMap = ServicesProvider.Instance.FormatMapService.GetClassificationFormatMap(category: "text");
            if (ClassificationManager.TryGetDefaultNonIdentifierClassification(classificationName, out var defaultClassification))
            {
                return GetDefaultFormatting(classificationFormatMap, defaultClassification);
            }
            return GetDefaultFormatting(classificationFormatMap, ClassificationManager.DefaultIdentifierClassification);
        }

        public static TextFormattingRunProperties GetDefaultIdentifierFormatting()
        {
            var classificationFormatMap = ServicesProvider.Instance.FormatMapService.GetClassificationFormatMap(category: "text");
            return GetDefaultFormatting(classificationFormatMap, ClassificationManager.DefaultIdentifierClassification);
        }

        public static TextFormattingRunProperties GetDefaultFormatting(IClassificationType classification)
        {
            var classificationFormatMap = ServicesProvider.Instance.FormatMapService.GetClassificationFormatMap(category: "text");
            return GetDefaultFormatting(classificationFormatMap, classification);
        }

        public static void SetFormattingOptions(Option option)
        {
            var classificationTypes = ClassificationManager.GetClassifications();
            var classificationFormatMap = ServicesProvider.Instance.FormatMapService.GetClassificationFormatMap(category: "text");

            var classificationsMap = new Dictionary<string, IClassificationType>(23);
            foreach (var classifications in classificationTypes.Values)
            {
                foreach (var item in classifications)
                {
                    classificationsMap.Add(item.Classification, item);
                }
            }

            var defaultIdentifierFormatting =
                GetDefaultFormatting(classificationFormatMap, ClassificationManager.DefaultIdentifierClassification);

            foreach (var language in option.Languages)
            {
                // TODO: do need to write in a log if the classification after preparing still not exists?
                if (!classificationTypes.ContainsKey(language.Name)) continue;
                foreach (var classification in language.Classifications)
                {
                    if (classificationsMap.TryGetValue(classification.Name, out var classificationType))
                    {
                        var defaultFormatting = defaultIdentifierFormatting;
                        if (ClassificationManager.TryGetDefaultNonIdentifierClassification(
                            classification.Name, out var defaultClassification))
                        {
                            defaultFormatting = GetDefaultFormatting(classificationFormatMap, defaultClassification);
                        }

                        var formatting = classificationFormatMap.GetExplicitTextProperties(classificationType);
                        formatting = Apply(formatting, classification, defaultFormatting);
                        classificationFormatMap.SetExplicitTextProperties(classificationType, formatting);
                    }
                }
            }
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

            formatting = ApplyDecoration(formatting, classification.IsOverline, TextDecorations.OverLine[0]);
            formatting = ApplyDecoration(formatting, classification.IsUnderline, TextDecorations.Underline[0]);
            formatting = ApplyDecoration(formatting, classification.IsStrikethrough, TextDecorations.Strikethrough[0]);
            formatting = ApplyDecoration(formatting, classification.IsBaseline, TextDecorations.Baseline[0]);

            return formatting;
        }

        /// <summary>
        /// Try to add or remove <paramref name="decoration"/> to the <paramref name="formatting"/> using <paramref name="needToAddDecoration"/>
        /// </summary>
        /// <param name="formatting"></param>
        /// <param name="needToAddDecoration">Determines when the input <paramref name="decoration"/> should be added or removed</param>
        private static TextFormattingRunProperties ApplyDecoration(
            TextFormattingRunProperties formatting, bool needToAddDecoration, TextDecoration decoration)
        {
            if (formatting.TextDecorations.Contains(decoration) ^ needToAddDecoration)
            {
                // NOTE: directly creates a new instance from existing collection to correctly determines
                // in the future that items are contained or not
                var clone = new TextDecorationCollection(formatting.TextDecorations);
                if (needToAddDecoration)
                {
                    clone.Add(decoration);
                }
                else
                {
                    clone.Remove(decoration);
                }
                return formatting.SetTextDecorations(clone);
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