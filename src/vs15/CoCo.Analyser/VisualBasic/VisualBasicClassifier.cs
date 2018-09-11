using System.Collections.Generic;
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

        internal VisualBasicClassifier(
            IReadOnlyDictionary<string, IClassificationType> classifications,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : base(textDocumentFactoryService, buffer)
        {
            InitializeClassifications(classifications);
        }

        internal VisualBasicClassifier(IReadOnlyDictionary<string, IClassificationType> classifications)
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
                        spans.Add(CreateClassificationSpan(span.Snapshot, aliasSyntax.Identifier.Span, _aliasNamespaceType));
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
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, fieldType));
                        break;

                    case SymbolKind.RangeVariable:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _rangeVariableType));
                        break;

                    case SymbolKind.Local:
                        var localSymbol = symbol as ILocalSymbol;
                        var localType =
                            localSymbol.IsStatic ? _staticLocalVariableType :
                            localSymbol.IsFunctionValue ? _functionVariableType :
                            _localVariableType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, localType));
                        break;

                    case SymbolKind.Method:
                        // TODO: shared methods
                        var methodSymbol = symbol as IMethodSymbol;
                        var methodType =
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.ReturnType.SpecialType == SpecialType.System_Void ? _subType :
                            _functionType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, methodType));
                        break;

                    case SymbolKind.Parameter:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _parameterType));
                        break;

                    case SymbolKind.Property:
                        var propertySymbol = symbol as IPropertySymbol;
                        var propertyType = propertySymbol.IsWithEvents ? _withEventsPropertyType : _propertyType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, propertyType));
                        break;

                    case SymbolKind.Namespace:
                        var namespaceType = node.IsAliasNamespace(symbol) ? _aliasNamespaceType : _namespaceType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, namespaceType));
                        break;

                    default:
                        Log.Debug("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        Log.Debug("Node is: {0}", node);
                        break;
                }
            }

            return spans;
        }

        private static ClassificationSpan CreateClassificationSpan(ITextSnapshot snapshot, TextSpan span, IClassificationType type) =>
           new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), type);

        private void InitializeClassifications(IReadOnlyDictionary<string, IClassificationType> classifications)
        {
            _localVariableType = classifications[VisualBasicNames.LocalVariableName];
            _rangeVariableType = classifications[VisualBasicNames.RangeVariableName];
            _functionVariableType = classifications[VisualBasicNames.FunctionVariableName];
            _extensionMethodType = classifications[VisualBasicNames.ExtensionMethodName];
            _sharedMethodType = classifications[VisualBasicNames.SharedMethodName];
            _subType = classifications[VisualBasicNames.SubName];
            _functionType = classifications[VisualBasicNames.FunctionName];
            _fieldType = classifications[VisualBasicNames.FieldName];
            _constantFieldType = classifications[VisualBasicNames.ConstantFieldName];
            _enumFieldType = classifications[VisualBasicNames.EnumFieldName];
            _parameterType = classifications[VisualBasicNames.ParameterName];
            _propertyType = classifications[VisualBasicNames.PropertyName];
            _withEventsPropertyType = classifications[VisualBasicNames.WithEventsPropertyName];
            _namespaceType = classifications[VisualBasicNames.NamespaceName];
            _aliasNamespaceType = classifications[VisualBasicNames.AliasNamespaceName];
            _staticLocalVariableType = classifications[VisualBasicNames.StaticLocalVariableName];
        }
    }
}