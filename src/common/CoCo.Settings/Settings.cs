using System.Collections.Generic;

namespace CoCo.Settings
{
    public struct Settings
    {
        public ICollection<LanguageSettings> Languages { get; set; }
    }
}