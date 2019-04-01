using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.VisualBasic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser.QuickInfo.VisualBasic
{
    internal class VisualBasicSemanticProvider : SemanticProvider
    {
        protected override async Task<QuickInfoItem> GetQuickInfoAsync(
            ITextBuffer textBuffer, Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            return
                await GetBuiltInOperationsQuickInfoAsync(textBuffer, document, token, cancellationToken) ??
                await base.GetQuickInfoAsync(textBuffer, document, token, cancellationToken);
        }

        protected override Task<SymbolDescriptionInfo> GetDescriptionAsync(
            ITextBuffer textBuffer,
            SemanticModel semanticModel,
            int position,
            ImmutableArray<ISymbol> symbols,
            CancellationToken cancellationToken)
        {
            var converter = GetConverter(textBuffer);
            return new VisualBasicSymbolDescriptionProvider(converter, semanticModel, position, cancellationToken)
                .GetDescriptionAsync(symbols);
        }

        protected override SyntaxNode GetRelevantParent(SyntaxToken token)
        {
            var node = token.Parent;
            while (!(node is null))
            {
                var parent = node.Parent;

                // NOTE: don't try to up when you look at a descendant nodes of object creation or something else
                if (parent is QualifiedNameSyntax qualifiedSyntax && qualifiedSyntax.Left == node ||
                    parent is MemberAccessExpressionSyntax memberSyntax && memberSyntax.Expression == node)
                {
                    return node;
                }

                if (parent is ObjectCreationExpressionSyntax objectCreation && objectCreation.Type == node) return parent;

                // NOTE: try to up until node's parent is name
                if (!(parent is NameSyntax)) break;
                node = parent;
            }
            return token.Parent;
        }

        protected override bool TryGetIdentifierByOverrideToken(SyntaxToken token, out SyntaxToken identifier)
        {
            if (token.IsKind(SyntaxKind.OverridesKeyword))
            {
                switch (token.Parent)
                {
                    case MethodStatementSyntax member:
                        identifier = member.Identifier;
                        return true;

                    case PropertyStatementSyntax member:
                        identifier = member.Identifier;
                        return true;
                }
            }

            identifier = default;
            return false;
        }

        protected override bool TryGetLambdaByLambdaToken(SyntaxToken token, out SyntaxNode found)
        {
            if ((token.IsKind(SyntaxKind.SubKeyword) || token.IsKind(SyntaxKind.FunctionKeyword)) &&
                (token.Parent.IsKind(SyntaxKind.SubLambdaHeader) || token.Parent.IsKind(SyntaxKind.FunctionLambdaHeader)))
            {
                found = token.Parent.Parent;
                return true;
            }

            found = default;
            return false;
        }

        private async Task<QuickInfoItem> GetBuiltInOperationsQuickInfoAsync(
            ITextBuffer textBuffer, Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var converter = GetConverter(textBuffer);
            var provider = new VisualBasicSymbolDescriptionProvider(converter, semanticModel, token.SpanStart, cancellationToken);

            if (token.IsKind(SyntaxKind.DimKeyword))
            {
                SymbolDescriptionInfo descriptionInfo = default;
                if (token.Parent is LocalDeclarationStatementSyntax localDeclaration)
                {
                    var commonType = GetDeclarationType(semanticModel, localDeclaration.Declarators);
                    descriptionInfo = await provider.GetDimDescriptionAsync(commonType);
                }
                // NOTE: Dim also can be part of field declaration
                else if (token.Parent is FieldDeclarationSyntax fieldDeclaration)
                {
                    var commonType = GetDeclarationType(semanticModel, fieldDeclaration.Declarators);
                    descriptionInfo = await provider.GetDimDescriptionAsync(commonType);
                }

                if (descriptionInfo.HasDescriptions) return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is AddRemoveHandlerStatementSyntax)
            {
                var descriptionInfo = provider.GetAddRemoveHandlerDescription(token);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is BinaryConditionalExpressionSyntax)
            {
                var descriptionInfo = provider.GetNullCoalescingDescription(token);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is TernaryConditionalExpressionSyntax)
            {
                var descriptionInfo = provider.GetTernaryDescription(token);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is CTypeExpressionSyntax ctypeSyntax)
            {
                var descriptionInfo = provider.GetCTypeDescription(token, ctypeSyntax.Type);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is PredefinedCastExpressionSyntax castSyntax)
            {
                var descriptionInfo = provider.GetPredefinedCastDescription(token, castSyntax);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is DirectCastExpressionSyntax directCastSyntax)
            {
                var descriptionInfo = provider.GetDirectCastDescription(token, directCastSyntax.Type);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is TryCastExpressionSyntax tryCastSyntax)
            {
                var descriptionInfo = provider.GetTryCastDescription(token, tryCastSyntax.Type);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }
            if (token.Parent is GetTypeExpressionSyntax getTypeSyntax)
            {
                var descriptionInfo = provider.GetTypeDescription(token, getTypeSyntax.Type);
                return ExtractQuickInfoItem(token, descriptionInfo);
            }

            return null;
        }

        /// <summary>
        /// If all variables in <paramref name="declarators"/> have the same type returns it, otherwise returns <see langword="null"/>
        /// </summary>
        private static ITypeSymbol GetDeclarationType<T>(SemanticModel semanticModel, T declarators)
            where T : IEnumerable<VariableDeclaratorSyntax>
        {
            ITypeSymbol type = null;
            foreach (var declarator in declarators)
            {
                foreach (var name in declarator.Names)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(name);
                    var currentType =
                        symbol is ILocalSymbol local ? local.Type :
                        symbol is IFieldSymbol field ? field.Type
                        : null;
                    if (currentType is null) continue;

                    if (type is null)
                    {
                        type = currentType;
                    }
                    else if (!type.Equals(currentType)) return null;
                }
            }
            return type;
        }

        private SymbolDisplayPartConverter GetConverter(ITextBuffer textBuffer) =>
            textBuffer.Properties.TryGetProperty<VisualBasicClassifier>(typeof(VisualBasicClassifier), out var classifier)
                ? new SymbolDisplayPartConverter(classifier)
                : new SymbolDisplayPartConverter();
    }
}