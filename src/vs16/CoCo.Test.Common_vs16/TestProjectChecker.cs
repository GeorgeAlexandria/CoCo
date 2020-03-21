using System;
using CoCo.Analyser.Classifications.FSharp;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.FSharp.Control;

namespace CoCo.Test.Common
{
    /// <summary>
    /// Run checking synchronously for testing
    /// </summary>
    internal class TestProjectChecker : IProjectChecker
    {
        private TestProjectChecker()
        {
        }

        public static TestProjectChecker Instance { get; } = new TestProjectChecker();

        public ParseCheckResult ParseAndCheckFileInProject(IListener listener, FSharpProjectOptions projectOptions, string itemPath,
            SourceText itemContent, VersionStamp itemVersion)
        {
            // TODO: would be better to use a custom ReferenceResolver implementaion?
            var checker = FSharpChecker.Create(null, null, null, null, null, null);
            var result = checker.ParseAndCheckFileInProject(itemPath, itemVersion.GetHashCode(),
                new SourceTextWrapper(itemContent), projectOptions, null, "CoCo_Classifications");
            var (parseResult, checkAnswer) = FSharpAsync.RunSynchronously(result, null, null).ToValueTuple();

            if (checkAnswer.IsSucceeded && checkAnswer is FSharpCheckFileAnswer.Succeeded succeeded)
            {
                var checkResult = succeeded.Item;
                return new ParseCheckResult(parseResult, checkResult);
            }
            return default;
        }
    }
}