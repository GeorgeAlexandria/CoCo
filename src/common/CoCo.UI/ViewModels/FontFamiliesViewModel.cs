using System.ComponentModel;
using System.Windows.Data;

namespace CoCo.UI.ViewModels
{
    public class FontFamiliesViewModel : BaseViewModel
    {
        public FontFamiliesViewModel(string selectedFamily)
        {
            _selectedFamily = selectedFamily;
            Families = CollectionViewSource.GetDefaultView(FontFamilyService.SupportedFamilies.Keys);
        }

        public ICollectionView Families { get; }

        private string _selectedFamily;

        public string SelectedFamily
        {
            get
            {
                if (_selectedFamily == null && Families.MoveCurrentToFirst())
                {
                    SelectedFamily = (string)Families.CurrentItem;
                }
                return _selectedFamily;
            }
            set => SetProperty(ref _selectedFamily, value);
        }
    }
}