using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CoCo.Analyser.QuickInfo.CSharp
{
    internal class CSharpSymbolDescriptionProvider : SymbolDescriptionProvider
    {
        public CSharpSymbolDescriptionProvider(
            SymbolDisplayPartConverter converter,
            SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken)
            : base(converter, semanticModel, position, cancellationToken)
        {
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

        protected override ImmutableArray<SymbolDisplayPart>.Builder GetAnonymousTypeParts(
            SymbolDisplayPart part, ITypeSymbol anonymousType)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateSpaces(2));
            builder.Add(part);
            builder.Add(CreateSpaces(1));
            builder.Add(CreateText("is"));
            builder.Add(CreateSpaces(1));
            builder.Add(CreateKeyword("new"));
            builder.Add(CreateSpaces(1));
            builder.Add(CreatePunctuation("{"));

            var wasAdded = false;
            foreach (var member in anonymousType.GetMembers())
            {
                if (!(member is IPropertySymbol prop) || !prop.CanBeReferencedByName) continue;

                if (wasAdded)
                {
                    builder.Add(CreatePunctuation(","));
                }

                wasAdded = true;
                builder.Add(CreateSpaces(1));
                builder.AddRange(ToMinimalDisplayParts(prop.Type));
                builder.Add(CreateSpaces(1));
                builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.PropertyName, prop, prop.Name));
            }

            builder.Add(CreateSpaces(1));
            builder.Add(CreatePunctuation("}"));
            return builder;
        }

        protected override async Task<ImmutableArray<SymbolDisplayPart>> GetInitializerPartsAsync(ISymbol symbol)
        {
            object evaluatedValue = null;
            EqualsValueClauseSyntax initializer = null;
            switch (symbol)
            {
                case IFieldSymbol field:
                    evaluatedValue = field.ConstantValue;
                    var fieldDeclarator = await symbol.GetDeclarationAsync<VariableDeclaratorSyntax>(CancellationToken);
                    if (fieldDeclarator is null)
                    {
                        var enumMemberDeclaration = await symbol.GetDeclarationAsync<EnumMemberDeclarationSyntax>(CancellationToken);
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
                    evaluatedValue = local.ConstantValue;
                    var localDeclarator = await symbol.GetDeclarationAsync<VariableDeclaratorSyntax>(CancellationToken);
                    if (!(localDeclarator is null))
                    {
                        initializer = localDeclarator.Initializer;
                    }
                    break;

                case IParameterSymbol parameter:
                    evaluatedValue = parameter.ExplicitDefaultValue;
                    var parameterSyntax = await symbol.GetDeclarationAsync<ParameterSyntax>(CancellationToken);
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

            if (initializer is null) return builder.TryMoveToImmutable();
            return GetInitializerParts(builder, initializer);
        }

        private ImmutableArray<SymbolDisplayPart> GetInitializerParts(
            ImmutableArray<SymbolDisplayPart>.Builder parts, EqualsValueClauseSyntax equalsValue)
        {
            // TODO: use Microsoft.CodeAnalysis.Classification.Classifier to get parts from equalsValue
            return parts.TryMoveToImmutable();
        }
    }
}