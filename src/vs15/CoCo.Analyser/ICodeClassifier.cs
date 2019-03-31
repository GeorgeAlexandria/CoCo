using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    internal interface ICodeClassifier
    {
        IClassificationType GetClassification(ISymbol symbol);
    }
}