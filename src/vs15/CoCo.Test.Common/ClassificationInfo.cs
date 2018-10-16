using CoCo.Analyser;

namespace CoCo.Test.Common
{
    public struct SimplifiedClassificationInfo
    {
        public string Name;
        public bool IsDisabled;
        public bool IsDisabledInXml;

        private SimplifiedClassificationInfo(string name)
        {
            Name = name;
            var info = ClassificationService.GetDefaultInfo(new ClassificationType(name));
            IsDisabled = !info.IsDisabled;
            IsDisabledInXml = !info.IsDisabledInXml;
        }

        public SimplifiedClassificationInfo Disable() => new SimplifiedClassificationInfo
        {
            IsDisabled = false,
            IsDisabledInXml = IsDisabledInXml
        };

        public SimplifiedClassificationInfo Enable() => new SimplifiedClassificationInfo
        {
            IsDisabled = true,
            IsDisabledInXml = IsDisabledInXml
        };

        public SimplifiedClassificationInfo EnableInXml() => new SimplifiedClassificationInfo
        {
            IsDisabled = IsDisabled,
            IsDisabledInXml = true
        };

        public static implicit operator SimplifiedClassificationInfo(string name) => new SimplifiedClassificationInfo(name);
    }
}