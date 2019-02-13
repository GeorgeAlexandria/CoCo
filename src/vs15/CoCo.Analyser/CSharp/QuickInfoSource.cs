using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.QuickInfo;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser.CSharp
{
    internal sealed class QuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer textBuffer;

        public QuickInfoSource(ITextBuffer textBuffer)
        {
            this.textBuffer = textBuffer;
        }

        public void Dispose()
        {
        }

        public async Task<Microsoft.VisualStudio.Language.Intellisense.QuickInfoItem> GetQuickInfoItemAsync(
            IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot);
            if (!triggerPoint.HasValue) return null;

            var document = triggerPoint.Value.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document is null) return null;

            cancellationToken.ThrowIfCancellationRequested();

            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var quickInfoService = new QuickInfoService(syntaxRoot.Language);

            var item = await quickInfoService.GetQuickInfoItemAsync(document, triggerPoint.Value, cancellationToken);
            if (item is null) return null;

            // TODO: map custom QII to MQII
            return null;
        }
    }
}