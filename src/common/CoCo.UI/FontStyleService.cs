using System.Collections.Generic;
using System.Windows;

namespace CoCo.UI
{
    public static class FontStyleService
    {
        public const string Italic = "Italic";
        public const string Normal = "Normal";

        public static IDictionary<string, FontStyle> SupportedStyleByNames = new SortedDictionary<string, FontStyle>
        {
            ["Italic"] = FontStyles.Italic,
            ["Oblique"] = FontStyles.Oblique,
            ["Normal"] = FontStyles.Normal,
        };

        public static IDictionary<FontStyle, string> SupportedStyles = new Dictionary<FontStyle, string>
        {
            [FontStyles.Italic] = "Italic",
            [FontStyles.Oblique] = "Oblique",
            [FontStyles.Normal] = "Normal",
        };
    }
}