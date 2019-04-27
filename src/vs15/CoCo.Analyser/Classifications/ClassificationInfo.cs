using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications
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
        public ClassificationOption(bool isDisabled, bool isDisabledInXml, bool isDisabledInEditor, bool isDisabledInQuickInfo)
        {
            IsDisabled = isDisabled;
            IsDisabledInXml = isDisabledInXml;
            IsDisabledInEditor = isDisabledInEditor;
            IsDisabledInQuickInfo = isDisabledInQuickInfo;
        }

        public bool IsDisabled { get; }

        public bool IsDisabledInXml { get; }

        public bool IsDisabledInEditor { get; }

        public bool IsDisabledInQuickInfo { get; }
    }
}