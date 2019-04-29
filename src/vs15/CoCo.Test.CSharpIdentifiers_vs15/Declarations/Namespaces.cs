using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Namespaces : CSharpIdentifierTests
    {
        [Test]
        public void NamespaceTest_Declaration()
        {
            GetContext(@"Declarations\Namespaces\SimpleDeclaration.cs").GetClassifications().AssertIsEquivalent(
                CSharpNames.NamespaceName.ClassifyAt(87, 6),
                CSharpNames.NamespaceName.ClassifyAt(102, 6),
                CSharpNames.NamespaceName.ClassifyAt(109, 11),
                CSharpNames.NamespaceName.ClassifyAt(121, 7));
        }

        [Test]
        public void NamespaceTest_DeclarationWithAlias()
        {
            GetContext(@"Declarations\Namespaces\Alias.cs").GetClassifications().AssertIsEquivalent(
                CSharpNames.AliasNamespaceName.ClassifyAt(87, 8),
                CSharpNames.NamespaceName.ClassifyAt(98, 6),
                CSharpNames.NamespaceName.ClassifyAt(105, 11),
                CSharpNames.NamespaceName.ClassifyAt(117, 7));
        }

        [Test]
        public void NamespaceTest_TypeAlias()
        {
            GetContext(@"Declarations\Namespaces\TypeAlias.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(67, 6),
                CSharpNames.NamespaceName.ClassifyAt(74, 11),
                CSharpNames.NamespaceName.ClassifyAt(86, 7));
        }

        [Test]
        public void NamespaceTest_StaticType()
        {
            GetContext(@"Declarations\Namespaces\StaticType.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(13, 6),
                CSharpNames.NamespaceName.ClassifyAt(20, 9));
        }

        [Test]
        public void NamespaceTest_InsideNamespace()
        {
            GetContext(@"Declarations\Namespaces\InsideNamespace.cs").GetClassifications().AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(66, 6),
                CSharpNames.NamespaceName.ClassifyAt(73, 11),
                CSharpNames.NamespaceName.ClassifyAt(85, 7));
        }

        [Test]
        public void NamespaceTest_CustomAlias()
        {
            GetContext(@"Declarations\Namespaces\CustomAlias.cs").GetClassifications().AssertContains(
                CSharpNames.AliasNamespaceName.ClassifyAt(6, 10),
                CSharpNames.AliasNamespaceName.ClassifyAt(69, 3));
        }
    }
}