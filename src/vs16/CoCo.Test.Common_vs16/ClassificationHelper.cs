using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoCo.Analyser.Classifications;
using CoCo.Analyser.Classifications.CSharp;
using CoCo.Analyser.Classifications.FSharp;
using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Analyser.Editor;
using CoCo.Logging;
using CoCo.Utils;
using FSharp.Compiler;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Project = CoCo.MsBuild.ProjectInfo;

namespace CoCo.Test.Common
{

    public static class ClassificationHelper
    {
        private static readonly List<SimplifiedClassificationSpan> _empty = new List<SimplifiedClassificationSpan>();

        public static SimplifiedClassificationSpan ClassifyAt(this string name, int start, int length) => IsUnknownClassification(name)
            ? throw new ArgumentOutOfRangeException(nameof(name), "Argument must be one of constant names")
            : new SimplifiedClassificationSpan(new Span(start, length), new ClassificationType(name));

        public static SimplifiedClassificationInfo EnableInEditor(this string name)
        {
            if (IsUnknownClassification(name)) throw new ArgumentOutOfRangeException(nameof(name), "Argument must be one of constant names");
            SimplifiedClassificationInfo info = name;
            return info.EnableInEditor();
        }

        public static SimplifiedClassificationInfo DisableInEditor(this string name)
        {
            if (IsUnknownClassification(name)) throw new ArgumentOutOfRangeException(nameof(name), "Argument must be one of constant names");
            SimplifiedClassificationInfo info = name;
            return info.DisableInEditor();
        }

        public static SimplifiedClassificationInfo DisableInXml(this string name)
        {
            if (IsUnknownClassification(name)) throw new ArgumentOutOfRangeException(nameof(name), "Argument must be one of constant names");
            SimplifiedClassificationInfo info = name;
            return info.DisableInXml();
        }

        public static List<SimplifiedClassificationSpan> GetClassifications(
            string path, Project project, IReadOnlyList<SimplifiedClassificationInfo> infos = null)
        {
            using (var logger = LogManager.GetLogger("Test execution"))
            {
                path = Path.Combine(project.ProjectPath.GetDirectoryName(), path);
                if (!File.Exists(path))
                {
                    logger.Warn("File {0} doesn't exist.", path);
                    return _empty;
                }

                string code = default;
                CompilationUnit compilation = default;
                foreach (var unit in ExtractCompilationUnits(project))
                {
                    if (unit.TryGetSourceCode(path, out code))
                    {
                        compilation = unit;
                        break;
                    }
                }

                if (code is null)
                {
                    logger.Warn("Project {0} doesn't have the file {1}. Check that it's included.", project.ProjectPath, path);
                    return _empty;
                }

                List<ClassificationSpan> actualSpans = null;
                // TODO: cache workspaces by project
                using (var workspace = new AdhocWorkspace())
                {
                    var buffer = new TextBuffer(GetContentType(compilation.Language), new StringOperand(code));
                    var snapshotSpan = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);
                    // TODO: append comment about workspace
                    var newProject = workspace.AddProject(project.ProjectName, LanguageNames.CSharp);
                    var newDocument = workspace.AddDocument(newProject.Id, Path.GetFileName(path), snapshotSpan.Snapshot.AsText());

                    var classifier = GetClassifier(compilation, infos);
                    actualSpans = classifier.GetClassificationSpans(workspace, path, code, snapshotSpan);
                }
                return actualSpans.Select(x => new SimplifiedClassificationSpan(x.Span.Span, x.ClassificationType)).ToList();
            }
        }

        private static ITestFileClassifier GetClassifier(
            CompilationUnit compilation, IReadOnlyList<SimplifiedClassificationInfo> infos)
        {
            var language = compilation.Language;
            var dictionary = infos?.ToDictionary(x => x.Name);
            var classificationTypes = new Dictionary<string, ClassificationInfo>(32);
            var names =
                language == ProgrammingLanguage.VisualBasic ? VisualBasicNames.All :
                language == ProgrammingLanguage.FSharp ? FSharpNames.All :
                CSharpNames.All;
            foreach (var name in names)
            {
                var option = dictionary is null || !dictionary.TryGetValue(name, out var simplifiedInfo)
                    ? ClassificationService.GetDefaultOption(name)
                    : simplifiedInfo;
                classificationTypes.Add(name, new ClassificationInfo(new ClassificationType(name), option));
            }

            if (language == ProgrammingLanguage.FSharp)
            {
                return new FSharpTestFileClassifier(compilation as FSharpCompilationUnit,
                    new FSharpTextBufferClassifier(classificationTypes, null));
            }

            if (language == ProgrammingLanguage.VisualBasic)
            {
                VisualBasicClassifierService.Reset();
                return new RoslynTestFileClassifier(
                    compilation as RoslynCompilationUnit, new VisualBasicTextBufferClassifier(classificationTypes));
            }

            CSharpClassifierService.Reset();
            return new RoslynTestFileClassifier(
                compilation as RoslynCompilationUnit, new CSharpTextBufferClassifier(classificationTypes));
        }

