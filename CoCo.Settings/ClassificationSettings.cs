using System.Diagnostics;
using System.Windows.Media;

namespace CoCo.Settings
{
    [DebuggerDisplay("{Name}")]
    public struct ClassificationSettings
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsBold { get; set; }

        public bool IsItalic { get; set; }

        public Color Foreground { get; set; }

        public Color Background { get; set; }

        public int FontRenderingSize { get; set; }

        public bool IsEnabled { get; set; }
    }
}