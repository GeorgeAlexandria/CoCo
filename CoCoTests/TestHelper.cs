using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace CoCoTests
{
    internal static class TestHelper
    {
        internal static void AssertIsEquivalent(this IEnumerable<SimplifiedClassificationSpan> actualSpans, params SimplifiedClassificationSpan[] expectedSpans)
        {
            var (isEquivalent, errorMessage) = ClassificationHelper.AreEquivalent(expectedSpans, actualSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }

        internal static string GetPathRelativeToTest(string projectPath) => GetPathRelativeToThis(projectPath);

        private static string GetPathRelativeToThis(string path, [CallerFilePath] string sourceCallerPath = null)
        {
            // NOTE: ../CoCoTests/TestHelper.cs
            var sourceDirectory = sourceCallerPath.GetDirectoryName().GetDirectoryName();
            return Path.Combine(sourceDirectory, path);
        }
    }
}