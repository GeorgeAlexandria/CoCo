using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Test.Common
{
    public static class ClassificationHelper
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

        public static (bool, string message) Contains(
            this IEnumerable<SimplifiedClassificationSpan> actualSpans,
            IEnumerable<SimplifiedClassificationSpan> expectedSpans)
        {
            var actualSet = new HashSet<SimplifiedClassificationSpan>(actualSpans);
            var expectedList = new List<SimplifiedClassificationSpan>(expectedSpans);

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

            if (expectedList.Count == 0) return (true, String.Empty);

            var builder = new StringBuilder(1 << 12);
            var actualSetBySpan = actualSet.ToDictionary(x => x.Span);
            i = 0;
            while (i < expectedList.Count && actualSetBySpan.Count > 0)
            {
                var expectedClassification = expectedList[i];
                if (actualSetBySpan.TryRemoveValue(expectedClassification.Span, out var value))
                {
                    /// NOTE: expected <see cref="SimplifiedClassificationSpan"/> has incorrect <see cref="IClassificationType"/>
                    builder
                        .AppendSpan(expectedClassification.Span)
                        .AppendLine("Expected type:").AppenClassificationType(expectedClassification.ClassificationType)
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

            return (false, builder.ToString());
        }

        public static (bool, string) AreEquivalent(
            IEnumerable<SimplifiedClassificationSpan> expectedSpans,
            IEnumerable<SimplifiedClassificationSpan> actualSpans)
        {
            var actualList = actualSpans.ToList();
            var expectedList = expectedSpans.ToList();

            int i = 0;
            while (i < actualList.Count && expectedList.Count > 0)
            {
                var hasEquals = false;
                for (int j = 0; j < expectedList.Count; ++j)
                {
                    if (ClassificationComparer.Instance.Equals(expectedList[j], actualList[i]))
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

        private const string tabs = "    ";

        private static void AppendClassificationSpan(this StringBuilder builder, SimplifiedClassificationSpan span) =>
            builder
            .AppendLine("Item:")
            .AppenClassificationType(span.ClassificationType)
            .AppendSpan(span.Span);

        private static StringBuilder AppenClassificationType(this StringBuilder builder, IClassificationType classificationType) =>
            builder
                .Append(tabs).AppendLine("Type:")
                .Append(tabs).Append(tabs).AppendFormat("Classification: {0}", classificationType.Classification).AppendLine()
                .Append(tabs).Append(tabs).AppendFormat("Base types count: {0}", classificationType.BaseTypes.Count()).AppendLine();

        private static StringBuilder AppendSpan(this StringBuilder builder, Span span) =>
            builder.Append(tabs).AppendLine("Span: ").Append(span).AppendLine();

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