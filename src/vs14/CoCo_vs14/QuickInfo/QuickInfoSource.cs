using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
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

            var items = new List<UIElement>();
            foreach (var item in quickInfo.Descriptions)
            {
                var textBlock = ToTextBlock(item);
                if (item.Kind == SymbolDescriptionKind.Main)
                {
                    var panel = new WrapPanel();
                    Populate(panel, textBlock.Enumerate());
                    items.Insert(0, panel);
                }
                else
                {
                    items.Add(textBlock);
                }
            }

            var container = new StackPanel();
            Populate(container, items);
            quickInfoContent.Add(container);
            applicableToSpan = trackingSpan;
        }

        private void Populate<T>(Panel panel, T uiElements) where T : IEnumerable<UIElement>
        {
            panel.HorizontalAlignment = HorizontalAlignment.Left;
            panel.VerticalAlignment = VerticalAlignment.Top;

            var builder = StringBuilderCache.Acquire();
            foreach (var uiElement in uiElements)
            {
                panel.Children.Add(uiElement);
                var text = uiElement.GetValue(AutomationProperties.NameProperty)?.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    builder.Append(text).Append("\r\n");
                }
            }
            panel.SetValue(AutomationProperties.NameProperty, StringBuilderCache.Release(builder));
        }

        private TextBlock ToTextBlock(SymbolDescription symbolDescription)
        {
            var formatMapService = ServicesProvider.Instance.FormatMapService;
            var tooltipFormatMap = formatMapService.GetClassificationFormatMap("tooltip");
            var textFormatMap = formatMapService.GetClassificationFormatMap("text");
            var defaultRunProperties = tooltipFormatMap.DefaultTextProperties;
            var registryService = ServicesProvider.Instance.RegistryService;

            var textBlock = new TextBlock();
            var builder = StringBuilderCache.Acquire();
            foreach (var classifiedText in symbolDescription.TaggedParts)
            {
                var textRunProperties = defaultRunProperties;
                var classificationType = registryService.GetClassificationType(classifiedText.Tag);
                if (!(classificationType is null))
                {
                    // TODO: compare classification with known CoCo classifications
                    textRunProperties = classificationType.Classification.StartsWith("CoCo")
                        ? textFormatMap.GetExplicitTextProperties(classificationType)
                        : tooltipFormatMap.GetTextProperties(classificationType);
                }

                textBlock.Inlines.Add(new Run
                {
                    Background = textRunProperties.BackgroundBrush,
                    BaselineAlignment = textRunProperties.BaselineAlignment,
                    Foreground = textRunProperties.ForegroundBrush,
                    Text = classifiedText.Text,
                    TextDecorations = textRunProperties.TextDecorations,
                    TextEffects = textRunProperties.TextEffects,
                    FontSize = defaultRunProperties.FontRenderingEmSize,
                    FontFamily = defaultRunProperties.Typeface.FontFamily,
                    FontStretch = defaultRunProperties.Typeface.Stretch,
                    FontStyle = defaultRunProperties.Typeface.Style,
                    FontWeight = defaultRunProperties.Typeface.Weight
                });
                builder.Append(classifiedText.Text);
            }

            textBlock.SetValue(AutomationProperties.NameProperty, StringBuilderCache.Release(builder));
            textBlock.TextWrapping = TextWrapping.Wrap;
            return textBlock;
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