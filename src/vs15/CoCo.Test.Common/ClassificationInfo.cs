using CoCo.Analyser;

namespace CoCo.Test.Common
{
    public struct SimplifiedClassificationInfo
    {
        public string Name;
        public bool IsDisabled;
        public bool IsDisabledInXml;
        public bool IsDisabledInEditor;
        public bool IsDisabledInQuickInfo;

        private SimplifiedClassificationInfo(string name)
        {
            Name = name;
            var info = ClassificationService.GetDefaultOption(name);
            IsDisabled = info.IsDisabled;
            IsDisabledInXml = info.IsDisabledInXml;
            IsDisabledInEditor = info.IsDisabledInEditor;
            IsDisabledInQuickInfo = info.IsDisabledInQuickInfo;
        }

        public SimplifiedClassificationInfo Disable() => new SimplifiedClassificationInfo(Name)
        {
            IsDisabled = true,
            IsDisabledInXml = IsDisabledInXml
        };

        public SimplifiedClassificationInfo DisableInXml() => new SimplifiedClassificationInfo(Name)
        {
            IsDisabled = IsDisabled,
            IsDisabledInXml = true
        };

        public SimplifiedClassificationInfo Enable() => new SimplifiedClassificationInfo(Name)
        {
            IsDisabled = false,
            IsDisabledInXml = IsDisabledInXml
        };

        public SimplifiedClassificationInfo EnableInXml() => new SimplifiedClassificationInfo(Name)
        {
            IsDisabled = IsDisabled,
            IsDisabledInXml = false
        };

        public static implicit operator SimplifiedClassificationInfo(string name) => new SimplifiedClassificationInfo(name);

        public static implicit operator ClassificationOption(SimplifiedClassificationInfo info) =>
            new ClassificationOption(info.IsDisabled, info.IsDisabledInXml, info.IsDisabledInEditor, info.IsDisabledInQuickInfo);
    }
}