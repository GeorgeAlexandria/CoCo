using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Access
{
    internal class Namespaces : VisualBasicIdentifierTests
    {
        [Test]
        public void NamespaceTest()
        {
            GetContext(@"Access\Namespaces\Namespace.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(69, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(76, 2));
        }

        [Test]
        public void NamespaceTest_Alias()
        {
            GetContext(@"Access\Namespaces\AliasNamespace.vb").GetClassifications().AssertContains(
                VisualBasicNames.AliasNamespaceName.ClassifyAt(358, 6),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(404, 13),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(453, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(460, 13),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(512, 3),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(551, 3));
        }

        [Test]
        public void NamespaceTest_Custom()
        {
            GetContext(@"Access\Namespaces\CustomNamespace.vb").GetClassifications().AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(236, 22),
                VisualBasicNames.NamespaceName.ClassifyAt(259, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(266, 10),
                VisualBasicNames.NamespaceName.ClassifyAt(314, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(321, 10),
                VisualBasicNames.NamespaceName.ClassifyAt(416, 10));
        }
    }
}