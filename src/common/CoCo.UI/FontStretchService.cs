using System.Collections.Generic;
using System.Windows;

namespace CoCo.UI
{
    public static class FontStretchService
    {
        public const int Normal = 5;

        public static IDictionary<int, FontStretch> SupportedStretches = new SortedDictionary<int, FontStretch>
        {
            [1] = FontStretches.UltraCondensed,
            [2] = FontStretches.ExtraCondensed,
            [3] = FontStretches.Condensed,
            [4] = FontStretches.SemiCondensed,
            [5] = FontStretches.Normal,
            [6] = FontStretches.SemiExpanded,
            [7] = FontStretches.Expanded,
            [8] = FontStretches.ExtraExpanded,
            [9] = FontStretches.UltraExpanded,
        };

        public static IDictionary<string, FontStretch> SupportedStretchByNames = new SortedDictionary<string, FontStretch>
        {
            ["Ultra Condensed"] = FontStretches.UltraCondensed,
            ["Extra Condensed"] = FontStretches.ExtraCondensed,
            ["Condensed"] = FontStretches.Condensed,
            ["Semi Condensed"] = FontStretches.SemiCondensed,
            ["Normal"] = FontStretches.Normal,
            ["Semi Expanded"] = FontStretches.SemiExpanded,
            ["Expanded"] = FontStretches.Expanded,
            ["Extra Expanded"] = FontStretches.ExtraExpanded,
            ["Ultra Expanded"] = FontStretches.UltraExpanded,
        };

        public static IDictionary<int, string> SupportedStretchNames = new SortedDictionary<int, string>
        {
            [1] = "Ultra Condensed",
            [2] = "Extra Condensed",
            [3] = "Condensed",
            [4] = "Semi Condensed",
            [5] = "Normal",
            [6] = "Semi Expanded",
            [7] = "Expanded",
            [8] = "Extra Expanded",
            [9] = "Ultra Expanded",
        };
    }
}