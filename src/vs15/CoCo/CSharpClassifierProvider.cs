using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser;
using CoCo.Analyser.CSharp;
using CoCo.Utils;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    /// <summary>
    /// Classifier provider which adds <see cref="CSharpClassifier"/> to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("CSharp")]
    // TODO: uncomment when will try to add analyzing of an annotates, texts and etx
    //[ContentType("text")]
    internal class CSharpClassifierProvider : IClassifierProvider
    {
        public CSharpClassifierProvider()
        {
            _classificationsInfo = new Dictionary<string, ClassificationInfo>();
            foreach (var item in CSharpNames.All)
            {
                _classificationsInfo[item] = default;
            }
            AnalyzingService.Instance.ClassificationChanged += OnAnalyzeOptionChanged;
        }

        /// <summary>
        /// Determines that settings was set to avoid a many sets settings from the classifier
        /// </summary>
        private static bool _wasSettingsSet;

        private Dictionary<string, ClassificationInfo> _classificationsInfo;

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
            MigrationService.MigrateSettingsTo_2_0_0();
            if (!_wasSettingsSet)
            {
                var settings = Settings.SettingsManager.LoadSettings(Paths.CoCoSettingsFile);
                settings = MigrationService.MigrateSettingsTo_2_3_0(settings);
                var option = FormattingService.SetFormatting(settings);
                AnalyzingService.SetAnalyzingOptions(option);
                _wasSettingsSet = true;
            }

            return textBuffer.Properties.GetOrCreateSingletonProperty(() =>
                new CSharpClassifier(_classificationsInfo, AnalyzingService.Instance, _textDocumentFactoryService, textBuffer));
        }

        private void OnAnalyzeOptionChanged(Analyser.ClassificationChangedEventArgs args)
        {
            foreach (var (classificationType, info) in args.ChangedClassifications)
            {
                if (_classificationsInfo.ContainsKey(classificationType.Classification))
                {
                    _classificationsInfo[classificationType.Classification] = info;
                }
            }
        }
    }
}