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
                await GetDimQuickInfoAsync(textBuffer, document, token, cancellationToken) ??
                await base.GetQuickInfoAsync(textBuffer, document, token, cancellationToken);
        }

        protected override Task<SymbolDescriptionInfo> GetDescriptionAsync(
            ITextBuffer textBuffer,
            SemanticModel semanticModel,
            int position,
            ImmutableArray<ISymbol> symbols,
            CancellationToken cancellationToken)
        {
            if (textBuffer.Properties.TryGetProperty<VisualBasicClassifier>(typeof(VisualBasicClassifier), out var classifier))
            {
                return new VisualBasicSymbolDescriptionProvider(classifier, semanticModel, position, symbols, cancellationToken)
                    .GetDescriptionAsync();
            }
            return Task.FromResult<SymbolDescriptionInfo>(default);
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

        private async Task<QuickInfoItem> GetDimQuickInfoAsync(
            ITextBuffer textBuffer, Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            QuickInfoItem quickInfoItem = default;

            if (token.IsKind(SyntaxKind.DimKeyword))
            {
                ITypeSymbol type = null;
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
                if (token.Parent is LocalDeclarationStatementSyntax localDeclaration)
                {
                    foreach (var declarator in localDeclaration.Declarators)
                    {
                        foreach (var name in declarator.Names)
                        {
                            var symbol = semanticModel.GetDeclaredSymbol(name);
                            if (symbol is ILocalSymbol local)
                            {
                                if (type is null)
                                {
                                    type = local.Type;
                                    quickInfoItem = await GetQuickInfoAsync(
                                        textBuffer, semanticModel, token, ImmutableArray.Create<ISymbol>(type), cancellationToken);
                                }
                                else if (!type.Equals(local.Type))
                                {
                                    // TODO: would be nice to show all the types
                                    var text = new TaggedText("text", "<Multiply Types>");
                                    var description = new SymbolDescription(SymbolDescriptionKind.Main, ImmutableArray.Create(text));
                                    return new QuickInfoItem(token.Span, ImageKind.None, ImmutableArray.Create(description));
                                }
                            }
                        }
                    }
                }
                // NOTE: Dim also can be part of field declaration
                else if (token.Parent is FieldDeclarationSyntax fieldDeclaration)
                {
                    foreach (var declarator in fieldDeclaration.Declarators)
                    {
                        foreach (var name in declarator.Names)
                        {
                            var symbol = semanticModel.GetDeclaredSymbol(name);
                            if (symbol is IFieldSymbol field)
                            {
                                if (type is null)
                                {
                                    type = field.Type;
                                    quickInfoItem = await GetQuickInfoAsync(
                                        textBuffer, semanticModel, token, ImmutableArray.Create<ISymbol>(type), cancellationToken);
                                }
                                else if (!type.Equals(field.Type))
                                {
                                    // TODO: would be nice to show all the types
                                    var text = new TaggedText("text", "<Multiply Types>");
                                    var description = new SymbolDescription(SymbolDescriptionKind.Main, ImmutableArray.Create(text));
                                    return new QuickInfoItem(token.Span, ImageKind.None, ImmutableArray.Create(description));
                                }
                            }
                        }
                    }
                }
            }

            return quickInfoItem;
        }
    }
}