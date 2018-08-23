using System.Windows.Forms;
using System.Windows.Media;
using CoCo.Utils;

namespace CoCo.UI.ViewModels
{
    /// <summary>
    /// View model for classification's foreground and background
    /// </summary>
    public class ClassificationColorViewModel : BaseViewModel
    {
        public ClassificationColorViewModel(Color classificationColor, bool colorWasReset, IResetValuesProvider resetValuesProvider)
        {
            _color = classificationColor;
            _colorText = _color.ToString();

            ColorWasReset = colorWasReset;

            CustomizeColor = new DelegateCommand(() =>
            {
                if (TryGetColor(out var color))
                {
                    Color = color;
                }
            });
            ResetColor = new DelegateCommand(() =>
            {
                Color = resetValuesProvider.Foreground;
                ColorWasReset = true;
            });

            ColorLostFocus = new DelegateCommand(SetColorText);
        }

        public DelegateCommand CustomizeColor { get; }

        public DelegateCommand ResetColor { get; }

        /// <remarks>
        /// It's used to restore text from the current color's value
        /// when focused was lost to avoid non full or invalid input
        /// </remarks>
        public DelegateCommand ColorLostFocus { get; }

        public bool ColorWasReset { get; private set; }

        private Color _color;

        public Color Color
        {
            get => _color;
            set
            {
                SetColor(value);
                SetColorText();
            }
        }

        private string _colorText;

        public string ColorText
        {
            get => _colorText;
            set
            {
                if (TryParseColor(value, out var color))
                {
                    SetColor(color);
                }
                SetProperty(ref _colorText, value);
            }
        }

        /// <summary>
        /// Sets only color, raise on it and set state of reset
        /// </summary>
        private void SetColor(Color value)
        {
            SetProperty(ref _color, value, nameof(Color));
            ColorWasReset = false;
        }

        /// <summary>
        /// Sets only the text of color and raise on it
        /// </summary>
        private void SetColorText() => SetProperty(ref _colorText, _color.ToString(), nameof(ColorText));

        private static bool TryParseColor(string value, out Color color)
        {
            // NOTE: #ARGB – 9 chars
            return ColorHelpers.TryParseColor(value.Length == 9 ? value.Substring(1) : value, out color);
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