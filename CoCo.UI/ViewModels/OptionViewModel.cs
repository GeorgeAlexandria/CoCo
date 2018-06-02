using System.Collections.ObjectModel;
using CoCo.UI.Models;

namespace CoCo.UI.ViewModels
{
    public class OptionViewModel : BaseViewModel
    {
        private readonly IModelProvider _provider;
        private readonly OptionModel _model;

        public OptionViewModel(IModelProvider provider)
        {
            _provider = provider;
            _model = provider.GetOption();

            // TODO: it will invoke one event at invocation of clear and by one event per added item
            // Write custom BulkObservableCollection to avoid so many events
            Languages.Clear();
            foreach (var languageModel in _model.Languages)
            {
                Languages.Add(new LanguageViewModel(languageModel));
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
            var optionModel = new OptionModel();
            foreach (var languageViewModel in Languages)
            {
                optionModel.Languages.Add(languageViewModel.SaveToModel());
            }
            _provider.SaveOption(optionModel);
        }
    }
}