using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.Utils;

namespace CoCo.UI.ViewModels
{
    public class FontStylesViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _styles;

        public FontStylesViewModel(string selectedStyle)
        {
            _selectedStyle = selectedStyle;

            _styles = new ObservableCollection<string>(FontStyleService.SupportedStyles.Keys);
            /// NOTE: avoid redundant creation of <see cref="ListCollectionView"/>
            if (!(CollectionViewSource.GetDefaultView(_styles) is ListCollectionView listView))
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
                if (_selectedStyle is null && Styles.MoveCurrentToFirst())
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

            var selectedStyle = _selectedStyle;

            // HACK: Removing only italic item seems not working: combobox selector cannot retrieve the correct changed selection
            // => clear all and add them again
            _styles.Clear();
            // NOTE: assume that all fonts have normal and oblique styles
            _styles.Add(FontStyleService.Normal);
            _styles.Add(FontStyleService.Oblique);

            var selectedFamily = FontFamilyService.SupportedFamilies[familiesViewModel.SelectedFamily];
            var italic = FontStyleService.SupportedStyles[FontStyleService.Italic];
            foreach (var typeFace in selectedFamily.FamilyTypefaces)
            {
                if (typeFace.Style.Equals(italic))
                {
                    _styles.Add(FontStyleService.Italic);
                    _selectedStyle = selectedStyle;
                    break;
                }
            }

            if (_styles.Count == 2 && Styles.MoveCurrentToFirst())
            {
                _selectedStyle = (string)Styles.CurrentItem;
            }
            RaisePropertyChanged(nameof(SelectedStyle));
        }
    }
}