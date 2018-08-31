using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoCo.Logging;
using CoCo.Utils;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NUnit.Framework;

namespace CoCo.Test.Common
{
    public static class AssertionHelper
    {
        private const string _tabs = "    ";

        // TODO: think how to convert it to a some of fluent api (test.Contains(...).NotContains(...))
        public static void AssertIsEquivalent(
            this IEnumerable<SimplifiedClassificationSpan> actualSpans, params SimplifiedClassificationSpan[] expectedSpans)
        {
            var actualSet = actualSpans.ToHashSet();
            var expectedList = expectedSpans.ToList();

            int i = 0;
            while (i < expectedList.Count && actualSet.Count > 0)
            {
                if (actualSet.Remove(expectedList[i]))
                {
                    expectedList.RemoveAt(i);
                    continue;
                }
                ++i;
            }

            if ((actualSet.Count | expectedList.Count) == 0) return;

            var builder = new StringBuilder(1 << 12);
            if (expectedList.Count > 0) builder.AppendLine().AppendLine("This items were not found:");
            foreach (var item in expectedList)
            {
                AppendClassificationSpan(builder, item);
            }

            if (actualSet.Count > 0) builder.AppendLine().AppendLine("This items were redundant:");
            foreach (var item in actualSet)
            {
                AppendClassificationSpan(builder, item);
            }
            Assert.Fail(builder.ToString());
        }

        public static void AssertContains(
            this IEnumerable<SimplifiedClassificationSpan> actualSpans, params SimplifiedClassificationSpan[] expectedSpans)
        {
            var actualSet = actualSpans.ToHashSet();
            var expectedList = expectedSpans.ToList();

            int i = 0;
            while (i < expectedList.Count && actualSet.Count > 0)
            {
                if (actualSet.Remove(expectedList[i]))
                {
                    expectedList.RemoveAt(i);
                    continue;
                }
                ++i;
            }

            if (expectedList.Count == 0) return;

            var builder = new StringBuilder(1 << 12);
            var actualSetBySpan = actualSet.ToDictionary(x => x.Span);
            i = 0;
            while (i < expectedList.Count && actualSetBySpan.Count > 0)
            {
                var expectedClassification = expectedList[i];
                if (actualSetBySpan.TryRemoveValue(expectedClassification.Span, out var value))
                {
                    builder
                        .AppendLine().AppendLine($"Classification at {expectedClassification.Span} has incorrect type:")
                        .AppendLine("Expected:").AppenClassificationType(expectedClassification.ClassificationType)
                        .AppendLine("But was:").AppenClassificationType(value.ClassificationType);
                }
                else
                {
                    builder.AppendLine().AppendLine("Classification was not found:").AppendClassificationSpan(expectedClassification);
                }
                expectedList.RemoveAt(i++);
            }

            if (expectedList.Count > 0)
            {
                foreach (var item in expectedList)
                {
                    builder.AppendLine().AppendLine("Classification was not found:").AppendClassificationSpan(item);
                }
            }
            Assert.Fail(builder.ToString());
        }

        public static void AssertNotContains(
            this IEnumerable<SimplifiedClassificationSpan> actualSpans, params SimplifiedClassificationSpan[] expectedSpans)
        {
            var actualtSet = new HashSet<Span>();
            using (var logger = LogManager.GetLogger("Test execution"))
            {
                foreach (var item in actualSpans)
                {
                    if (!actualtSet.Add(item.Span))
                    {
                        logger.Warn($"Input collection has the same item {item.Span}");
                    }
                }
            }
            var expectedList = expectedSpans.ToList();

            int i = 0;
            while (i < expectedList.Count && actualtSet.Count > 0)
            {
                if (!actualtSet.Remove(expectedList[i].Span))
                {
                    expectedList.RemoveAt(i);
                    continue;
                }
                ++i;
            }

            if (expectedList.Count == 0) return;

            var builder = new StringBuilder(1 << 12);
            builder.AppendLine("This items were exist:");
            foreach (var item in expectedList)
            {
                builder.AppendClassificationSpan(item);
            }
            Assert.Fail(builder.ToString());
        }

        private static void AppendClassificationSpan(this StringBuilder builder, SimplifiedClassificationSpan span) =>
           builder
           .AppendLine("Item:")
           .AppendSpan(span.Span)
           .AppenClassificationType(span.ClassificationType);

        private static StringBuilder AppenClassificationType(this StringBuilder builder, IClassificationType classificationType) =>
            builder
                .Append(_tabs).AppendLine("Type:")
                .Append(_tabs).Append(_tabs).AppendFormat("Classification: {0}", classificationType.Classification).AppendLine()
                .Append(_tabs).Append(_tabs).AppendFormat("Base types count: {0}", classificationType.BaseTypes.Count()).AppendLine();

        private static StringBuilder AppendSpan(this StringBuilder builder, Span span) =>
            builder.Append(_tabs).Append("Span: ").Append(span).AppendLine();
    }
}