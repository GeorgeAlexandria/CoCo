using CoCo.Analyser;
using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Namespaces : CSharpIdentifierTests
    {
        [Test]
        public void NamespaceTest()
        {
            GetClassifications(@"Access\Namespaces\ByNamespace.cs").AssertContains(
                CSharpNames.NamespaceName.ClassifyAt(142, 6),
                CSharpNames.NamespaceName.ClassifyAt(149, 11),
                CSharpNames.NamespaceName.ClassifyAt(161, 7),
                CSharpNames.NamespaceName.ClassifyAt(253, 6),
                CSharpNames.NamespaceName.ClassifyAt(260, 11));
        }

        [Test]
        public void NamespaceTest_GlobalNotExists()
        {
            GetClassifications(@"Access\Namespaces\ByNamespace.cs")
                .AssertNotContains(CSharpNames.NamespaceName.ClassifyAt(245, 6));
        }

        [Test]
        public void NamesapceTest_Alias()
        {
            GetClassifications(@"Access\Namespaces\ByNamespaceAlias.cs").AssertContains(
                CSharpNames.AliasNamespaceName.ClassifyAt(381, 8),
                CSharpNames.AliasNamespaceName.ClassifyAt(432, 11),
                CSharpNames.AliasNamespaceName.ClassifyAt(494, 3),
                CSharpNames.AliasNamespaceName.ClassifyAt(542, 3));
        }
    }
}