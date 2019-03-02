using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

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

        public Span GetSpan() => new Span(Span.Start, Span.Length);
    }
}