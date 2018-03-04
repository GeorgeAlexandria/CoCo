using CoCo;
using NUnit.Framework;

namespace CoCoTests
{
    [TestFixture]
    internal class SimpleTest : CSharpIdentifierTests
    {
        [Test]
        public void CommonTest()
        {
            var buffer = "csharp".CreateTextBuffer(@"Tests\CSharpIdentifiers\CSharpIdentifiers\SimpleExample.cs");
            buffer.GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(buffer, 10, 17),
                    Names.MethodName.ClassifyAt(buffer, 94, 6),
                    Names.ParameterName.ClassifyAt(buffer, 108, 6),
                    Names.LocalFieldName.ClassifyAt(buffer, 144, 5));
        }

        [Test]
        public void NamespaceIdentifierTest()
        {
            var buffer = "csharp".CreateTextBuffer(@"Tests\CSharpIdentifiers\CSharpIdentifiers\NamespaceIdentifier.cs");
            buffer.GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(buffer, 10, 17),
                    Names.MethodName.ClassifyAt(buffer, 100, 6),
                    Names.NamespaceName.ClassifyAt(buffer, 133, 6),
                    Names.StaticMethodName.ClassifyAt(buffer, 148, 9));
        }
    }
}