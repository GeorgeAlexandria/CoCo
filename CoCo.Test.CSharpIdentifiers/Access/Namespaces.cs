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
                    Names.NamespaceName.ClassifyAt(161, 7),
                    Names.NamespaceName.ClassifyAt(253, 6),
                    Names.NamespaceName.ClassifyAt(260, 11));
        }

        [Test]
        public void NamespaceTest_GlobalNotExists()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Namespaces\ByNamespace.cs".GetClassifications(ProjectInfo)
                .AssertNotContains(Names.NamespaceName.ClassifyAt(245, 6));
        }

        [Test]
        public void NamesapceTest_Alias()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Namespaces\ByNamespaceAlias.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.AliasNamespaceName.ClassifyAt(381, 8),
                    Names.AliasNamespaceName.ClassifyAt(432, 11),
                    Names.AliasNamespaceName.ClassifyAt(494, 3));
        }
    }
}