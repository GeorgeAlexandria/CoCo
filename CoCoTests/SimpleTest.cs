using System.Collections.Generic;
using CoCo;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using NUnit.Framework;

namespace CoCoTests
{
    [TestFixture]
    internal class SimpleTest
    {
        [Test]
        public void CommonTest()
        {
            var code = TestHelper.ReadCode(@"Tests\CSharpIdentifiers\CSharpIdentifiers\SimpleExample.cs");
            var buffer = new TextBuffer(new ContentType("csharp"), new StringOperand(code));

            var actualSpans = TestHelper.GetClassificationSpans(@"Tests\CSharpIdentifiers\CSharpIdentifiers\CSharpIdentifiers.csproj", buffer);
            var expectedSpans = new List<ClassificationSpan>
            {
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 10, 17), new ClassificationType(Names.NamespaceName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 94, 6), new ClassificationType(Names.MethodName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 108, 6), new ClassificationType(Names.ParameterName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 144, 5), new ClassificationType(Names.LocalFieldName)),
            };

            var (isEquivalent, errorMessage) = TestHelper.IsEquivalent(expectedSpans, actualSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }

        [Test]
        public void NamespaceIdentifierTest()
        {
            var code = TestHelper.ReadCode(@"Tests\CSharpIdentifiers\CSharpIdentifiers\NamespaceIdentifier.cs");
            var buffer = new TextBuffer(new ContentType("csharp"), new StringOperand(code));

            var actualSpans = TestHelper.GetClassificationSpans(@"Tests\CSharpIdentifiers\CSharpIdentifiers\CSharpIdentifiers.csproj", buffer);
            var expectedSpans = new List<ClassificationSpan>
            {
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 10, 17), new ClassificationType(Names.NamespaceName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 100, 6), new ClassificationType(Names.MethodName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 133, 6), new ClassificationType(Names.NamespaceName)),
                new ClassificationSpan(new SnapshotSpan(buffer.CurrentSnapshot, 148, 9), new ClassificationType(Names.StaticMethodName)),
            };

            var (isEquivalent, errorMessage) = TestHelper.IsEquivalent(expectedSpans, actualSpans);
            if (!isEquivalent) Assert.Fail(errorMessage);
        }
    }
}