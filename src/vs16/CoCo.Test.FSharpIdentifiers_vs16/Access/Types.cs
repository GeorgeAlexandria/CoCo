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
    }
}