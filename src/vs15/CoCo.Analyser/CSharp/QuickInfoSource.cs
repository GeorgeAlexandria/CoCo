using System;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}