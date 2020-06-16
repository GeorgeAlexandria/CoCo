using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Identifiers.Common
{
    public sealed class ClassificationComparer : IEqualityComparer<SimplifiedClassificationSpan>
    {
        private ClassificationComparer()
        {
        }

        public static readonly ClassificationComparer Instance = new ClassificationComparer();

        public bool Equals(SimplifiedClassificationSpan x, SimplifiedClassificationSpan y) =>
            x is null ^ y is null ? false :
            x is null || x.Span == y.Span && AreClassificationTypeEquals(x.ClassificationType, y.ClassificationType);

        public int GetHashCode(SimplifiedClassificationSpan obj) => obj is null
            ? throw new ArgumentNullException(nameof(obj))
            : obj.ClassificationType.Classification.GetHashCode() ^ obj.Span.GetHashCode();

        private static bool AreClassificationTypeEquals(IClassificationType expected, IClassificationType actual)
        {
            if (expected is null ^ actual is null) return false;
            if (expected is null) return true;
            if (!expected.Classification.Equals(actual.Classification, StringComparison.OrdinalIgnoreCase)) return false;

            var expectedBaseTypes = expected.BaseTypes.ToList();
            var actualBaseTypes = actual.BaseTypes.ToList();

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