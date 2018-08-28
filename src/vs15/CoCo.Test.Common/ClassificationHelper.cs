using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CoCo.Analyser;
using CoCo.Logging;
using CoCo.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Project = CoCo.MsBuild.ProjectInfo;

namespace CoCo.Test.Common
{
    public static class ClassificationHelper
    {
        private static readonly List<SimplifiedClassificationSpan> _empty = new List<SimplifiedClassificationSpan>();

        public static SimplifiedClassificationSpan ClassifyAt(this string name, int start, int length)
        {
            if (!CSharpNames.All.Contains(name) && !VisualBasicNames.All.Contains(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Argument must be one of constant names");
            }
            return new SimplifiedClassificationSpan(new Span(start, length), new ClassificationType(name));
        }

        public static List<SimplifiedClassificationSpan> GetClassifications(this string path, Project project)
        {
            using (var logger = LogManager.GetLogger("Test execution"))
            {
                path = TestHelper.GetPathRelativeToTest(path);
                if (!File.Exists(path))
                {
                    logger.Warn("File {0} doesn't exist.", path);
                    return _empty;
                }

                var compilationUnits = ExtractCompilationUnits(project);

                SemanticModel semanticModel = null;
                ProgrammingLanguage language = default;
                foreach (var unit in compilationUnits)
                {
                    var roslynCompilation = unit.Compilation;
                    var syntaxTree = roslynCompilation.SyntaxTrees.FirstOrDefault(x => x.FilePath.EqualsNoCase(path));
                    if (!(syntaxTree is null))
                    {
                        semanticModel = roslynCompilation.GetSemanticModel(syntaxTree, true);
                        language = unit.Language;
                        break;
                    }
                }

                if (semanticModel is null)
                {
                    logger.Warn("Project {0} doesn't have the file {1}. Check that it's included.", project.ProjectPath, path);
                    return _empty;
                }

                List<ClassificationSpan> actualSpans = null;
                // TODO: cache workspaces by project
                using (var workspace = new AdhocWorkspace())
                {
                    var buffer = new TextBuffer(GetContentType(language), new StringOperand(semanticModel.SyntaxTree.ToString()));
                    var snapshotSpan = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);

                    var newProject = workspace.AddProject(project.ProjectName, LanguageNames.CSharp);
                    var newDocument = workspace.AddDocument(newProject.Id, Path.GetFileName(path), snapshotSpan.Snapshot.AsText());

                    var classifier = GetClassifier(language);
                    actualSpans = classifier.GetClassificationSpans(workspace, semanticModel, snapshotSpan);
                }
                return actualSpans.Select(x => new SimplifiedClassificationSpan(x.Span.Span, x.ClassificationType)).ToList();
            }
        }

        public static (bool, string message) NotContains(
            IEnumerable<SimplifiedClassificationSpan> currentCollection,
            IEnumerable<SimplifiedClassificationSpan> otherCollection)
        {
            var currentSet = new HashSet<Span>();
            using (var logger = LogManager.GetLogger("Test execution"))
            {
                foreach (var item in currentCollection)
                {
                    if (!currentSet.Add(item.Span))
                    {
                        logger.Warn($"Input collection has the same item {item.Span}");
                    }
                }
            }
            var otherList = new List<SimplifiedClassificationSpan>(otherCollection);

            int i = 0;
            while (i < otherList.Count && currentSet.Count > 0)
            {
                if (!currentSet.Remove(otherList[i].Span))
                {
                    otherList.RemoveAt(i);
                    continue;
                }
                ++i;
            }

            if (otherList.Count == 0) return (true, String.Empty);

            var builder = new StringBuilder(1 << 12);
            builder.AppendLine("This items were exist:");
            foreach (var item in otherList)
            {
                builder.AppendClassificationSpan(item);
            }
            return (false, builder.ToString());
        }

