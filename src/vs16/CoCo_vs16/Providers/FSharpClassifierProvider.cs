using System.ComponentModel.Composition;
using CoCo.Analyser.Classifications.FSharp;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.Providers
{
    /// <summary>
    /// Classifier provider which adds <see cref="FSharpTextBufferClassifier"/> to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("F#")]
    internal class FSharpClassifierProvider : IClassifierProvider
    {
        public FSharpClassifierProvider()
        {
        }

        // Disable "Field is never assigned to..." compiler's warning. The field is assigned by MEF.
#pragma warning disable 649

        /// <summary>
        /// Text document factory to be used for getting a event of text document disposed.
        /// </summary>
        [Import]
        private ITextDocumentFactoryService _textDocumentFactoryService;

#pragma warning restore 649

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new FSharpTextBufferClassifier());
        }
    }
}