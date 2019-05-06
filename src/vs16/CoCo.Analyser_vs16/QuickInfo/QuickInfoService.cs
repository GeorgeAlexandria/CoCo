using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using CoCo.Analyser.QuickInfo.CSharp;
using CoCo.Analyser.QuickInfo.VisualBasic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace CoCo.Analyser.QuickInfo
{
    internal sealed class QuickInfoService
    {
        private readonly string _language;

        // TODO: append implementations. Assumes that providers will be divide to semantic and syntax providers
        private readonly ImmutableArray<QuickInfoItemProvider> _csharpProviders;

        private readonly ImmutableArray<QuickInfoItemProvider> _visualBasicProviders;

        private ImmutableArray<QuickInfoItemProvider> Providers => _language.Equals(LanguageNames.CSharp)
            ? _csharpProviders
            : _visualBasicProviders;

        private QuickInfoService(string language)
        {
            _csharpProviders = ImmutableArray.Create<QuickInfoItemProvider>(new CSharpSemanticProvider());
            _visualBasicProviders = ImmutableArray.Create<QuickInfoItemProvider>(new VisualBasicSemanticProvider());

            _language = language;
        }

        public static async Task<QuickInfoItem> GetQuickInfoAsync(
            ITextBuffer textBuffer, SnapshotPoint triggerPoint, CancellationToken cancellationToken)
        {
            var document = triggerPoint.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
            if (document is null) return null;

            cancellationToken.ThrowIfCancellationRequested();

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var quickInfoService = new QuickInfoService(root.Language);

            return await quickInfoService.GetQuickInfoAsync(textBuffer, document, triggerPoint, cancellationToken);
        }

        private async Task<QuickInfoItem> GetQuickInfoAsync(
            ITextBuffer textBuffer, Document document, int position, CancellationToken cancellationToken)
        {
            foreach (var provider in Providers)
            {
                var info = await provider.GetQuickInfoAsync(textBuffer, document, position, cancellationToken).ConfigureAwait(false);

                // NOTE: returns the first non null item
                if (!(info is null)) return info;
            }
            return default;
        }
    }
}