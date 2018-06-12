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
            // TODO: initialize from the input model
            foreach (var item in new string[] { "Preset1", "Preset2", "Preset3", "Preset4", "Preset5" })
            {
                Presets.Add(new PresetViewModel(new Preset(item), Apply, CanApply, Delete));
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
            set => SetProperty(ref _createdName, value);
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

        private void Create()
        {
            var classifications = _provider.GetCurrentClassificaions();
            var preset = new Preset(CreatedName);
            foreach (var item in classifications)
            {
                preset.Classifications.Add(item.ExtractData());
            }
            Presets.Add(new PresetViewModel(preset, Apply, CanApply, Delete));
            CreatedName = string.Empty;
        }

        private bool CanCreate() => !string.IsNullOrWhiteSpace(CreatedName);
    }
}