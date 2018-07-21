using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using CoCo.Utils;
using NUnit.Framework;

namespace CoCo.Test.Common
{
    public static class TestHelper
    {
        // TODO: think how to convert it to a some of fluent api (test.Contains(...).NotContains(...))
        public static void AssertIsEquivalent(this IEnumerable<SimplifiedClassificationSpan> actualSpans, params SimplifiedClassificationSpan[] expectedSpans)
        {
            var (isEquivalent, errorMessage) = ClassificationHelper.AreEquivalent(expectedSpans, actualSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }

        public static void AssertContains(this IEnumerable<SimplifiedClassificationSpan> actualSpans, params SimplifiedClassificationSpan[] expectedSpans)
        {
            var (isEquivalent, errorMessage) = ClassificationHelper.Contains(actualSpans, expectedSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }

        public static void AssertNotContains(this IEnumerable<SimplifiedClassificationSpan> actualSpans, params SimplifiedClassificationSpan[] expectedSpans)
        {
            var (isEquivalent, errorMessage) = ClassificationHelper.NotContains(actualSpans, expectedSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }

        internal static string GetPathRelativeToTest(string projectPath) => GetPathRelativeToThis(projectPath);

        private static string GetPathRelativeToThis(string path, [CallerFilePath] string sourceCallerPath = null)
        {
            // NOTE: ../../../CoCo.Test.Common/TestHelper.cs
            var sourceDirectory = sourceCallerPath.GetDirectoryName().GetDirectoryName().GetDirectoryName().GetDirectoryName();
            return Path.Combine(sourceDirectory, path);
        }
    }
}