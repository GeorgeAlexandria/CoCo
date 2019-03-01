using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace CoCo.Analyser.QuickInfo
{
    /// <summary>
    /// Provides info about quick info content
    /// </summary>
    public sealed class QuickInfoItem
    {
        // TODO: understand what kind of info must be provided

        public TextSpan Span { get; }

        public ImmutableArray<SymbolDescription> Descriptions { get; }

        public QuickInfoItem(TextSpan span, ImmutableArray<SymbolDescription> descriptions)
        {
            Span = span;
            Descriptions = descriptions.IsDefault ? ImmutableArray<SymbolDescription>.Empty : descriptions;
        }
    }

    internal class QuickInfoService
    {
        private readonly string _language;

        // TODO: append implementations. Assumes that providers will be divide to semantic and syntax providers
        private readonly ImmutableArray<QuickInfoItemProvider> _csharpProviders = new ImmutableArray<QuickInfoItemProvider>
        {
            new CSharpSemanticProvider()
        };

        private readonly ImmutableArray<QuickInfoItemProvider> _visualBasicProviders = new ImmutableArray<QuickInfoItemProvider>();

        private ImmutableArray<QuickInfoItemProvider> Providers => _language.Equals(LanguageNames.CSharp)
            ? _csharpProviders
            : _visualBasicProviders;

        public QuickInfoService(string language)
        {
            _language = language;
        }

        public async Task<QuickInfoItem> GetQuickInfoAsync(Document document, int position, CancellationToken cancellationToken)
        {
            foreach (var provider in Providers)
            {
                var info = await provider.GetQuickInfoAsync(document, position, cancellationToken).ConfigureAwait(false);

                // NOTE: returns the first non null item
                if (!(info is null)) return info;
            }
            return default;
        }
    }

    // TODO: append implementation for c# and vb
    internal abstract class QuickInfoItemProvider
    {
        public virtual async Task<QuickInfoItem> GetQuickInfoAsync(Document document, int position, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var token = await syntaxTree.GetIntersectTokenAsync(position, true, cancellationToken);
            if (token != default && token.Span.IntersectsWith(position))
            {
                return await GetQuickInfoAsync(document, token, cancellationToken);
            }

            return default;
        }

        protected virtual bool CheckPreviousToken(SyntaxToken token) => true;

        protected abstract Task<QuickInfoItem> GetQuickInfoAsync(
            Document document, SyntaxToken token, CancellationToken cancellationToken);
    }
}