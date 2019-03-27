using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.VisualBasic
{
    // TODO: Do we need to write visual basic classifier on VB?
    internal class VisualBasicClassifier : RoslynEditorClassifier
    {
        private IClassificationType _localVariableType;
        private IClassificationType _rangeVariableType;
        private IClassificationType _functionVariableType;
        private IClassificationType _functionType;
        private IClassificationType _subType;
        private IClassificationType _extensionMethodType;
        private IClassificationType _sharedMethodType;
        private IClassificationType _fieldType;
        private IClassificationType _constantFieldType;
        private IClassificationType _enumFieldType;
        private IClassificationType _parameterType;
        private IClassificationType _propertyType;
        private IClassificationType _withEventsPropertyType;
        private IClassificationType _namespaceType;
        private IClassificationType _aliasNamespaceType;
        private IClassificationType _staticLocalVariableType;
        private IClassificationType _eventType;
        private IClassificationType _classType;
        private IClassificationType _structureType;
        private IClassificationType _moduleType;
        private IClassificationType _interfaceType;
        private IClassificationType _delegateType;
        private IClassificationType _enumType;
        private IClassificationType _typeParameterType;

        internal VisualBasicClassifier(
            IReadOnlyDictionary<string, ClassificationInfo> classifications,
            IClassificationChangingService analyzingService,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : base(analyzingService, textDocumentFactoryService, buffer)
        {
            InitializeClassifications(classifications);
        }

        internal VisualBasicClassifier(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            InitializeClassifications(classifications);
        }

        internal override List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, SemanticModel semanticModel, SnapshotSpan span)
        {
            var spans = new List<ClassificationSpan>();

            var root = semanticModel.SyntaxTree.GetCompilationUnitRoot();
            var textSpan = new TextSpan(span.Start.Position, span.Length);
            foreach (var item in Classifier.GetClassifiedSpans(semanticModel, textSpan, workspace))
            {
                if (!ClassificationHelper.IsSupportedClassification(item.ClassificationType)) continue;

                var node = root.FindNode(item.TextSpan, true).HandleNode();
                if (!semanticModel.TryGetSymbolInfo(node, out var symbol, out var reason))
                {
                    // NOTE: handle alias in imports directive
                    if (node is ImportAliasClauseSyntax && node.Parent is SimpleImportsClauseSyntax importSyntax)
                    {
                        var aliasNameSymbol = semanticModel.GetSymbolInfo(importSyntax.Name).Symbol;
                        var aliasType =
                            aliasNameSymbol.Kind == SymbolKind.Namespace ? _aliasNamespaceType :
                            aliasNameSymbol.Kind == SymbolKind.NamedType ? GetTypeClassification(aliasNameSymbol as INamedTypeSymbol) :
                            null;

                        if (!(aliasType is null))
                        {
                            AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, aliasType);
                            continue;
                        }
                    }

                    Log.Debug("Nothing is found. Span start at {0} and end at {1}", item.TextSpan.Start, item.TextSpan.End);
                    Log.Debug("Candidate Reason is {0}", reason);
                    Log.Debug("Node is {0}", node);
                    continue;
                }

                // TODO: add posibility to turn off classification a type characters as part of identifiers
                switch (symbol.Kind)
                {
                    case SymbolKind.Field:
                        var fieldSymbol = symbol as IFieldSymbol;
                        var fieldType =
                            fieldSymbol.Type.TypeKind == TypeKind.Enum ? _enumFieldType :
                            fieldSymbol.IsConst ? _constantFieldType :
                            _fieldType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, fieldType, node);
                        break;

                    case SymbolKind.RangeVariable:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _rangeVariableType);
                        break;

                    case SymbolKind.Local:
                        var localSymbol = symbol as ILocalSymbol;
                        var localType =
                            localSymbol.IsStatic ? _staticLocalVariableType :
                            localSymbol.IsFunctionValue ? _functionVariableType :
                            _localVariableType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, localType);
                        break;

                    case SymbolKind.Method:
                        var methodSymbol = symbol as IMethodSymbol;
                        var methodType =
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.IsShared() || methodSymbol.ContainingType?.TypeKind == TypeKind.Module ? _sharedMethodType :
                            methodSymbol.ReturnType.SpecialType == SpecialType.System_Void ? _subType :
                            _functionType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, methodType, node);
                        break;

                    case SymbolKind.Parameter:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _parameterType, node);
                        break;

                    case SymbolKind.Property:
                        var propertySymbol = symbol as IPropertySymbol;
                        var propertyType = propertySymbol.IsWithEvents ? _withEventsPropertyType : _propertyType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, propertyType, node);
                        break;

                    case SymbolKind.Namespace:
                        var namespaceType = node.IsAliasNamespace(symbol, semanticModel) ? _aliasNamespaceType : _namespaceType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, namespaceType, node);
                        break;

                    case SymbolKind.Event:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _eventType, node);
                        break;

                    case SymbolKind.TypeParameter:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _typeParameterType, node);
                        break;

                    case SymbolKind.NamedType:
                        var type = GetTypeClassification(symbol as INamedTypeSymbol);
                        if (!(type is null))
                        {
                            AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, type, node);
                        }
                        break;

                    default:
                        Log.Debug("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        Log.Debug("Node is: {0}", node);
                        break;
                }
            }

            return spans;
        }

        public IClassificationType GetClassification(ISymbol symbol)
        {
            IClassificationType GetClassification()
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Field:
                        var fieldSymbol = symbol as IFieldSymbol;
                        return
                            fieldSymbol.Type.TypeKind == TypeKind.Enum ? _enumFieldType :
                            fieldSymbol.IsConst ? _constantFieldType :
                            _fieldType;

                    case SymbolKind.RangeVariable:
                        return _rangeVariableType;

                    case SymbolKind.Local:
                        var localSymbol = symbol as ILocalSymbol;
                        return
                            localSymbol.IsStatic ? _staticLocalVariableType :
                            localSymbol.IsFunctionValue ? _functionVariableType :
                            _localVariableType;

                    case SymbolKind.Method:
                        var methodSymbol = symbol as IMethodSymbol;
                        return
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.IsShared() || methodSymbol.ContainingType?.TypeKind == TypeKind.Module ? _sharedMethodType :
                            methodSymbol.ReturnType.SpecialType == SpecialType.System_Void ? _subType :
                            _functionType;

                    case SymbolKind.Parameter:
                        return _parameterType;

                    case SymbolKind.Property:
                        var propertySymbol = symbol as IPropertySymbol;
                        return propertySymbol.IsWithEvents ? _withEventsPropertyType : _propertyType;

                    case SymbolKind.Namespace:
                        return _namespaceType;

                    case SymbolKind.Event:
                        return _eventType;

                    case SymbolKind.TypeParameter:
                        return _typeParameterType;

                    case SymbolKind.NamedType:
                        return GetTypeClassification(symbol as INamedTypeSymbol);
                }
                return null;
            }

            var classification = GetClassification();
            return classification is null || options[classification].IsDisabled
                ? null
                : classification;
        }

        private IClassificationType GetTypeClassification(INamedTypeSymbol typeSymbol) =>
           typeSymbol.TypeKind == TypeKind.Class ? _classType :
           typeSymbol.TypeKind == TypeKind.Struct ? _structureType :
           typeSymbol.TypeKind == TypeKind.Module ? _moduleType :
           typeSymbol.TypeKind == TypeKind.Interface ? _interfaceType :
           typeSymbol.TypeKind == TypeKind.Enum ? _enumType :
           typeSymbol.TypeKind == TypeKind.Delegate ? _delegateType :
           null;

        private void AppendClassificationSpan(
           List<ClassificationSpan> spans, ITextSnapshot snapshot, TextSpan span, IClassificationType type, SyntaxNode node = null)
        {
            var info = options[type];
            if (info.IsDisabled) return;

            if (node is null || !info.IsDisabledInXml || !node.IsPartOfStructuredTrivia() || !node.IsDescendantXmlDocComment())
            {
                spans.Add(new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), type));
            }
        }

        private void InitializeClassifications(IReadOnlyDictionary<string, ClassificationInfo> classifications)
        {
            var builder = ImmutableArray.CreateBuilder<IClassificationType>(17);
            void InitializeClassification(string name, ref IClassificationType type)
            {
                var info = classifications[name];
                type = info.ClassificationType;
                options[type] = info.Option;
                builder.Add(type);
            }

            InitializeClassification(VisualBasicNames.LocalVariableName, ref _localVariableType);
            InitializeClassification(VisualBasicNames.RangeVariableName, ref _rangeVariableType);
            InitializeClassification(VisualBasicNames.FunctionVariableName, ref _functionVariableType);
            InitializeClassification(VisualBasicNames.ExtensionMethodName, ref _extensionMethodType);
            InitializeClassification(VisualBasicNames.SharedMethodName, ref _sharedMethodType);
            InitializeClassification(VisualBasicNames.SubName, ref _subType);
            InitializeClassification(VisualBasicNames.FunctionName, ref _functionType);
            InitializeClassification(VisualBasicNames.FieldName, ref _fieldType);
            InitializeClassification(VisualBasicNames.ConstantFieldName, ref _constantFieldType);
            InitializeClassification(VisualBasicNames.EnumFieldName, ref _enumFieldType);
            InitializeClassification(VisualBasicNames.ParameterName, ref _parameterType);
            InitializeClassification(VisualBasicNames.PropertyName, ref _propertyType);
            InitializeClassification(VisualBasicNames.WithEventsPropertyName, ref _withEventsPropertyType);
            InitializeClassification(VisualBasicNames.NamespaceName, ref _namespaceType);
            InitializeClassification(VisualBasicNames.AliasNamespaceName, ref _aliasNamespaceType);
            InitializeClassification(VisualBasicNames.StaticLocalVariableName, ref _staticLocalVariableType);
            InitializeClassification(VisualBasicNames.EventName, ref _eventType);
            InitializeClassification(VisualBasicNames.ClassName, ref _classType);
            InitializeClassification(VisualBasicNames.StructureName, ref _structureType);
            InitializeClassification(VisualBasicNames.ModuleName, ref _moduleType);
            InitializeClassification(VisualBasicNames.InterfaceName, ref _interfaceType);
            InitializeClassification(VisualBasicNames.DelegateName, ref _delegateType);
            InitializeClassification(VisualBasicNames.EnumName, ref _enumType);
            InitializeClassification(VisualBasicNames.TypeParameterName, ref _typeParameterType);

            base.classifications = builder.ToImmutable();
        }
    }
}