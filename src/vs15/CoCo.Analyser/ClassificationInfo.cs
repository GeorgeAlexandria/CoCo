using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    public struct ClassificationInfo
    {
        public ClassificationInfo(IClassificationType classificationType, bool isClassified)
        {
            ClassificationType = classificationType;
            IsClassified = isClassified;
        }

        public IClassificationType ClassificationType { get; }

        public bool IsClassified { get; }
    }
}