using System.Collections.Generic;

namespace CoCo.UI.Data
{
    public class Option
    {
        public ICollection<Language> Languages { get; } = new List<Language>();
    }
}