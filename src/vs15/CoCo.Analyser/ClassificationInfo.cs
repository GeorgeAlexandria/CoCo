using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    public struct ClassificationInfo
    {
        public ClassificationInfo(IClassificationType classificationType, bool isDisabled, bool isDisabledInXml)
        {
            ClassificationType = classificationType;
            IsDisabled = isDisabled;
            IsDisabledInXml = isDisabledInXml;
        }

        public IClassificationType ClassificationType { get; }

        public bool IsDisabled { get; }

        public bool IsDisabledInXml { get; }
    }
}