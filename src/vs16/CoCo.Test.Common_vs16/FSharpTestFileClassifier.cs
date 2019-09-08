using System.Collections.Generic;
using CoCo.Analyser.Classifications.FSharp;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Common
{
    internal sealed class FSharpTestFileClassifier : ITestFileClassifier
    {
        private readonly FSharpCompilationUnit _compilationUnit;
        private readonly FSharpTextBufferClassifier _textBufferClassifier;

        public FSharpTestFileClassifier(FSharpCompilationUnit compilationUnit, FSharpTextBufferClassifier textBufferClassifier)
        {
            _compilationUnit = compilationUnit;
            _textBufferClassifier = textBufferClassifier;
        }

        public List<ClassificationSpan> GetClassificationSpans(Workspace workspace, string itemPath, SnapshotSpan snapshotSpan)
        {
            return _textBufferClassifier.GetClassificationSpans(workspace, snapshotSpan);
        }
    }
}