using System.Collections.Generic;
using CoCo.Analyser.Classifications;
using CoCo.Analyser.Classifications.FSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.FSharp.Compiler.SourceCodeServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Editor
{
    internal class FSharpTextBufferClassifier : TextBufferClassifier, IListener
    {
        private readonly FSharpClassifierService _service;

        private (VersionStamp Version, List<ClassificationSpan> Spans) _cache;
        private ITextSnapshot _textSnapshot;

        protected override string Language => Languages.FSharp;

        internal FSharpTextBufferClassifier(Dictionary<string, ClassificationInfo> classifications) : base()
        {
            _service = FSharpClassifierService.GetClassifier(classifications);
        }

        internal FSharpTextBufferClassifier(
            Dictionary<string, ClassificationInfo> classifications,
            IClassificationChangingService classificationChangingService,
            bool isEnable,
            IEditorChangingService editorChangingService,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer) : base(isEnable, editorChangingService, textDocumentFactoryService, buffer)
        {
            _service = FSharpClassifierService.GetClassifier(classifications, classificationChangingService);
        }

        public void Invoke()
        {
            _cache = default;
            RaiseClassificationChanded(new SnapshotSpan(_textSnapshot, 0, _textSnapshot.Length));
        }

        public List<ClassificationSpan> GetClassificationSpans(FSharpProjectOptions projectOptions,
            SnapshotSpan snapshotSpan, string itemPath, SourceText itemContent, VersionStamp itemVersion, IProjectChecker projectChecker)
        {
            var parseCheckResult = projectChecker.ParseAndCheckFileInProject(this, projectOptions, itemPath, itemContent,
                itemVersion);

            var spans = !parseCheckResult.IsDefault
                ? _service.GetClassificationSpans(parseCheckResult.ParseResult, parseCheckResult.CheckResult, snapshotSpan)
                : new List<ClassificationSpan>();

            _cache = (itemVersion, spans);
            return spans;
        }

        protected override IList<ClassificationSpan> GetClassificationSpans(Workspace workspace, SnapshotSpan span)
        {
            var document = workspace.GetDocument(span.Snapshot.AsText());
            var versionStamp = document.GetTextVersionAsync().Result;
            if (versionStamp.Equals(_cache.Version)) return _cache.Spans;

            _textSnapshot = span.Snapshot;

            var sourceText = document.GetTextAsync().Result;
            var projectOptions = ProjectChecker.Instance.GetOptions(workspace, document);

            return GetClassificationSpans(projectOptions, span, document.FilePath, sourceText, versionStamp, ProjectChecker.Instance);
        }
    }
}