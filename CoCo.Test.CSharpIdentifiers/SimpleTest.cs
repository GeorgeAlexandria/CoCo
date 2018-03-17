using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers
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
        public void NamespaceTest_FromAccess()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\AccessFromNamespaceIdentifier.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(10, 17),
                    Names.MethodName.ClassifyAt(110, 6),
                    Names.NamespaceName.ClassifyAt(143, 6),
                    Names.StaticMethodName.ClassifyAt(158, 9));
        }
    }
}