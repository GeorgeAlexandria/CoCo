﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;

namespace CoCo.Analyser.QuickInfo
{
    /// <summary>
    /// It's a abstract provider for full description of symbols in quick info (tooltip)
    /// </summary>
    public abstract partial class SymbolDescriptionProvider
    {
        [Flags]
        protected enum PrefixKind
        {
            None = 0,
            Awaitable = 1 << 0,
            Extension = 1 << 1,
            All = Awaitable | Extension,
        }

        /// <summary>
        /// Contains collected classified texts for input symbols
        /// </summary>
        private Dictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>.Builder> _description;

        private readonly SemanticModel _semanticModel;
        private readonly int _position;
        private readonly ImmutableArray<ISymbol> _symbols;

        protected readonly CancellationToken CancellationToken;

        protected SymbolDescriptionProvider(
            SemanticModel semanticModel, int position, ImmutableArray<ISymbol> symbols, CancellationToken cancellationToken)
        {
            _semanticModel = semanticModel;
            _position = position;
            _symbols = symbols;
            CancellationToken = cancellationToken;
        }

        public async Task<IDictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>>> GetDescriptionAsync()
        {
            // TODO: cache empty value
            if (_symbols.IsDefaultOrEmpty) return new Dictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>>();

            if (_description is null)
            {
                _description = new Dictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>.Builder>();
                await AppendPartsAsync(_symbols);
            }

            var result = new Dictionary<SymbolDescriptionKind, ImmutableArray<TaggedText>>();
            foreach (var item in _description)
            {
                result[item.Key] = item.Value.ToImmutable();
            }
            return result;
        }

        protected abstract TaggedText ToTag(SymbolDisplayPart displayPart);

        /// <remarks>
        /// C# and VB has a different style for braces and for the word's case
        /// </remarks>
        protected abstract void AppenDeprecatedParts();

        /// <remarks>
        /// C# and VB has a different style for braces and for the word's case
        /// </remarks>
        protected abstract void AppendPrefixParts(PrefixKind prefix);

        protected abstract Task<ImmutableArray<SymbolDisplayPart>> GetInitializerPartsAsync(ISymbol symbol);

        protected async Task AppendPartsAsync(ImmutableArray<ISymbol> symbols)
        {
            var main = symbols[0];

            AppendDeprecatedParts(main);
            await AppendDescriptionPartsAsync(main);
            AppendOverloadCountParts(symbols);
            AppendCaptureParts(main);
        }

        protected void AppendDeprecatedParts(ISymbol symbol)
        {
            foreach (var item in symbol.GetAttributes())
            {
                if (item.AttributeClass.MetadataName.Equals("ObsoleteAttribute"))
                {
                    AppenDeprecatedParts();
                    break;
                }
            }
        }

        protected async Task AppendDescriptionPartsAsync(ISymbol symbol)
        {
            // TODO: miss something?
            switch (symbol)
            {
                case IFieldSymbol field:
                    await AppendFieldPartsAsync(field);
                    break;

                case ILabelSymbol label:
                    AppendLabelParts(label);
                    break;

                case ILocalSymbol local:
                    await AppendLocalPartsAsync(local);
                    break;

                case IMethodSymbol method:
                    AppendMethodParts(method);
                    break;

                case INamedTypeSymbol namedType:
                    await AppendNamedTypePartsAsync(namedType);
                    break;

                case INamespaceSymbol @namespace:
                    AppendNamespaceParts(@namespace);
                    break;

                case IParameterSymbol parameter:
                    await AppendParameterPartsAsync(parameter);
                    break;

                case IPropertySymbol property:
                    AppendPropertyParts(property);
                    break;

                case ITypeParameterSymbol typeParameter:
                    AppendTypeParameterParts(typeParameter);
                    break;

                default:
                    AppendSymbolParts(symbol);
                    break;
            }
        }

        protected void AppendOverloadCountParts(ImmutableArray<ISymbol> symbols)
        {
            int GetOverloadCount()
            {
                if (symbols.IsDefaultOrEmpty) { return 0; }

                var main = symbols[0];
                var overloadCount = 0;
                foreach (var symbol in symbols)
                {
                    var original = symbol.OriginalDefinition;
                    if (!original.Equals(main) &&
                        (original is IMethodSymbol || original is IPropertySymbol property && property.IsIndexer))
                    {
                        ++overloadCount;
                    }
                }
                return overloadCount;
            }

            var count = GetOverloadCount();
            if (count >= 1)
            {
                AppendParts(SymbolDescriptionKind.Main,
                    CreateSpaces(), CreatePunctuation("("),
                    CreatePunctuation("+"), CreateSpaces(),
                    CreateText($"{count}"), CreateSpaces(),
                    count == 1 ? CreateText("overload") : CreateText("overloads"),
                    CreatePunctuation(")"));
            }
        }

