using System.Collections.Generic;

namespace CoCo.UI.Data
{
    public sealed class ClassificationData
    {
        public ICollection<Language> Languages { get; } = new List<Language>();
    }
}