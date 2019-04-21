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

            public override bool VisitArrayType(IArrayTypeSymbol symbol) => Visit(symbol.ElementType);

            public override bool VisitAlias(IAliasSymbol symbol) => Visit(symbol.Target);

            public override bool DefaultVisit(ISymbol symbol) => true;

            public override bool VisitMethod(IMethodSymbol symbol)
            {
                foreach (var item in symbol.Parameters)
                {
                    if (Visit(item)) return true;
                }
                foreach (var item in symbol.TypeParameters)
                {
                    if (Visit(item)) return true;
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

        protected abstract Task<SymbolDescriptionInfo> GetDescriptionAsync(
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

        protected async Task<QuickInfoItem> GetQuickInfoAsync(
           ITextBuffer textBuffer,
           SemanticModel semanticModel,
           SyntaxToken token,
           ImmutableArray<ISymbol> symbols,
           CancellationToken cancellationToken)
        {
            var descriptionInfo = await GetDescriptionAsync(textBuffer, semanticModel, token.SpanStart, symbols, cancellationToken);
            if (descriptionInfo.IsDefault) return new QuickInfoItem(token.Span, ImageKind.None, ImmutableArray<SymbolDescription>.Empty);

            return ExtractQuickInfoItem(token, descriptionInfo);
        }

        protected QuickInfoItem ExtractQuickInfoItem(SyntaxToken token, SymbolDescriptionInfo descriptionInfo)
        {
            void Append(
               IReadOnlyDictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>> map,
               SymbolDescriptionKind kind,
               ImmutableArray<SymbolDescription>.Builder descriptions)
            {
                if (map.TryGetValue(kind, out var parts) && !parts.IsDefaultOrEmpty)
                {
                    descriptions.Add(new SymbolDescription(kind, parts));
                }
            }

            var descriptionsMap = descriptionInfo.Descriptions;

            var sections = ImmutableArray.CreateBuilder<SymbolDescription>();
            Append(descriptionsMap, SymbolDescriptionKind.Main, sections);
            Append(descriptionsMap, SymbolDescriptionKind.Additional, sections);
            Append(descriptionsMap, SymbolDescriptionKind.Captures, sections);
            Append(descriptionsMap, SymbolDescriptionKind.TypeParameter, sections);
            Append(descriptionsMap, SymbolDescriptionKind.AnonymousTypes, sections);
            Append(descriptionsMap, SymbolDescriptionKind.Exceptions, sections);

            return new QuickInfoItem(token.Span, descriptionInfo.Image, sections.TryMoveToImmutable());
        }

        private ImmutableArray<ISymbol> GetSymbolsByTokenAsync(
            SemanticModel semanticModel, SyntaxToken token, CancellationToken cancellationToken)
        {
            bool IsErrorType(ISymbol input) => input is null || input.IsErrorType();

            var relevantParent = GetRelevantParent(token);
            var overloads = semanticModel.GetMemberGroup(relevantParent, cancellationToken).ToHashSet();
            var symbols = ImmutableArray.CreateBuilder<ISymbol>();
            if (TryGetLambdaByLambdaToken(token, out SyntaxNode lambda))
            {
                var symbol = semanticModel.GetSymbolInfo(lambda).Symbol;
                if (!IsErrorType(symbol) && !symbols.Contains(symbol))
                {
                    symbols.Add(symbol);
                    overloads.Remove(symbol);
                }
            }
            else
            {
                foreach (var item in GetSymbolsByToken(semanticModel, token, cancellationToken))
                {
                    if (!IsErrorType(item) && !symbols.Contains(item))
                    {
                        symbols.Add(item);
                        overloads.Remove(item);
                    }
                }
            }

            if (overloads.Count > 0)
            {
                symbols.AddRange(overloads);
            }
            return symbols.TryMoveToImmutable();
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

            // NOTE: and try to get any other symbols (overloads or something else)
            foreach (var item in semanticModel.GetSymbolOrCandidates(relevantParent, cancellationToken))
            {
                var symbol = GetRelevantSymbol(item);
                if (symbol is null || symbol.Equals(declaredSymbol)) continue;
                builder.Add(symbol);
            }
            AddToBuilder(type);
            AddToBuilder(convertedType);
            return builder.TryMoveToImmutable();
        }

        private ISymbol GetRelevantSymbol(ISymbol symbol)
        {
            // NOTE: return actual type for param reference to this|Me
            return symbol is IParameterSymbol parameter && parameter.IsThis
                ? parameter.Type
                : symbol;
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