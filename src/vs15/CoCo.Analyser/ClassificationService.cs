using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    internal class ClassificationService
    {
        public static ClassificationInfo GetDefaultInfo(IClassificationType type) => new ClassificationInfo(type, true, true);
    }
}