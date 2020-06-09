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
    internal abstract class RoslynTextBufferClassifier : TextBufferClassifier
    {
        protected RoslynTextBufferClassifier() : base()
        {
        }

        protected RoslynTextBufferClassifier(
            bool isEnable,
            IEditorChangingService editorChangingService,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : base(isEnable, editorChangingService, textDocumentFactoryService, buffer)
        {
        }

        internal abstract List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, SemanticModel semanticModel, SnapshotSpan span);

        internal abstract ICodeClassifier CodeClassifier { get; }

        protected override IList<ClassificationSpan> GetClassificationSpans(Workspace workspace, SnapshotSpan span)
        {
            // NOTE: to use the previously classified result as a cached result classifier must correctly determine
            // when it can use it or not. To correctly determine it, classifier must handle all bellow events:
            //
            // * All source code files changing
            // * Source file appending/removing/including/excluding to projects
            // * All project files changing
            // * Project file appending/removing/including/excluding to solution
            // * Solution changing
            // * Solution/projects current configuration changing
            //
            // because all of them may affect to a semantic model of code. Subscribing and unsubscribing on all of them is expensive
            // and all of them are raised very often that make subscribing non effective. So much better way
            // is the get actual data to the everyone requests

            var document = workspace.GetDocument(span.Snapshot.AsText());
            var semanticModel = document.GetSemanticModelAsync().Result;
            return GetClassificationSpans(workspace, semanticModel, span);
        }
    }
}