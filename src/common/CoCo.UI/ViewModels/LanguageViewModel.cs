using System.Collections.Generic;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    // TODO: at this moment it's a bad design to have one parent view model for two differents view models
    // to pass a changings from the one to another, instead of raising events and correct handled them.
    public class LanguageViewModel : BaseViewModel, IClassificationProvider
    {
        public LanguageViewModel(Language language, IResetValuesProvider resetValuesProvider)
        {
            Name = language.Name;
            ClassificationsContainer = new ClassificationsViewModel(language.Classifications, resetValuesProvider);
            PresetsContainer = new PresetsViewModel(language.Presets, this, resetValuesProvider);
        }

        public string Name { get; }

        public ClassificationsViewModel ClassificationsContainer { get; }

        public PresetsViewModel PresetsContainer { get; }

        public Language ExtractData()
        {
            var language = new Language(Name);
            foreach (var item in ClassificationsContainer.Classifications)
            {
                language.Classifications.Add(item.ExtractData());
            }

            foreach (var item in PresetsContainer.Presets)
            {
                language.Presets.Add(item.ExtractData());
            }

            return language;
        }

        ICollection<ClassificationViewModel> IClassificationProvider.GetCurrentClassificaions() =>
            ClassificationsContainer.Classifications;

        void IClassificationProvider.SetCurrentClassificaions(ICollection<ClassificationViewModel> classifications)
        {
            var currentClassifications = ClassificationsContainer.Classifications;
            /// TODO: again bulk operation under a <see cref="ObservableCollection{T}"/>
            while (currentClassifications.Count > 0)
            {
                currentClassifications.RemoveAt(currentClassifications.Count - 1);
            }

            foreach (var item in classifications)
            {
                currentClassifications.Add(item);
            }
            // NOTE: Reset selected classification from old items
            ClassificationsContainer.SelectedClassification = null;
        }
    }
}