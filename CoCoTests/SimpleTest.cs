using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoCo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NUnit.Framework;

namespace CoCoTests
{
    [TestFixture]
    internal class SimpleTest
    {
        [Test]
        public void Test()
        {
            var code = "class X { void Q(string caller) { var q = 5; } }";
            var stringOperand = new StringOperand(code);
            var buffer = new TextBuffer(new ContentType("csharp"), stringOperand);

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var semanticModel = CSharpCompilation.Create("TestCompilation").AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, true);

            List<ClassificationSpan> actualSpans = null;

            using (var workspace = new AdhocWorkspace())
            {
                var snapshotSpan = new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length);

                var newProject = workspace.AddProject("TestProject", LanguageNames.CSharp);
                var newDocument = workspace.AddDocument(newProject.Id, "TestFile.cs", snapshotSpan.Snapshot.AsText());

                var root = syntaxTree.GetCompilationUnitRoot();

                var classificationTypes = new Dictionary<string, IClassificationType>(32);
                foreach (var item in Names.All)
                {
                    classificationTypes.Add(item, new ClassificationType(item));
                }

                var classifier = new EditorClassifier(classificationTypes, buffer);
                actualSpans = classifier.GetClassificationSpans(workspace, semanticModel, root, snapshotSpan);
            }

            var expectedSpans = new List<ClassificationSpan>
            {
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 15, 1), new ClassificationType(Names.MethodName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 24, 6), new ClassificationType(Names.ParameterName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 38, 1), new ClassificationType(Names.LocalFieldName)),
            };

            AssertIsEquivalent(expectedSpans, actualSpans);
        }

        private static void AssertIsEquivalent(IEnumerable<ClassificationSpan> expectedSpans, IEnumerable<ClassificationSpan> actualSpans)
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
                return;
            }

            void AppendClassificationSpan(StringBuilder appendBuilder, ClassificationSpan span)
            {
                var classificationType = span.ClassificationType;
                appendBuilder.AppendLine("Item:")
                    .AppendLine("    Type:")
                    .Append("        ").AppendFormat("Classification: {0}", classificationType.Classification).AppendLine()
                    .Append("        ").AppendFormat("Base types count: {0}", classificationType.BaseTypes.Count()).AppendLine()
                    .Append("    Span: ").Append(span.Span).AppendLine();
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
            Assert.Fail(builder.ToString());
        }

        private static bool AreClassificationSpanEquals(ClassificationSpan expected, ClassificationSpan actual)
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
    }
}