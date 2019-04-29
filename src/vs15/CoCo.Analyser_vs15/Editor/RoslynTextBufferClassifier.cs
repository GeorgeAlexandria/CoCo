using System;
using System.Collections.Generic;
using CoCo.Analyser.Classifications;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Editor
{
    /// <summary>
    /// Common text buffer classifier for roslyn based languages.
    /// </summary>
    internal abstract class RoslynTextBufferClassifier : IClassifier
    {
        private readonly IEditorChangingService _editorChangingService;
        private readonly ITextBuffer _textBuffer;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;

        private static readonly List<ClassificationSpan> _emptyClassifications = new List<ClassificationSpan>();

        private SemanticModel _semanticModel;
        private bool _isEnable;

        protected RoslynTextBufferClassifier()
        {
            _isEnable = true;
        }

        protected RoslynTextBufferClassifier(
            bool isEnable,
            IEditorChangingService editorChangingService,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer)
        {
            _textBuffer = buffer;
            _textDocumentFactoryService = textDocumentFactoryService;

            _isEnable = isEnable;
            _editorChangingService = editorChangingService;

            _editorChangingService.EditorOptionsChanged += OnEditorOptionsChanged;
            _textBuffer.Changed += OnTextBufferChanged;
            _textDocumentFactoryService.TextDocumentDisposed += OnTextDocumentDisposed;
        }

        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (!_isEnable) return _emptyClassifications;

            Log.Debug("Span start position is={0} and end position is={1}", span.Start.Position, span.End.Position);

            /// NOTE: <see cref="Workspace"/> can be null for "Using directive is unnecessary". Also workspace can
            /// be null when solution|project failed to load and VS gave some reasons of it or when
            /// try to open a file doesn't contained in the current solution
            var workspace = span.Snapshot.TextBuffer.GetWorkspace();
            if (workspace is null)
            {
                // TODO: Add supporting a files that doesn't included to the current solution
                return _emptyClassifications;
            }

            var document = workspace.GetDocument(span.Snapshot.AsText());
            var semanticModel = _semanticModel ?? (_semanticModel = document.GetSemanticModelAsync().Result);

            return GetClassificationSpans(workspace, semanticModel, span);
        }

        internal abstract ICodeClassifier CodeClassifier { get; }

        internal abstract List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, SemanticModel semanticModel, SnapshotSpan span);

        protected abstract string Language { get; }

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) => _semanticModel = null;

        private void OnEditorOptionsChanged(EditorChangedEventArgs args)
        {
            // NOTE: if the state of editor option was changed => raise that classifications were changed for the current buffer
            if (args.Changes.TryGetValue(Language, out var isEnable) && isEnable != _isEnable)
            {
                _isEnable = isEnable;

                var span = new SnapshotSpan(_textBuffer.CurrentSnapshot, new Span(0, _textBuffer.CurrentSnapshot.Length));
                ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(span));
            }
        }

        // TODO: it's not good idea to subscribe on text document disposed. Try to subscribe on text
        // document closed.
        private void OnTextDocumentDisposed(object sender, TextDocumentEventArgs e)
        {
            if (e.TextDocument.TextBuffer == _textBuffer)
            {
                _semanticModel = null;
                _textBuffer.Changed -= OnTextBufferChanged;
                _textDocumentFactoryService.TextDocumentDisposed -= OnTextDocumentDisposed;
                _editorChangingService.EditorOptionsChanged -= OnEditorOptionsChanged;
            }
        }
    }
}