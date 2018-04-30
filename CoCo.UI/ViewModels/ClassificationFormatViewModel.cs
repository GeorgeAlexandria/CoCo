using System.Windows.Forms;
using System.Windows.Media;
using CoCo.UI.Models;

namespace CoCo.UI.ViewModels
{
    public class ClassificationFormatViewModel : BaseViewModel
    {
        private readonly IClassificationModel _model;

        public ClassificationFormatViewModel(IClassificationModel model)
        {
            _model = model;

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

        public bool IsChecked
        {
            get => _model.IsEnabled;
            set
            {
                if (_model.IsEnabled != value)
                {
                    _model.IsEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsBold
        {
            get => _model.IsBold;
            set
            {
                if (_model.IsBold != value)
                {
                    _model.IsBold = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsItalic
        {
            get => _model.IsItalic;
            set
            {
                if (_model.IsItalic != value)
                {
                    _model.IsItalic = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _size;

        public string Size
        {
            get => _size;
            set
            {
                if (int.TryParse(value, out var size) && size > 0)
                {
                    _size = value;
                }
                RaisePropertyChanged();
            }
        }

        public string Name => _model.DisplayName;

        public Color Foreground
        {
            get => _model.Foreground;
            set
            {
                if (_model.Foreground.Equals(value))
                {
                    _model.Foreground = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Color Background
        {
            get => _model.Background;
            set
            {
                if (_model.Background.Equals(value))
                {
                    _model.Background = value;
                    RaisePropertyChanged();
                }
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