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
        public TextSpan Span { get; }

        public ImmutableArray<SymbolDescription> Descriptions { get; }

        public ImageKind Image { get; }

        public QuickInfoItem(TextSpan span, ImageKind image, ImmutableArray<SymbolDescription> descriptions)
        {
            Span = span;
            Descriptions = descriptions.IsDefault ? ImmutableArray<SymbolDescription>.Empty : descriptions;
            Image = image;
        }

        public Span GetSpan() => new Span(Span.Start, Span.Length);
    }
}