using System;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class PresetViewModel : BaseViewModel
    {
        private readonly Preset _preset;

        public PresetViewModel(Preset preset, Action<PresetViewModel> apply, Action<PresetViewModel> delete)
        {
            _preset = preset;
            ApplyPreset = new DelegateCommand<PresetViewModel>(apply);
            DeletePreset = new DelegateCommand<PresetViewModel>(delete);
        }

        public string Name => _preset.Name;

        public DelegateCommand<PresetViewModel> ApplyPreset { get; }

        public DelegateCommand<PresetViewModel> DeletePreset { get; }

        public Preset ExtractData() => _preset;

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}