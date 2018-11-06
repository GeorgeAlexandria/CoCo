using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.Utils;

namespace CoCo.UI.ViewModels
{
    public class FontStylesViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _styles = new ObservableCollection<string>();

        public FontStylesViewModel(string selectedStyle)
        {
            _selectedStyle = selectedStyle;

            _styles = new ObservableCollection<string>(FontStyleService.SupportedFontStyles.Keys);
            /// NOTE: avoid redundant creation of <see cref="ListCollectionView"/>
            if (!(CollectionViewSource.GetDefaultView(FontFamilyService.SupportedFamilies.Keys) is ListCollectionView listView))
            {
                listView = new ListCollectionView(_styles);
            }
            listView.CustomSort = StringComparer.Ordinal;
            Styles = listView;
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

        public void OnSelectedFontFamilyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.EqualsNoCase(nameof(FontFamiliesViewModel.SelectedFamily))) return;
            if (!(sender is FontFamiliesViewModel familiesViewModel)) return;

            var selectedFamily = FontFamilyService.SupportedFamilies[familiesViewModel.SelectedFamily];
            var italic = FontStyleService.SupportedFontStyles[FontStyleService.Italic];
            foreach (var typeFace in selectedFamily.FamilyTypefaces)
            {
                if (typeFace.Style.Equals(italic))
                {
                    // NOTE: assume that all fonts have normal and oblique style
                    if (_styles.Count != 3)
                    {
                        _styles.Add(FontStyleService.Italic);
                    }
                    return;
                }
            }
            _styles.Remove(FontStyleService.Italic);
        }
    }
}