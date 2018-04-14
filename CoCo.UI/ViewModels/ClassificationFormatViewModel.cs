using System.Collections.Generic;
using System.Windows.Media;

namespace CoCo.UI.ViewModels
{
    public static class ClassificationFormatProvider
    {
        public static IEnumerable<ClassificationFormatViewModel> Get(string language)
        {
            return new[] { new ClassificationFormatViewModel(), new ClassificationFormatViewModel() };
        }
    }

    public class ClassificationFormatViewModel : BaseViewModel
    {
        public bool IsCheked { get; set; } = true;

        public string Name { get; set; } = "Sample";

        private Color _foreground = Color.FromRgb(128, 128, 128);

        public Color Foreground
        {
            get => _foreground;
            set
            {
                _foreground = value;
                RaisePropertyChanged();
            }
        }
    }
}