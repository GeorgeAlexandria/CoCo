using System.Collections.Generic;
using System.ComponentModel.Composition;
using CoCo.Analyser;
using CoCo.Services;
using CoCo.Utils;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CoCo
{
    [Export(typeof(IToolTipPresenterFactory))]
    [Order(Before = "default")]
    internal class ToolTipPresenterFactory : IToolTipPresenterFactory
    {
        /// <summary>
        /// Determines that settings were set to avoid a many sets settings from the classifier
        /// </summary>
        private static bool _wereSettingsSet;

        private readonly Dictionary<string, QuickInfoState> _quickInfoOptions;

        [Import]
        private IViewElementFactoryService _viewElementFactoryService;

        public ToolTipPresenterFactory()
        {
            _quickInfoOptions = new Dictionary<string, QuickInfoState>
            {
                [Languages.CSharp] = default,
                [Languages.VisualBasic] = default
            };

            QuickInfoChangingService.Instance.QuickInfoChanged += OnQuickInfoChanged;
        }

        public IToolTipPresenter Create(ITextView textView, ToolTipParameters parameters)
        {
            if (!_wereSettingsSet)
            {
                var settings = Settings.SettingsManager.LoadQuickInfoSettings(Paths.CoCoQuickInfoSettingsFile);
                var options = OptionService.ToOption(settings);
                QuickInfoChangingService.SetQuickInfoOptions(options);
                _wereSettingsSet = true;
            }

            var language = textView.TextBuffer.GetLanguage();
            if (_quickInfoOptions.TryGetValue(language, out var state) && state != QuickInfoState.Override)
            {
                // NOTE: the next tooltip presenter would be invoked when an one from the exported returns null
                return null;
            }

            return parameters.TrackMouse
                ? new MouseTrackToolTipPresenter(_viewElementFactoryService, textView, parameters)
                : new ToolTipPresenter(_viewElementFactoryService, textView, parameters);
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