        /// <summary>
        /// Adds captured variables
        /// </summary>
        protected void AppendCaptureParts(ISymbol symbol)
        {
            // NOTE: lambda initializer in type cannot reference to a non static fields
            if (symbol is IMethodSymbol method && method.ContainingSymbol.Kind == SymbolKind.Method)
            {
                if (method.DeclaringSyntaxReferences.IsDefaultOrEmpty) return;

                var syntax = method.DeclaringSyntaxReferences[0].GetSyntax();
                // TODO: should directly check that syntax is lambda or delegate?
                if (!(syntax is null))
                {
                    var dataFlow = GetSemanticModel(syntax.SyntaxTree)?.AnalyzeDataFlow(syntax);
                    if (dataFlow is null) return;

                    var declaredVariables = new HashSet<ISymbol>(dataFlow.VariablesDeclared);

                    List<SymbolDisplayPart> parts = null;
                    for (int i = 0; i < dataFlow.CapturedInside.Length; ++i)
                    {
                        var captureVariable = dataFlow.CapturedInside[i];
                        if (declaredVariables.Contains(captureVariable)) continue;

                        if (parts is null)
                        {
                            parts = new List<SymbolDisplayPart> { CreateText("\r\nVariables captured:") };
                        }
                        else
                        {
                            parts.Add(CreatePunctuation(","));
                        }
                        parts.Add(CreateSpaces());
                        parts.AddRange(ToMinimalDisplayParts(captureVariable, _capturesFormat));
                    }

                    if (!(parts is null))
                    {
                        AppendParts(SymbolDescriptionKind.Captures, parts);
                    }
                }
            }
        }

        protected void AppendPropertyParts(IPropertySymbol symbol) =>
            AppendParts(SymbolDescriptionKind.Main, ToMinimalDisplayParts(symbol, _memberFormat));

        protected async Task AppendNamedTypePartsAsync(INamedTypeSymbol symbol)
        {
            if (symbol.IsAwaitable())
            {
                AppendPrefixParts(PrefixKind.Awaitable);
            }

            var format = symbol.TypeKind == TypeKind.Delegate ? _delegateFormat : _descriptionFormat;
            AppendParts(SymbolDescriptionKind.Main, symbol.OriginalDefinition.ToDisplayParts(format));
        }

        protected void AppendNamespaceParts(INamespaceSymbol symbol)
        {
            var parts = symbol.IsGlobalNamespace
                ? symbol.ToDisplayParts(_globalNamespaceFormat)
                : symbol.ToDisplayParts(_descriptionFormat);
            AppendParts(SymbolDescriptionKind.Main, parts);
        }

        protected async Task AppendFieldPartsAsync(IFieldSymbol symbol)
        {
            var parts = await GetDeclarationPartsAsync(symbol, symbol.IsConst);

            // NOTE: don't show redundant info for enum fields
            if (symbol.Type.TypeKind != TypeKind.Enum)
            {
                InsertRange(parts, 0, symbol.IsConst ? CreateDescription("constant") : CreateDescription("field"));
            }
            AppendParts(SymbolDescriptionKind.Main, parts);
        }

        protected async Task AppendLocalPartsAsync(ILocalSymbol symbol)
        {
            var parts = await GetDeclarationPartsAsync(symbol, symbol.IsConst);
            InsertRange(parts, 0, symbol.IsConst ? CreateDescription("local constant") : CreateDescription("local variable"));
            AppendParts(SymbolDescriptionKind.Main, parts);
        }

        protected void AppendLabelParts(ILabelSymbol symbol)
        {
            var parts = CreateDescription("label");
            parts.AddRange(ToMinimalDisplayParts(symbol));
            AppendParts(SymbolDescriptionKind.Main, parts);
        }

        protected void AppendMethodParts(IMethodSymbol method)
        {
            // NOTE: VS shows special prefix: (awaitable), (extension) and (awaitable, extension)
            // for the corresponding method's type and doesn't show anything else for regular methods
            var isAwaitable = method.IsAwaitable();
            var isExtension = method.IsExtensionMethod || method.MethodKind == MethodKind.ReducedExtension;

            var prefix =
                isAwaitable && isExtension ? PrefixKind.All :
                isAwaitable ? PrefixKind.Awaitable :
                isExtension ? PrefixKind.Extension :
                PrefixKind.None;

            if (prefix != PrefixKind.None)
            {
                AppendPrefixParts(prefix);
            }
            AppendParts(SymbolDescriptionKind.Main, ToMinimalDisplayParts(method, _memberFormat));
        }

        protected async Task AppendParameterPartsAsync(IParameterSymbol symbol)
        {
            var parts = await GetDeclarationPartsAsync(symbol, symbol.IsOptional);
            InsertRange(parts, 0, CreateDescription("parameter"));
            AppendParts(SymbolDescriptionKind.Main, parts);
        }

