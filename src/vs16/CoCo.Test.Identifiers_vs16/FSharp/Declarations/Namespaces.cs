using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.FSharp.Declarations
{
    internal class Namespaces : FSharpIdentifierTests
    {
        [Test]
        public void SimpleTest()
        {
            GetContext(@"Declarations\Namespaces\Simple.fs").GetClassifications().AssertContains(
                FSharpNames.NamespaceName.ClassifyAt(10, 6));
        }

        [Test]
        public void NestedNamespaceTest()
        {
            GetContext(@"Declarations\Namespaces\Nested.fs").GetClassifications().AssertContains(
                FSharpNames.NamespaceName.ClassifyAt(10, 4),
                FSharpNames.NamespaceName.ClassifyAt(28, 4),
                FSharpNames.NamespaceName.ClassifyAt(33, 6));
        }

        [Test]
        public void RecursiveNamespaceTest()
        {
            GetContext(@"Declarations\Namespaces\Recursive.fs").GetClassifications().AssertContains(
                FSharpNames.NamespaceName.ClassifyAt(14, 9));
        }
    }
}