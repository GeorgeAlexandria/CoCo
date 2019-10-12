using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Access
{
    internal class Parameters : FSharpIdentifierTests
    {
        [Test]
        public void ImplicitCtorParameterTest()
        {
            GetContext(@"Access\Parameters\ImplicitCtorParams.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(72, 3),
                FSharpNames.ParameterName.ClassifyAt(105, 3));
        }

        [Test]
        public void ImplicitCtorParameterShadowingTest()
        {
            var classifications = GetContext(@"Access\Parameters\ImplicitCtorParamsShadowing.fs").GetClassifications();
            classifications.AssertContains(
                FSharpNames.ParameterName.ClassifyAt(99, 3),
                FSharpNames.ParameterName.ClassifyAt(132, 3),
                FSharpNames.ParameterName.ClassifyAt(249, 1));

            classifications.AssertNotContains(FSharpNames.ParameterName.ClassifyAt(199, 3));
        }
    }
}