using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    public struct ClassificationInfo
    {
        public ClassificationInfo(IClassificationType classificationType, bool isClassified, bool classifyInXml)
        {
            ClassificationType = classificationType;
            IsClassified = isClassified;
            ClassifyInXml = classifyInXml;
        }

        public IClassificationType ClassificationType { get; }

        public bool IsClassified { get; }

        public bool ClassifyInXml { get; }
    }
}