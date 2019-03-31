using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace CoCo.Analyser.QuickInfo.VisualBasic
{
    internal class VisualBasicSymbolDescriptionProvider : SymbolDescriptionProvider
    {
        public VisualBasicSymbolDescriptionProvider(
            SymbolDisplayPartConverter converter,
            SemanticModel semanticModel,
            int position,
            CancellationToken cancellationToken)
            : base(converter, semanticModel, position, cancellationToken)
        {
        }

        public async Task<SymbolDescriptionInfo> GetDimDescriptionAsync(ITypeSymbol type)
        {
            // NOTE: if type is null assume that the variables have the different types
            if (type is null)
            {
                // TODO: would be nice to show all the types
                AppendParts(SymbolDescriptionKind.Main, CreateText("<Multiply Types>").Enumerate());
                return BuildDescription();
            }
            return await GetDescriptionAsync(ImmutableArray.Create<ISymbol>(type));
        }

        public SymbolDescriptionInfo GetAddRemoveHandlerDescription(SyntaxToken token)
        {
            var isAddHandler = token.IsKind(SyntaxKind.AddHandlerKeyword);

            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateKeyword(isAddHandler ? "AddHandler" : "RemoveHandler"));
            builder.Add(CreateSpaces());
            builder.Add(CreateText("<event>"));
            builder.Add(CreatePunctuation(","));
            builder.Add(CreateText("<handler>"));
            AppendParts(SymbolDescriptionKind.Main, builder);
            builder.Clear();

            var description = isAddHandler
                ? "Associates an event with an event handler delegate or lambda expression at run time"
                : "Removes the association between an event and an event handler or delegate at run time";
            builder.Add(CreateText(description));
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.Keyword);
            return BuildDescription();
        }

        public SymbolDescriptionInfo GetNullCoalescingDescription(SyntaxToken token)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateKeyword("If"));
            builder.Add(CreatePunctuation("("));
            builder.Add(CreateText("<expression>"));
            builder.Add(CreatePunctuation(","));
            builder.Add(CreateSpaces());
            builder.Add(CreateText("<expressionIfNothing>"));
            builder.Add(CreatePunctuation(")"));
            var typeInfo = SemanticModel.GetTypeInfo(token.Parent, CancellationToken);
            if (!(typeInfo.Type is null))
            {
                builder.Add(CreateSpaces());
                builder.Add(CreateKeyword("As"));
                builder.Add(CreateSpaces());
                builder.AddRange(typeInfo.Type.ToMinimalDisplayParts(SemanticModel, token.SpanStart));
            }
            AppendParts(SymbolDescriptionKind.Main, builder);
            builder.Clear();

            builder.Add(CreateText
                ("If <expression> evaluates to a reference or Nullable value that is not Nothing the " +
                "function returns that value. Otherwise, it calculates and returns <expressionIfNothing>."));
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.MethodPublic);
            return BuildDescription();
        }

        public SymbolDescriptionInfo GetTernaryDescription(SyntaxToken token)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateKeyword("If"));
            builder.Add(CreatePunctuation("("));
            builder.Add(CreateText("<condition>"));
            builder.Add(CreateSpaces());
            builder.Add(CreateKeyword("As"));
            builder.Add(CreateKeyword("Boolean"));
            builder.Add(CreatePunctuation(","));
            builder.Add(CreateSpaces());
            builder.Add(CreateText("<expressionIfTrue>"));
            builder.Add(CreatePunctuation(","));
            builder.Add(CreateSpaces());
            builder.Add(CreateText("<expressionIfFalse>"));
            builder.Add(CreatePunctuation(")"));
            var typeInfo = SemanticModel.GetTypeInfo(token.Parent, CancellationToken);
            if (!(typeInfo.Type is null))
            {
                builder.Add(CreateSpaces());
                builder.Add(CreateKeyword("As"));
                builder.Add(CreateSpaces());
                builder.AddRange(typeInfo.Type.ToMinimalDisplayParts(SemanticModel, token.SpanStart));
            }
            AppendParts(SymbolDescriptionKind.Main, builder);
            builder.Clear();

            builder.Add(CreateText(
                "If <condition> returns True the function calculates and returns <expressionIfTrue>. Otherwise, it returns <expressionIfFalse>"));
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.MethodPublic);
            return BuildDescription();
        }

        public SymbolDescriptionInfo GetCTypeDescription(SyntaxToken token, TypeSyntax typeSyntax)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateKeyword("CType"));
            builder.Add(CreatePunctuation("("));
            builder.Add(CreateText("<expression>"));
            builder.Add(CreatePunctuation(","));
            builder.Add(CreateSpaces());
            var typeInfo = SemanticModel.GetTypeInfo(typeSyntax, CancellationToken);
            if (!(typeInfo.Type is null))
            {
                var typeParts = typeInfo.Type.ToMinimalDisplayParts(SemanticModel, token.SpanStart);
                builder.AddRange(typeParts);
                builder.Add(CreatePunctuation(")"));
                builder.Add(CreateSpaces());
                builder.Add(CreateKeyword("As"));
                builder.Add(CreateSpaces());
                builder.AddRange(typeParts);
            }
            else
            {
                builder.Add(CreateText("<unknown data type>"));
                builder.Add(CreatePunctuation(")"));
                builder.Add(CreateSpaces());
                builder.Add(CreateKeyword("As"));
                builder.Add(CreateSpaces());
                builder.Add(CreateText("<unknown data type>"));
            }
            AppendParts(SymbolDescriptionKind.Main, builder);
            builder.Clear();

            builder.Add(CreateText("Returns the result of explicitly converting an <expression> to a specified data type"));
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.MethodPublic);
            return BuildDescription();
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
            builder.Add(CreateKeyword("New"));
            builder.Add(CreateSpaces(1));
            builder.Add(CreateKeyword("With"));
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
                    builder.Add(CreateKeyword("Key"));
                }

                builder.Add(CreateSpaces(1));
                builder.Add(CreatePunctuation("."));
                builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.PropertyName, property, property.Name));
                builder.Add(CreateSpaces(1));
                builder.Add(CreateKeyword("As"));
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

        private ImmutableArray<SymbolDisplayPart> GetInitializerParts(
            ImmutableArray<SymbolDisplayPart>.Builder parts, EqualsValueSyntax equalsValue)
        {
            // TODO: use Microsoft.CodeAnalysis.Classification.Classifier to get parts from equalsValue
            return parts.ToImmutable();
        }
    }
}