using System.Collections.Generic;
using System.Diagnostics;

namespace CoCo.Settings
{
    [DebuggerDisplay("{Name}")]
    public struct LanguageSettings
    {
        public string Name { get; set; }

        public ICollection<ClassificationSettings> CurrentClassifications { get; set; }

        public ICollection<PresetSettings> Presets { get; set; }
    }
}