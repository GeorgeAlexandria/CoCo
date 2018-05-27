using System.Windows.Media;

namespace CoCo.UI.Models
{
    public class ClassificationModel
    {
        public ClassificationModel(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; }

        public bool IsBold { get; set; }

        public bool IsItalic { get; set; }

        public Color Foreground { get; set; }

        public Color Background { get; set; }

        public bool IsEnabled { get; set; }

        public string DisplayName { get; }

        public int FontRenderingSize { get; set; }
    }
}