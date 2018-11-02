using System.ComponentModel;
using System.Windows.Data;

namespace CoCo.UI.ViewModels
{
    public class FontStylesViewModel : BaseViewModel
    {
        public FontStylesViewModel(string fontStyle)
        {
            _selectedFontStyle = fontStyle;
            FontStyles = CollectionViewSource.GetDefaultView(FontStyleService.SupportedFontStyles.Keys);
        }

        public ICollectionView FontStyles { get; }

        private string _selectedFontStyle;

        public string SelectedFontStyle
        {
            get
            {
                if (_selectedFontStyle == null && FontStyles.MoveCurrentToFirst())
                {
                    SelectedFontStyle = (string)FontStyles.CurrentItem;
                }
                return _selectedFontStyle;
            }
            set => SetProperty(ref _selectedFontStyle, value);
        }

        internal Data.FontStyle FontStyle =>
            new Data.FontStyle(_selectedFontStyle, FontStyleService.SupportedFontStyles[_selectedFontStyle]);
    }
}