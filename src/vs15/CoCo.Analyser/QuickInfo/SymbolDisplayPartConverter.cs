using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;

namespace CoCo.Analyser.QuickInfo
{
    internal class SymbolDisplayPartConverter
    {
        private readonly ICodeClassifier _classifier;

        public SymbolDisplayPartConverter(ICodeClassifier classifier = null)
        {
            _classifier = classifier;
        }

        public bool IsDefault => _classifier is null;

        public ImmutableArray<TaggedText> ToTags<T>(T displayParts) where T : IEnumerable<SymbolDisplayPart>
        {
            var builder = ImmutableArray.CreateBuilder<TaggedText>();
            foreach (var item in displayParts)
            {
                var tag = ToTag(item);
                if (!tag.IsDefault)
                {
                    builder.Add(tag);
                }
            }
            return builder.ToImmutable();
        }

        public TaggedText ToTag(SymbolDisplayPart displayPart)
        {
            TaggedText ToTag()
            {
                if (displayPart.Symbol is null) return default;

                var classificationType = _classifier?.GetClassification(displayPart.Symbol);
                if (classificationType?.Classification is null) return default;

                return new TaggedText(classificationType.Classification, displayPart.ToString());
            }

            var classification =
                displayPart.Kind == SymbolDisplayPartKind.Keyword ? ClassificationTypeNames.Keyword :
                displayPart.Kind == SymbolDisplayPartKind.LineBreak ? ClassificationTypeNames.WhiteSpace :
                displayPart.Kind == SymbolDisplayPartKind.Operator ? ClassificationTypeNames.Operator :
                displayPart.Kind == SymbolDisplayPartKind.Punctuation ? ClassificationTypeNames.Punctuation :
                displayPart.Kind == SymbolDisplayPartKind.Space ? ClassificationTypeNames.WhiteSpace :
                displayPart.Kind == SymbolDisplayPartKind.Text ? ClassificationTypeNames.Text :
                displayPart.Kind == SymbolDisplayPartKind.StringLiteral ? ClassificationTypeNames.StringLiteral :
                displayPart.Kind == SymbolDisplayPartKind.NumericLiteral ? ClassificationTypeNames.NumericLiteral :
                null;

            var tag = displayPart.Symbol is null || !(classification is null)
                ? new TaggedText(classification, displayPart.ToString())
                : ToTag();

            if (!tag.IsDefault) return tag;

            // NOTE: use fallback classifications if classifier returned nothing
            if (SymbolDisplayPartHelper.TryGetClassificationName(displayPart, out classification))
            {
                return new TaggedText(classification, displayPart.ToString());
            }
            return default;
        }
    }
}