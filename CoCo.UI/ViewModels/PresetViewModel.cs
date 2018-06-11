using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class PresetViewModel : BaseViewModel
    {
        private readonly Preset _preset;

        public PresetViewModel(Preset preset)
        {
            _preset = preset;
        }

        public string Name => _preset.Name;

        public Preset ExtractData() => _preset;

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}