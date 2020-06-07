using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoCo.Analyser.Editor;
using FSharp.Compiler;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications.FSharp
{
    public class FSharpTextBufferClassifier : IClassifier, IListener
    {
        private readonly FSharpClassifierService _service;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly IEditorChangingService _editorChangingService;
        private readonly ITextBuffer _textBuffer;

        private bool _isEnable;
        private (VersionStamp Version, List<ClassificationSpan> Spans) _cache;
        private ITextSnapshot _textSnapshot;

        internal FSharpTextBufferClassifier(
            Dictionary<string, ClassificationInfo> classifications,
            IClassificationChangingService classificationChangingService,
            bool isEnable,
            IEditorChangingService editorChangingService,
            ITextDocumentFactoryService textDocumentFactoryService,
            ITextBuffer buffer)
        {
            _service = FSharpClassifierService.GetClassifier(classifications, classificationChangingService);
            _isEnable = isEnable;

            _textBuffer = buffer;
            _textDocumentFactoryService = textDocumentFactoryService;

            _isEnable = isEnable;
            _editorChangingService = editorChangingService;

            _editorChangingService.EditorOptionsChanged += OnEditorOptionsChanged;
            _textDocumentFactoryService.TextDocumentDisposed += OnTextDocumentDisposed;
        }

        internal FSharpTextBufferClassifier(
            Dictionary<string, ClassificationInfo> classifications,
            IClassificationChangingService classificationChangingService)
        {
            _service = FSharpClassifierService.GetClassifier(classifications);
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (!_isEnable) return Array.Empty<ClassificationSpan>();

            var workspace = span.Snapshot.TextBuffer.GetWorkspace();
            if (workspace is null) return Array.Empty<ClassificationSpan>();

            var document = workspace.GetDocument(span.Snapshot.AsText());
            var versionStamp = document.GetTextVersionAsync().Result;
            if (versionStamp.Equals(_cache.Version)) return _cache.Spans;

            _textSnapshot = span.Snapshot;

            var sourceText = document.GetTextAsync().Result;
            var projectOptions = GetOptions(workspace, document);

            return GetClassificationSpans(projectOptions, span, document.FilePath, sourceText, versionStamp, ProjectChecker.Instance);
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

        public void Invoke()
        {
            _cache = default;
            ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(new SnapshotSpan(_textSnapshot, 0, _textSnapshot.Length)));
        }

        private void OnEditorOptionsChanged(EditorChangedEventArgs args)
        {
            // NOTE: if the state of editor option was changed => raise that classifications were changed for the current buffer
            if (args.Changes.TryGetValue(Languages.FSharp, out var isEnable) && isEnable != _isEnable)
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
                _textDocumentFactoryService.TextDocumentDisposed -= OnTextDocumentDisposed;
                _editorChangingService.EditorOptionsChanged -= OnEditorOptionsChanged;
            }
        }

        private FSharpProjectOptions GetOptions(Workspace workspace, Document document)
        {
            if (string.IsNullOrWhiteSpace(document.Project.FilePath))
            {
                var checker = FSharpChecker.Create(null, true, false, null, null, null);
                var task = checker.GetProjectOptionsFromScript(document.FilePath, new SourceTextWrapper(document.GetTextAsync().Result),
                    null, null, null, null, null, null, null, "CoCo_script_options");
                return FSharpAsync.RunSynchronously(task, null, null).Item1;
            }

            return GetOptions(workspace, document.Project);
        }

        private FSharpProjectOptions GetOptions(Workspace workspace, Project project)
        {
            var referencedProjectsOptions = new List<Tuple<string, FSharpProjectOptions>>();
            foreach (var item in project.ProjectReferences)
            {
                var referencedProject = project.Solution.GetProject(item.ProjectId);
                if (string.Equals(project.Language, "F#"))
                {
                    var projectOptions = GetOptions(workspace, referencedProject);
                    referencedProjectsOptions.Add((referencedProject.OutputFilePath, projectOptions).ToTuple());
                }
            }

            var options = new List<string>();
            foreach (var item in new FscOptionsBuilder(project.FilePath).Build())
            {
                if (!item.StartsWith("-r:"))
                {
                    options.Add(item);
                }
            }
            foreach (var item in project.ProjectReferences)
            {
                options.Add("-r:" + project.Solution.GetProject(item.ProjectId).OutputFilePath);
            }
            foreach (var item in project.MetadataReferences.OfType<PortableExecutableReference>())
            {
                options.Add("-r:" + item.FilePath);
            }

            return new FSharpProjectOptions(
                project.FilePath,
                project.Id.Id.ToString("D").ToLowerInvariant(),
                project.Documents.Select(x => x.FilePath).ToArray(),
                options.ToArray(),
                referencedProjectsOptions.ToArray(),
                false,
                SourceFile.MustBeSingleFileProject(Path.GetFileName(project.FilePath)),
                DateTime.Now,
                null,
                FSharpList<Tuple<Range.range, string>>.Empty,
                null,
                FSharpOption<long>.Some(project.Version.GetHashCode()));
        }
    }
}