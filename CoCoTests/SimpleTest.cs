using System.Collections.Generic;
using CoCo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NUnit.Framework;

namespace CoCoTests
{
    [TestFixture]
    internal class SimpleTest
    {
        // TODO: move a tested code to the separate project|solution
        [Test]
        public void CommonTest()
        {
            var code = "class X { void Q(string caller) { var q = 5; } }";
            var buffer = new TextBuffer(new ContentType("csharp"), new StringOperand(code));

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var semanticModel = CSharpCompilation.Create("TestCompilation").AddSyntaxTrees(syntaxTree).GetSemanticModel(syntaxTree, true);
            var actualSpans = TestHelper.GetClassificationSpans(syntaxTree, semanticModel, buffer);

            var expectedSpans = new List<ClassificationSpan>
            {
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 15, 1), new ClassificationType(Names.MethodName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 24, 6), new ClassificationType(Names.ParameterName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 38, 1), new ClassificationType(Names.LocalFieldName)),
            };

            var (isEquivalent, errorMessage) = TestHelper.IsEquivalent(expectedSpans, actualSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }

        [Test]
        public void NamespaceIdentifierTest()
        {
            var code = "class X { void Q() {  System.Console.WriteLine() } }";
            var buffer = new TextBuffer(new ContentType("csharp"), new StringOperand(code));

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var semanticModel = CSharpCompilation.Create("TestCompilation")
                .AddSyntaxTrees(syntaxTree)
                .AddReferences(MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location))
                .GetSemanticModel(syntaxTree, true);

            var actualSpans = TestHelper.GetClassificationSpans(syntaxTree, semanticModel, buffer);
            var expectedSpans = new List<ClassificationSpan>
            {
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 15, 1), new ClassificationType(Names.MethodName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 22, 6), new ClassificationType(Names.NamespaceName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 37, 9), new ClassificationType(Names.StaticMethodName)),
            };

            var (isEquivalent, errorMessage) = TestHelper.IsEquivalent(expectedSpans, actualSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }
    }
}