using System.Collections.Generic;

namespace CoCo.UI.ViewModels
{
    /// <summary>
    /// Provides access to current classifications viewmodel
    /// </summary>
    public interface IClassificationProvider
    {
        ICollection<ClassificationViewModel> GetCurrentClassificaions();

        void SetCurrentClassificaions(ICollection<ClassificationViewModel> classifications);
    }
}