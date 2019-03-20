using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class QuickInfoOptionViewModel : BaseViewModel
    {
        private readonly ObservableCollection<QuickInfoViewModel> _languages = new ObservableCollection<QuickInfoViewModel>();

        public QuickInfoOptionViewModel(QuickInfoOption option)
        {
            _languages.Clear();
            foreach (var language in option.Languages)
            {
                _languages.Add(new QuickInfoViewModel(language));
            }

            Languages = CollectionViewSource.GetDefaultView(_languages);
            Languages.SortDescriptions.Add(new SortDescription(nameof(QuickInfoViewModel.Language), ListSortDirection.Ascending));
        }

        public ICollectionView Languages { get; }

        private QuickInfoViewModel _selectedLanguage;

        public QuickInfoViewModel SelectedLanguage
        {
            get
            {
                if (_selectedLanguage is null && Languages.MoveCurrentToFirst())
                {
                    SelectedLanguage = (QuickInfoViewModel)Languages.CurrentItem;
                }
                return _selectedLanguage;
            }
            set => SetProperty(ref _selectedLanguage, value);
        }

        public QuickInfoOption ExtractData()
        {
            var option = new QuickInfoOption();
            foreach (var languageViewModel in _languages)
            {
                option.Languages.Add(languageViewModel.ExtractData());
            }
            return option;
        }
    }
}