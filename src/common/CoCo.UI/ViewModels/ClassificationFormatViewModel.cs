using System.Globalization;
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
            _foregroundText = _foreground.ToString();
            _background = classification.Background;
            _backgroundText = _background.ToString();
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

            ForegroundLostFocus = new DelegateCommand(SetForegroundText);
            BackgroundLostFocus = new DelegateCommand(SetBackgroundText);
        }

        public DelegateCommand CustomizeForeground { get; }

        public DelegateCommand CustomizeBackground { get; }

        public DelegateCommand ResetForeground { get; }

        public DelegateCommand ResetBackground { get; }

        public DelegateCommand ResetFontRenderingSize { get; }

        /// <remarks>
        /// It's used to restore foreground's text from the current foreground's value
        /// </remarks>
        public DelegateCommand ForegroundLostFocus { get; }

        /// <remarks>
        /// It's used to restore background's text from the current background's value
        /// </remarks>
        public DelegateCommand BackgroundLostFocus { get; }

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
                SetForeground(value);
                SetForegroundText();
            }
        }

        private string _foregroundText;

        public string ForegroundText
        {
            get => _foregroundText;
            set
            {
                if (TryParseColor(value, out var color))
                {
                    SetForeground(color);
                }
                SetProperty(ref _foregroundText, value);
            }
        }

        private string _backgroundText;

        public string BackgroundText
        {
            get => _backgroundText;
            set
            {
                if (TryParseColor(value, out var color))
                {
                    SetBackground(color);
                }
                SetProperty(ref _backgroundText, value);
            }
        }

        private Color _background;

        public Color Background
        {
            get => _background;
            set
            {
                SetBackground(value);
                SetBackgroundText();
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

        /// <summary>
        /// Sets only the color of foreground, raise on it and set state of reset
        /// </summary>
        private void SetForeground(Color value)
        {
            SetProperty(ref _foreground, value, nameof(Foreground));
            _foregroundWasReset = false;
        }

        /// <summary>
        /// Sets only the text of foreground and raise on it
        /// </summary>
        private void SetForegroundText() => SetProperty(ref _foregroundText, _foreground.ToString(), nameof(ForegroundText));

        /// <summary>
        /// Sets only the color of background, raise on it and set state of reset
        /// </summary>
        private void SetBackground(Color value)
        {
            SetProperty(ref _background, value, nameof(Background));
            _backgroundWasReset = false;
        }

        /// <summary>
        /// Sets only the text of background and raise on it
        /// </summary>
        private void SetBackgroundText() => SetProperty(ref _backgroundText, _background.ToString(), nameof(BackgroundText));

        private static bool TryParseColor(string value, out Color color)
        {
            byte ToByte(int integer, int offset) => (byte)(integer >> offset & 0xFF);

            // NOTE: #ARGB – 9 chars
            if (value.Length == 9)
            {
                value = value.Substring(1);
            }

            // NOTE: ARGB - 8 chars
            if (value.Length == 8 && int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var res))
            {
                color = Color.FromArgb(ToByte(res, 24), ToByte(res, 16), ToByte(res, 8), ToByte(res, 0));
                return true;
            }

            color = new Color();
            return false;
        }

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