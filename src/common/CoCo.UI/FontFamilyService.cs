using System.Collections.Generic;
using System.Windows.Media;

namespace CoCo.UI
{
    public static class FontFamilyService
    {
        private static readonly SortedDictionary<string, FontFamily> _supportedFamilies;

        static FontFamilyService()
        {
            _supportedFamilies = new SortedDictionary<string, FontFamily>();
            foreach (var item in Fonts.SystemFontFamilies)
            {
                _supportedFamilies[item.Source] = item;
            }
        }

        public static IReadOnlyDictionary<string, FontFamily> SupportedFamilies => _supportedFamilies;
    }
}