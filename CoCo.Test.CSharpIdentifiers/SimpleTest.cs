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

        [Test]
        public void NamespaceTest_Declaration()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\NamespaceDeclarations.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(87, 6),
                    Names.NamespaceName.ClassifyAt(102, 6),
                    Names.NamespaceName.ClassifyAt(109, 11),
                    Names.NamespaceName.ClassifyAt(121, 7));
        }

        [Test]
        public void NamespaceTest_DeclarationWithAlias()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\NamespaceAlias.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.AliasNamespaceName.ClassifyAt(87, 8),
                    Names.NamespaceName.ClassifyAt(98, 6),
                    Names.NamespaceName.ClassifyAt(105, 11),
                    Names.NamespaceName.ClassifyAt(117, 7));
        }

        [Test]
        public void NamespaceTest_TypeAlias()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\TypeAlias.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(67, 6),
                    Names.NamespaceName.ClassifyAt(74, 11),
                    Names.NamespaceName.ClassifyAt(86, 7));
        }
    }
}