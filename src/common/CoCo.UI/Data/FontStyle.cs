using System.Diagnostics;

namespace CoCo.UI.Data
{
    [DebuggerDisplay("{Name}")]
    public struct FontStyle
    {
        public FontStyle(string name, System.Windows.FontStyle style)
        {
            Name = name;
            Style = style;
        }

        public string Name;

        public System.Windows.FontStyle Style;
    }
}