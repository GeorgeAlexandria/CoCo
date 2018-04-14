using System.Collections.ObjectModel;
using CoCo.Utils;

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

        private string _selectedLanguage;

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                // TODO: improve compare
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
    }
}