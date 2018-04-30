using System.Windows.Media;

namespace CoCo.UI.Models
{
    public interface IClassificationModel
    {
        bool IsBold { get; set; }

        bool IsItalic { get; set; }

        Color Foreground { get; set; }

        Color Background { get; set; }

        bool IsEnabled { get; set; }

        string DisplayName { get; }
    }
}