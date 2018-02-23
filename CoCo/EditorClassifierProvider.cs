using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    /// <summary>
    /// Classifier provider. It adds the classifier to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("CSharp")]
    // TODO: uncomment when will try to add analyzing of an annotates, texts and etx
    //[ContentType("text")]
    internal class EditorClassifierProvider : IClassifierProvider
    {
        // Disable "Field is never assigned to..." compiler's warning. The field is assigned by MEF.
#pragma warning disable 649

        /// <summary>
        /// Classification registry to be used for getting a reference to the custom classification
        /// type later.
        /// </summary>
        [Import]
        private IClassificationTypeRegistryService _classificationRegistry;

        /// <summary>
        /// Text document factory to be used for getting a event of text document disposed.
        /// </summary>
        [Import]
        private ITextDocumentFactoryService _textDocumentFactoryService;

#pragma warning restore 649

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="textBuffer">The <see cref="ITextBuffer"/> to classify.</param>
        /// <returns>
        /// A classifier for the text buffer, or null if the provider cannot do so in its current state.
        /// </returns>
        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            var classificationTypes = new Dictionary<string, IClassificationType>(32);
            foreach (var name in Names.All)
            {
                classificationTypes.Add(name, _classificationRegistry.GetClassificationType(name));
            }

            return textBuffer.Properties.GetOrCreateSingletonProperty(() =>
                new EditorClassifier(classificationTypes, _textDocumentFactoryService, textBuffer));
        }
    }
}