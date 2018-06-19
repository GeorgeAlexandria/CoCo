using System;
using System.Collections.Generic;
using System.Windows.Media;
using CoCo.Settings;
using CoCo.UI.Data;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class FormattingService
    {
        public static void SetFormatting(Settings.Settings settings)
        {
            var classifications = ClassificationManager.Instance.GetClassifications();
            var classificationFormatMap = ClassificationManager.Instance.FormatMapService.GetClassificationFormatMap(category: "text");

            var classificationsSettings = new Dictionary<string, ClassificationSettings>(23);
            foreach (var language in settings.Languages)
            {
                foreach (var classification in language.CurrentClassifications)
                {
                    if (!classificationsSettings.TryGetValue(classification.Name, out var t))
                    {
                        classificationsSettings.Add(classification.Name, classification);
                    }
                }
            }

            var defaultFormatting =
                classificationFormatMap.GetExplicitTextProperties(ClassificationManager.Instance.DefaultClassification);
            foreach (var classificationType in classifications)
            {
                if (classificationsSettings.TryGetValue(classificationType.Classification, out var classificationSettings))
                {
                    var formatting = classificationFormatMap.GetExplicitTextProperties(classificationType);
                    formatting = Apply(formatting, defaultFormatting, classificationSettings);
                    classificationFormatMap.SetExplicitTextProperties(classificationType, formatting);
                }
                else
                {
                    classificationFormatMap.SetExplicitTextProperties(classificationType, defaultFormatting);
                }
            }
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

            foreach (var classificationType in classifications)
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