using System.Collections.Generic;

namespace CoCo.Settings
{
    public struct GeneralSettings
    {
        public ICollection<GeneralLanguageSettings> Languages { get; set; }
    }
}