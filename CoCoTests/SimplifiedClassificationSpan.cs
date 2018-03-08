using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCoTests
{
    internal class SimplifiedClassificationSpan
    {
        public IClassificationType ClassificationType { get; }

        public Span Span { get; }

        public SimplifiedClassificationSpan(Span span, IClassificationType classificationType)
        {
            ClassificationType = classificationType;
            Span = span;
        }
    }
}