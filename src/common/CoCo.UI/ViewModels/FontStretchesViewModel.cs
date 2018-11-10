﻿using System;
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

        public FontStretchesViewModel(int selectedStretch)
        {
            _selectedStretch = FontStretchService.SupportedStretchesNames[selectedStretch];

            _stretches = new ObservableCollection<string>(FontStretchService.SupportedStretchesByNames.Keys);
            /// NOTE: avoid redundant creation of <see cref="ListCollectionView"/>
            if (!(CollectionViewSource.GetDefaultView(_stretches) is ListCollectionView listView))
            {
                listView = new ListCollectionView(_stretches);
            }
            listView.CustomSort = StringComparer.Ordinal;
            Stretches = listView;
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

        public int Stretch => FontStretchService.SupportedStretchesByNames[SelectedStretch].ToOpenTypeStretch();

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
            var addedStretchesMask = 0;
            var isSelectionUnchanged = false;

            var selectedFamily = FontFamilyService.SupportedFamilies[_selectedFamily];
            var selectedStyle = FontStyleService.SupportedStyles[_selectedStyle];
            foreach (var typeFace in selectedFamily.FamilyTypefaces)
            {
                if (typeFace.Style.Equals(selectedStyle))
                {
                    var stretch = typeFace.Stretch.ToOpenTypeStretch();
                    // NOTE: i bit is 1 => stretch with i usWidthClass was added
                    var oldValue = addedStretchesMask;
                    if (oldValue != (addedStretchesMask |= 1 << (stretch - 1)))
                    {
                        var stretchName = FontStretchService.SupportedStretchesNames[stretch];
                        isSelectionUnchanged = isSelectionUnchanged || stretchName.EqualsNoCase(selectedStretch);
                        _stretches.Add(stretchName);
                    }
                }
            }

            if (isSelectionUnchanged)
            {
                _selectedStretch = selectedStretch;
            }
            RaisePropertyChanged(nameof(SelectedStretch));
        }
    }
}