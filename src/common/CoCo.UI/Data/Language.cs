using System.Collections.Generic;
using System.Diagnostics;

namespace CoCo.UI.Data
{
    [DebuggerDisplay("{Name}")]
    public class Language
    {
        public Language(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public ICollection<Preset> Presets { get; } = new List<Preset>();

        public ICollection<Classification> Classifications { get; } = new List<Classification>();
    }
}