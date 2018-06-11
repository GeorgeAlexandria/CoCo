using System;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class PresetViewModel : BaseViewModel
    {
        private readonly Preset _preset;

        public PresetViewModel(Preset preset, Action apply, Func<bool> canApply, Action delete)
        {
            _preset = preset;
            ApplyPreset = new DelegateCommand(apply, canApply);
            DeletePreset = new DelegateCommand(delete);
        }

        public string Name => _preset.Name;

        public DelegateCommand ApplyPreset { get; }

        public DelegateCommand DeletePreset { get; }

        public Preset ExtractData() => _preset;

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}