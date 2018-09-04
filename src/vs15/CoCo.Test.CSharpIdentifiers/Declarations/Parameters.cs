using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Parameters : CSharpIdentifierTests
    {
        [Test]
        public void ParameterTest()
        {
            GetClassifications(@"Declarations\Parameters\SimpleParameter.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(133, 4));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            GetClassifications(@"Declarations\Parameters\Optional.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(126, 4));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            GetClassifications(@"Declarations\Parameters\Variable.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(135, 5));
        }

        [Test]
        public void ParameterTest_RefOutIn()
        {
            GetClassifications(@"Declarations\Parameters\RefOutIn.cs")
                .AssertContains(
                    CSharpNames.ParameterName.ClassifyAt(130, 4),
                    CSharpNames.ParameterName.ClassifyAt(147, 4),
                    CSharpNames.ParameterName.ClassifyAt(160, 4));
        }

        // TODO: add a scpecial type for lambda|delegate parameter?
        [Test]
        public void ParameterTest_LambdaParameter()
        {
            GetClassifications(@"Declarations\Parameters\LambdaParameter.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(160, 5));
        }

        [Test]
        public void ParameterTest_DelegateParameter()
        {
            GetClassifications(@"Declarations\Parameters\DelegateParameter.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(175, 5));
        }
    }
}