using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo.Analyser.QuickInfo
{
    internal class CSharpSymbolDescriptionProvider : SymbolDescriptionProvider
    {
        private readonly CSharpClassifier _classifier;

        public CSharpSymbolDescriptionProvider(
            CSharpClassifier classifier,
            SemanticModel semanticModel,
            int position,
            ImmutableArray<ISymbol> symbols,
            CancellationToken cancellationToken)
            : base(semanticModel, position, symbols, cancellationToken)
        {
            _classifier = classifier;
        }

        protected override void AppenDeprecatedParts() => AppendParts(SymbolDescriptionKind.Main,
            CreatePunctuation("["), CreateText("deprecated"), CreatePunctuation("]"), CreateSpaces());

        protected override void AppendPrefixParts(PrefixKind prefix)
        {
            var prefixString =
                prefix == PrefixKind.Awaitable ? "awaitable" :
                prefix == PrefixKind.Extension ? "extension" :
                "awaitable, extension";

            AppendParts(
                SymbolDescriptionKind.Main, CreatePunctuation("("), CreateText(prefixString), CreatePunctuation(")"), CreateSpaces());
        }

        protected override async Task<ImmutableArray<SymbolDisplayPart>> GetInitializerPartsAsync(ISymbol symbol)
        {
            EqualsValueClauseSyntax initializer = null;
            switch (symbol)
            {
                case IFieldSymbol field:
                    var fieldDeclarator = await GetDeclaration<VariableDeclaratorSyntax>(symbol);
                    if (fieldDeclarator is null)
                    {
                        var enumMemberDeclaration = await GetDeclaration<EnumMemberDeclarationSyntax>(symbol);
                        if (!(enumMemberDeclaration is null))
                        {
                            initializer = enumMemberDeclaration.EqualsValue;
                        }
                    }
                    else
                    {
                        initializer = fieldDeclarator.Initializer;
                    }
                    break;

                case ILocalSymbol local:
                    var localDeclarator = await GetDeclaration<VariableDeclaratorSyntax>(symbol);
                    if (!(localDeclarator is null))
                    {
                        initializer = localDeclarator.Initializer;
                    }
                    break;

                case IParameterSymbol parameter:
                    var parameterSyntax = await GetDeclaration<ParameterSyntax>(symbol);
                    if (!(parameterSyntax is null))
                    {
                        initializer = parameterSyntax.Default;
                    }
                    break;
            }

            if (initializer is null) return ImmutableArray<SymbolDisplayPart>.Empty;
            return GetInitializerParts(initializer);
        }

        protected override TaggedText ToTag(SymbolDisplayPart displayPart)
        {
            if (displayPart.Symbol is null) return default;

            var classificationType = _classifier.GetClassification(displayPart.Symbol);
            if (classificationType?.Classification is null) return default;

            return new TaggedText(classificationType.Classification, displayPart.ToString());
        }

        /// <summary>
        /// Returns the first declartation of type <typeparamref name="T"/>
        /// </summary>
        private async Task<T> GetDeclaration<T>(ISymbol symbol) where T : SyntaxNode
        {
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                var node = await reference.GetSyntaxAsync(CancellationToken);
                if (node is T castedNode) return castedNode;
            }
            return null;
        }

        private ImmutableArray<SymbolDisplayPart> GetInitializerParts(EqualsValueClauseSyntax equalsValue)
        {
            // TODO: how to get symbol display parts from nodes?
            return ImmutableArray<SymbolDisplayPart>.Empty;
        }
    }
}