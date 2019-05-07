using Microsoft.CodeAnalysis.Classification;

namespace CoCo.Analyser.Classifications
{
    public static class ClassificationHelper
    {
        public static bool IsSupportedClassification(string classification) =>
            classification == ClassificationTypeNames.ClassName ||
            classification == ClassificationTypeNames.DelegateName ||
            classification == ClassificationTypeNames.EnumName ||
            classification == ClassificationTypeNames.Identifier ||
            classification == ClassificationTypeNames.InterfaceName ||
            classification == ClassificationTypeNames.ModuleName ||
            classification == ClassificationTypeNames.StructName ||
            classification == ClassificationTypeNames.TypeParameterName;
    }
}