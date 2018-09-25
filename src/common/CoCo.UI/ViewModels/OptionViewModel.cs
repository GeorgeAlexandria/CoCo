using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class OptionViewModel : BaseViewModel
    {
        private readonly ObservableCollection<LanguageViewModel> _languages = new ObservableCollection<LanguageViewModel>();

        public OptionViewModel(Option option, IResetValuesProvider resetValuesProvider)
        {
            // TODO: it will invoke one event at invocation of clear and by one event per added item
            // Write custom BulkObservableCollection to avoid so many events
            _languages.Clear();
            foreach (var language in option.Languages)
            {
                _languages.Add(new LanguageViewModel(language, resetValuesProvider));
            }

            Languages = CollectionViewSource.GetDefaultView(_languages);
            Languages.SortDescriptions.Add(new SortDescription(nameof(LanguageViewModel.Name), ListSortDirection.Ascending));
        }

        public ICollectionView Languages { get; }

        private LanguageViewModel _selectedLanguage;

        public LanguageViewModel SelectedLanguage
        {
            get
            {
                if (_selectedLanguage is null && Languages.MoveCurrentToFirst())
                {
                    SelectedLanguage = (LanguageViewModel)Languages.CurrentItem;
                }
                return _selectedLanguage;
            }
            set => SetProperty(ref _selectedLanguage, value);
        }

        public Option ExtractData()
        {
            var option = new Option();
            foreach (var languageViewModel in _languages)
            {
                option.Languages.Add(languageViewModel.ExtractData());
            }
            return option;
        }
    }
}