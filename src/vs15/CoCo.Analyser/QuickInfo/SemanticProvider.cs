using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser.QuickInfo
{
    internal abstract class SemanticProvider : QuickInfoItemProvider
    {
        private sealed class ErrorTypesVisitor : SymbolVisitor<bool>
        {
            public static ErrorTypesVisitor Instance { get; } = new ErrorTypesVisitor();

            public override bool VisitLocal(ILocalSymbol symbol) => Visit(symbol.Type);

            public override bool VisitField(IFieldSymbol symbol) => Visit(symbol.Type);

            public override bool VisitProperty(IPropertySymbol symbol) => Visit(symbol.Type);

            public override bool VisitParameter(IParameterSymbol symbol) => Visit(symbol.Type);

            public override bool VisitEvent(IEventSymbol symbol) => Visit(symbol.Type);

            public override bool DefaultVisit(ISymbol symbol) => true;

            public override bool VisitMethod(IMethodSymbol symbol)
            {
                foreach (var item in symbol.Parameters)
                {
                    if (!Visit(item)) return true;
                }
                foreach (var item in symbol.TypeParameters)
                {
                    if (!Visit(item)) return true;
                }
                return false;
            }

            public override bool VisitNamedType(INamedTypeSymbol symbol)
            {
                foreach (var item in symbol.TypeArguments)
                {
                    if (Visit(item)) return true;
                }
                foreach (var item in symbol.TypeParameters)
                {
                    if (Visit(item)) return true;
                }
                return symbol.IsErrorType();
            }
        }

        protected override async Task<QuickInfoItem> GetQuickInfoAsync(
            ITextBuffer textBuffer, Document document, SyntaxToken token, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbols = GetSymbolsByTokenAsync(semanticModel, token, cancellationToken);

            // NOTE: if something goes wrong (it can be for linked files in a project) try to get any symbols by linked documents
            if (SymbolsContainErrors(symbols))
            {
                var linkedDocumentIds = document.GetLinkedDocumentIds();
                if (!linkedDocumentIds.IsDefaultOrEmpty)
                {
                    SemanticModel firstModelCandidate = null;
                    ImmutableArray<ISymbol> firstSymbolsCandidate;
                    var candidateWasFound = false;
                    foreach (var linkedId in linkedDocumentIds)
                    {
                        var linkedDocument = document.Project.Solution.GetDocument(linkedId);
                        var linkedToken = await FindTokenDocumentAsync(token, linkedDocument, cancellationToken);
                        if (linkedToken != default)
                        {
                            var candidateModel = await linkedDocument.GetSemanticModelAsync(cancellationToken);
                            var candidateSymbols = GetSymbolsByTokenAsync(candidateModel, linkedToken, cancellationToken);
                            if (!SymbolsContainErrors(candidateSymbols))
                            {
                                semanticModel = candidateModel;
                                symbols = candidateSymbols;
                                candidateWasFound = true;
                                break;
                            }
                            if (firstModelCandidate is null)
                            {
                                firstModelCandidate = candidateModel;
                                firstSymbolsCandidate = candidateSymbols;
                            }
                        }
                    }

                    if (!candidateWasFound)
                    {
                        if (firstSymbolsCandidate.IsDefaultOrEmpty) return null;
                        (semanticModel, symbols) = (firstModelCandidate, firstSymbolsCandidate);
                    }
                }
            }

            return await GetQuickInfoAsync(textBuffer, semanticModel, token, symbols, cancellationToken);
        }

        protected abstract Task<IDictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>>> GetDescriptionAsync(
            ITextBuffer textBuffer,
            SemanticModel semanticModel,
            int position,
            ImmutableArray<ISymbol> symbols,
            CancellationToken cancellationToken);

        protected abstract SyntaxNode GetRelevantParent(SyntaxToken token);

        /// <summary>
        /// Try to get the relevant identifier when <paramref name="token"/> is `override` keyword
        /// </summary>
        protected abstract bool TryGetIdentifierByOverrideToken(SyntaxToken token, out SyntaxToken identifier);

        /// <summary>
        /// Try to get the relevant node for lambda (delegate) when <paramref name="token"/> is lambda token or `delegate` keyword
        /// </summary>
        protected abstract bool TryGetLambdaByLambdaToken(SyntaxToken token, out SyntaxNode found);

        private async Task<QuickInfoItem> GetQuickInfoAsync(
           ITextBuffer textBuffer,
           SemanticModel semanticModel,
           SyntaxToken token,
           ImmutableArray<ISymbol> symbols,
           CancellationToken cancellationToken)
        {
            var description = await GetDescriptionAsync(textBuffer, semanticModel, token.SpanStart, symbols, cancellationToken);
            var sections = ImmutableArray.CreateBuilder<SymbolDescription>();
            if (description.TryGetValue(SymbolDescriptionKind.Main, out var mainParts) && !mainParts.IsDefaultOrEmpty)
            {
                sections.Add(new SymbolDescription(SymbolDescriptionKind.Main, mainParts));
            }
            if (description.TryGetValue(SymbolDescriptionKind.Additional, out var additionalParts) && !additionalParts.IsDefaultOrEmpty)
            {
                sections.Add(new SymbolDescription(SymbolDescriptionKind.Additional, additionalParts));
            }
            if (description.TryGetValue(SymbolDescriptionKind.Captures, out var capturesParts) && !capturesParts.IsDefaultOrEmpty)
            {
                sections.Add(new SymbolDescription(SymbolDescriptionKind.Captures, capturesParts));
            }
            if (description.TryGetValue(SymbolDescriptionKind.TypeParameter, out var typeParameterParts) &&
                !typeParameterParts.IsDefaultOrEmpty)
            {
                sections.Add(new SymbolDescription(SymbolDescriptionKind.TypeParameter, typeParameterParts));
            }
            return new QuickInfoItem(token.Span, sections.ToImmutable());
        }

        private ImmutableArray<ISymbol> GetSymbolsByTokenAsync(
            SemanticModel semanticModel, SyntaxToken token, CancellationToken cancellationToken)
        {
            bool IsErrorType(ISymbol input) => input == null || input.IsErrorType();

            var relevantParent = GetRelevantParent(token);
            var overloads = semanticModel.GetMemberGroup(relevantParent, cancellationToken).ToHashSet();
            var symbols = ImmutableArray.CreateBuilder<ISymbol>();
            if (TryGetLambdaByLambdaToken(token, out SyntaxNode lambda))
            {
                var symbol = semanticModel.GetSymbolInfo(lambda).Symbol;
                if (!IsErrorType(symbol) && !overloads.Contains(symbol))
                {
                    symbols.Add(symbol);
                }
            }
            else
            {
                foreach (var item in GetSymbolsByToken(semanticModel, token, cancellationToken))
                {
                    if (!IsErrorType(item) && !overloads.Contains(item))
                    {
                        symbols.Add(item);
                    }
                }
            }
            symbols.AddRange(overloads);
            return symbols.ToImmutable();
        }

        private ImmutableArray<ISymbol> GetSymbolsByToken(
            SemanticModel semanticModel, SyntaxToken token, CancellationToken cancellationToken)
        {
            // NOTE: try to get overridden symbol when token is `override` keyword
            if (TryGetIdentifierByOverrideToken(token, out var identifier))
            {
                var overriddenSymbol = semanticModel.GetDeclaredSymbol(identifier, cancellationToken).GetOverriddenSymbol();
                return overriddenSymbol is null ? ImmutableArray<ISymbol>.Empty : ImmutableArray.Create(overriddenSymbol);
            }

            var builder = ImmutableArray.CreateBuilder<ISymbol>();
            void AddToBuilder(ISymbol input)
            {
                if (!(input is null))
                {
                    builder.Add(input);
                }
            }

            var relevantParent = GetRelevantParent(token);
            var typeInfo = semanticModel.GetTypeInfo(relevantParent, cancellationToken);
            var type = typeInfo.Type;
            var convertedType = typeInfo.ConvertedType;
            var declaredSymbol = semanticModel.GetDeclaredSymbol(token, cancellationToken);
            var aliasSymbol = semanticModel.GetAliasInfo(relevantParent, cancellationToken);

            AddToBuilder(declaredSymbol);
            AddToBuilder(aliasSymbol);
            if (!(declaredSymbol is null))
            {
                // NOTE: and try to get any other symbols (overloads or something else)
                foreach (var item in semanticModel.GetSymbolOrCandidates(relevantParent, cancellationToken))
                {
                    if (item.Equals(declaredSymbol)) continue;
                    builder.Add(item);
                }
            }
            AddToBuilder(type);
            AddToBuilder(convertedType);
            return builder.ToImmutable();
        }

        private static bool SymbolsContainErrors(ImmutableArray<ISymbol> symbols) =>
            symbols.IsDefaultOrEmpty || ErrorTypesVisitor.Instance.Visit(symbols[0]);

        private static async Task<SyntaxToken> FindTokenDocumentAsync(
            SyntaxToken token, Document document, CancellationToken cancellationToken)
        {
            if (!document.SupportsSyntaxTree) return default;

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var finedToken = root.FindToken(token.SpanStart);
            return finedToken.Span == token.Span ? finedToken : default;
        }
    }
}