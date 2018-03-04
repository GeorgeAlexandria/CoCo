using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Text.Classification;
using NUnit.Framework;

namespace CoCoTests
{
    public static class TestHelper
    {
        public static void AssertIsEquivalent(this IEnumerable<ClassificationSpan> actualSpans, params ClassificationSpan[] expectedSpans)
        {
            var (isEquivalent, errorMessage) = ClassificationHelper.IsEquivalent(expectedSpans, actualSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }

        public static string GetPathRelativeToTest(string projectPath) => GetPathRelativeToThis(projectPath);

        private static string GetPathRelativeToThis(string path, [CallerFilePath] string sourceCallerPath = null)
        {
            // NOTE: ../CoCoTests/TestHelper.cs
            var sourceDirectory = sourceCallerPath.GetDirectoryName().GetDirectoryName();
            return Path.Combine(sourceDirectory, path);
        }
    }
}