using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser;
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
    internal class ClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Determines that settings was set to avoid a many sets settings from the classifier
        /// </summary>
        private static bool _wasSettingsSet;

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

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            MigrationService.MigrateSettings();
            if (!_wasSettingsSet)
            {
                var settings = Settings.SettingsManager.LoadSettings(Paths.CoCoSettingsFile);
                FormattingService.SetFormatting(settings);
                _wasSettingsSet = true;
            }

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