using System.Collections.Generic;

namespace CoCo.UI.Data
{
    public sealed class ClassificationData
    {
        public ICollection<ClassificationLanguage> Languages { get; } = new List<ClassificationLanguage>();
    }
}