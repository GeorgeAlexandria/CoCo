using System.Collections.Generic;

namespace CoCo.UI.Data
{
    public sealed class GeneralOption
    {
        public ICollection<GeneralLanguage> Languages { get; } = new List<GeneralLanguage>();
    }
}