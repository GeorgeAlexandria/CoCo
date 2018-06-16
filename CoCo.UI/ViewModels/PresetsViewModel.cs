using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class PresetsViewModel : BaseViewModel
    {
        private readonly IClassificationProvider _provider;

        public PresetsViewModel(ICollection<Preset> presets, IClassificationProvider provider)
        {
            foreach (var item in presets)
            {
                Presets.Add(new PresetViewModel(item, Apply, Delete));
            }

            CreatePreset = new DelegateCommand(Create, CanCreate);
            _provider = provider;
        }

        public ObservableCollection<PresetViewModel> Presets { get; } = new ObservableCollection<PresetViewModel>();

        public DelegateCommand CreatePreset { get; }

        private string _createdName;

        public string CreatedName
        {
            get => _createdName;
            set
            {
                SetProperty(ref _createdName, value);
                CreatePreset.RaiseCanExecuteChanged();
            }
        }

        private void Apply(PresetViewModel preset)
        {
            var data = preset.ExtractData();
            var list = new List<ClassificationFormatViewModel>(data.Classifications.Count);
            foreach (var item in data.Classifications)
            {
                list.Add(new ClassificationFormatViewModel(item));
            }
            _provider.SetCurrentClassificaions(list);
        }

        private void Delete(PresetViewModel arg)
        {
            var i = 0;
            while (i < Presets.Count)
            {
                var preset = Presets[i++];
                if (preset.IsSelected || ReferenceEquals(preset, arg))
                {
                    Presets.RemoveAt(--i);
                }
            }
        }

        private void Create()
        {
            var classifications = _provider.GetCurrentClassificaions();
            var preset = new Preset(CreatedName);
            foreach (var item in classifications)
            {
                preset.Classifications.Add(item.ExtractData());
            }
            Presets.Add(new PresetViewModel(preset, Apply, Delete));
            CreatedName = string.Empty;
        }

        private bool CanCreate() => !string.IsNullOrWhiteSpace(CreatedName);
    }
}