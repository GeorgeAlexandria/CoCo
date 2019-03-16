using System;
using System.Collections.Generic;
using System.Threading;
using CoCo.Analyser;
using CoCo.Analyser.QuickInfo;
using CoCo.Utils;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace CoCo.QuickInfo
{
    internal sealed class QuickInfoSource : IQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;
        private readonly string _language;
        private readonly ITextDocumentFactoryService _documentFactoryService;

        private QuickInfoState _state;

        public QuickInfoSource(
           ITextBuffer textBuffer,
           IReadOnlyDictionary<string, QuickInfoState> quickInfoOptions,
           ITextDocumentFactoryService documentFactoryService)
        {
            _textBuffer = textBuffer;
            _documentFactoryService = documentFactoryService;
            _language = _textBuffer.GetLanguage();
            _state = quickInfoOptions.TryGetValue(_language, out var state) ? state : QuickInfoState.Disable;

            _documentFactoryService.TextDocumentDisposed += OnTextDocumentDisposed;
            QuickInfoChangingService.Instance.QuickInfoChanged += OnQuickInfoChanged;
        }

        public void Dispose()
        {
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            if (_state == QuickInfoState.Disable)
            {
                applicableToSpan = default;
                return;
            }

            if (_state == QuickInfoState.Override)
            {
                quickInfoContent.Clear();
            }

            var triggerPoint = session.GetTriggerPoint(_textBuffer.CurrentSnapshot);
            if (!triggerPoint.HasValue)
            {
                applicableToSpan = default;
                return;
            }

            var quickInfo = QuickInfoService.GetQuickInfo(_textBuffer, triggerPoint.Value, CancellationToken.None).Result;
            if (quickInfo is null || quickInfo.Descriptions.Length == 0)
            {
                applicableToSpan = default;
                return;
            }

            var trackingSpan = triggerPoint.Value.Snapshot.CreateTrackingSpan(quickInfo.GetSpan(), SpanTrackingMode.EdgeInclusive);

            // TODO: cast quick info item to UIElements
            applicableToSpan = trackingSpan;
        }

        private void OnQuickInfoChanged(QuickInfoChangedEventArgs args)
        {
            foreach (var (language, state) in args.Changes)
            {
                if (_language.Equals(language))
                {
                    _state = state;
                }
            }
        }

        private void OnTextDocumentDisposed(object sender, TextDocumentEventArgs e)
        {
            if (e.TextDocument.TextBuffer == _textBuffer)
            {
                QuickInfoChangingService.Instance.QuickInfoChanged -= OnQuickInfoChanged;
            }
        }
    }
}