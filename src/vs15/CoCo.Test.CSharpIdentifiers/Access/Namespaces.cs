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
                CSharpNames.NamespaceName.ClassifyAt(143, 6),
                CSharpNames.NamespaceName.ClassifyAt(150, 11),
                CSharpNames.NamespaceName.ClassifyAt(162, 7),
                CSharpNames.NamespaceName.ClassifyAt(254, 6),
                CSharpNames.NamespaceName.ClassifyAt(261, 11));
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
                CSharpNames.AliasNamespaceName.ClassifyAt(382, 8),
                CSharpNames.AliasNamespaceName.ClassifyAt(433, 11),
                CSharpNames.AliasNamespaceName.ClassifyAt(495, 3),
                CSharpNames.AliasNamespaceName.ClassifyAt(543, 3));
        }
    }
}