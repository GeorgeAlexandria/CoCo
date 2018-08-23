using System.Globalization;
using System.Windows.Media;

namespace CoCo.Utils
{
    public static class ColorHelpers
    {
        /// <summary>
        /// Try to parse input <paramref name="value"/> to the out <paramref name="color"/>
        /// </summary>
        /// <returns>True if <paramref name="value"/> represented as ARGB else false</returns>
        public static bool TryParseColor(string value, out Color color)
        {
            byte ToByte(int integer, int offset) => (byte)(integer >> offset & 0xFF);

            // NOTE: ARGB - 8 chars
            if (value.Length == 8 && int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var res))
            {
                color = Color.FromArgb(ToByte(res, 24), ToByte(res, 16), ToByte(res, 8), ToByte(res, 0));
                return true;
            }

            color = new Color();
            return false;
        }
    }
}