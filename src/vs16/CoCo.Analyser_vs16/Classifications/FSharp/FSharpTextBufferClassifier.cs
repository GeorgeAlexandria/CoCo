using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class FSharpTextBufferClassifier : IClassifier
    {
        private readonly FSharpClassifierService _service;

        private (VersionStamp Version, List<ClassificationSpan> Spans) _cache;

        internal FSharpTextBufferClassifier(
            Dictionary<string, ClassificationInfo> classifications,
            IClassificationChangingService classificationChangingService)
        {
            _service = FSharpClassifierService.GetClassifier(classifications);
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var workspace = span.Snapshot.TextBuffer.GetWorkspace();
            if (workspace is null) return Array.Empty<ClassificationSpan>();

            return GetClassificationSpans(workspace, span);
        }

        public List<ClassificationSpan> GetClassificationSpans(Workspace workspace, SnapshotSpan span)
        {
            var document = workspace.GetDocument(span.Snapshot.AsText());
            var versionStamp = document.GetTextVersionAsync().Result;
            if (versionStamp.Equals(_cache.Version)) return _cache.Spans;

            var sourceText = document.GetTextAsync().Result;
            var projectOptions = GetOptions(workspace, document.Project);

            return GetClassificationSpans(projectOptions, span, document.FilePath, sourceText, versionStamp);
        }

        public List<ClassificationSpan> GetClassificationSpans(FSharpProjectOptions projectOptions, SnapshotSpan snapshotSpan, 
            string itemPath, SourceText itemContent, VersionStamp itemVersion)
        {
            // TODO: would be better to use a custom ReferenceResolver implementaion?
            var checker = FSharpChecker.Create(null, null, null, null, null, null);
            var result = checker.ParseAndCheckFileInProject(itemPath, itemVersion.GetHashCode(),
                new SourceTextWrapper(itemContent), projectOptions, null, "CoCo_Classifications");
            var (parseResult, checkAnswer) = FSharpAsync.RunSynchronously(result, null, null).ToValueTuple();

            if (checkAnswer.IsSucceeded && checkAnswer is FSharpCheckFileAnswer.Succeeded succeeded)
            {
                var checkResult = succeeded.Item;
                // TODO: append classification options
                var spans = _service.GetClassificationSpans(parseResult, checkResult, snapshotSpan);

                _cache = (itemVersion, spans);
                return spans;
            }
            return new List<ClassificationSpan>();
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