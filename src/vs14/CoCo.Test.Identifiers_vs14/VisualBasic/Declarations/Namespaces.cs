using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Declarations
{
    internal class Namespaces : VisualBasicIdentifierTests
    {
        [Test]
        public void NamespaceTest()
        {
            GetContext(@"Declarations\Namespaces\Simple.vb").GetClassifications().AssertIsEquivalent(
                VisualBasicNames.NamespaceName.ClassifyAt(10, 8));
        }

        [Test]
        public void NamespaceTest_Import()
        {
            GetContext(@"Declarations\Namespaces\SimpleImport.vb").GetClassifications().AssertIsEquivalent(
                VisualBasicNames.NamespaceName.ClassifyAt(89, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(105, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(112, 11),
                VisualBasicNames.NamespaceName.ClassifyAt(124, 7));
        }

        [Test]
        public void NamespaceTest_StaticType()
        {
            GetContext(@"Declarations\Namespaces\StaticType.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(8, 6));
        }

        [Test]
        public void NamespaceTest_Alias()
        {
            GetContext(@"Declarations\Namespaces\Alias.vb").GetClassifications().AssertIsEquivalent(
                VisualBasicNames.AliasNamespaceName.ClassifyAt(8, 2),
                VisualBasicNames.NamespaceName.ClassifyAt(13, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(20, 2));
        }

        [Test]
        public void NamespaceTest_TypeAlias()
        {
            GetContext(@"Declarations\Namespaces\TypeAlias.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(13, 6));
        }

        [Test]
        public void NamespaceTest_CustomAlias()
        {
            GetContext(@"Declarations\Namespaces\CustomAlias.vb").GetClassifications().AssertContains(
                VisualBasicNames.AliasNamespaceName.ClassifyAt(89, 4),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(183, 6));
        }
    }
}