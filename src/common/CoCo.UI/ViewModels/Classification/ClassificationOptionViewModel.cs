using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class ClassificationOptionViewModel : BaseViewModel
    {
        private readonly ObservableCollection<ClassificationLanguageViewModel> _languages = new ObservableCollection<ClassificationLanguageViewModel>();

        public ClassificationOptionViewModel(ClassificationData option, IResetValuesProvider resetValuesProvider)
        {
            // TODO: it will invoke one event at invocation of clear and by one event per added item
            // Write custom BulkObservableCollection to avoid so many events
            _languages.Clear();
            foreach (var language in option.Languages)
            {
                _languages.Add(new ClassificationLanguageViewModel(language, resetValuesProvider));
            }

            Languages = CollectionViewSource.GetDefaultView(_languages);
            Languages.SortDescriptions.Add(new SortDescription(nameof(ClassificationLanguageViewModel.Name), ListSortDirection.Ascending));
        }

        public ICollectionView Languages { get; }

        private ClassificationLanguageViewModel _selectedLanguage;

        public ClassificationLanguageViewModel SelectedLanguage
        {
            get
            {
                if (_selectedLanguage is null && Languages.MoveCurrentToFirst())
                {
                    SelectedLanguage = (ClassificationLanguageViewModel)Languages.CurrentItem;
                }
                return _selectedLanguage;
            }
            set => SetProperty(ref _selectedLanguage, value);
        }

        public ClassificationData ExtractData()
        {
            var option = new ClassificationData();
            foreach (var languageViewModel in _languages)
            {
                option.Languages.Add(languageViewModel.ExtractData());
            }
            return option;
        }
    }
}