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
                part.Kind == SymbolDisplayPartKind.EnumName ? ClassificationTypeNames.EnumName : // TODO: enum meber
                part.Kind == SymbolDisplayPartKind.ErrorTypeName ? ClassificationTypeNames.Identifier :
                part.Kind == SymbolDisplayPartKind.EventName ? ClassificationTypeNames.EventName :
                part.Kind == SymbolDisplayPartKind.FieldName ? ClassificationTypeNames.FieldName :
                part.Kind == SymbolDisplayPartKind.InterfaceName ? ClassificationTypeNames.InterfaceName :
                part.Kind == SymbolDisplayPartKind.LabelName ? ClassificationTypeNames.Identifier : // TODO: label name
                part.Kind == SymbolDisplayPartKind.LocalName ? ClassificationTypeNames.LocalName :
                part.Kind == SymbolDisplayPartKind.MethodName ? ClassificationTypeNames.MethodName :
                part.Kind == SymbolDisplayPartKind.ModuleName ? ClassificationTypeNames.ModuleName :
                part.Kind == SymbolDisplayPartKind.NamespaceName ? ClassificationTypeNames.Identifier : // TODO: namespace name
                part.Kind == SymbolDisplayPartKind.ParameterName ? ClassificationTypeNames.ParameterName :
                part.Kind == SymbolDisplayPartKind.PropertyName ? ClassificationTypeNames.PropertyName :
                part.Kind == SymbolDisplayPartKind.RangeVariableName ? ClassificationTypeNames.Identifier : // TODO: range variable name
                part.Kind == SymbolDisplayPartKind.StructName ? ClassificationTypeNames.StructName :
                part.Kind == SymbolDisplayPartKind.TypeParameterName ? ClassificationTypeNames.TypeParameterName :
                null;

            return !(classification is null);
        }

        public static SymbolDisplayPart ToKeywordPart(this string text) =>
            new SymbolDisplayPart(SymbolDisplayPartKind.Keyword, null, text);

        public static SymbolDisplayPart ToTextPart(this string text) =>
            new SymbolDisplayPart(SymbolDisplayPartKind.Text, null, text);

        public static SymbolDisplayPart ToPunctuationPart(this string text) =>
            new SymbolDisplayPart(SymbolDisplayPartKind.Punctuation, null, text);

        public static SymbolDisplayPart ToSpacesPart(this string text) =>
            new SymbolDisplayPart(SymbolDisplayPartKind.Space, null, text);
    }
}