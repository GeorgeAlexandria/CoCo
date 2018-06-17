using System.Collections.Generic;
using System.Diagnostics;

namespace CoCo.Settings
{
    [DebuggerDisplay("{LanguageName}")]
    public struct LanguageSettings
    {
        public string LanguageName { get; set; }

        public ICollection<ClassificationSettings> CurrentClassifications { get; set; }

        public ICollection<PresetSettings> Presets { get; set; }
    }
}