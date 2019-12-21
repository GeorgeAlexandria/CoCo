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

        [Test]
        public void ModuleFunctionParameterTest()
        {
            GetContext(@"Declarations\Parameters\ModuleFunctionParams.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(43, 3),
                FSharpNames.ParameterName.ClassifyAt(84, 3));
        }

        [Test]
        public void TypeFunctionParameterTest()
        {
            GetContext(@"Declarations\Parameters\TypeFunctionParams.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(60, 3),
                FSharpNames.ParameterName.ClassifyAt(109, 3));
        }

        [Test]
        public void TypeMethodParameterTest()
        {
            GetContext(@"Declarations\Parameters\TypeMethodParams.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(64, 3),
                FSharpNames.ParameterName.ClassifyAt(120, 3));
        }

        [Test]
        public void OptionalParameterTest()
        {
            GetContext(@"Declarations\Parameters\OptionalParameter.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(66, 3));
        }

        [Test]
        public void AbstractMemberParameterTest()
        {
            GetContext(@"Declarations\Parameters\AbstractMemberParameter.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(67, 9));
        }
    }
}