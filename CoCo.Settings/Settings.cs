using System.Collections.Generic;

namespace CoCo.Settings
{
    public struct Settings
    {
        public IEnumerable<LanguageSettings> Languages { get; set; }
    }
}