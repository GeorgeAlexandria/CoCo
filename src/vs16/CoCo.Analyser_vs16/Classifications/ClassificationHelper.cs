using Microsoft.CodeAnalysis.Classification;

namespace CoCo.Analyser.Classifications
{
    public static class ClassificationHelper
    {
        public static bool IsSupportedClassification(string classification) =>
            classification == ClassificationTypeNames.ClassName ||
            classification == ClassificationTypeNames.ConstantName ||
            classification == ClassificationTypeNames.DelegateName ||
            classification == ClassificationTypeNames.EnumMemberName ||
            classification == ClassificationTypeNames.EnumName ||
            classification == ClassificationTypeNames.EventName ||
            classification == ClassificationTypeNames.ExtensionMethodName ||
            classification == ClassificationTypeNames.FieldName ||
            classification == ClassificationTypeNames.Identifier ||
            classification == ClassificationTypeNames.InterfaceName ||
            classification == ClassificationTypeNames.LabelName ||
            classification == ClassificationTypeNames.LocalName ||
            classification == ClassificationTypeNames.MethodName ||
            classification == ClassificationTypeNames.ModuleName ||
            classification == ClassificationTypeNames.NamespaceName ||
            classification == ClassificationTypeNames.ParameterName ||
            classification == ClassificationTypeNames.PropertyName ||
            classification == ClassificationTypeNames.StaticSymbol ||
            classification == ClassificationTypeNames.StructName ||
            classification == ClassificationTypeNames.TypeParameterName || 
            classification == ClassificationTypeNames.ControlKeyword;
    }
}