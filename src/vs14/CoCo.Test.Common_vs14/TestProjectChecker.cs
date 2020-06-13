using System;
using System.Collections.Generic;
using System.Linq;
using CoCo.Analyser.Classifications.FSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.FSharp.Compiler.SourceCodeServices;
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
            var checker = FSharpChecker.Create(null, null, null);
            var result = checker.ParseAndCheckFileInProject(itemPath, itemVersion.GetHashCode(),
                itemContent.ToString(), projectOptions, null, null);
            var (parseResult, checkAnswer) = FSharpAsync.RunSynchronously(result, null, null).ToValueTuple();

            if (checkAnswer.IsSucceeded && checkAnswer is FSharpCheckFileAnswer.Succeeded succeeded)
            {
                var checkResult = succeeded.Item;
                return new ParseCheckResult(parseResult, checkResult);
            }
            return default;
        }

        public FSharpProjectOptions GetOptions(MsBuild.ProjectInfo project)
        {
            var referencedProjectsOptions = new List<Tuple<string, FSharpProjectOptions>>();
            foreach (var referencedProject in project.ProjectReferences)
            {
                if (string.Equals(project.Language, "F#"))
                {
                    var projectOptions = GetOptions(referencedProject);
                    referencedProjectsOptions.Add((referencedProject.OutputFilePath, projectOptions).ToTuple());
                }
            }

            var options = new List<string>();
            foreach (var item in new FscOptionsBuilder(project.ProjectPath).Build())
            {
                if (!item.StartsWith("-r:"))
                {
                    options.Add(item);
                }
            }
            foreach (var item in project.ProjectReferences)
            {
                options.Add("-r:" + item.OutputFilePath);
            }
            foreach (var item in project.AssemblyReferences)
            {
                options.Add("-r:" + item);
            }

            return new FSharpProjectOptions(
                project.ProjectPath,
                project.CompileItems.ToArray(),
                options.ToArray(),
                referencedProjectsOptions.ToArray(),
                false,
                false/*SourceFile.MustBeSingleFileProject(Path.GetFileName(project.ProjectPath))*/,
                DateTime.Now,
                null);
        }
    }
}