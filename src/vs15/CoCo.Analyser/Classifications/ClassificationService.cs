using System.Collections.Generic;
using CoCo.Analyser.Classifications.CSharp;
using CoCo.Analyser.Classifications.VisualBasic;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications
{
    internal static class ClassificationService
    {
        private static readonly HashSet<string> _disabledClassifications = new HashSet<string>
        {
            CSharpNames.ClassName,
            CSharpNames.StructureName,
            CSharpNames.InterfaceName,
            CSharpNames.EnumName,
            CSharpNames.DelegateName,
            CSharpNames.TypeParameterName,

            VisualBasicNames.ClassName,
            VisualBasicNames.StructureName,
            VisualBasicNames.ModuleName,
            VisualBasicNames.InterfaceName,
            VisualBasicNames.EnumName,
            VisualBasicNames.DelegateName,
            VisualBasicNames.TypeParameterName
        };

        public static ClassificationOption GetDefaultInfo(IClassificationType type) => GetDefaultOption(type.Classification);

        public static ClassificationOption GetDefaultOption(string name) => _disabledClassifications.Contains(name)
            ? new ClassificationOption(true, true, true, true)
            : new ClassificationOption(false, false, false, false);
    }
}