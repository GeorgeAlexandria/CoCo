using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class Namespaces : VisualBasicIdentifierTests
    {
        [Test]
        public void NamespaceTest()
        {
            GetClassifications(@"Declarations\Namespaces\Simple.vb").AssertIsEquivalent(
                VisualBasicNames.NamespaceName.ClassifyAt(89, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(106, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(113, 11),
                VisualBasicNames.NamespaceName.ClassifyAt(125, 7));
        }

        [Test]
        public void NamespaceTest_StaticType()
        {
            GetClassifications(@"Declarations\Namespaces\StaticType.vb")
                .AssertIsEquivalent(VisualBasicNames.NamespaceName.ClassifyAt(8, 6));
        }

        [Test]
        public void NamespaceTest_Alias()
        {
            GetClassifications(@"Declarations\Namespaces\Alias.vb").AssertIsEquivalent(
                VisualBasicNames.AliasNamespaceName.ClassifyAt(8, 2),
                VisualBasicNames.NamespaceName.ClassifyAt(13, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(20, 2));
        }

        [Test]
        public void NamespaceTest_TypeAlias()
        {
            GetClassifications(@"Declarations\Namespaces\TypeAlias.vb")
                .AssertIsEquivalent(VisualBasicNames.NamespaceName.ClassifyAt(13, 6));
        }
    }
}