using System.Collections.Generic;
using CoCo.Analyser.Classifications;
using CoCo.Analyser.Classifications.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Editor
{
    internal class CSharpTextBufferClassifier : RoslynTextBufferClassifier
    {
        private readonly CSharpClassifierService _service;

        internal CSharpTextBufferClassifier(Dictionary<string, ClassificationInfo> classifications) : base()
        {
            _service = CSharpClassifierService.GetClassifier(classifications);
        }

        internal CSharpTextBufferClassifier(
            Dictionary<string, ClassificationInfo> classifications,
            IClassificationChangingService classificationChangingService,
            bool isEnable,
            IEditorChangingService editorChangingService,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : base(isEnable, editorChangingService, textDocumentFactoryService, buffer)
        {
            _service = CSharpClassifierService.GetClassifier(classifications, classificationChangingService);
        }

        internal override ICodeClassifier CodeClassifier => _service;

        protected override string Language => Languages.CSharp;

        internal override List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, SemanticModel semanticModel, SnapshotSpan span)
        {
            return _service.GetClassificationSpans(workspace, semanticModel, span);
        }
    }
}