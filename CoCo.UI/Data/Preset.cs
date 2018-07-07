using System.Collections.Generic;
using System.Diagnostics;

namespace CoCo.UI.Data
{
    [DebuggerDisplay("{Name")]
    public class Preset
    {
        public Preset(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public ICollection<Classification> Classifications { get; } = new List<Classification>();
    }
}