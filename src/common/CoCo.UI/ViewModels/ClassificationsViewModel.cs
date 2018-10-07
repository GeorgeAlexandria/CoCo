using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public class ClassificationsViewModel : BaseViewModel
    {
        private readonly ObservableCollection<ClassificationViewModel> _classifications =
           new ObservableCollection<ClassificationViewModel>();

        public ClassificationsViewModel(ICollection<Classification> classifications, IResetValuesProvider resetValuesProvider)
        {
            foreach (var classification in classifications)
            {
                var classificationViewModel = new ClassificationViewModel(classification, resetValuesProvider);
                _classifications.Add(classificationViewModel);
            }

            ClassificationsView = CollectionViewSource.GetDefaultView(_classifications);
            ClassificationsView.SortDescriptions.Add(
                new SortDescription(nameof(ClassificationViewModel.DisplayName), ListSortDirection.Ascending));
        }

        public ICollectionView ClassificationsView { get; }

        public IList<ClassificationViewModel> Classifications => _classifications;

        private ClassificationViewModel _selectedClassification;

        public ClassificationViewModel SelectedClassification
        {
            get
            {
                if (_selectedClassification == null && ClassificationsView.MoveCurrentToFirst())
                {
                    SelectedClassification = (ClassificationViewModel)ClassificationsView.CurrentItem;
                }
                return _selectedClassification;
            }
            set => SetProperty(ref _selectedClassification, value);
        }
    }
}