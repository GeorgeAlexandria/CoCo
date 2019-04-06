using System.Collections.Generic;
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

            builder.Add(CreateText("If <expression> evaluates to a reference or Nullable value that is not "));
            builder.Add(CreateKeyword("Nothing"));
            builder.Add(CreateText(" the function returns that value. Otherwise, it calculates and returns <expressionIfNothing>."));
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

            builder.Add(CreateText("If <condition> returns "));
            builder.Add(CreateKeyword("True"));
            builder.Add(CreateText(" the function calculates and returns <expressionIfTrue>. Otherwise, it returns <expressionIfFalse>"));
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.MethodPublic);
            return BuildDescription();
        }

        public SymbolDescriptionInfo GetTypeDescription(SyntaxToken token, TypeSyntax typeSyntax)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateKeyword("GetType"));
            builder.Add(CreatePunctuation("("));
            var typeInfo = SemanticModel.GetTypeInfo(typeSyntax, CancellationToken);
            var typeParts = typeInfo.Type is null
                ? CreateText("<unknown data type>").Enumerate()
                : typeInfo.Type.ToMinimalDisplayParts(SemanticModel, token.SpanStart);
            builder.AddRange(typeParts);
            builder.Add(CreatePunctuation(")"));
            builder.Add(CreateSpaces());
            builder.Add(CreateKeyword("As"));
            builder.Add(CreateSpaces());

            var systemTypeParts = SemanticModel.Compilation
                .GetTypeByMetadataName("System.Type")
                .ToMinimalDisplayParts(SemanticModel, token.SpanStart);
            builder.AddRange(systemTypeParts);
            AppendParts(SymbolDescriptionKind.Main, builder);
            builder.Clear();

            builder.Add(CreateText("Returns a System.Type object for the specified type name"));
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.MethodPublic);
            return BuildDescription();
        }

        public SymbolDescriptionInfo GetPredefinedCastDescription(SyntaxToken token, PredefinedCastExpressionSyntax castSyntax)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateKeyword(castSyntax.Keyword.ValueText));
            builder.Add(CreatePunctuation("("));
            builder.Add(CreateText("<expression>"));
            builder.Add(CreatePunctuation(")"));
            builder.Add(CreateSpaces());
            builder.Add(CreateKeyword("As"));
            builder.Add(CreateSpaces());

            var type = GetTypeByPredefinedCast(SemanticModel.Compilation, castSyntax.Keyword.Kind());
            var typeParts = type is null
                ? CreateText("<unknown data type>").Enumerate()
                : type.ToMinimalDisplayParts(SemanticModel, token.SpanStart);
            builder.AddRange(typeParts);
            AppendParts(SymbolDescriptionKind.Main, builder);
            builder.Clear();

            builder.Add(CreateText("Converts an <expression> to the "));
            builder.AddRange(typeParts);
            builder.Add(CreateText(" data type"));
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.MethodPublic);
            return BuildDescription();
        }

        public SymbolDescriptionInfo GetCTypeDescription(SyntaxToken token, TypeSyntax typeSyntax)
        {
            var additionalPart = CreateText("Returns the result of explicitly converting an <expression> to a specified data type");
            return GetCastDescription(token, typeSyntax, "CType", additionalPart.Enumerate());
        }

        public SymbolDescriptionInfo GetDirectCastDescription(SyntaxToken token, TypeSyntax typeSyntax)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateText("Introduces a type conversion operation similar to "));
            builder.Add(CreateKeyword("CType"));
            builder.Add(CreateText(". The difference is that "));
            builder.Add(CreateKeyword("CType"));
            builder.Add(CreateText(" succeeds as long as there is a valid conversion, whereas "));
            builder.Add(CreateKeyword("DirectCast"));
            builder.Add(CreateText(" requires that one type inherit from or implement the other type."));
            return GetCastDescription(token, typeSyntax, "DirectCast", builder);
        }

        public SymbolDescriptionInfo GetTryCastDescription(SyntaxToken token, TypeSyntax typeSyntax)
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateText("Introduces a type conversion operation that does not throw an exception. If an attempted conversion fails, "));
            builder.Add(CreateKeyword("TryCast"));
            builder.Add(CreateText(" returns "));
            builder.Add(CreateKeyword("Nothing"));
            builder.Add(CreateText(" which your program can test for."));
            return GetCastDescription(token, typeSyntax, "TryCast", builder);
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

        private SymbolDescriptionInfo GetCastDescription<T>(
            SyntaxToken token, TypeSyntax typeSyntax, string keyword, T additionalParts) where T : IEnumerable<SymbolDisplayPart>
        {
            var builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();
            builder.Add(CreateKeyword(keyword));
            builder.Add(CreatePunctuation("("));
            builder.Add(CreateText("<expression>"));
            builder.Add(CreatePunctuation(","));
            builder.Add(CreateSpaces());
            var typeInfo = SemanticModel.GetTypeInfo(typeSyntax, CancellationToken);
            var typeParts = typeInfo.Type is null
                ? CreateText("<unknown data type>").Enumerate()
                : typeInfo.Type.ToMinimalDisplayParts(SemanticModel, token.SpanStart);

            builder.AddRange(typeParts);
            builder.Add(CreatePunctuation(")"));
            builder.Add(CreateSpaces());
            builder.Add(CreateKeyword("As"));
            builder.Add(CreateSpaces());
            builder.AddRange(typeParts);
            AppendParts(SymbolDescriptionKind.Main, builder);
            builder.Clear();

            builder.AddRange(additionalParts);
            AppendParts(SymbolDescriptionKind.Additional, builder);
            SetImage(ImageKind.MethodPublic);
            return BuildDescription();
        }

        // TODO: Move to extension
        private static ITypeSymbol GetTypeByPredefinedCast(Compilation compilation, SyntaxKind kind)
        {
            SpecialType GetSpecialType()
            {
                switch (kind)
                {
                    case SyntaxKind.CBoolKeyword:
                        return SpecialType.System_Boolean;

                    case SyntaxKind.CByteKeyword:
                        return SpecialType.System_Byte;

                    case SyntaxKind.CCharKeyword:
                        return SpecialType.System_Char;

                    case SyntaxKind.CDateKeyword:
                        return SpecialType.System_DateTime;

                    case SyntaxKind.CDecKeyword:
                        return SpecialType.System_Decimal;

                    case SyntaxKind.CDblKeyword:
                        return SpecialType.System_Double;

                    case SyntaxKind.CIntKeyword:
                        return SpecialType.System_Int32;

                    case SyntaxKind.CLngKeyword:
                        return SpecialType.System_Int64;

                    case SyntaxKind.CObjKeyword:
                        return SpecialType.System_Object;

                    case SyntaxKind.CSByteKeyword:
                        return SpecialType.System_SByte;

                    case SyntaxKind.CSngKeyword:
                        return SpecialType.System_Single;

                    case SyntaxKind.CStrKeyword:
                        return SpecialType.System_String;

                    case SyntaxKind.CShortKeyword:
                        return SpecialType.System_Int16;

                    case SyntaxKind.CUIntKeyword:
                        return SpecialType.System_UInt32;

                    case SyntaxKind.CULngKeyword:
                        return SpecialType.System_UInt64;

                    case SyntaxKind.CUShortKeyword:
                        return SpecialType.System_UInt16;

                    default:
                        return SpecialType.None;
                }
            }

            var specialType = GetSpecialType();
            return specialType != SpecialType.None
                ? compilation.GetSpecialType(specialType)
                : null;
        }
    }
}