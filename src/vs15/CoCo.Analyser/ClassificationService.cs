using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    internal static class ClassificationService
    {
        public static ClassificationInfo GetDefaultInfo(IClassificationType type) => new ClassificationInfo(type, false, false);
    }
}