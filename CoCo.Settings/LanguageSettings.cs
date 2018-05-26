using System.Collections.Generic;
using System.Diagnostics;

namespace CoCo.Settings
{
    [DebuggerDisplay("{LanguageName}")]
    public struct LanguageSettings
    {
        public string LanguageName { get; set; }

        public IEnumerable<ClassificationSettings> CurrentSettings { get; set; }

        public IEnumerable<PresetSettings> Presettings { get; set; }
    }
}