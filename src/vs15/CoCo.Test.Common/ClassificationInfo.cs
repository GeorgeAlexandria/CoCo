using CoCo.Analyser;

namespace CoCo.Test.Common
{
    public static class SimplifiedClassificationInfoExtensions
    {
        public static SimplifiedClassificationInfo Disable(this string name) =>
            new SimplifiedClassificationInfo { Name = name, IsDisabled = true };

        public static SimplifiedClassificationInfo DisableInXml(this string name) =>
            new SimplifiedClassificationInfo { Name = name, IsDisabledInXml = true };
    }

    public struct SimplifiedClassificationInfo
    {
        public string Name;
        public bool IsDisabled;
        public bool IsDisabledInXml;

        private SimplifiedClassificationInfo(string name)
        {
            Name = name;
            var info = ClassificationService.GetDefaultInfo(new ClassificationType(name));
            IsDisabled = !info.IsClassified;
            IsDisabledInXml = !info.ClassifyInXml;
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