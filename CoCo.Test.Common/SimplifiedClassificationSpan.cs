using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Common
{
    public class SimplifiedClassificationSpan : IEquatable<SimplifiedClassificationSpan>
    {
        public IClassificationType ClassificationType { get; }

        public Span Span { get; }

        public SimplifiedClassificationSpan(Span span, IClassificationType classificationType)
        {
            ClassificationType = classificationType;
            Span = span;
        }

        public override bool Equals(object obj) => ClassificationComparer.Instance.Equals(this, (SimplifiedClassificationSpan)obj);

        public override int GetHashCode() => ClassificationComparer.Instance.GetHashCode(this);

        public bool Equals(SimplifiedClassificationSpan other) => ClassificationComparer.Instance.Equals(this, other);

        public static bool operator ==(SimplifiedClassificationSpan x, SimplifiedClassificationSpan y) =>
            ClassificationComparer.Instance.Equals(x, y);

        public static bool operator !=(SimplifiedClassificationSpan x, SimplifiedClassificationSpan y) =>
            !ClassificationComparer.Instance.Equals(x, y);
    }
}