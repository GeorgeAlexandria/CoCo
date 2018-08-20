using System.Collections.ObjectModel;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class OptionViewModel : BaseViewModel
    {
        public OptionViewModel(Option option, IResetValuesProvider resetValuesProvider)
        {
            // TODO: it will invoke one event at invocation of clear and by one event per added item
            // Write custom BulkObservableCollection to avoid so many events
            Languages.Clear();
            foreach (var language in option.Languages)
            {
                Languages.Add(new LanguageViewModel(language, resetValuesProvider));
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

        public Option ExtractData()
        {
            var option = new Option();
            foreach (var languageViewModel in Languages)
            {
                option.Languages.Add(languageViewModel.ExtractData());
            }
            return option;
        }
    }
}