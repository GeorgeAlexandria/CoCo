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
                _classifications.Add(classificationViewModel);
            }

            Classifications = CollectionViewSource.GetDefaultView(_classifications);
            Classifications.SortDescriptions.Add(
                new SortDescription(nameof(ClassificationFormatViewModel.DisplayName), ListSortDirection.Ascending));

            PresetsContainer = new PresetsViewModel(language.Presets, this, resetValuesProvider);
        }

        public PresetsViewModel PresetsContainer { get; }

        public string Name { get; }

        public ICollectionView Classifications { get; }

        private ClassificationFormatViewModel _selectedClassification;

        public ClassificationFormatViewModel SelectedClassification
        {
            get
            {
                if (_selectedClassification == null && Classifications.MoveCurrentToFirst())
                {
                    SelectedClassification = (ClassificationFormatViewModel)Classifications.CurrentItem;
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

        ICollection<ClassificationFormatViewModel> IClassificationProvider.GetCurrentClassificaions() => _classifications;

        void IClassificationProvider.SetCurrentClassificaions(ICollection<ClassificationFormatViewModel> classifications)
        {
            /// TODO: again bulk operation under a <see cref="ObservableCollection{T}"/>
            while (_classifications.Count > 0)
            {
                _classifications.RemoveAt(_classifications.Count - 1);
            }

            foreach (var item in classifications)
            {
                _classifications.Add(item);
            }
            // NOTE: Reset selected classification from old items
            SelectedClassification = null;
        }
    }
}