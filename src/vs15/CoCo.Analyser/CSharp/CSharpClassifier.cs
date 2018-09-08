using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.CSharp
{
    /// <summary>
    /// Classifies csharp code
    /// </summary>
    internal class CSharpClassifier : RoslynEditorClassifier
    {
        private IClassificationType _localVariableType;
        private IClassificationType _rangeVariableType;
        private IClassificationType _namespaceType;
        private IClassificationType _parameterType;
        private IClassificationType _extensionMethodType;
        private IClassificationType _methodType;
        private IClassificationType _eventType;
        private IClassificationType _propertyType;
        private IClassificationType _fieldType;
        private IClassificationType _staticMethodType;
        private IClassificationType _enumFieldType;
        private IClassificationType _aliasNamespaceType;
        private IClassificationType _constructorType;
        private IClassificationType _labelType;
        private IClassificationType _localMethodType;
        private IClassificationType _constantFieldType;
        private IClassificationType _destructorType;

        internal CSharpClassifier(
            IReadOnlyDictionary<string, IClassificationType> classifications,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : base(textDocumentFactoryService, buffer)
        {
            InitializeClassifications(classifications);
        }

        internal CSharpClassifier(IReadOnlyDictionary<string, IClassificationType> classifications)
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

                /// NOTE: Some kind of nodes, for example <see cref="ArgumentSyntax"/>, should are handled with a specific way
                var node = root.FindNode(item.TextSpan, true).HandleNode();
                if (!semanticModel.TryGetSymbolInfo(node, out var symbol, out var reason))
                {
                    // NOTE: handle alias in using directive
                    if ((node.Parent as NameEqualsSyntax)?.Parent is UsingDirectiveSyntax)
                    {
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _aliasNamespaceType));
                        continue;
                    }

                    Log.Debug("Nothing is found. Span start at {0} and end at {1}", item.TextSpan.Start, item.TextSpan.End);
                    Log.Debug("Candidate Reason is {0}", reason);
                    Log.Debug("Node is {0}", node);
                    continue;
                }

                switch (symbol.Kind)
                {
                    case SymbolKind.Label:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _labelType));
                        break;

                    case SymbolKind.RangeVariable:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _rangeVariableType));
                        break;

                    case SymbolKind.Field:
                        var fieldSymbol = symbol as IFieldSymbol;
                        var fieldClassification =
                            fieldSymbol.Type.TypeKind == TypeKind.Enum ? _enumFieldType :
                            fieldSymbol.IsConst ? _constantFieldType :
                            _fieldType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, fieldClassification));
                        break;

                    case SymbolKind.Property:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _propertyType));
                        break;

                    case SymbolKind.Event:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _eventType));
                        break;

                    case SymbolKind.Local:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _localVariableType));
                        break;

                    case SymbolKind.Namespace:
                        var namesapceType = node.IsAliasNamespace(symbol) ? _aliasNamespaceType : _namespaceType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, namesapceType));
                        break;

                    case SymbolKind.Parameter:
                        // NOTE: Skip argument in summaries
                        // TODO: add tests for it!
                        if (node.Parent.Kind() != SyntaxKind.XmlNameAttribute)
                        {
                            spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _parameterType));
                        }
                        break;

                    case SymbolKind.Method:
                        var methodSymbol = symbol as IMethodSymbol;
                        var methodType =
                            methodSymbol.MethodKind == MethodKind.Constructor ? _constructorType :
                            methodSymbol.MethodKind == MethodKind.Destructor ? _destructorType :
                            methodSymbol.MethodKind == MethodKind.LocalFunction ? _localMethodType :
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.IsStatic ? _staticMethodType :
                            _methodType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, methodType));
                        break;

                    default:
                        Log.Debug("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        Log.Debug("Node is: {0}", node);
                        break;
                }
            }

            return spans;
        }

        private void InitializeClassifications(IReadOnlyDictionary<string, IClassificationType> classifications)
        {
            _localVariableType = classifications[CSharpNames.LocalVariableName];
            _rangeVariableType = classifications[CSharpNames.RangeVariableName];
            _namespaceType = classifications[CSharpNames.NamespaceName];
            _parameterType = classifications[CSharpNames.ParameterName];
            _extensionMethodType = classifications[CSharpNames.ExtensionMethodName];
            _methodType = classifications[CSharpNames.MethodName];
            _eventType = classifications[CSharpNames.EventName];
            _propertyType = classifications[CSharpNames.PropertyName];
            _fieldType = classifications[CSharpNames.FieldName];
            _staticMethodType = classifications[CSharpNames.StaticMethodName];
            _enumFieldType = classifications[CSharpNames.EnumFieldName];
            _aliasNamespaceType = classifications[CSharpNames.AliasNamespaceName];
            _constructorType = classifications[CSharpNames.ConstructorName];
            _labelType = classifications[CSharpNames.LabelName];
            _localMethodType = classifications[CSharpNames.LocalMethodName];
            _constantFieldType = classifications[CSharpNames.ConstantFieldName];
            _destructorType = classifications[CSharpNames.DestructorName];
        }

        private ClassificationSpan CreateClassificationSpan(ITextSnapshot snapshot, TextSpan span, IClassificationType type) =>
            new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), type);
    }
}