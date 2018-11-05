using System.ComponentModel;
using System.Windows.Data;

namespace CoCo.UI.ViewModels
{
    public class FontStylesViewModel : BaseViewModel
    {
        public FontStylesViewModel(string selectedStyle)
        {
            _selectedStyle = selectedStyle;
            Styles = CollectionViewSource.GetDefaultView(FontStyleService.SupportedFontStyles.Keys);
        }

        public ICollectionView Styles { get; }

        private string _selectedStyle;

        public string SelectedStyle
        {
            get
            {
                if (_selectedStyle == null && Styles.MoveCurrentToFirst())
                {
                    SelectedStyle = (string)Styles.CurrentItem;
                }
                return _selectedStyle;
            }
            set => SetProperty(ref _selectedStyle, value);
        }
    }
}