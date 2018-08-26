using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Namespaces : CSharpIdentifierTests
    {
        [Test]
        public void NamespaceTest_Declaration()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\SimpleDeclaration.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    CSharpNames.NamespaceName.ClassifyAt(87, 6),
                    CSharpNames.NamespaceName.ClassifyAt(102, 6),
                    CSharpNames.NamespaceName.ClassifyAt(109, 11),
                    CSharpNames.NamespaceName.ClassifyAt(121, 7));
        }

        [Test]
        public void NamespaceTest_DeclarationWithAlias()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\Alias.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    CSharpNames.AliasNamespaceName.ClassifyAt(87, 8),
                    CSharpNames.NamespaceName.ClassifyAt(98, 6),
                    CSharpNames.NamespaceName.ClassifyAt(105, 11),
                    CSharpNames.NamespaceName.ClassifyAt(117, 7));
        }

        [Test]
        public void NamespaceTest_TypeAlias()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\TypeAlias.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    CSharpNames.NamespaceName.ClassifyAt(67, 6),
                    CSharpNames.NamespaceName.ClassifyAt(74, 11),
                    CSharpNames.NamespaceName.ClassifyAt(86, 7));
        }

        [Test]
        public void NamespaceTest_StaticType()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\StaticType.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    CSharpNames.NamespaceName.ClassifyAt(13, 6),
                    CSharpNames.NamespaceName.ClassifyAt(20, 9));
        }

        [Test]
        public void NamespaceTest_InsideNamespace()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Namespace\InsideNamespace.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.NamespaceName.ClassifyAt(65, 6),
                    CSharpNames.NamespaceName.ClassifyAt(72, 11),
                    CSharpNames.NamespaceName.ClassifyAt(84, 7));
        }
    }
}