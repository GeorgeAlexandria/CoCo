using System.Collections.Generic;
using System.Collections.Immutable;
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
        private IClassificationType _constructorMethodType;
        private IClassificationType _labelType;
        private IClassificationType _constantFieldType;
        private IClassificationType _destructorMethodType;

        internal CSharpClassifier(
            IReadOnlyDictionary<string, ClassificationInfo> classifications,
            IAnalyzingService analyzingService,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : base(analyzingService, textDocumentFactoryService, buffer)
        {
            InitializeClassifications(classifications);
        }

        internal CSharpClassifier(IReadOnlyDictionary<string, ClassificationInfo> classifications)
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
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _aliasNamespaceType);
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
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _labelType);
                        break;

                    case SymbolKind.RangeVariable:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _rangeVariableType);
                        break;

                    case SymbolKind.Field:
                        var fieldSymbol = symbol as IFieldSymbol;
                        var fieldClassification =
                            fieldSymbol.Type.TypeKind == TypeKind.Enum ? _enumFieldType :
                            fieldSymbol.IsConst ? _constantFieldType :
                            _fieldType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, fieldClassification);
                        break;

                    case SymbolKind.Property:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _propertyType);
                        break;

                    case SymbolKind.Event:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _eventType);
                        break;

                    case SymbolKind.Local:
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _localVariableType);
                        break;

                    case SymbolKind.Namespace:
                        var namesapceType = node.IsAliasNamespace(symbol, semanticModel) ? _aliasNamespaceType : _namespaceType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, namesapceType);
                        break;

                    case SymbolKind.Parameter:
                        // NOTE: Skip argument in summaries
                        if (node.Parent.Kind() != SyntaxKind.XmlNameAttribute)
                        {
                            AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, _parameterType);
                        }
                        break;

                    case SymbolKind.Method:
                        var methodSymbol = symbol as IMethodSymbol;
                        var methodType =
                            methodSymbol.MethodKind == MethodKind.Destructor ? _destructorMethodType :
                            methodSymbol.MethodKind == MethodKind.Constructor ? _constructorMethodType :
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.IsStatic ? _staticMethodType :
                            _methodType;
                        AppendClassificationSpan(spans, span.Snapshot, item.TextSpan, methodType);
                        break;

                    default:
                        Log.Debug("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        Log.Debug("Node is: {0}", node);
                        break;
                }
            }

            return spans;
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

            InitializeClassification(CSharpNames.LocalVariableName, ref _localVariableType);
            InitializeClassification(CSharpNames.RangeVariableName, ref _rangeVariableType);
            InitializeClassification(CSharpNames.NamespaceName, ref _namespaceType);
            InitializeClassification(CSharpNames.ParameterName, ref _parameterType);
            InitializeClassification(CSharpNames.ExtensionMethodName, ref _extensionMethodType);
            InitializeClassification(CSharpNames.MethodName, ref _methodType);
            InitializeClassification(CSharpNames.EventName, ref _eventType);
            InitializeClassification(CSharpNames.PropertyName, ref _propertyType);
            InitializeClassification(CSharpNames.FieldName, ref _fieldType);
            InitializeClassification(CSharpNames.StaticMethodName, ref _staticMethodType);
            InitializeClassification(CSharpNames.EnumFieldName, ref _enumFieldType);
            InitializeClassification(CSharpNames.AliasNamespaceName, ref _aliasNamespaceType);
            InitializeClassification(CSharpNames.ConstructorName, ref _constructorMethodType);
            InitializeClassification(CSharpNames.LabelName, ref _labelType);
            InitializeClassification(CSharpNames.ConstantFieldName, ref _constantFieldType);
            InitializeClassification(CSharpNames.DestructorName, ref _destructorMethodType);

            base.classifications = builder.ToImmutable();
        }
    }
}