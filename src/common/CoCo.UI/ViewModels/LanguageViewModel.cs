using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class LanguageViewModel : BaseViewModel, IClassificationProvider
    {
        private readonly ObservableCollection<ClassificationFormatViewModel> _classifications =
            new ObservableCollection<ClassificationFormatViewModel>();

        public LanguageViewModel(Language language, IResetValuesProvider resetValuesProvider)
        {
            Name = language.Name;
            foreach (var classification in language.Classifications)
            {
                var classificationViewModel = new ClassificationFormatViewModel(classification, resetValuesProvider);
                classificationViewModel.PropertyChanged += OnClassificationPropertyChanged;
                _classifications.Add(classificationViewModel);
            }

            Classifications = CollectionViewSource.GetDefaultView(_classifications);
            Classifications.SortDescriptions.Add(
                new SortDescription(nameof(ClassificationFormatViewModel.DisplayName), ListSortDirection.Ascending));

            PresetsContainer = new PresetsViewModel(language.Presets, this, resetValuesProvider);
        }

        public PresetsViewModel PresetsContainer { get; }

        public string Name { get; }

        private bool? _allAreCheked;

        public bool? AllAreChecked
        {
            get
            {
                // NOTE: 01 – all uncheked, 10 – all cheked, 11 – has different states
                var flag = 0;
                foreach (var item in _classifications)
                {
                    flag |= item.IsChecked ? 0b10 : 0b01;
                }

                return _allAreCheked =
                    flag == 0b10 ? true :
                    flag == 0b01 ? (bool?)false :
                    null;
            }
            set
            {
                var isCheked = false;
                if (value.HasValue && _allAreCheked != value)
                {
                    isCheked = value.Value;
                }

                // TODO: add some mechanism to suspend notification at a few time
                foreach (var item in _classifications)
                {
                    item.IsChecked = isCheked;
                }
            }
        }

        public ICollectionView Classifications { get; }

        private ClassificationFormatViewModel _selectedClassification;

        public ClassificationFormatViewModel SelectedClassification
        {
            get
            {
                if (_selectedClassification == null && _classifications.Count > 0)
                {
                    SelectedClassification = _classifications[0];
                }
                return _selectedClassification;
            }
            set => SetProperty(ref _selectedClassification, value);
        }

        public Language ExtractData()
        {
            var language = new Language(Name);
            foreach (var classificationViewModel in _classifications)
            {
                language.Classifications.Add(classificationViewModel.ExtractData());
            }

            foreach (var item in PresetsContainer.Presets)
            {
                language.Presets.Add(item.ExtractData());
            }

            return language;
        }

        private void OnClassificationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClassificationFormatViewModel.IsChecked))
            {
                RaisePropertyChanged(nameof(AllAreChecked));
            }
        }

        ICollection<ClassificationFormatViewModel> IClassificationProvider.GetCurrentClassificaions() => _classifications;

        void IClassificationProvider.SetCurrentClassificaions(ICollection<ClassificationFormatViewModel> classifications)
        {
            /// TODO: again bulk operation under a <see cref="ObservableCollection{T}"/>
            while (_classifications.Count > 0)
            {
                _classifications[0].PropertyChanged -= OnClassificationPropertyChanged;
                _classifications.RemoveAt(0);
            }

            foreach (var item in classifications)
            {
                item.PropertyChanged += OnClassificationPropertyChanged;
                _classifications.Add(item);
            }
            // NOTE: Reset selected classification from old items
            SelectedClassification = null;
        }
    }
}