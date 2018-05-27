using System.Collections.Generic;

namespace CoCo.UI.Models
{
    public class OptionModel
    {
        public ICollection<LanguageModel> Languages { get; } = new List<LanguageModel>();
    }
}