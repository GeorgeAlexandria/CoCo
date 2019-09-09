using System.Collections.Generic;
using CoCo.Analyser.Editor;
using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Common
{
    internal sealed class RoslynTestFileClassifier : ITestFileClassifier
    {
        private readonly RoslynCompilationUnit _compilationUnit;
        private readonly RoslynTextBufferClassifier _textBufferClassifier;

        public RoslynTestFileClassifier(RoslynCompilationUnit compilationUnit, RoslynTextBufferClassifier textBufferClassifier)
        {
            _compilationUnit = compilationUnit;
            _textBufferClassifier = textBufferClassifier;
        }

        public List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, string itemPath, string code, SnapshotSpan snapshotSpan)
        {
            foreach (var item in _compilationUnit.Compilation.SyntaxTrees)
            {
                if (item.FilePath.EqualsNoCase(itemPath))
                {
                    var semanticModel = _compilationUnit.Compilation.GetSemanticModel(item, true);
                    return _textBufferClassifier.GetClassificationSpans(workspace, semanticModel, snapshotSpan);
                }
            }
            // TODO: cache
            return new List<ClassificationSpan>();
        }
    }
}