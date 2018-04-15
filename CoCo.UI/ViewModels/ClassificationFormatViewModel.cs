using System.Collections.Generic;
using System.Windows.Forms;
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
        public ClassificationFormatViewModel()
        {
            CustomizeForeground = new DelegateCommand(() =>
            {
                if (TryGetColor(out var color))
                {
                    Foreground = color;
                }
            });
            CustomizeBackground = new DelegateCommand(() =>
            {
                if (TryGetColor(out var color))
                {
                    Background = color;
                }
            });
        }

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

        private Color _background = Color.FromRgb(128, 128, 128);

        public Color Background
        {
            get => _background;
            set
            {
                _background = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand CustomizeForeground { get; }

        public DelegateCommand CustomizeBackground { get; }

        // TODO: would be a better solution to implement a custom color picker in wpf...
        // or move all logic of setter color from button to control code behind.
        private bool TryGetColor(out Color color)
        {
            var dialog = new ColorDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var outColor = dialog.Color;
                color = Color.FromArgb(outColor.A, outColor.R, outColor.G, outColor.B);
                return true;
            }
            color = default(Color);
            return false;
        }
    }
}