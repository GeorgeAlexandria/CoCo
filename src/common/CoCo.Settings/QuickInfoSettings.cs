using System.Collections.Generic;

namespace CoCo.Settings
{
    public struct QuickInfoSettings
    {
        public ICollection<QuickInfoLanguageSettings> Languages { get; set; }
    }
}