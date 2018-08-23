using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class ClassificationFormatViewModel : BaseViewModel
    {
        private readonly string _classificationName;

        private bool _fontRenderingSizeWasReset;

        public ClassificationFormatViewModel(Classification classification, IResetValuesProvider resetValuesProvider)
        {
            _classificationName = classification.Name;
            _isEnabled = classification.IsEnabled;
            _isBold = classification.IsBold;
            _isItalic = classification.IsItalic;
            _fontRenderingSize = classification.FontRenderingSize;

            Foreground = new ClassificationColorViewModel(
                classification.Foreground, classification.ForegroundWasReset, resetValuesProvider);
            Background = new ClassificationColorViewModel(
                classification.Background, classification.BackgroundWasReset, resetValuesProvider);

            _fontRenderingSizeWasReset = classification.FontRenderingSizeWasReset;

            DisplayName = classification.DisplayName;

            ResetFontRenderingSize = new DelegateCommand(() =>
            {
                SetProperty(ref _fontRenderingSize, resetValuesProvider.FontRenderingSize, nameof(Size));
                _fontRenderingSizeWasReset = true;
            });
        }

        public ClassificationColorViewModel Foreground { get; set; }

        public ClassificationColorViewModel Background { get; set; }

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

        public Classification ExtractData() => new Classification(_classificationName, DisplayName)
        {
            Background = Background.Color,
            Foreground = Foreground.Color,
            IsBold = IsBold,
            IsItalic = IsItalic,
            FontRenderingSize = _fontRenderingSize,
            IsEnabled = IsChecked,

            ForegroundWasReset = Foreground.ColorWasReset,
            BackgroundWasReset = Background.ColorWasReset,
            FontRenderingSizeWasReset = _fontRenderingSizeWasReset,
        };
    }
}