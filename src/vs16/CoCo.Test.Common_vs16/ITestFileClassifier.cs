using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Common
{
    internal interface ITestFileClassifier
    {
        List<ClassificationSpan> GetClassificationSpans(Workspace workspace, string itemPath, SnapshotSpan snapshotSpan);
    }
}