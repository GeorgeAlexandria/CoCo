using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NLog;

namespace CoCo
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "EditorClassifier" classification type.
    /// </summary>
    internal class EditorClassifier : IClassifier
    {
        private readonly IClassificationType _localFieldType;
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
        private readonly IClassificationType _constructorMethodType;

        private readonly ITextBuffer _textBuffer;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;

        private SemanticModel _semanticModel;

        // NOTE: Logger is thread-safe
        private static readonly Logger _logger;

#if DEBUG

        static EditorClassifier()
        {
            NLog.Initialize();
            _logger = LogManager.GetLogger(nameof(_logger));
        }

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorClassifier"/> class.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="textDocumentFactoryService"></param>
        /// <param name="buffer"></param>
        internal EditorClassifier(
            IClassificationTypeRegistryService registry,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer)
        {
            _localFieldType = registry.GetClassificationType(Names.LocalFieldName);
            _namespaceType = registry.GetClassificationType(Names.NamespaceName);
            _parameterType = registry.GetClassificationType(Names.ParameterName);
            _extensionMethodType = registry.GetClassificationType(Names.ExtensionMethodName);
            _methodType = registry.GetClassificationType(Names.MethodName);
            _eventType = registry.GetClassificationType(Names.EventName);
            _propertyType = registry.GetClassificationType(Names.PropertyName);
            _fieldType = registry.GetClassificationType(Names.FieldName);
            _staticMethodType = registry.GetClassificationType(Names.StaticMethodName);
            _enumFieldType = registry.GetClassificationType(Names.EnumFiedName);
            _aliasNamespaceType = registry.GetClassificationType(Names.AliasNamespaceName);
            _constructorMethodType = registry.GetClassificationType(Names.ConstructorMethodName);

            _textDocumentFactoryService = textDocumentFactoryService;
            _textBuffer = buffer;

            _textBuffer.Changed += OnTextBufferChanged;
            _textDocumentFactoryService.TextDocumentDisposed += OnTextDocumentDisposed;
        }

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
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
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>
        /// A list of ClassificationSpans that represent spans identified to be of this classification.
        /// </returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            _logger.ConditionalInfo("Span start position is={0} and end position is={1}", span.Start.Position, span.End.Position);
            var result = new List<ClassificationSpan>();

            // NOTE: Workspace can be null for "Using directive is unnecessary". Also workspace can
            // be null when solution/project failed to load and VS gave some reasons of it or when
            // try to open a file doesn't contained in the current solution
            Workspace workspace = span.Snapshot.TextBuffer.GetWorkspace();
            if (workspace == null)
            {
                // TODO: Add supporting a files that doesn't included to the current solution
                return result;
            }
            Document document = workspace.GetDocument(span.Snapshot.AsText());

            SemanticModel semanticModel = _semanticModel ?? (_semanticModel = document.GetSemanticModelAsync().Result);
            SyntaxTree syntaxTree = semanticModel.SyntaxTree;

            TextSpan textSpan = new TextSpan(span.Start.Position, span.Length);
            var classifiedSpans = Classifier.GetClassifiedSpans(semanticModel, textSpan, workspace)
                .Where(item => item.ClassificationType == "identifier");

            CompilationUnitSyntax unitCompilation = syntaxTree.GetCompilationUnitRoot();
            foreach (var item in classifiedSpans)
            {
                // NOTE: Some kind of nodes, for example ArgumentSyntax, should are handled with a
                // specific way
                SyntaxNode node = unitCompilation.FindNode(item.TextSpan, true).HandleNode();

                ISymbol symbol = semanticModel.GetSymbolInfo(node).Symbol ?? semanticModel.GetDeclaredSymbol(node);
                if (symbol == null)
                {
                    // NOTE: handle alias in using directive
                    if ((node.Parent as NameEqualsSyntax)?.Parent is UsingDirectiveSyntax)
                    {
                        result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _aliasNamespaceType));
                        continue;
                    }

                    // TODO: Log information about a node and semantic model, because semantic model
                    // didn't retrive information from node in this case
                    _logger.ConditionalInfo("Nothing is found. Span start at {0} and end at {1}", span.Start.Position, span.End.Position);
                    _logger.ConditionalInfo("Node is {0} {1}", node.Kind(), node.RawKind);
                    continue;
                }
                switch (symbol.Kind)
                {
                    case SymbolKind.Alias:
                    case SymbolKind.ArrayType:
                    case SymbolKind.Assembly:
                    case SymbolKind.DynamicType:
                    case SymbolKind.ErrorType:
                    case SymbolKind.Label:
                    case SymbolKind.NetModule:
                    case SymbolKind.NamedType:
                    case SymbolKind.PointerType:
                    case SymbolKind.RangeVariable:
                    case SymbolKind.TypeParameter:
                    case SymbolKind.Preprocessing:
                        //case SymbolKind.Discard:
                        _logger.ConditionalInfo("Symbol kind={0} was on position [{1}..{2}]", symbol.Kind, item.TextSpan.Start, item.TextSpan.End);
                        _logger.ConditionalInfo("Text was: {0}", node.GetText().ToString());
                        break;

                    case SymbolKind.Field:
                        var fieldSymbol = (symbol as IFieldSymbol).Type;
                        var fieldType = fieldSymbol.TypeKind == TypeKind.Enum ? _enumFieldType : _fieldType;
                        result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, fieldType));
                        break;

                    case SymbolKind.Property:
                        result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _propertyType));
                        break;

                    case SymbolKind.Event:
                        result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _eventType));
                        break;

                    case SymbolKind.Local:
                        result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _localFieldType));
                        break;

                    case SymbolKind.Namespace:
                        var namesapceType = IsAliasNamespace(symbol, node) ? _namespaceType : _aliasNamespaceType;
                        result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, namesapceType));
                        break;

                    case SymbolKind.Parameter:
                        // NOTE: Skip argument in summaries
                        if (node.Parent.Kind() != SyntaxKind.XmlNameAttribute)
                        {
                            result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, _parameterType));
                        }
                        break;

                    case SymbolKind.Method:
                        var methodSymbol = symbol as IMethodSymbol;
                        var methodType = methodSymbol.MethodKind == MethodKind.Constructor
                            ? _constructorMethodType
                            : methodSymbol.IsExtensionMethod
                                ? _extensionMethodType
                                : methodSymbol.IsStatic ? _staticMethodType : _methodType;
                        result.Add(CreateClassificationSpan(span.Snapshot, item.TextSpan, methodType));
                        break;

                    default:
                        break;
                }
            }

            return result;
        }

        private bool IsAliasNamespace(ISymbol symbol, SyntaxNode node)
        {
            var strSymbol = symbol.ToString();
            if (strSymbol == (node as IdentifierNameSyntax).Identifier.Text)
                return true;

            var fullNamespaceNode = node;
            while (strSymbol != fullNamespaceNode.ToString() && fullNamespaceNode.Parent is QualifiedNameSyntax)
            {
                fullNamespaceNode = fullNamespaceNode.Parent;
            }

            return strSymbol == fullNamespaceNode.ToString();
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