using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.Utils;

namespace CoCo.UI.ViewModels
{
    public class FontStretchesViewModel : BaseViewModel
    {
        private string _selectedFamily;
        private string _selectedStyle;

        private readonly ObservableCollection<string> _stretches;

        public FontStretchesViewModel(int selectedStretch, string selectedFamily, string selectedStyle)
        {
            _selectedStretch = FontStretchService.SupportedStretchNames[selectedStretch];

            _stretches = new ObservableCollection<string>();
            InitializeStretches(_stretches, selectedFamily, selectedStyle);

            Stretches = _stretches.GetDefaultListView();
        }

        public ICollectionView Stretches { get; }

        private string _selectedStretch;

        public string SelectedStretch
        {
            get
            {
                if (_selectedStretch is null && Stretches.MoveCurrentToFirst())
                {
                    SelectedStretch = (string)Stretches.CurrentItem;
                }
                return _selectedStretch;
            }
            set => SetProperty(ref _selectedStretch, value);
        }

        public int Stretch => FontStretchService.SupportedStretchByNames[SelectedStretch].ToOpenTypeStretch();

        public void OnSelectedFontFamilyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.EqualsNoCase(nameof(FontFamiliesViewModel.SelectedFamily))) return;
            if (!(sender is FontFamiliesViewModel familiesViewModel)) return;

            _selectedFamily = familiesViewModel.SelectedFamily;
            if (_selectedStyle is null) return;

            UpdateStretches();
        }

        public void OnSelectedFontStyleChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.EqualsNoCase(nameof(FontStylesViewModel.SelectedStyle))) return;
            if (!(sender is FontStylesViewModel stylesViewModel)) return;

            _selectedStyle = stylesViewModel.SelectedStyle;
            if (_selectedFamily is null) return;

            UpdateStretches();
        }

        private void UpdateStretches()
        {
            var selectedStretch = _selectedStretch;
            _stretches.Clear();

            InitializeStretches(_stretches, _selectedFamily, _selectedStyle);

            foreach (var stretch in _stretches)
            {
                if (stretch.EqualsNoCase(selectedStretch))
                {
                    _selectedStretch = selectedStretch;
                    break;
                }
            }
            RaisePropertyChanged(nameof(SelectedStretch));
        }

        private static void InitializeStretches(ICollection<string> stretches, string selectedFamilyName, string selectedStyleName)
        {
            var addedStretchesMask = 0;

            var selectedFamily = FontFamilyService.SupportedFamilies[selectedFamilyName];
            var selectedStyle = FontStyleService.SupportedStyleByNames[selectedStyleName];
            foreach (var typeFace in selectedFamily.FamilyTypefaces)
            {
                if (typeFace.Style.Equals(selectedStyle))
                {
                    var stretch = typeFace.Stretch.ToOpenTypeStretch();
                    // NOTE: i bit is 1 => stretch with i usWidthClass was added
                    var oldValue = addedStretchesMask;
                    if (oldValue != (addedStretchesMask |= 1 << (stretch - 1)) &&
                        FontStretchService.SupportedStretchNames.TryGetValue(stretch, out var stretchName))
                    {
                        stretches.Add(stretchName);
                        if (stretches.Count == FontStretchService.SupportedStretches.Count) return;
                    }
                }
            }
        }
    }
}