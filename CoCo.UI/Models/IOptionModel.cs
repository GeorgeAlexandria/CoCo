using System.Collections.Generic;

namespace CoCo.UI.Models
{
    public interface IOptionModel
    {
        ICollection<ILanguageModel> Languages { get; }
    }
}