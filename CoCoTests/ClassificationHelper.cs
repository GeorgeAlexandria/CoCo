using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CoCo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCoTests
{
    internal static class ClassificationHelper
    {
        public static SimplifiedClassificationSpan ClassifyAt(this string name, int start, int length)
        {
            if (!Names.All.Contains(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name), $"Argument must be one of {nameof(Names)} constants");
            }
            return new SimplifiedClassificationSpan(new Span(start, length), new ClassificationType(name));
        }

        public static List<SimplifiedClassificationSpan> GetClassifications(this string path, ProjectInfo project)
        {
            path = TestHelper.GetPathRelativeToTest(path);
            var code = File.ReadAllText(path);
            var buffer = new TextBuffer(new ContentType("csharp"), new StringOperand(code));

            var compilation = CreateCompilation(project);
            var syntaxTree = compilation.SyntaxTrees.FirstOrDefault(x => string.Equals(x.FilePath, path, StringComparison.OrdinalIgnoreCase));
            var semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            List<ClassificationSpan> actualSpans = null;
            using (var workspace = new AdhocWorkspace())
            {
                var snapshotSpan = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);

                var newProject = workspace.AddProject(project.ProjectName, LanguageNames.CSharp);
                var newDocument = workspace.AddDocument(newProject.Id, Path.GetFileName(path), snapshotSpan.Snapshot.AsText());

                var root = syntaxTree.GetCompilationUnitRoot();

                var classificationTypes = new Dictionary<string, IClassificationType>(32);
                foreach (var item in Names.All)
                {
                    classificationTypes.Add(item, new ClassificationType(item));
                }

                var classifier = new EditorClassifier(classificationTypes);
                actualSpans = classifier.GetClassificationSpans(workspace, semanticModel, root, snapshotSpan);
            }
            return actualSpans.Select(x => new SimplifiedClassificationSpan(x.Span.Span, x.ClassificationType)).ToList();
        }

        public static (bool, string) AreEquivalent(
            IEnumerable<SimplifiedClassificationSpan> expectedSpans,
            IEnumerable<SimplifiedClassificationSpan> actualSpans)
        {
            var actualList = actualSpans.ToList();
            var expectedList = expectedSpans.ToList();

            int i = 0;
            while (i < actualList.Count)
            {
                var hasEquals = false;
                for (int j = 0; j < expectedList.Count; ++j)
                {
                    if (AreClassificationSpanEquals(expectedList[j], actualList[i]))
                    {
                        expectedList.RemoveAt(j);
                        actualList.RemoveAt(i);
                        hasEquals = true;
                        break;
                    }
                }
                if (!hasEquals) ++i;
            }

            if ((actualList.Count | expectedList.Count) == 0)
            {
                return (true, String.Empty);
            }

            void AppendClassificationSpan(StringBuilder appendBuilder, SimplifiedClassificationSpan span)
            {
                const string tabs = "    ";
                var classificationType = span.ClassificationType;
                appendBuilder.AppendLine("Item:")
                    .Append(tabs).AppendLine("Type:")
                    .Append(tabs).Append(tabs).AppendFormat("Classification: {0}", classificationType.Classification).AppendLine()
                    .Append(tabs).Append(tabs).AppendFormat("Base types count: {0}", classificationType.BaseTypes.Count()).AppendLine()
                    .Append(tabs).AppendLine("Span: ").Append(span.Span).AppendLine();
            }

            var builder = new StringBuilder(1 << 12);
            if (expectedList.Count > 0) builder.AppendLine().AppendLine("This items were not found:");
            foreach (var item in expectedList)
            {
                AppendClassificationSpan(builder, item);
            }

            if (actualList.Count > 0) builder.AppendLine().AppendLine("This items were redundant:");
            foreach (var item in actualList)
            {
                AppendClassificationSpan(builder, item);
            }
            return (false, builder.ToString());
        }

        private static bool AreClassificationSpanEquals(SimplifiedClassificationSpan expected, SimplifiedClassificationSpan actual)
        {
            if (expected == null ^ actual == null) return false;
            if (expected == null) return true;
            return expected.Span == actual.Span && AreClassificationTypeEquals(expected.ClassificationType, actual.ClassificationType);
        }

        private static bool AreClassificationTypeEquals(IClassificationType expected, IClassificationType actual)
        {
            if (expected == null ^ actual == null) return false;
            if (expected == null) return true;
            if (!expected.Classification.Equals(actual.Classification, StringComparison.OrdinalIgnoreCase)) return false;
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

        private static Compilation CreateCompilation(ProjectInfo project)
        {
            var trees = new List<SyntaxTree>(project.CompileItems.Length);
            foreach (var item in project.CompileItems)
            {
                var code = File.ReadAllText(item);
                trees.Add(CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default, item));
            }
            // TODO: improve
            return CSharpCompilation.Create(project.ProjectName)
                .AddSyntaxTrees(trees)
                .AddReferences(project.References.Select(x => MetadataReference.CreateFromFile(x)))
                .AddReferences(project.ProjectReferences.Select(x => CreateCompilation(x).ToMetadataReference()));
        }
    }
}