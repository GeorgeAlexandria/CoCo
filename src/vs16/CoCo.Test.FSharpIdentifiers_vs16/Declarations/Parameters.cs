using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Declarations
{
    internal class Parameters : FSharpIdentifierTests
    {
        [Test]
        public void ImplicitCtorParameterTest()
        {
            GetContext(@"Declarations\Parameters\ImplicitCtorParams.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(39, 3),
                FSharpNames.ParameterName.ClassifyAt(48, 9));
        }
    }
}