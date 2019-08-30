using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser.Classifications;
using CoCo.Analyser.Classifications.FSharp;
using CoCo.Editor;
using CoCo.Settings;
using CoCo.Utils;
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
        private readonly Dictionary<string, ClassificationInfo> _classificationsInfo;

        /// <summary>
        /// Determines that settings was set to avoid a many sets settings from the classifier
        /// </summary>
        private bool _wereSettingsSet;

        public FSharpClassifierProvider()
        {
            _classificationsInfo = new Dictionary<string, ClassificationInfo>(FSharpNames.All.Length);
            foreach (var item in FSharpNames.All)
            {
                _classificationsInfo[item] = default;
            }

            ClassificationChangingService.Instance.ClassificationChanged += OnClassificationsChanged;
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
            MigrationService.MigrateSettingsTo_2_0_0();
            MigrationService.MigrateSettingsTo_3_1_0();
            if (!_wereSettingsSet)
            {
                var editorSettings = SettingsManager.LoadEditorSettings(Paths.CoCoClassificationSettingsFile, MigrationService.Instance);
                var editorOption = OptionService.ToOption(editorSettings);
                FormattingService.SetFormattingOptions(editorOption);
                ClassificationChangingService.SetAnalyzingOptions(editorOption);

                var generalSettings = SettingsManager.LoadGeneralSettings(Paths.CoCoGeneralSettingsFile, MigrationService.Instance);
                var generalOption = OptionService.ToOption(generalSettings);
                GeneralChangingService.SetGeneralOptions(generalOption);

                _wereSettingsSet = true;
            }

            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new FSharpTextBufferClassifier(
                _classificationsInfo, ClassificationChangingService.Instance));
        }

        private void OnClassificationsChanged(ClassificationsChangedEventArgs args)
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