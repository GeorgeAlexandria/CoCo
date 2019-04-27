using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.Utils;

namespace CoCo.UI.ViewModels
{
    public class FontStylesViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _styles;

        public FontStylesViewModel(string selectedStyle, string selectedFamily)
        {
            _selectedStyle = selectedStyle;

            _styles = new ObservableCollection<string>();
            InitializeStyles(_styles, selectedFamily);

            Styles = _styles.GetDefaultListView();
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

            // HACK: Removing only italic item seems not working: combobox selector cannot retrieve
            // the correct changed selection => clear all and add them again
            _styles.Clear();

            InitializeStyles(_styles, familiesViewModel.SelectedFamily);

            foreach (var style in _styles)
            {
                if (style.EqualsNoCase(selectedStyle))
                {
                    _selectedStyle = selectedStyle;
                    break;
                }
            }
            RaisePropertyChanged(nameof(SelectedStyle));
        }

        private static void InitializeStyles(ICollection<string> styles, string selectedFamilyName)
        {
            var selectedFamily = FontFamilyService.SupportedFamilies[selectedFamilyName];
            foreach (var typeFace in selectedFamily.FamilyTypefaces)
            {
                if (FontStyleService.SupportedStyles.TryGetValue(typeFace.Style, out var styleName) && !styles.Contains(styleName))
                {
                    styles.Add(styleName);
                    if (styles.Count == FontStyleService.SupportedStyleByNames.Count) return;
                }
            }
        }
    }
}