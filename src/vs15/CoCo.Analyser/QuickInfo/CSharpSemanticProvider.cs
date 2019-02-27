using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo.Analyser.QuickInfo
{
    internal class CSharpSemanticProvider : SemanticProvider
    {
        protected override SyntaxNode GetRelevantParent(SyntaxToken token) => token.Parent;

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

        protected override Task<IDictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>>> GetDescriptionAsync(
            SemanticModel semanticModel, int position, ImmutableArray<ISymbol> symbols, CancellationToken cancellationToken)
        {
            return new CSharpSymbolDescriptionProvider(semanticModel, position, symbols, cancellationToken).GetDescriptionAsync();
        }

        protected override bool CheckPreviousToken(SyntaxToken token) => true;
    }
}