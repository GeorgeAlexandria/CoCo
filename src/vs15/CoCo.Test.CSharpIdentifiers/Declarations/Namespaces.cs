using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Namespaces : CSharpIdentifierTests
    {
        [Test]
        public void NamespaceTest_Declaration()
        {
            GetClassifications(@"Declarations\Namespaces\SimpleDeclaration.cs").AssertIsEquivalent(
                CSharpNames.NamespaceName.ClassifyAt(87, 6),
                CSharpNames.NamespaceName.ClassifyAt(102, 6),
                CSharpNames.NamespaceName.ClassifyAt(109, 11),
                CSharpNames.NamespaceName.ClassifyAt(121, 7));
        }

        [Test]
        public void NamespaceTest_DeclarationWithAlias()
        {
            GetClassifications(@"Declarations\Namespaces\Alias.cs").AssertIsEquivalent(
                CSharpNames.AliasNamespaceName.ClassifyAt(87, 8),
                CSharpNames.NamespaceName.ClassifyAt(98, 6),
                CSharpNames.NamespaceName.ClassifyAt(105, 11),
                CSharpNames.NamespaceName.ClassifyAt(117, 7));
        }

        [Test]
        public void NamespaceTest_TypeAlias()
        {
            GetClassifications(@"Declarations\Namespaces\TypeAlias.cs").AssertIsEquivalent(
                CSharpNames.NamespaceName.ClassifyAt(67, 6),
                CSharpNames.NamespaceName.ClassifyAt(74, 11),
                CSharpNames.NamespaceName.ClassifyAt(86, 7));
        }

        [Test]
        public void NamespaceTest_StaticType()
        {
            GetClassifications(@"Declarations\Namespaces\StaticType.cs").AssertIsEquivalent(
                CSharpNames.NamespaceName.ClassifyAt(13, 6),
                CSharpNames.NamespaceName.ClassifyAt(20, 9));
        }

        [Test]
        public void NamespaceTest_InsideNamespace()
        {
            GetClassifications(@"Declarations\Namespaces\InsideNamespace.cs").AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(66, 6),
                CSharpNames.NamespaceName.ClassifyAt(73, 11),
                CSharpNames.NamespaceName.ClassifyAt(85, 7));
        }

        [Test]
        public void NamespaceTest_CustomAlias()
        {
            GetClassifications(@"Declarations\Namespaces\CustomAlias.cs").AssertContains(
                CSharpNames.AliasNamespaceName.ClassifyAt(6, 10),
                CSharpNames.AliasNamespaceName.ClassifyAt(69, 3));
        }
    }
}