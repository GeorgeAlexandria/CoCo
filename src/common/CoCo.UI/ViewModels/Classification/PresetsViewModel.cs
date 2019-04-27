using CoCo.UI.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace CoCo.UI.ViewModels
{
    public class PresetsViewModel : BaseViewModel
    {
        private readonly IClassificationProvider _provider;
        private readonly IResetValuesProvider _resetValuesProvider;
        private readonly ObservableCollection<PresetViewModel> _presets = new ObservableCollection<PresetViewModel>();

        public PresetsViewModel(
            ICollection<Preset> presets, IClassificationProvider provider, IResetValuesProvider resetValuesProvider)
        {
            foreach (var item in presets)
            {
                _presets.Add(new PresetViewModel(item, Apply, Delete));
            }

            PresetsView = CollectionViewSource.GetDefaultView(_presets);
            PresetsView.SortDescriptions.Add(new SortDescription(nameof(PresetViewModel.Name), ListSortDirection.Ascending));

            CreatePreset = new DelegateCommand(Create, CanCreate);
            _provider = provider;
            _resetValuesProvider = resetValuesProvider;
        }

        public ICollectionView PresetsView { get; }

        public IEnumerable<PresetViewModel> Presets => _presets;

        public DelegateCommand CreatePreset { get; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

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
            var list = new List<ClassificationViewModel>(data.Classifications.Count);
            foreach (var item in data.Classifications)
            {
                list.Add(new ClassificationViewModel(item, _resetValuesProvider));
            }
            _provider.SetCurrentClassificaions(list);
        }

        private void Delete(PresetViewModel arg)
        {
            var i = 0;
            while (i < _presets.Count)
            {
                var preset = _presets[i++];
                if (preset.IsSelected || ReferenceEquals(preset, arg))
                {
                    _presets.RemoveAt(--i);
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
            _presets.Add(new PresetViewModel(preset, Apply, Delete));
            CreatedName = string.Empty;
        }

        private bool CanCreate() => !string.IsNullOrWhiteSpace(CreatedName);
    }
}