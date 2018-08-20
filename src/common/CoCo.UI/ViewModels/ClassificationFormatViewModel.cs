using System.Windows.Forms;
using System.Windows.Media;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class ClassificationFormatViewModel : BaseViewModel
    {
        private readonly IResetValuesProvider _resetValuesProvider;
        private readonly string _classificationName;

        private bool _foregroundWasReset;
        private bool _backgroundWasReset;
        private bool _fontRenderingSizeWasReset;

        public ClassificationFormatViewModel(Classification classification, IResetValuesProvider resetValuesProvider)
        {
            _resetValuesProvider = resetValuesProvider;

            _classificationName = classification.Name;
            _isEnabled = classification.IsEnabled;
            _isBold = classification.IsBold;
            _isItalic = classification.IsItalic;
            _foreground = classification.Foreground;
            _background = classification.Background;
            _fontRenderingSize = classification.FontRenderingSize;

            _foregroundWasReset = classification.ForegroundWasReset;
            _backgroundWasReset = classification.BackgroundWasReset;
            _fontRenderingSizeWasReset = classification.FontRenderingSizeWasReset;

            DisplayName = classification.DisplayName;

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
            ResetForeground = new DelegateCommand(() =>
            {
                Foreground = _resetValuesProvider.Foreground;
                _foregroundWasReset = true;
            });
            ResetBackground = new DelegateCommand(() =>
            {
                Background = _resetValuesProvider.Background;
                _backgroundWasReset = true;
            });
            ResetFontRenderingSize = new DelegateCommand(() =>
            {
                SetProperty(ref _fontRenderingSize, _resetValuesProvider.FontRenderingSize, nameof(Size));
                _fontRenderingSizeWasReset = true;
            });
        }

        public DelegateCommand CustomizeForeground { get; }

        public DelegateCommand CustomizeBackground { get; }

        public DelegateCommand ResetForeground { get; }

        public DelegateCommand ResetBackground { get; }

        public DelegateCommand ResetFontRenderingSize { get; }

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
                    _fontRenderingSizeWasReset = false;
                }
                RaisePropertyChanged();
            }
        }

        public string DisplayName { get; }

        private Color _foreground;

        public Color Foreground
        {
            get => _foreground;
            set
            {
                SetProperty(ref _foreground, value);
                _foregroundWasReset = false;
            }
        }

        private Color _background;

        public Color Background
        {
            get => _background;
            set
            {
                SetProperty(ref _background, value);
                _backgroundWasReset = false;
            }
        }

        public Classification ExtractData() => new Classification(_classificationName, DisplayName)
        {
            Background = Background,
            Foreground = Foreground,
            IsBold = IsBold,
            IsItalic = IsItalic,
            FontRenderingSize = _fontRenderingSize,
            IsEnabled = IsChecked,

            ForegroundWasReset = _foregroundWasReset,
            BackgroundWasReset = _backgroundWasReset,
            FontRenderingSizeWasReset = _fontRenderingSizeWasReset,
        };

        // TODO: would be a better solution to implement a custom color picker in wpf...
        // or move all logic of setter color from button to control code behind.
        private static bool TryGetColor(out Color color)
        {
            using (var dialog = new ColorDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var outColor = dialog.Color;
                    color = Color.FromArgb(outColor.A, outColor.R, outColor.G, outColor.B);
                    return true;
                }
                color = default;
                return false;
            }
        }
    }
}