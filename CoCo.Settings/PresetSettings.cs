using System.Collections.Generic;
using System.Diagnostics;

namespace CoCo.Settings
{
    [DebuggerDisplay("{Name}")]
    public struct PresetSettings
    {
        public string Name { get; set; }

        public IEnumerable<ClassificationSettings> Classifications { get; set; }
    }
}