        protected void AppendTypeParameterParts(ITypeParameterSymbol symbol)
        {
            var parts = ToMinimalDisplayParts(symbol);
            parts.Add(CreateSpaces());
            parts.Add(CreateText("in"));
            parts.Add(CreateSpaces());
            parts.AddRange(ToMinimalDisplayParts(symbol.ContainingSymbol, _typeParameterFormat));
            AppendParts(SymbolDescriptionKind.Main, parts);
        }

        protected void AppendSymbolParts(ISymbol symbol) =>
            AppendParts(SymbolDescriptionKind.Main, ToMinimalDisplayParts(symbol));

        protected async Task<ImmutableArray<SymbolDisplayPart>.Builder> GetDeclarationPartsAsync(ISymbol symbol, bool isConst)
        {
            if (isConst)
            {
                var initializerParts = await GetInitializerPartsAsync(symbol);
                if (!initializerParts.IsDefaultOrEmpty)
                {
                    var parts = ToMinimalDisplayParts(symbol, _minimallyQualifiedFormat);
                    parts.Add(CreateSpaces());
                    parts.Add(CreatePunctuation("="));
                    parts.Add(CreateSpaces());
                    parts.AddRange(initializerParts);
                    return parts;
                }
            }
            return ToMinimalDisplayParts(symbol, _minimallyQualifiedFormatWithConstants);
        }

        protected SymbolDisplayPart CreateText(string text) => CreatePart(SymbolDisplayPartKind.Text, text);

        protected SymbolDisplayPart CreatePunctuation(string text) => CreatePart(SymbolDisplayPartKind.Punctuation, text);

        protected SymbolDisplayPart CreateSpaces(int count = 1)
        {
            var text =
                count == 1 ? " " :
                count == 2 ? "  " :
                count == 3 ? "   " :
                count == 4 ? "    " :
                new string(' ', count);

            return CreatePart(SymbolDisplayPartKind.Space, text);
        }

        protected List<SymbolDisplayPart> CreateDescription(string description) => new List<SymbolDisplayPart>
        {
            CreatePunctuation("("),
            CreateText(description),
            CreatePunctuation(")"),
            CreateSpaces(),
        };

        protected SymbolDisplayPart CreatePart(SymbolDisplayPartKind kind, string text) => new SymbolDisplayPart(kind, null, text);

        protected void AppendParts(SymbolDescriptionKind kind, params SymbolDisplayPart[] partsArray) =>
            AppendParts<SymbolDisplayPart[]>(kind, partsArray);

        protected void AppendParts<T>(SymbolDescriptionKind descriptionKind, T parts) where T : IEnumerable<SymbolDisplayPart>
        {
            if (!_description.TryGetValue(descriptionKind, out var builder))
            {
                builder = ImmutableArray.CreateBuilder<TaggedText>();
                _description.Add(descriptionKind, builder);
            }

            foreach (var part in parts)
            {
                TaggedText tag;
                if (part.Symbol is null)
                {
                    var classification =
                        part.Kind == SymbolDisplayPartKind.Keyword ? ClassificationTypeNames.Keyword :
                        part.Kind == SymbolDisplayPartKind.LineBreak ? ClassificationTypeNames.WhiteSpace :
                        part.Kind == SymbolDisplayPartKind.Operator ? ClassificationTypeNames.Operator :
                        part.Kind == SymbolDisplayPartKind.Punctuation ? ClassificationTypeNames.Punctuation :
                        part.Kind == SymbolDisplayPartKind.Space ? ClassificationTypeNames.WhiteSpace :
                        part.Kind == SymbolDisplayPartKind.Text ? ClassificationTypeNames.Text :
                        null;

                    tag = new TaggedText(classification, part.ToString());
                }
                else
                {
                    tag = ToTag(part);
                }

                if (!tag.IsDefault)
                {
                    builder.Add(tag);
                }
            }
        }

        protected ImmutableArray<SymbolDisplayPart>.Builder ToMinimalDisplayParts(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            format = format ?? _minimallyQualifiedFormat;
            return symbol.ToMinimalDisplayParts(_semanticModel, _position, format).ToBuilder();
        }

        private SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
        {
            if (_semanticModel.SyntaxTree.Equals(syntaxTree)) return _semanticModel;

            var compilation = _semanticModel.Compilation;
            if (compilation.ContainsSyntaxTree(syntaxTree)) return compilation.GetSemanticModel(syntaxTree);

            // NOTE: try to find from referenced projects
            foreach (var referencedCompilation in compilation.GetReferencedCompilations())
            {
                if (referencedCompilation.ContainsSyntaxTree(syntaxTree)) return referencedCompilation.GetSemanticModel(syntaxTree);
            }
            return null;
        }

        // TODO: move to some extension class
        private static void InsertRange<T>(ImmutableArray<T>.Builder builder, int index, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                builder.Insert(index++, item);
            }
        }
    }
}