using System.Collections.Generic;

namespace CoCo.UI.ViewModels
{
    public interface IClassificationProvider
    {
        ICollection<ClassificationFormatViewModel> GetCurrentClassificaions();

        void SetCurrentClassificaions(ICollection<ClassificationFormatViewModel> classifications);
    }
}