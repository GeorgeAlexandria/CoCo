using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class GeneralOptionViewModel : BaseViewModel
    {
        private readonly ObservableCollection<GeneralLanguageViewModel> _languages = new ObservableCollection<GeneralLanguageViewModel>();

        public GeneralOptionViewModel(GeneralOption option)
        {
            foreach (var language in option.Languages)
            {
                _languages.Add(new GeneralLanguageViewModel(language));
            }

            Languages = CollectionViewSource.GetDefaultView(_languages);
            Languages.SortDescriptions.Add(new SortDescription(nameof(GeneralLanguageViewModel.Language), ListSortDirection.Ascending));
        }

        public ICollectionView Languages { get; }

        private GeneralLanguageViewModel _selectedLanguage;

        public GeneralLanguageViewModel SelectedLanguage
        {
            get
            {
                if (_selectedLanguage is null && Languages.MoveCurrentToFirst())
                {
                    SelectedLanguage = (GeneralLanguageViewModel)Languages.CurrentItem;
                }
                return _selectedLanguage;
            }
            set => SetProperty(ref _selectedLanguage, value);
        }

        public GeneralOption ExtractData()
        {
            var option = new GeneralOption();
            foreach (var languageViewModel in _languages)
            {
                option.Languages.Add(languageViewModel.ExtractData());
            }
            return option;
        }
    }
}