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
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\SimpleExample.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(10, 17),
                    Names.MethodName.ClassifyAt(94, 6),
                    Names.ParameterName.ClassifyAt(108, 6),
                    Names.LocalFieldName.ClassifyAt(144, 5));
        }

        [Test]
        public void NamespaceIdentifierTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\NamespaceIdentifier.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(10, 17),
                    Names.MethodName.ClassifyAt(100, 6),
                    Names.NamespaceName.ClassifyAt(133, 6),
                    Names.StaticMethodName.ClassifyAt(148, 9));
        }
    }
}