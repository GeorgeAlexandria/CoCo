using System.Collections.Generic;
using System.Windows;

namespace CoCo.UI
{
    public static class FontStyleService
    {
        public static IDictionary<string, FontStyle> SupportedFontStyles = new SortedDictionary<string, FontStyle>
        {
            ["Italic"] = FontStyles.Italic,
            ["Oblique"] = FontStyles.Oblique,
            ["Normal"] = FontStyles.Normal,
        };
    }
}