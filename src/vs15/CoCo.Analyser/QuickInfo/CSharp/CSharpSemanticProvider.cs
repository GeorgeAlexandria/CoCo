using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser.QuickInfo.CSharp
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

        protected override Task<SymbolDescriptionInfo> GetDescriptionAsync(
            ITextBuffer textBuffer,
            SemanticModel semanticModel,
            int position,
            ImmutableArray<ISymbol> symbols,
            CancellationToken cancellationToken)
        {
            if (textBuffer.Properties.TryGetProperty<CSharpClassifier>(typeof(CSharpClassifier), out var classifier))
            {
                return new CSharpSymbolDescriptionProvider(classifier, semanticModel, position, symbols, cancellationToken).GetDescriptionAsync();
            }
            return Task.FromResult(new SymbolDescriptionInfo(
                new Dictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>>(), ImageKind.None));
        }
    }
}