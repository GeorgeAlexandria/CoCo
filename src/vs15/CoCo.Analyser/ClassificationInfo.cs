using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    public struct ClassificationInfo
    {
        public ClassificationInfo(IClassificationType classificationType, ClassificationOption option)
        {
            ClassificationType = classificationType;
            Option = option;
        }

        public IClassificationType ClassificationType { get; }

        public ClassificationOption Option { get; }
    }

    public struct ClassificationOption
    {
        public ClassificationOption(bool isDisabled, bool isDisabledInXml)
        {
            IsDisabled = isDisabled;
            IsDisabledInXml = isDisabledInXml;
        }

        public bool IsDisabled { get; }

        public bool IsDisabledInXml { get; }
    }
}