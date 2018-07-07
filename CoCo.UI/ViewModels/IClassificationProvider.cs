using System.Collections.Generic;

namespace CoCo.UI.ViewModels
{
    /// <summary>
    /// Provides access to current classifications viewmodel
    /// </summary>
    public interface IClassificationProvider
    {
        ICollection<ClassificationFormatViewModel> GetCurrentClassificaions();

        void SetCurrentClassificaions(ICollection<ClassificationFormatViewModel> classifications);
    }
}