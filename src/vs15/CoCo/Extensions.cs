﻿using System.Windows;
using System.Windows.Media;
using CoCo.Analyser;
using CoCo.Settings;
using CoCo.UI;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class Extensions
    {
        /// <summary>
        /// Creates the default classification from <paramref name="formatting"/>
        /// which doesn't set the default values for properties that can be reset
        /// </summary>
        public static ClassificationSettings ToDefaultSettings(
           this TextFormattingRunProperties formatting, string classificationName)
        {
            var defaultOption = ClassificationService.GetDefaultOption(classificationName);
            return new ClassificationSettings
            {
                Name = classificationName,
                IsBold = formatting.Bold,
                FontStyle = formatting.GetFontStyleName(),
                IsOverline = formatting.TextDecorations.Contains(TextDecorations.OverLine[0]),
                IsUnderline = formatting.TextDecorations.Contains(TextDecorations.Underline[0]),
                IsStrikethrough = formatting.TextDecorations.Contains(TextDecorations.Strikethrough[0]),
                IsBaseline = formatting.TextDecorations.Contains(TextDecorations.Baseline[0]),
                IsDisabled = defaultOption.IsDisabled,
                IsDisabledInXml = defaultOption.IsDisabledInXml,
            };
        }

        /// <summary>
        /// Returns the relevant font style name for <paramref name="formatting"/> if it exists or the fallback name
        /// </summary>
        public static string GetFontStyleName(this TextFormattingRunProperties formatting)
        {
            if (formatting.Italic) return "Italic";
            var styleName = formatting.Typeface.Style.ToString();
            return FontStyleService.SupportedFontStyles.ContainsKey(styleName) ? styleName : "Normal";
        }

        /// <summary>
        /// Gets <see cref="SolidColorBrush.Color"/> if <paramref name="brush"/> is <see cref="SolidColorBrush"/>
        /// or <see cref="Colors.Black"/>
        /// </summary>
        public static Color GetColor(this Brush brush) => brush is SolidColorBrush colorBrush ? colorBrush.Color : Colors.Black;
    }
}