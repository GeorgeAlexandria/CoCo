using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace CoCo.Analyser.QuickInfo
{
    /// <summary>
    /// Provides info about quick info content
    /// </summary>
    public sealed class QuickInfoItem
    {
        // TODO: understand what kind of info must be provided
    }

    internal class QuickInfoService
    {
        private readonly string _language;

        // TODO: append implementations. Assumes that providers will be divide to semantic and syntax providers
        private readonly ImmutableArray<QuickInfoItemProvider> _csharpProviders = new ImmutableArray<QuickInfoItemProvider>();

        private readonly ImmutableArray<QuickInfoItemProvider> _visualBasicProviders = new ImmutableArray<QuickInfoItemProvider>();

        private ImmutableArray<QuickInfoItemProvider> Providers => _language.Equals(LanguageNames.CSharp)
            ? _csharpProviders
            : _visualBasicProviders;

        public QuickInfoService(string language)
        {
            _language = language;
        }

        public async Task<QuickInfoItem> GetQuickInfoItemAsync(Document document, int position, CancellationToken cancellationToken)
        {
            foreach (var provider in Providers)
            {
                var info = await provider.GetQuickInfoItemAsync(document, position, cancellationToken).ConfigureAwait(false);

                // NOTE: returns the first non null item
                if (!(info is null)) return info;
            }
            return default;
        }
    }

    // TODO: append implementation for c# and vb
    internal abstract class QuickInfoItemProvider
    {
        public virtual async Task<QuickInfoItem> GetQuickInfoItemAsync(Document document, int position, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken);
            var token = await GetIntersectTokenAsync(syntaxTree, position, cancellationToken, true);
            if (token != default && token.Span.IntersectsWith(position))
            {
                return await GetQuickInfoItemAsync(document, token, cancellationToken);
            }

            return default;
        }

        protected virtual bool CheckPreviousToken(SyntaxToken token) => true;

        protected abstract Task<QuickInfoItem> GetQuickInfoItemAsync(
            Document document, SyntaxToken token, CancellationToken cancellationToken);

        private static async Task<SyntaxToken> GetIntersectTokenAsync(
           SyntaxTree syntaxTree, int position, CancellationToken cancellationToken, bool findInsideTrivia)
        {
            if (position >= syntaxTree.Length) return default;

            var root = await syntaxTree.GetRootAsync(cancellationToken);
            var token = root.FindToken(position, findInsideTrivia);
            if (token.Span.Contains(position) || token.Span.End == position) return token;

            // NOTE: if position is the end of previous token => return it.
            token = token.GetPreviousToken();
            return token.Span.End == position ? token : default;
        }
    }
}