using System.Collections.Generic;

namespace CoCo.UI.Data
{
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