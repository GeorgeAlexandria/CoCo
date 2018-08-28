using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    // TODO: implement
    internal class VisualBasicClassifier : RoslynEditorClassifier
    {
        internal VisualBasicClassifier(IReadOnlyDictionary<string, IClassificationType> classifications)
        {
        }

        internal override List<ClassificationSpan> GetClassificationSpans(
            Workspace workspace, SemanticModel semanticModel, SnapshotSpan span)
        {
            throw new NotImplementedException();
        }
    }
}