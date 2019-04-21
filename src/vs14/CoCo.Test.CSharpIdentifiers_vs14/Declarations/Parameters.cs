using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Parameters : CSharpIdentifierTests
    {
        [Test]
        public void ParameterTest()
        {
            GetContext(@"Declarations\Parameters\SimpleParameter.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(133, 4));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            GetContext(@"Declarations\Parameters\Optional.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(126, 4));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            GetContext(@"Declarations\Parameters\Variable.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(135, 5));
        }

        [Test]
        public void ParameterTest_RefOutIn()
        {
            GetContext(@"Declarations\Parameters\RefOutIn.cs").GetClassifications().AssertContains(
                    CSharpNames.ParameterName.ClassifyAt(130, 4),
                    CSharpNames.ParameterName.ClassifyAt(147, 4),
                    CSharpNames.ParameterName.ClassifyAt(160, 4));
        }

        // TODO: add a scpecial type for lambda|delegate parameter?
        [Test]
        public void ParameterTest_LambdaParameter()
        {
            GetContext(@"Declarations\Parameters\LambdaParameter.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(160, 5));
        }

        [Test]
        public void ParameterTest_DelegateParameter()
        {
            GetContext(@"Declarations\Parameters\DelegateParameter.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(175, 5));
        }
    }
}