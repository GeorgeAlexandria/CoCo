using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Common
{
    public class ClassificationComparer : IEqualityComparer<SimplifiedClassificationSpan>
    {
        private ClassificationComparer()
        {
        }

        public static readonly ClassificationComparer Instance = new ClassificationComparer();

        public bool Equals(SimplifiedClassificationSpan x, SimplifiedClassificationSpan y)
        {
            if (x == null ^ y == null) return false;
            if (x == null) return true;

            return x.Span == y.Span && AreClassificationTypeEquals(x.ClassificationType, y.ClassificationType);
        }

        public int GetHashCode(SimplifiedClassificationSpan obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.ClassificationType.Classification.GetHashCode() ^ obj.Span.GetHashCode();
        }

        private static bool AreClassificationTypeEquals(IClassificationType expected, IClassificationType actual)
        {
            if (expected == null ^ actual == null) return false;
            if (expected == null) return true;
            if (!expected.Classification.Equals(actual.Classification, StringComparison.OrdinalIgnoreCase)) return false;

            var expectedBaseTypes = new List<IClassificationType>(expected.BaseTypes);
            var actualBaseTypes = new List<IClassificationType>(actual.BaseTypes);

            int i = 0;
            while (i < expectedBaseTypes.Count && actualBaseTypes.Count > 0)
            {
                var hasEqualItem = false;
                for (int j = 0; j < actualBaseTypes.Count; ++j)
                {
                    if (AreClassificationTypeEquals(expectedBaseTypes[i], actualBaseTypes[j]))
                    {
                        actualBaseTypes.RemoveAt(j);
                        expectedBaseTypes.RemoveAt(i);
                        hasEqualItem = true;
                        break;
                    }
                }
                if (!hasEqualItem) return false;
            }

            return (expectedBaseTypes.Count | actualBaseTypes.Count) == 0;
        }
    }
}