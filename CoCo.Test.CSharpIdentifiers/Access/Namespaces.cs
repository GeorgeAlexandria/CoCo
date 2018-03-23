using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Namespaces : CSharpIdentifierTests
    {
        [Test]
        public void NamespaceTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Namespaces\ByNamespace.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.NamespaceName.ClassifyAt(142, 6),
                    Names.NamespaceName.ClassifyAt(149, 11),
                    Names.NamespaceName.ClassifyAt(161, 7));
        }

        [Test]
        public void NamesapceTest_Alias()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Namespaces\ByNamespaceAlias.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.AliasNamespaceName.ClassifyAt(375, 8),
                    Names.AliasNamespaceName.ClassifyAt(426, 11),
                    Names.AliasNamespaceName.ClassifyAt(488, 3));
        }
    }
}