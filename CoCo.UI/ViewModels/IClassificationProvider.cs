using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CoCo.UI.Data;

namespace CoCo.UI.ViewModels
{
    public interface IClassificationProvider
    {
        ICollection<ClassificationFormatViewModel> GetCurrentClassificaions();

        void SetCurrentClassificaions(ICollection<ClassificationFormatViewModel> classifications);
    }
}