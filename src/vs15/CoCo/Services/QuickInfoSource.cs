using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.QuickInfo;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

namespace CoCo.Services
{
    using MsQuickInfoItem = Microsoft.VisualStudio.Language.Intellisense.QuickInfoItem;

    internal sealed class QuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;

        public QuickInfoSource(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
        }

        public void Dispose()
        {
        }

        public async Task<MsQuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(_textBuffer.CurrentSnapshot);
            if (!triggerPoint.HasValue) return null;

            var quickInfo = await QuickInfoService.GetQuickInfo(_textBuffer, session, cancellationToken);
            if (quickInfo is null) return null;

            var trackingSpan = triggerPoint.Value.Snapshot.CreateTrackingSpan(quickInfo.GetSpan(), SpanTrackingMode.EdgeInclusive);

            var items = new List<object>();
            foreach (var item in quickInfo.Descriptions)
            {
                if (item.Kind == SymbolDescriptionKind.Main)
                {
                    items.Insert(0, new ContainerElement(ContainerElementStyle.Wrapped, ToClassifiedTextElement(item)));
                }
                else
                {
                    items.Add(ToClassifiedTextElement(item));
                }
            }

            return new MsQuickInfoItem(trackingSpan, new ContainerElement(ContainerElementStyle.Stacked, items));
        }

        private static ClassifiedTextElement ToClassifiedTextElement(SymbolDescription symbolDescription)
        {
            var list = new List<ClassifiedTextRun>();
            foreach (var item in symbolDescription.TaggedParts)
            {
                list.Add(new ClassifiedTextRun(item.Tag, item.Text));
            }
            return new ClassifiedTextElement(list);
        }
    }
}