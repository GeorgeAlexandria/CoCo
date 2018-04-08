using System;
using System.Collections.Generic;
using System.Linq;
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
            // TODO: improve
            if (expected.BaseTypes.Count() != actual.BaseTypes.Count()) return false;

            foreach (var expectedBaseType in expected.BaseTypes)
            {
                var hasEqualsItem = false;
                foreach (var actualBaseType in actual.BaseTypes)
                {
                    if (AreClassificationTypeEquals(expectedBaseType, actualBaseType))
                    {
                        hasEqualsItem = true;
                        break;
                    }
                }
                if (!hasEqualsItem) return false;
            }
            return true;
        }
    }
}