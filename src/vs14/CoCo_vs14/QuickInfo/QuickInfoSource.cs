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
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace CoCo.QuickInfo
{
    using CoCoImageKind = CoCo.Analyser.QuickInfo.ImageKind;

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
            _state = !(_language is null) && quickInfoOptions.TryGetValue(_language, out var state)
                ? state
                : QuickInfoChangingService.Instance.GetDefaultValue();

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

            var quickInfo = QuickInfoService.GetQuickInfoAsync(_textBuffer, triggerPoint.Value, CancellationToken.None).Result;
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

                    var uiElements = TryGetImageElement(quickInfo.Image, out var image)
                        ? image.Enumerate().Concat(textBlock.Enumerate())
                        : textBlock.Enumerate();
                    Populate(panel, uiElements);

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

        private static bool TryGetImageElement(CoCoImageKind image, out UIElement imageElement)
        {
            var id =
                image == CoCoImageKind.ClassPublic ? KnownImageIds.ClassPublic :
                image == CoCoImageKind.ClassInternal ? KnownImageIds.ClassInternal :
                image == CoCoImageKind.ClassProtected ? KnownImageIds.ClassProtected :
                image == CoCoImageKind.ClassPrivate ? KnownImageIds.ClassPrivate :

                image == CoCoImageKind.ConstPublic ? KnownImageIds.ConstantPublic :
                image == CoCoImageKind.ConstInternal ? KnownImageIds.ConstantInternal :
                image == CoCoImageKind.ConstProtected ? KnownImageIds.ConstantProtected :
                image == CoCoImageKind.ConstPrivate ? KnownImageIds.ConstantPrivate :

                image == CoCoImageKind.DelegatePublic ? KnownImageIds.DelegatePublic :
                image == CoCoImageKind.DelegateInternal ? KnownImageIds.DelegateInternal :
                image == CoCoImageKind.DelegateProtected ? KnownImageIds.DelegateProtected :
                image == CoCoImageKind.DelegatePrivate ? KnownImageIds.DelegatePrivate :

                image == CoCoImageKind.EnumPublic ? KnownImageIds.EnumerationPublic :
                image == CoCoImageKind.EnumInternal ? KnownImageIds.EnumerationInternal :
                image == CoCoImageKind.EnumProtected ? KnownImageIds.EnumerationProtected :
                image == CoCoImageKind.EnumPrivate ? KnownImageIds.EnumerationPrivate :

                image == CoCoImageKind.EnumMemberPublic ? KnownImageIds.EnumerationItemPublic :
                image == CoCoImageKind.EnumMemberInternal ? KnownImageIds.EnumerationItemInternal :
                image == CoCoImageKind.EnumMemberProtected ? KnownImageIds.EnumerationItemProtected :
                image == CoCoImageKind.EnumMemberPrivate ? KnownImageIds.EnumerationItemPrivate :

                image == CoCoImageKind.EventPublic ? KnownImageIds.EventPublic :
                image == CoCoImageKind.EventInternal ? KnownImageIds.EventInternal :
                image == CoCoImageKind.EventProtected ? KnownImageIds.EventProtected :
                image == CoCoImageKind.EventPrivate ? KnownImageIds.EventPrivate :

                image == CoCoImageKind.ExtensionMethodPublic ? KnownImageIds.ExtensionMethod :
                image == CoCoImageKind.ExtensionMethodInternal ? KnownImageIds.ExtensionMethod :
                image == CoCoImageKind.ExtensionMethodProtected ? KnownImageIds.ExtensionMethod :
                image == CoCoImageKind.ExtensionMethodPrivate ? KnownImageIds.ExtensionMethod :

                image == CoCoImageKind.FieldPublic ? KnownImageIds.FieldPublic :
                image == CoCoImageKind.FieldInternal ? KnownImageIds.FieldInternal :
                image == CoCoImageKind.FieldProtected ? KnownImageIds.FieldProtected :
                image == CoCoImageKind.FieldPrivate ? KnownImageIds.FieldPrivate :

                image == CoCoImageKind.InterfacePublic ? KnownImageIds.InterfacePublic :
                image == CoCoImageKind.InterfaceInternal ? KnownImageIds.InterfaceInternal :
                image == CoCoImageKind.InterfaceProtected ? KnownImageIds.InterfaceProtected :
                image == CoCoImageKind.InterfacePrivate ? KnownImageIds.InterfacePrivate :

                image == CoCoImageKind.MethodPublic ? KnownImageIds.MethodPublic :
                image == CoCoImageKind.MethodInternal ? KnownImageIds.MethodInternal :
                image == CoCoImageKind.MethodProtected ? KnownImageIds.MethodProtected :
                image == CoCoImageKind.MethodPrivate ? KnownImageIds.MethodPrivate :

                image == CoCoImageKind.ModulePublic ? KnownImageIds.ModulePublic :
                image == CoCoImageKind.ModuleInternal ? KnownImageIds.ModuleInternal :
                image == CoCoImageKind.ModuleProtected ? KnownImageIds.ModuleProtected :
                image == CoCoImageKind.ModulePrivate ? KnownImageIds.ModulePrivate :

                image == CoCoImageKind.PropertyPublic ? KnownImageIds.PropertyPublic :
                image == CoCoImageKind.PropertyInternal ? KnownImageIds.PropertyPublic :
                image == CoCoImageKind.PropertyProtected ? KnownImageIds.PropertyProtected :
                image == CoCoImageKind.PropertyPrivate ? KnownImageIds.PropertyPrivate :

                image == CoCoImageKind.StructPublic ? KnownImageIds.ValueTypePublic :
                image == CoCoImageKind.StructInternal ? KnownImageIds.ValueTypeInternal :
                image == CoCoImageKind.StructProtected ? KnownImageIds.ValueTypeProtected :
                image == CoCoImageKind.StructPrivate ? KnownImageIds.ValueTypePrivate :

                image == CoCoImageKind.Label ? KnownImageIds.Label :
                image == CoCoImageKind.Local ? KnownImageIds.LocalVariable :
                image == CoCoImageKind.Namespace ? KnownImageIds.Namespace :
                image == CoCoImageKind.Parameter ? KnownImageIds.Parameter :
                image == CoCoImageKind.TypeParameter ? KnownImageIds.Type :
                image == CoCoImageKind.RangeVariable ? KnownImageIds.FieldPublic :
                image == CoCoImageKind.Error ? KnownImageIds.StatusError :

                image == CoCoImageKind.Keyword ? KnownImageIds.IntellisenseKeyword :
                -27;

            if (id == -27)
            {
                imageElement = default;
                return false;
            }

            imageElement = new CrispImage
            {
                Moniker = new ImageMoniker { Guid = KnownImageIds.ImageCatalogGuid, Id = id },
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 4, 4),
            };
            return true;
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
            if (_language is null) return;

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