using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class QuickInfoViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _states;

        public QuickInfoViewModel(QuickInfo quickInfo)
        {
            Language = quickInfo.Language;

            _states = new ObservableCollection<string>(QuickInfoStateService.SupportedStateByNames.Keys);
            _selectedState = QuickInfoStateService.SupportedState[quickInfo.State];

            /// NOTE: avoid redundant creation of <see cref="ListCollectionView"/>
            if (!(CollectionViewSource.GetDefaultView(_states) is ListCollectionView listView))
            {
                listView = new ListCollectionView(_states);
            }
            listView.CustomSort = StringComparer.Ordinal;
            States = listView;
        }

        public string Language { get; }

        public ICollectionView States { get; }

        private string _selectedState;

        public string SelectedState
        {
            get
            {
                if (_selectedState is null && States.MoveCurrentToFirst())
                {
                    SelectedState = (string)States.CurrentItem;
                }
                return _selectedState;
            }
            set => SetProperty(ref _selectedState, value);
        }

        public QuickInfo ExtractData() => new QuickInfo(Language)
        {
            State = QuickInfoStateService.SupportedStateByNames[SelectedState]
        };
    }
}