        public static (bool, string message) Contains(
            IEnumerable<SimplifiedClassificationSpan> currentCollection,
            IEnumerable<SimplifiedClassificationSpan> otherCollection)
        {
            var currentSet = new HashSet<SimplifiedClassificationSpan>(currentCollection);
            var otherList = new List<SimplifiedClassificationSpan>(otherCollection);

            int i = 0;
            while (i < otherList.Count && currentSet.Count > 0)
            {
                if (currentSet.Remove(otherList[i]))
                {
                    otherList.RemoveAt(i);
                    continue;
                }
                ++i;
            }

            if (otherList.Count == 0) return (true, String.Empty);

            var builder = new StringBuilder(1 << 12);
            var actualSetBySpan = currentSet.ToDictionary(x => x.Span);
            i = 0;
            while (i < otherList.Count && actualSetBySpan.Count > 0)
            {
                var expectedClassification = otherList[i];
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
                otherList.RemoveAt(i++);
            }

            if (otherList.Count > 0)
            {
                foreach (var item in otherList)
                {
                    builder.AppendLine().AppendLine("Classification was not found:").AppendClassificationSpan(item);
                }
            }
            return (false, builder.ToString());
        }

        public static (bool, string) AreEquivalent(
            IEnumerable<SimplifiedClassificationSpan> leftSpans,
            IEnumerable<SimplifiedClassificationSpan> rightSpans)
        {
            var leftSet = new HashSet<SimplifiedClassificationSpan>(rightSpans);
            var rightList = leftSpans.ToList();

            int i = 0;
            while (i < rightList.Count && leftSet.Count > 0)
            {
                if (leftSet.Remove(rightList[i]))
                {
                    rightList.RemoveAt(i);
                    continue;
                }
                ++i;
            }

            if ((leftSet.Count | rightList.Count) == 0) return (true, String.Empty);

            var builder = new StringBuilder(1 << 12);
            if (rightList.Count > 0) builder.AppendLine().AppendLine("This items were not found:");
            foreach (var item in rightList)
            {
                AppendClassificationSpan(builder, item);
            }

            if (leftSet.Count > 0) builder.AppendLine().AppendLine("This items were redundant:");
            foreach (var item in leftSet)
            {
                AppendClassificationSpan(builder, item);
            }
            return (false, builder.ToString());
        }

        private const string _tabs = "    ";

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

        private static RoslynEditorClassifier GetClassifier(ProgrammingLanguage language)
        {
            var classificationTypes = new Dictionary<string, IClassificationType>(32);
            var names = language == ProgrammingLanguage.VisualBasic ? VisualBasicNames.All : CSharpNames.All;
            foreach (var item in names)
            {
                classificationTypes.Add(item, new ClassificationType(item));
            }

            return language == ProgrammingLanguage.VisualBasic
                ? new VisualBasicClassifier(classificationTypes)
                : (RoslynEditorClassifier)new CSharpClassifier(classificationTypes);
        }

        private static ContentType GetContentType(ProgrammingLanguage language) =>
            new ContentType(language == ProgrammingLanguage.VisualBasic ? "basic" : "csharp");

        private static CompilationUnit[] ExtractCompilationUnits(Project project)
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
                    if (Path.GetExtension(item).EqualsNoCase(".vb"))
                    {
                        visualBasicTrees.Add(VisualBasicSyntaxTree.ParseText(code, VisualBasicParseOptions.Default));
                    }
                    else
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
                        references.Add(unit.Compilation.ToMetadataReference());
                    }
                }

                return new CompilationUnit[2]
                {
                    CSharpCompilation.Create($"{project.ProjectName}_{LanguageNames.CSharp}")
                        .AddSyntaxTrees(csharpTrees)
                        .AddReferences(references),
                    // TODO: set Infer, Strict and other vb compilation options.
                    VisualBasicCompilation.Create($"{project.ProjectName}_{LanguageNames.VisualBasic}")
                        .AddSyntaxTrees(visualBasicTrees)
                        .AddReferences(references)
                };
            }
        }
    }
}