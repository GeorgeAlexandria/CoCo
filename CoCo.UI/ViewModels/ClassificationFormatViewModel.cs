using System.Windows.Forms;
using System.Windows.Media;
using CoCo.UI.Models;

namespace CoCo.UI.ViewModels
{
    public class ClassificationFormatViewModel : BaseViewModel
    {
        private readonly string _modelName;

        public ClassificationFormatViewModel(ClassificationModel model)
        {
            _modelName = model.Name;
            _isEnabled = model.IsEnabled;
            _isBold = model.IsBold;
            _isItalic = model.IsItalic;
            _foreground = model.Foreground;
            _background = model.Background;
            _fontRenderingSize = model.FontRenderingSize;
            DisplayName = model.DisplayName;

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

        public DelegateCommand CustomizeForeground { get; }

        public DelegateCommand CustomizeBackground { get; }

        private bool _isEnabled;

        public bool IsChecked
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private bool _isBold;

        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        private bool _isItalic;

        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }

        private int _fontRenderingSize;

        public string Size
        {
            get => $"{_fontRenderingSize}";
            set
            {
                /// NOTE: <see cref="Size"/> is string, so it can contains incorrect input data –
                /// avoid this modification and notify UI to get a data again from view model
                if (int.TryParse(value, out var size) && size > 0)
                {
                    _fontRenderingSize = size;
                }
                RaisePropertyChanged();
            }
        }

        public string DisplayName { get; }

        private Color _foreground;

        public Color Foreground
        {
            get => _foreground;
            set => SetProperty(ref _foreground, value);
        }

        private Color _background;

        public Color Background
        {
            get => _background;
            set => SetProperty(ref _background, value);
        }

        public ClassificationModel SaveToModel() => new ClassificationModel(_modelName, DisplayName)
        {
            Background = Background,
            Foreground = Foreground,
            IsBold = IsBold,
            IsItalic = IsItalic,
            FontRenderingSize = _fontRenderingSize,
            IsEnabled = IsChecked
        };

        // TODO: would be a better solution to implement a custom color picker in wpf...
        // or move all logic of setter color from button to control code behind.
        private bool TryGetColor(out Color color)
        {
            using (var dialog = new ColorDialog())
            {
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
}