using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser.QuickInfo
{
    // TODO: append implementation for c# and vb
    internal abstract class QuickInfoItemProvider
    {
        public virtual async Task<QuickInfoItem> GetQuickInfoAsync(
            ITextBuffer textBuffer, Document document, int position, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var token = await syntaxTree.GetIntersectTokenAsync(position, true, cancellationToken);
            if (token != default && token.Span.IntersectsWith(position))
            {
                return await GetQuickInfoAsync(textBuffer, document, token, cancellationToken);
            }

            return default;
        }

        protected abstract Task<QuickInfoItem> GetQuickInfoAsync(
            ITextBuffer buffer, Document document, SyntaxToken token, CancellationToken cancellationToken);
    }
}