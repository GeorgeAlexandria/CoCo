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

        public static IDictionary<string, FontStretch> SupportedStretchesByNames = new SortedDictionary<string, FontStretch>
        {
            ["UltraCondensed"] = FontStretches.UltraCondensed,
            ["ExtraCondensed"] = FontStretches.ExtraCondensed,
            ["Condensed"] = FontStretches.Condensed,
            ["SemiCondensed"] = FontStretches.SemiCondensed,
            ["Normal"] = FontStretches.Normal,
            ["SemiExpanded"] = FontStretches.SemiExpanded,
            ["Expanded"] = FontStretches.Expanded,
            ["ExtraExpanded"] = FontStretches.ExtraExpanded,
            ["UltraExpanded"] = FontStretches.UltraExpanded,
        };

        public static IDictionary<int, string> SupportedStretchesNames = new SortedDictionary<int, string>
        {
            [1] = "UltraCondensed",
            [2] = "ExtraCondensed",
            [3] = "Condensed",
            [4] = "SemiCondensed",
            [5] = "Normal",
            [6] = "SemiExpanded",
            [7] = "Expanded",
            [8] = "ExtraExpanded",
            [9] = "UltraExpanded",
        };
    }
}