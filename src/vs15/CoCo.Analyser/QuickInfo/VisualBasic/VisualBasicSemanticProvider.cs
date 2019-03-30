using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.VisualBasic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
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
            return new VisualBasicSymbolDescriptionProvider(converter, semanticModel, position, symbols, cancellationToken)
                .GetDescriptionAsync();
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
                                    var text = new TaggedText(ClassificationTypeNames.Text, "<Multiply Types>");
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
                                    var text = new TaggedText(ClassificationTypeNames.Text, "<Multiply Types>");
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

        private async Task<QuickInfoItem> GetBuiltInOperationsQuickInfoAsync(
         ITextBuffer textBuffer, Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var converter = GetConverter(textBuffer);
            if (token.Parent is AddRemoveHandlerStatementSyntax)
            {
                var isAddHandler = token.IsKind(SyntaxKind.AddHandlerKeyword);

                var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
                builder.Add((isAddHandler ? "AddHandler" : "RemoveHandler").ToKeywordPart());
                builder.Add(" ".ToSpacesPart());
                builder.Add("<event>".ToTextPart());
                builder.Add(",".ToPunctuationPart());
                builder.Add("<handler>".ToTextPart());
                var main = new SymbolDescription(SymbolDescriptionKind.Main, converter.ToTags(builder));
                builder.Clear();

                var description = isAddHandler
                    ? "Associates an event with an event handler delegate or lambda expression at run time"
                    : "Removes the association between an event and an event handler or delegate at run time";
                builder.Add(description.ToTextPart());
                var additional = new SymbolDescription(SymbolDescriptionKind.Additional, converter.ToTags(builder));

                return new QuickInfoItem(token.Span, ImageKind.Keyword, ImmutableArray.Create(main, additional));
            }
            if (token.Parent is BinaryConditionalExpressionSyntax binaryExpression)
            {
                var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
                builder.Add("If".ToKeywordPart());
                builder.Add("(".ToPunctuationPart());
                builder.Add("<expression>".ToTextPart());
                builder.Add(",".ToPunctuationPart());
                builder.Add(" ".ToSpacesPart());
                builder.Add("<expressionIfNothing>".ToTextPart());
                builder.Add(")".ToPunctuationPart());
                var typeInfo = semanticModel.GetTypeInfo(binaryExpression, cancellationToken);
                if (!(typeInfo.Type is null))
                {
                    builder.Add(" ".ToSpacesPart());
                    builder.Add("As".ToKeywordPart());
                    builder.Add(" ".ToSpacesPart());
                    builder.AddRange(typeInfo.Type.ToMinimalDisplayParts(semanticModel, token.SpanStart));
                }
                var main = new SymbolDescription(SymbolDescriptionKind.Main, converter.ToTags(builder));
                builder.Clear();

                builder.Add(
                    ("If <expression> evaluates to a reference or Nullable value that is not Nothing the " +
                    "function returns that value Otherwise it calculates and returns <expressionIfNothing>.").ToTextPart());
                var additional = new SymbolDescription(SymbolDescriptionKind.Additional, converter.ToTags(builder));

                return new QuickInfoItem(token.Span, ImageKind.Keyword, ImmutableArray.Create(main, additional));
            }

            return null;
        }

        private SymbolDisplayPartConverter GetConverter(ITextBuffer textBuffer) =>
            textBuffer.Properties.TryGetProperty<VisualBasicClassifier>(typeof(VisualBasicClassifier), out var classifier)
                ? new SymbolDisplayPartConverter(classifier)
                : new SymbolDisplayPartConverter();
    }
}