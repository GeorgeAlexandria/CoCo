using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.FSharp.Access
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
                FSharpNames.ParameterName.ClassifyAt(132, 3));

            classifications.AssertNotContains(FSharpNames.ParameterName.ClassifyAt(199, 3));
        }

        [Test]
        public void FunctionParameterTest()
        {
            GetContext(@"Access\Parameters\FunctionParams.fs").GetClassifications().AssertContains(
               FSharpNames.ParameterName.ClassifyAt(58, 3),
               FSharpNames.ParameterName.ClassifyAt(136, 3),
               FSharpNames.LocalBindingValueName.ClassifyAt(71, 3),
               FSharpNames.ParameterName.ClassifyAt(201, 3));
        }

        [Test]
        public void MethodParameterTest()
        {
            GetContext(@"Access\Parameters\MethodParams.fs").GetClassifications().AssertContains(
               FSharpNames.ParameterName.ClassifyAt(81, 3),
               FSharpNames.LocalBindingValueName.ClassifyAt(98, 3));
        }

        [Test]
        public void LambdaExpressionParamTest()
        {
            GetContext(@"Access\Parameters\LambdaExpressionParams.fs").GetClassifications().AssertContains(
               FSharpNames.ParameterName.ClassifyAt(52, 3));
        }

        [Test]
        public void UnionCtorParamTest()
        {
            GetContext(@"Access\Parameters\UnionCtorParameter.fs").GetClassifications().AssertContains(
               FSharpNames.ParameterName.ClassifyAt(119, 5));
        }
    }
}