using System.Collections.Generic;

namespace CoCo.UI.Data
{
    public sealed class GeneralData
    {
        public ICollection<GeneralLanguage> Languages { get; } = new List<GeneralLanguage>();
    }
}