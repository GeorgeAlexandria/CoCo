using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser;
using CoCo.Analyser.VisualBasic;
using CoCo.Editor;
using CoCo.Settings;
using CoCo.Utils;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.Providers
{
    /// <summary>
    /// Classifier provider which adds <see cref="VisualBasicClassifier"/> to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("Basic")]
    public class VisualBasicClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Determines that settings was set to avoid a many sets settings from the classifier
        /// </summary>
        private static bool _wereSettingsSet;

        private readonly Dictionary<string, ClassificationInfo> _classificationsInfo;

        public VisualBasicClassifierProvider()
        {
            _classificationsInfo = new Dictionary<string, ClassificationInfo>(VisualBasicNames.All.Length);
            foreach (var item in VisualBasicNames.All)
            {
                _classificationsInfo[item] = default;
            }
            ClassificationChangingService.Instance.ClassificationChanged += OnAnalyzeOptionChanged;
        }

#pragma warning disable 649

        /// <summary>
        /// Text document factory to be used for getting a event of text document disposed.
        /// </summary>
        [Import]
        private ITextDocumentFactoryService _textDocumentFactoryService;

#pragma warning restore 649

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            MigrationService.MigrateSettingsTo_2_0_0();
            if (!_wereSettingsSet)
            {
                var settings = SettingsManager.LoadEditorSettings(Paths.CoCoClassificationSettingsFile, MigrationService.Instance);
                var option = OptionService.ToOption(settings);
                FormattingService.SetFormattingOptions(option);
                ClassificationChangingService.SetAnalyzingOptions(option);
                _wereSettingsSet = true;
            }

            return textBuffer.Properties.GetOrCreateSingletonProperty(() =>
                new VisualBasicClassifier(_classificationsInfo, ClassificationChangingService.Instance, _textDocumentFactoryService, textBuffer));
        }

        private void OnAnalyzeOptionChanged(ClassificationsChangedEventArgs args)
        {
            foreach (var (classificationType, info) in args.ChangedClassifications)
            {
                if (_classificationsInfo.ContainsKey(classificationType.Classification))
                {
                    _classificationsInfo[classificationType.Classification] = new ClassificationInfo(classificationType, info);
                }
            }
        }
    }
}