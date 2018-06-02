using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CoCo.UI.Models;

namespace CoCo.UI.ViewModels
{
    public class LanguageViewModel : BaseViewModel
    {
        private readonly LanguageModel _model;

        public LanguageViewModel(LanguageModel model)
        {
            _model = model;
            Classifications.CollectionChanged += OnClassificationsChanged;
            foreach (var item in model.Classifications)
            {
                Classifications.Add(new ClassificationFormatViewModel(item));
            }
        }

        public string Name => _model.Name;

        // TODO: initialize from the input model
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

        private void InitializeClassificationsFromPreset()
        {
            // TODO: implement
        }

        private bool? _allAreCheked;

        public bool? AllAreChecked
        {
            get
            {
                // NOTE: 01 – all uncheked, 10 – all cheked, 11 – has different states
                var flag = 0;
                foreach (var item in Classifications)
                {
                    flag |= item.IsChecked ? 0b10 : 0b01;
                }

                return _allAreCheked =
                    flag == 0b10 ? true :
                    flag == 0b01 ? (bool?)false :
                    null;
            }
            set
            {
                var isCheked = false;
                if (value.HasValue && _allAreCheked != value)
                {
                    isCheked = value.Value;
                }

                // TODO: add some mechanism to suspend notification at a few time
                foreach (var item in Classifications)
                {
                    item.IsChecked = isCheked;
                }
            }
        }

        public ObservableCollection<ClassificationFormatViewModel> Classifications { get; } =
            new ObservableCollection<ClassificationFormatViewModel>();

        private ClassificationFormatViewModel _selectedClassification;

        public ClassificationFormatViewModel SelectedClassification
        {
            get
            {
                if (_selectedClassification == null && Classifications.Count > 0)
                {
                    SelectedClassification = Classifications[0];
                }
                return _selectedClassification;
            }
            set => SetProperty(ref _selectedClassification, value);
        }

        private void OnClassificationsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // TODO: ObservableCollection doesn't get the old items on the Reset, so we, again, need to use a custom implementation
            if (e.OldItems?.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ClassificationFormatViewModel classification)
                    {
                        classification.PropertyChanged -= OnClassificationPropertyChanged;
                    }
                }
            }

            if (e.NewItems?.Count > 0)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ClassificationFormatViewModel classification)
                    {
                        classification.PropertyChanged += OnClassificationPropertyChanged;
                    }
                }
            }
        }

        private void OnClassificationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClassificationFormatViewModel.IsChecked))
            {
                RaisePropertyChanged(nameof(AllAreChecked));
            }
        }
    }
}