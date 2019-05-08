using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser;
using CoCo.Analyser.Classifications;
using CoCo.Analyser.Classifications.CSharp;
using CoCo.Analyser.Editor;
using CoCo.Editor;
using CoCo.Settings;
using CoCo.Utils;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.Providers
{
    /// <summary>
    /// Classifier provider which adds <see cref="CSharpTextBufferClassifier"/> to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("CSharp")]
    // TODO: uncomment when will try to add analyzing of an annotates, texts and etx
    //[ContentType("text")]
    internal class CSharpClassifierProvider : IClassifierProvider
    {
        private readonly Dictionary<string, ClassificationInfo> _classificationsInfo;

        /// <summary>
        /// Determines that settings was set to avoid a many sets settings from the classifier
        /// </summary>
        private bool _wereSettingsSet;

        /// <summary>
        /// Determines that classifications in editor is enable or not
        /// </summary>
        private bool _isEnable;

        public CSharpClassifierProvider()
        {
            _classificationsInfo = new Dictionary<string, ClassificationInfo>(CSharpNames.All.Length);
            foreach (var item in CSharpNames.All)
            {
                _classificationsInfo[item] = default;
            }
            ClassificationChangingService.Instance.ClassificationChanged += OnClassificationsChanged;
            GeneralChangingService.Instance.EditorOptionsChanged += OnEditorOptionsChanged;
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

            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new CSharpTextBufferClassifier(
                _classificationsInfo, ClassificationChangingService.Instance,
                _isEnable, GeneralChangingService.Instance,
                _textDocumentFactoryService, textBuffer));
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

        private void OnEditorOptionsChanged(EditorChangedEventArgs args)
        {
            if (args.Changes.TryGetValue(Languages.CSharp, out var isEnable))
            {
                _isEnable = isEnable;
            }
        }
    }
}