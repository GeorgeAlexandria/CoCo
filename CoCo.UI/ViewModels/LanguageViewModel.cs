using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class LanguageViewModel : BaseViewModel, IClassificationProvider
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

            PresetsContainer = new PresetsViewModel(language.Presets, this);
        }

        public PresetsViewModel PresetsContainer { get; }

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

            foreach (var item in PresetsContainer.Presets)
            {
                language.Presets.Add(item.ExtractData());
            }

            return language;
        }

        private void OnClassificationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClassificationFormatViewModel.IsChecked))
            {
                RaisePropertyChanged(nameof(AllAreChecked));
            }
        }

        ICollection<ClassificationFormatViewModel> IClassificationProvider.GetCurrentClassificaions() => Classifications;

        void IClassificationProvider.SetCurrentClassificaions(ICollection<ClassificationFormatViewModel> classifications)
        {
            /// TODO: again bulk operation under a <see cref="ObservableCollection{T}"/>
            while (Classifications.Count > 0)
            {
                Classifications[0].PropertyChanged -= OnClassificationPropertyChanged;
                Classifications.RemoveAt(0);
            }

            foreach (var item in classifications)
            {
                item.PropertyChanged += OnClassificationPropertyChanged;
                Classifications.Add(item);
            }
            // NOTE: Reset selected classification from old items
            SelectedClassification = null;
        }
    }
}