using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    /// <summary>
    /// Common editor classifier for roslyn based languages.
    /// </summary>
    internal abstract class RoslynEditorClassifier : IClassifier
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly IClassificationChangingService _analyzingService;

        private SemanticModel _semanticModel;

        protected readonly Dictionary<IClassificationType, ClassificationOption> options =
            new Dictionary<IClassificationType, ClassificationOption>();

        protected ImmutableArray<IClassificationType> classifications;

        protected RoslynEditorClassifier()
        {
        }

        protected RoslynEditorClassifier(
            IClassificationChangingService analyzingService, ITextDocumentFactoryService textDocumentFactoryService, ITextBuffer buffer)
        {
            _textBuffer = buffer;
            _textDocumentFactoryService = textDocumentFactoryService;
            _analyzingService = analyzingService;

            _textBuffer.Changed += OnTextBufferChanged;
            _textDocumentFactoryService.TextDocumentDisposed += OnTextDocumentDisposed;
            _analyzingService.ClassificationChanged += OnClassificationsChanged;
        }

        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<Microsoft.VisualStudio.Text.Classification.ClassificationChangedEventArgs> ClassificationChanged;

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            Log.Debug("Span start position is={0} and end position is={1}", span.Start.Position, span.End.Position);

            /// NOTE: <see cref="Workspace"/> can be null for "Using directive is unnecessary". Also workspace can
            /// be null when solution|project failed to load and VS gave some reasons of it or when
            /// try to open a file doesn't contained in the current solution
            var workspace = span.Snapshot.TextBuffer.GetWorkspace();
            if (workspace is null)
            {
                // TODO: Add supporting a files that doesn't included to the current solution
                return new List<ClassificationSpan>();
            }

            var document = workspace.GetDocument(span.Snapshot.AsText());
            var semanticModel = _semanticModel ?? (_semanticModel = document.GetSemanticModelAsync().Result);

            return GetClassificationSpans(workspace, semanticModel, span);
        }

        internal abstract List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, SemanticModel semanticModel, SnapshotSpan span);

        private void OnClassificationsChanged(ClassificationsChangedEventArgs args)
        {
            foreach (var classification in classifications)
            {
                if (args.ChangedClassifications.TryGetValue(classification, out var option))
                {
                    options[classification] = option;
                }
            }
        }

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) => _semanticModel = null;

        // TODO: it's not good idea to subscribe on text document disposed. Try to subscribe on text
        // document closed.
        private void OnTextDocumentDisposed(object sender, TextDocumentEventArgs e)
        {
            if (e.TextDocument.TextBuffer == _textBuffer)
            {
                _semanticModel = null;
                _textBuffer.Changed -= OnTextBufferChanged;
                _textDocumentFactoryService.TextDocumentDisposed -= OnTextDocumentDisposed;
                _analyzingService.ClassificationChanged -= OnClassificationsChanged;
            }
        }
    }
}