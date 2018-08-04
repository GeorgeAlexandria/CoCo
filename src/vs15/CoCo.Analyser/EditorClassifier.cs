using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the <see cref="EditorClassifier"/>" classification type.
    /// </summary>
    internal class EditorClassifier : IClassifier
    {
        private readonly IClassificationType _localVariableType;
        private readonly IClassificationType _rangeVariableType;
        private readonly IClassificationType _namespaceType;
        private readonly IClassificationType _parameterType;
        private readonly IClassificationType _extensionMethodType;
        private readonly IClassificationType _methodType;
        private readonly IClassificationType _eventType;
        private readonly IClassificationType _propertyType;
        private readonly IClassificationType _fieldType;
        private readonly IClassificationType _staticMethodType;
        private readonly IClassificationType _enumFieldType;
        private readonly IClassificationType _aliasNamespaceType;
        private readonly IClassificationType _constructorType;
        private readonly IClassificationType _labelType;
        private readonly IClassificationType _localMethodType;

        private readonly ITextBuffer _textBuffer;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;

        private SemanticModel _semanticModel;

        internal EditorClassifier(
            Dictionary<string, IClassificationType> classifications,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : this(classifications)
        {
            _textBuffer = buffer;
            _textDocumentFactoryService = textDocumentFactoryService;

            _textBuffer.Changed += OnTextBufferChanged;
            _textDocumentFactoryService.TextDocumentDisposed += OnTextDocumentDisposed;
        }

        internal EditorClassifier(Dictionary<string, IClassificationType> classifications)
        {
            _localVariableType = classifications[Names.LocalVariableName];
            _rangeVariableType = classifications[Names.RangeVariableName];
            _namespaceType = classifications[Names.NamespaceName];
            _parameterType = classifications[Names.ParameterName];
            _extensionMethodType = classifications[Names.ExtensionMethodName];
            _methodType = classifications[Names.MethodName];
            _eventType = classifications[Names.EventName];
            _propertyType = classifications[Names.PropertyName];
            _fieldType = classifications[Names.FieldName];
            _staticMethodType = classifications[Names.StaticMethodName];
            _enumFieldType = classifications[Names.EnumFieldName];
            _aliasNamespaceType = classifications[Names.AliasNamespaceName];
            _constructorType = classifications[Names.ConstructorName];
            _labelType = classifications[Names.LabelName];
            _localMethodType = classifications[Names.LocalMethodName];
        }

        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range
        /// of text.
        /// </summary>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            Log.Debug("Span start position is={0} and end position is={1}", span.Start.Position, span.End.Position);

            /// NOTE: <see cref="Workspace"/> can be null for "Using directive is unnecessary". Also workspace can
            /// be null when solution|project failed to load and VS gave some reasons of it or when
            /// try to open a file doesn't contained in the current solution
            var workspace = span.Snapshot.TextBuffer.GetWorkspace();
            if (workspace == null)
            {
                // TODO: Add supporting a files that doesn't included to the current solution
                return new List<ClassificationSpan>();
            }

            var document = workspace.GetDocument(span.Snapshot.AsText());
            var semanticModel = _semanticModel ?? (_semanticModel = document.GetSemanticModelAsync().Result);
            var root = semanticModel.SyntaxTree.GetCompilationUnitRoot();

            return GetClassificationSpans(workspace, semanticModel, root, span);
        }

        internal List<ClassificationSpan> GetClassificationSpans(Workspace workspace, SemanticModel semanticModel, SyntaxNode root, SnapshotSpan span)
        {
            bool IsSupportedClassification(string classification) =>
                classification == "identifier" || classification == "extension method name" || classification == "field name" ||
                classification == "property name" || classification == "method name" || classification == "local name" ||
                classification == "parameter name" || classification == "event name" || classification == "enum member name" ||
                classification == "constant name";

            var spans = new List<ClassificationSpan>();

            var textSpan = new TextSpan(span.Start.Position, span.Length);
            foreach (var item in Classifier.GetClassifiedSpans(semanticModel, textSpan, workspace))
            {
                if (!IsSupportedClassification(item.ClassificationType))
                {
                    continue;
                }

                /// NOTE: Some kind of nodes, for example <see cref="ArgumentSyntax"/>, should are handled with a specific way
                var node = root.FindNode(item.TextSpan, true).HandleNode();

                var info = semanticModel.GetSymbolInfo(node);
                var symbol = info.Symbol ?? semanticModel.GetDeclaredSymbol(node);
                if (symbol == null)
                {
                    // NOTE: handle alias in using directive
                    if ((node.Parent as NameEqualsSyntax)?.Parent is UsingDirectiveSyntax)
                    {
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _aliasNamespaceType));
                        continue;
                    }

                    Log.Debug("Nothing is found. Span start at {0} and end at {1}", span.Start.Position, span.End.Position);
                    Log.Debug("Candidate Reason {0}", info.CandidateReason);
                    Log.Debug("Node is {0}", node);
                    continue;
                }

                switch (symbol.Kind)
                {
                    case SymbolKind.Alias:
                    case SymbolKind.ArrayType:
                    case SymbolKind.Assembly:
                    case SymbolKind.DynamicType:
                    case SymbolKind.ErrorType:
                    case SymbolKind.NetModule:
                    case SymbolKind.NamedType:
                    case SymbolKind.PointerType:
                    case SymbolKind.TypeParameter:
                    case SymbolKind.Preprocessing:
                        //case SymbolKind.Discard:
                        Log.Debug("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        Log.Debug("Text was: {0}", node.GetText().ToString());
                        break;

                    case SymbolKind.Label:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _labelType));
                        break;

                    case SymbolKind.RangeVariable:
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _rangeVariableType));
                        break;

                    case SymbolKind.Field:
                        var fieldType = (symbol as IFieldSymbol).Type;
                        var fieldClassification = fieldType.TypeKind == TypeKind.Enum ? _enumFieldType : _fieldType;
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
                            methodSymbol.MethodKind == MethodKind.LocalFunction ? _localMethodType :
                            methodSymbol.IsExtensionMethod ? _extensionMethodType :
                            methodSymbol.IsStatic ? _staticMethodType :
                            _methodType;
                        spans.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, methodType));
                        break;
                }
            }

            return spans;
        }

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) => _semanticModel = null;

        // TODO: it's not good idea subscribe on text document disposed. Try to subscribe on text
        // document closed.
        private void OnTextDocumentDisposed(object sender, TextDocumentEventArgs e)
        {
            if (e.TextDocument.TextBuffer == _textBuffer)
            {
                _semanticModel = null;
                _textBuffer.Changed -= OnTextBufferChanged;
                _textDocumentFactoryService.TextDocumentDisposed -= OnTextDocumentDisposed;
            }
        }

        private ClassificationSpan CreateClassificationSpan(ITextSnapshot snapshot, TextSpan span, IClassificationType type) =>
            new ClassificationSpan(new SnapshotSpan(snapshot, span.Start, span.Length), type);
    }
}