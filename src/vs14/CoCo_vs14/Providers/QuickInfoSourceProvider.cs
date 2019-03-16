using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser;
using CoCo.QuickInfo;
using CoCo.Utils;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace CoCo.Providers
{
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("CoCo quickInfo provider")]
    [ContentType("any")]
    internal sealed class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        /// <summary>
        /// Determines that settings were set to avoid a many sets settings from the classifier
        /// </summary>
        private static bool _wereSettingsSet;

        private readonly Dictionary<string, QuickInfoState> _quickInfoOptions;

        /// <summary>
        /// Text document factory to be used for getting a event of text document disposed.
        /// </summary>
        [Import]
        private ITextDocumentFactoryService _textDocumentFactoryService;

        public QuickInfoSourceProvider()
        {
            _quickInfoOptions = new Dictionary<string, QuickInfoState>
            {
                [Languages.CSharp] = default,
                [Languages.VisualBasic] = default
            };

            QuickInfoChangingService.Instance.QuickInfoChanged += OnQuickInfoChanged;
        }

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            if (!_wereSettingsSet)
            {
                var settings = Settings.SettingsManager.LoadQuickInfoSettings(Paths.CoCoQuickInfoSettingsFile);
                var options = OptionService.ToOption(settings);
                QuickInfoChangingService.SetQuickInfoOptions(options);
                _wereSettingsSet = true;
            }

            return textBuffer.Properties.GetOrCreateSingletonProperty(() =>
                new QuickInfoSource(textBuffer, _quickInfoOptions, _textDocumentFactoryService));
        }

        private void OnQuickInfoChanged(QuickInfoChangedEventArgs args)
        {
            foreach (var (language, state) in args.Changes)
            {
                if (_quickInfoOptions.ContainsKey(language))
                {
                    _quickInfoOptions[language] = state;
                }
            }
        }
    }
}