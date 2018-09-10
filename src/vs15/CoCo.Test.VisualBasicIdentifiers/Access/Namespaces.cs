using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class Namespaces : VisualBasicIdentifierTests
    {
        [Test]
        public void NamespaceTest()
        {
            GetClassifications(@"Access\Namespaces\Namespace.vb").AssertContains(
                VisualBasicNames.NamespaceName.ClassifyAt(69, 6),
                VisualBasicNames.NamespaceName.ClassifyAt(76, 2));
        }

        [Test]
        public void NamespaceTest_Alias()
        {
            GetClassifications(@"Access\Namespaces\AliasNamespace.vb").AssertContains(
                VisualBasicNames.AliasNamespaceName.ClassifyAt(293, 7),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(341, 11),
                VisualBasicNames.NamespaceName.ClassifyAt(353, 7),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(403, 3),
                VisualBasicNames.AliasNamespaceName.ClassifyAt(442, 3));
        }
    }
}