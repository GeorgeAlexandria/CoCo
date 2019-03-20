using System.Collections.Generic;

namespace CoCo.UI.Data
{
    public sealed class EditorOption
    {
        public ICollection<Language> Languages { get; } = new List<Language>();
    }
}