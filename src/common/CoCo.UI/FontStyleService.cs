using System.Collections.Generic;
using System.Windows;

namespace CoCo.UI
{
    public static class FontStyleService
    {
        public const string Italic = "Italic";
        public const string Oblique = "Oblique";
        public const string Normal = "Normal";

        public static IDictionary<string, FontStyle> SupportedStyles = new SortedDictionary<string, FontStyle>
        {
            [Italic] = FontStyles.Italic,
            [Oblique] = FontStyles.Oblique,
            [Normal] = FontStyles.Normal,
        };
    }
}