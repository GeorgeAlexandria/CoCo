using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications
{
    internal interface ICodeClassifier
    {
        IClassificationType GetClassification(ISymbol symbol);
    }
}