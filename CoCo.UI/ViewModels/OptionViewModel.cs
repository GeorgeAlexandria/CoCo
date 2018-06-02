using System.Collections.ObjectModel;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class OptionViewModel : BaseViewModel
    {
        private readonly IOptionProvider _provider;

        public OptionViewModel(IOptionProvider provider)
        {
            _provider = provider;
            var option = provider.ReceiveOption();

            // TODO: it will invoke one event at invocation of clear and by one event per added item
            // Write custom BulkObservableCollection to avoid so many events
            Languages.Clear();
            foreach (var language in option.Languages)
            {
                Languages.Add(new LanguageViewModel(language));
            }
        }

        public ObservableCollection<LanguageViewModel> Languages { get; } = new ObservableCollection<LanguageViewModel>();

        private LanguageViewModel _selectedLanguage;

        public LanguageViewModel SelectedLanguage
        {
            get
            {
                if (_selectedLanguage is null && Languages.Count > 0)
                {
                    SelectedLanguage = Languages[0];
                }
                return _selectedLanguage;
            }
            set => SetProperty(ref _selectedLanguage, value);
        }

        public void SaveOption()
        {
            var option = new Option();
            foreach (var languageViewModel in Languages)
            {
                option.Languages.Add(languageViewModel.ExtractData());
            }
            _provider.ReleaseOption(option);
        }
    }
}