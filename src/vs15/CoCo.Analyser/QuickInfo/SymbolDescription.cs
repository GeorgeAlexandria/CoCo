using System.Collections.Immutable;
using System.Diagnostics;

namespace CoCo.Analyser.QuickInfo
{
    /// <summary>
    /// Contains description of the one of kind for quick info
    /// </summary>
    [DebuggerDisplay("{Kind}")]
    public sealed class SymbolDescription
    {
        public SymbolDescriptionKind Kind { get; }

        public ImmutableArray<TaggedText> TaggedParts { get; }

        public SymbolDescription(SymbolDescriptionKind kind, ImmutableArray<TaggedText> taggedParts)
        {
            Kind = kind;
            TaggedParts = taggedParts.IsDefault ? ImmutableArray<TaggedText>.Empty : taggedParts;
        }
    }
}