using System.Collections.ObjectModel;

namespace CoCo.UI.ViewModels
{
    public class OptionViewModel : BaseViewModel
    {
        public ObservableCollection<string> Languages { get; } = new ObservableCollection<string>
        {
            "CSharp1",
            "CSharp2",
            "CSharp3",
            "CSharp4",
            "CSharp5",
            "CSharp6",
            "CSharp7",
            "CSharp8",
            "CSharp9",
            "CSharp10",
        };

        public ObservableCollection<string> Presets { get; } = new ObservableCollection<string>
        {
            "Preset1",
            "Preset2",
            "Preset3",
            "Preset4",
            "Preset5",
        };

        // TODO: temporary string
        private string _selectedPreset;

        public string SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (_selectedPreset != value)
                {
                    _selectedPreset = value;
                    RaisePropertyChanged();
                    InitializeClassificationsFromPreset();
                }
            }
        }

        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get
            {
                if (_selectedLanguage == null && Languages.Count > 0)
                {
                    SelectedLanguage = Languages[0];
                }
                return _selectedLanguage;
            }
            set
            {
                if (!_selectedLanguage.EqualsNoCase(value))
                {
                    _selectedLanguage = value;
                    RaisePropertyChanged();

                    // TODO: it will invoke one event at invocation of clear and by one event per added item
                    // Write custom BulkObservableCollection to avoid so many events
                    Classifications.Clear();
                    foreach (var item in ClassificationFormatProvider.Get(_selectedLanguage))
                    {
                        Classifications.Add(item);
                    }
                }
            }
        }

        public ObservableCollection<ClassificationFormatViewModel> Classifications { get; } =
            new ObservableCollection<ClassificationFormatViewModel>();

        private ClassificationFormatViewModel _selectedClassification;

        public ClassificationFormatViewModel SelectedClassification
        {
            get => _selectedClassification;
            set
            {
                if (_selectedClassification != value)
                {
                    _selectedClassification = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void InitializeClassificationsFromPreset()
        {
            // TODO: implement
        }
    }
}