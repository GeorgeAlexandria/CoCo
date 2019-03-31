using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;

namespace CoCo.Analyser.QuickInfo
{
    internal static class SymbolDisplayPartHelper
    {
        /// <summary>
        /// Try to reteive relevant <see cref="ClassificationTypeNames"/> for the <paramref name="part"/>
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the <see cref="SymbolDisplayPart.Kind"/> of <paramref name="part"/>
        /// is not one of keyword or punctuation types
        /// </returns>
        public static bool TryGetClassificationName(SymbolDisplayPart part, out string classification)
        {
            classification =
                part.Kind == SymbolDisplayPartKind.AliasName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.AnonymousTypeIndicator ? ClassificationTypeNames.Text :
                part.Kind == SymbolDisplayPartKind.AssemblyName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.ClassName ? ClassificationTypeNames.ClassName :
                part.Kind == SymbolDisplayPartKind.DelegateName ? ClassificationTypeNames.DelegateName :
                part.Kind == SymbolDisplayPartKind.EnumName ? ClassificationTypeNames.EnumName :
                part.Kind == SymbolDisplayPartKind.ErrorTypeName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.EventName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.FieldName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.InterfaceName ? ClassificationTypeNames.InterfaceName :
                part.Kind == SymbolDisplayPartKind.LabelName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.LocalName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.MethodName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.ModuleName ? ClassificationTypeNames.ModuleName :
                part.Kind == SymbolDisplayPartKind.NamespaceName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.ParameterName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.PropertyName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.RangeVariableName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.StructName ? ClassificationTypeNames.StructName :
                part.Kind == SymbolDisplayPartKind.TypeParameterName ? ClassificationTypeNames.TypeParameterName :
                null;

            return !(classification is null);
        }
    }
}