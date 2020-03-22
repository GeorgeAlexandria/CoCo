using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Access
{
    internal class Types : FSharpIdentifierTests
    {
        [Test]
        public void InterfaceImplementationTest()
        {
            GetContext(@"Access\Types\InterfaceImplementation.fs").GetClassifications().AssertContains(
                FSharpNames.InterfaceName.ClassifyAt(114, 5),
                FSharpNames.MethodName.ClassifyAt(144, 3));
        }

        [Test]
        public void UnionCasesTest()
        {
            GetContext(@"Access\Types\UnionCases.fs").GetClassifications().AssertContains(
                FSharpNames.UnionName.ClassifyAt(111, 5),
                FSharpNames.UnionName.ClassifyAt(117, 5),
                FSharpNames.UnionName.ClassifyAt(139, 5),
                FSharpNames.UnionName.ClassifyAt(145, 6),
                FSharpNames.UnionName.ClassifyAt(212, 5),
                FSharpNames.UnionName.ClassifyAt(233, 6));
        }
    }
}