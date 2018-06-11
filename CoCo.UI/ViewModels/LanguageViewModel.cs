using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class LanguageViewModel : BaseViewModel
    {
        public LanguageViewModel(Language language)
        {
            Name = language.Name;
            foreach (var classification in language.Classifications)
            {
                var classificationViewModel = new ClassificationFormatViewModel(classification);
                classificationViewModel.PropertyChanged += OnClassificationPropertyChanged;
                Classifications.Add(classificationViewModel);
            }

            // TODO: initialize from the input model
            foreach (var item in new string[] { "Preset1", "Preset2", "Preset3", "Preset4", "Preset5" })
            {
                Presets.Add(new PresetViewModel(new Preset(item), Apply, CanApply, Delete));
            }
        }

        private void Apply()
        {
            // TODO: check that will works binding on the SelectedItem when selection mode is extended
            PresetViewModel selectedViewModel = null;
            foreach (var item in Presets)
            {
                if (item.IsSelected)
                {
                    selectedViewModel = item;
                    break;
                }
            }
            if (selectedViewModel == null) return;

            // TODO: implement
        }

        private bool CanApply()
        {
            var selectedCount = 0;
            foreach (var preset in Presets)
            {
                if (preset.IsSelected && selectedCount++ > 1) return false;
            }
            return selectedCount == 1;
        }

        private void Delete()
        {
            var i = 0;
            while (i < Presets.Count)
            {
                if (Presets[i++].IsSelected)
                {
                    Presets.RemoveAt(--i);
                }
            }
        }

        public string Name { get; }

        public ObservableCollection<PresetViewModel> Presets { get; } = new ObservableCollection<PresetViewModel>();

        private PresetViewModel _selectedPreset;

        public PresetViewModel SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (_selectedPreset != value)
                {
                    _selectedPreset = value;
                    RaisePropertyChanged();
                }
            }
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

        public Language ExtractData()
        {
            var language = new Language(Name);
            foreach (var classificationViewModel in Classifications)
            {
                language.Classifications.Add(classificationViewModel.ExtractData());
            }

            // TODO: PRESETS!
            return language;
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