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

        internal VisualBasicClassifier(
            IReadOnlyDictionary<string, ClassificationInfo> classifications,
            IAnalyzingService analyzingService,
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
                    if (node is ImportAliasClauseSyntax aliasSyntax)
                    {
                        AppendClassificationSpan(spans, span.Snapshot, aliasSyntax.Identifier.Span, _aliasNamespaceType);
                        continue;
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

                    default:
                        Log.Debug("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        Log.Debug("Node is: {0}", node);
                        break;
                }
            }

            return spans;
        }

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
                options[type] = info;
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

            base.classifications = builder.ToImmutable();
        }
    }
}