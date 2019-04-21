using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.Editor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser.QuickInfo.CSharp
{
    internal class CSharpSemanticProvider : SemanticProvider
    {
        protected override SyntaxNode GetRelevantParent(SyntaxToken token)
        {
            var node = token.Parent;
            while (!(node is null))
            {
                var parent = node.Parent;

                // NOTE: don't try to up when you look at a descendant nodes of object creation or something else
                if (parent is QualifiedNameSyntax qualifiedSyntax && qualifiedSyntax.Left == node ||
                    parent is MemberAccessExpressionSyntax memberSyntax && memberSyntax.Expression == node ||
                    parent is AliasQualifiedNameSyntax aliasSyntax && aliasSyntax.Alias == node)
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
            if (token.IsKind(SyntaxKind.OverrideKeyword))
            {
                switch (token.Parent)
                {
                    case EventDeclarationSyntax member:
                        identifier = member.Identifier;
                        return true;

                    case PropertyDeclarationSyntax member:
                        identifier = member.Identifier;
                        return true;

                    case MethodDeclarationSyntax member:
                        identifier = member.Identifier;
                        return true;

                    case IndexerDeclarationSyntax member:
                        identifier = member.ThisKeyword;
                        return true;
                }
            }

            identifier = default;
            return false;
        }

        protected override bool TryGetLambdaByLambdaToken(SyntaxToken token, out SyntaxNode node)
        {
            // NOTE: check that the token is `=>` or `delegate` keyword
            if (token.IsKind(SyntaxKind.EqualsGreaterThanToken) &&
                (token.Parent.IsKind(SyntaxKind.SimpleLambdaExpression) || token.Parent.IsKind(SyntaxKind.ParenthesizedLambdaExpression)) ||
                token.IsKind(SyntaxKind.DelegateKeyword) && token.Parent.IsKind(SyntaxKind.AnonymousMethodExpression))
            {
                node = token.Parent;
                return true;
            }

            node = null;
            return false;
        }

        protected override Task<SymbolDescriptionInfo> GetDescriptionAsync(
            ITextBuffer textBuffer,
            SemanticModel semanticModel,
            int position,
            ImmutableArray<ISymbol> symbols,
            CancellationToken cancellationToken)
        {
            if (textBuffer.Properties.TryGetProperty<CSharpTextBufferClassifier>(typeof(CSharpTextBufferClassifier), out var classifier))
            {
                var converter = new SymbolDisplayPartConverter(classifier.CodeClassifier);
                return new CSharpSymbolDescriptionProvider(converter, semanticModel, position, cancellationToken)
                    .GetDescriptionAsync(symbols);
            }
            return Task.FromResult<SymbolDescriptionInfo>(default);
        }
    }
}