        private static ContentType GetContentType(ProgrammingLanguage language) => new ContentType(
            language == ProgrammingLanguage.VisualBasic ? "basic" :
            language == ProgrammingLanguage.VisualBasic ? "f#" :
            "csharp");

        private static CompilationUnit[] ExtractCompilationUnits(Project project)
        {
            var roslynCompilations = ExtractRoslynCompilationUnits(project);
            if (project.Language.EqualsNoCase("f#"))
            {
                FSharpCompilationUnit compilation = GetOptions(project);

                var compilations = new CompilationUnit[roslynCompilations.Length + 1];
                Array.Copy(roslynCompilations, 0, compilations, 0, roslynCompilations.Length);
                compilations[roslynCompilations.Length] = compilation;
                return compilations;
            }
            return roslynCompilations;
        }

        private static RoslynCompilationUnit[] ExtractRoslynCompilationUnits(Project project)
        {
            using (var logger = LogManager.GetLogger("Test execution"))
            {
                var csharpTrees = new List<SyntaxTree>(project.CompileItems.Length);
                var visualBasicTrees = new List<SyntaxTree>(project.CompileItems.Length);
                foreach (var item in project.CompileItems)
                {
                    if (!File.Exists(item))
                    {
                        logger.Error($"File {item} doesn't exist");
                        continue;
                    }

                    var code = File.ReadAllText(item);
                    var extension = Path.GetExtension(item);
                    if (extension.EqualsNoCase(".vb"))
                    {
                        visualBasicTrees.Add(VisualBasicSyntaxTree.ParseText(code, VisualBasicParseOptions.Default, item));
                    }
                    else if (!extension.EqualsNoCase(".fs"))
                    {
                        // NOTE: currently is assumed that all this files is C#
                        // TODO: fix it in the future
                        csharpTrees.Add(CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default, item));
                    }
                }

                var references = new List<MetadataReference>(project.AssemblyReferences.Length + project.ProjectReferences.Length);
                foreach (var item in project.AssemblyReferences)
                {
                    references.Add(MetadataReference.CreateFromFile(item));
                }
                foreach (var item in project.ProjectReferences)
                {
                    foreach (var unit in ExtractCompilationUnits(item))
                    {
                        // TODO: f# project must be compiled to the assembly/executable to link it to c#/vb project by rolsyn
                        // so currently it doesn't support
                        if (unit is RoslynCompilationUnit roslynUnit)
                        {
                            references.Add(roslynUnit.Compilation.ToMetadataReference());
                        }
                    }
                }

                var visualBasicOptions = new VisualBasicCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    rootNamespace: project.RootNamespace,
                    globalImports: project.Imports.Select(GlobalImport.Parse),
                    optionCompareText: project.OptionCompare,
                    optionExplicit: project.OptionExplicit,
                    optionInfer: project.OptionInfer,
                    optionStrict: project.OptionStrict ? OptionStrict.On : OptionStrict.Off);
                return new RoslynCompilationUnit[]
                {
                    CSharpCompilation.Create($"{project.ProjectName}_{LanguageNames.CSharp}")
                        .AddSyntaxTrees(csharpTrees)
                        .AddReferences(references),

                    VisualBasicCompilation.Create($"{project.ProjectName}_{LanguageNames.VisualBasic}", options: visualBasicOptions)
                        .AddSyntaxTrees(visualBasicTrees)
                        .AddReferences(references)
                };
            }
        }

        private static FSharpProjectOptions GetOptions(Project project)
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
                Guid.NewGuid().ToString("D").ToLowerInvariant(),
                project.CompileItems.ToArray(),
                options.ToArray(),
                referencedProjectsOptions.ToArray(),
                false,
                SourceFile.MustBeSingleFileProject(Path.GetFileName(project.ProjectPath)),
                DateTime.Now,
                null,
                FSharpList<Tuple<Range.range, string>>.Empty,
                null,
                FSharpOption<long>.Some(VersionStamp.Default.GetHashCode()));
        }

        private static bool IsUnknownClassification(string name) =>
            !CSharpNames.All.Contains(name) && !VisualBasicNames.All.Contains(name) && !FSharpNames.All.Contains(name);
    }
}