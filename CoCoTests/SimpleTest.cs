using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

            Assert.AreEqual(expectedSpans.Count, actualSpans.Count);

            //Assert.That(actual, Is.All.Matches<ClassificationSpan>(x => Has.Some.Matches<ClassificationSpan>(y => AreClassificationSpanEquals(x, y)).ApplyTo(expected).IsSuccess));

            foreach (var expectedSpan in expectedSpans)
            {
                var hasEqualsItem = false;
                foreach (var actualSpan in actualSpans)
                {
                    if (AreClassificationSpanEquals(expectedSpan, actualSpan))
                    {
                        hasEqualsItem = true;
                        break;
                    }
                }
                Assert.IsTrue(hasEqualsItem);
            }
        }

        private bool AreClassificationSpanEquals(ClassificationSpan expected, ClassificationSpan actual)
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