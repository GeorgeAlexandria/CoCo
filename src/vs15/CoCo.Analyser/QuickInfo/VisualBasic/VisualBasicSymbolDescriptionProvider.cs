using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.VisualBasic;
using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CoCo.Analyser.QuickInfo.VisualBasic
{
    internal class VisualBasicSymbolDescriptionProvider : SymbolDescriptionProvider
    {
        private readonly VisualBasicClassifier _classifier;

        public VisualBasicSymbolDescriptionProvider(
            VisualBasicClassifier classifier,
            SemanticModel semanticModel,
            int position,
            ImmutableArray<ISymbol> symbols,
            CancellationToken cancellationToken)
            : base(semanticModel, position, symbols, cancellationToken)
        {
            _classifier = classifier;
        }

        protected override void AppenDeprecatedParts() => AppendParts(SymbolDescriptionKind.Main,
           CreatePunctuation("("), CreateText("Deprecated"), CreatePunctuation(")"), CreateSpaces());

        protected override void AppendPrefixParts(PrefixKind prefix)
        {
            var prefixString =
                prefix == PrefixKind.Awaitable ? "Awaitable" :
                prefix == PrefixKind.Extension ? "Extension" :
                "Awaitable, Extension";

            AppendParts(
                SymbolDescriptionKind.Main, CreatePunctuation("<"), CreateText(prefixString), CreatePunctuation(">"), CreateSpaces());
        }

        protected override ImmutableArray<SymbolDisplayPart>.Builder GetAnonymousTypeParts(
            SymbolDisplayPart part, ITypeSymbol anonymousType)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateSpaces(2));
            builder.Add(part);
            builder.Add(CreateSpaces(1));
            builder.Add(CreateText("is"));
            builder.Add(CreateSpaces(1));
            builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, "New"));
            builder.Add(CreateSpaces(1));
            builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, "With"));
            builder.Add(CreateSpaces(1));
            builder.Add(CreatePunctuation("{"));

            var wasAdded = false;
            foreach (var member in anonymousType.GetMembers())
            {
                if (!(member is IPropertySymbol property) || !property.CanBeReferencedByName) continue;

                if (wasAdded)
                {
                    builder.Add(CreatePunctuation(","));
                }

                wasAdded = true;
                // NOTE: Key shows only for readonly properties
                if (property.IsReadOnly)
                {
                    builder.Add(CreateSpaces(1));
                    builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, "Key"));
                }

                builder.Add(CreateSpaces(1));
                builder.Add(CreatePunctuation("."));
                builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.PropertyName, property, property.Name));
                builder.Add(CreateSpaces(1));
                builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, "As"));
                builder.AddRange(ToMinimalDisplayParts(property.Type));
                builder.Add(CreateSpaces(1));
            }

            builder.Add(CreateSpaces(1));
            builder.Add(CreatePunctuation("}"));
            return builder;
        }

        protected override async Task<ImmutableArray<SymbolDisplayPart>> GetInitializerPartsAsync(ISymbol symbol)
        {
            object evaluatedValue = null;
            EqualsValueSyntax initializer = null;
            switch (symbol)
            {
                case IFieldSymbol field:
                    evaluatedValue = field.ConstantValue;
                    var fieldDeclarator = await symbol.GetDeclaration<VariableDeclaratorSyntax>(CancellationToken);
                    if (fieldDeclarator is null)
                    {
                        var enumMemberDeclaration = await symbol.GetDeclaration<EnumMemberDeclarationSyntax>(CancellationToken);
                        if (!(enumMemberDeclaration is null))
                        {
                            initializer = enumMemberDeclaration.Initializer;
                        }
                    }
                    else
                    {
                        initializer = fieldDeclarator.Initializer;
                    }
                    break;

                case ILocalSymbol local:
                    evaluatedValue = local.ConstantValue;
                    var localDeclarator = await symbol.GetDeclaration<VariableDeclaratorSyntax>(CancellationToken);
                    if (!(localDeclarator is null))
                    {
                        initializer = localDeclarator.Initializer;
                    }
                    break;

                case IParameterSymbol parameter:
                    evaluatedValue = parameter.ExplicitDefaultValue;
                    var parameterSyntax = await symbol.GetDeclaration<ParameterSyntax>(CancellationToken);
                    if (!(parameterSyntax is null))
                    {
                        initializer = parameterSyntax.Default;
                    }
                    break;
            }

            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            if (!(evaluatedValue is null))
            {
                if (evaluatedValue is string str)
                {
                    builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.StringLiteral, null, $"\"{str}\""));
                }
                else if (evaluatedValue.IsNumber())
                {
                    builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.NumericLiteral, null, evaluatedValue.ToString()));
                }
            }

            if (initializer is null) return builder.ToImmutable();
            return GetInitializerParts(builder, initializer);
        }

        protected override TaggedText ToTag(SymbolDisplayPart displayPart)
        {
            if (displayPart.Symbol is null) return default;

            var classificationType = _classifier.GetClassification(displayPart.Symbol);
            if (classificationType?.Classification is null) return default;

            return new TaggedText(classificationType.Classification, displayPart.ToString());
        }

        private ImmutableArray<SymbolDisplayPart> GetInitializerParts(
            ImmutableArray<SymbolDisplayPart>.Builder parts, EqualsValueSyntax equalsValue)
        {
            // TODO: use Microsoft.CodeAnalysis.Classification.Classifier to get parts from equalsValue
            return parts.ToImmutable();
        }
    }
}