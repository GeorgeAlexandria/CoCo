using System.Collections.Generic;
using System.Diagnostics;

namespace CoCo.Settings
{
    [DebuggerDisplay("{Name}")]
    public struct PresetSettings
    {
        public string Name { get; set; }

        public ICollection<ClassificationSettings> Classifications { get; set; }
    }
}