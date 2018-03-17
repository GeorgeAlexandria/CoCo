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

        public override bool Equals(object obj) => Equals(obj as SimplifiedClassificationSpan);

        public override int GetHashCode() => ClassificationComparer.Instance.GetHashCode(this);

        public bool Equals(SimplifiedClassificationSpan other) => ClassificationComparer.Instance.Equals(this, other);
    }
}