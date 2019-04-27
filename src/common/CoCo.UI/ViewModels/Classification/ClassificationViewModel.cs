using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class ClassificationViewModel : BaseViewModel
    {
        private readonly string _classificationName;

        private bool _fontRenderingSizeWasReset;

        public ClassificationViewModel(Classification classification, IResetValuesProvider resetValuesProvider)
        {
            _classificationName = classification.Name;
            _isDisabled = classification.IsDisabled;
            _IsDisabledInEditor = classification.IsDisabledInEditor;
            _IsDisabledInQuickInfo = classification.IsDisabledInQuickInfo;
            _isDisabledInXml = classification.IsDisabledInXml;
            _isBold = classification.IsBold;
            _isOverline = classification.IsOverline;
            _isUnderline = classification.IsUnderline;
            _isStrikethrough = classification.IsStrikethrough;
            _isBaseLine = classification.IsBaseline;
            _fontRenderingSize = classification.FontRenderingSize;

            FontFamiliesContainer = new FontFamiliesViewModel(classification.FontFamily);
            FontStylesContainer = new FontStylesViewModel(classification.FontStyle, classification.FontFamily);
            FontStretchesContainer = new FontStretchesViewModel(
                classification.FontStretch, classification.FontFamily, classification.FontStyle);

            FontFamiliesContainer.PropertyChanged += FontStylesContainer.OnSelectedFontFamilyChanged;
            FontFamiliesContainer.PropertyChanged += FontStretchesContainer.OnSelectedFontFamilyChanged;
            FontStylesContainer.PropertyChanged += FontStretchesContainer.OnSelectedFontStyleChanged;

            Foreground = new ClassificationColorViewModel(
                _classificationName, classification.Foreground, classification.ForegroundWasReset, resetValuesProvider);
            Background = new ClassificationColorViewModel(
                _classificationName, classification.Background, classification.BackgroundWasReset, resetValuesProvider);

            _fontRenderingSizeWasReset = classification.FontRenderingSizeWasReset;

            DisplayName = classification.DisplayName;

            ResetFontRenderingSize = new DelegateCommand(() =>
            {
                SetProperty(ref _fontRenderingSize, resetValuesProvider.GetFontRenderingSize(_classificationName), nameof(Size));
                _fontRenderingSizeWasReset = true;
            });
        }

        public FontStylesViewModel FontStylesContainer { get; }

        public FontFamiliesViewModel FontFamiliesContainer { get; }

        public FontStretchesViewModel FontStretchesContainer { get; }

        public ClassificationColorViewModel Foreground { get; set; }

        public ClassificationColorViewModel Background { get; set; }

        public DelegateCommand ResetFontRenderingSize { get; }

        private bool _isDisabled;

        public bool IsDisabled
        {
            get => _isDisabled;
            set => SetProperty(ref _isDisabled, value);
        }

        private bool _IsDisabledInEditor;

        public bool IsDisabledInEditor
        {
            get => _IsDisabledInEditor;
            set => SetProperty(ref _IsDisabledInEditor, value);
        }

        private bool _IsDisabledInQuickInfo;

        public bool IsDisabledInQuickInfo
        {
            get => _IsDisabledInQuickInfo;
            set => SetProperty(ref _IsDisabledInQuickInfo, value);
        }

        private bool _isDisabledInXml;

        public bool IsDisabledInXml
        {
            get => _isDisabledInXml;
            set => SetProperty(ref _isDisabledInXml, value);
        }

        private bool _isBold;

        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        private bool _isOverline;

        public bool IsOverline
        {
            get => _isOverline;
            set => SetProperty(ref _isOverline, value);
        }

        private bool _isUnderline;

        public bool IsUnderline
        {
            get => _isUnderline;
            set => SetProperty(ref _isUnderline, value);
        }

        private bool _isStrikethrough;

        public bool IsStrikethrough
        {
            get => _isStrikethrough;
            set => SetProperty(ref _isStrikethrough, value);
        }

        private bool _isBaseLine;

        public bool IsBaseline
        {
            get => _isBaseLine;
            set => SetProperty(ref _isBaseLine, value);
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
            FontFamily = FontFamiliesContainer.SelectedFamily,
            IsBold = IsBold,
            FontStyle = FontStylesContainer.SelectedStyle,
            FontStretch = FontStretchesContainer.Stretch,
            IsOverline = IsOverline,
            IsUnderline = IsUnderline,
            IsStrikethrough = IsStrikethrough,
            IsBaseline = IsBaseline,
            FontRenderingSize = _fontRenderingSize,

            IsDisabled = IsDisabled,
            IsDisabledInEditor = IsDisabledInEditor,
            IsDisabledInQuickInfo = IsDisabledInQuickInfo,
            IsDisabledInXml = IsDisabledInXml,

            ForegroundWasReset = Foreground.ColorWasReset,
            BackgroundWasReset = Background.ColorWasReset,
            FontRenderingSizeWasReset = _fontRenderingSizeWasReset,
        };
    }
}