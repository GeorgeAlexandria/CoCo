using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Parameters : CSharpIdentifierTests
    {
        [Test]
        public void ParameterTest()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Parameters\SimpleParameter.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(133, 4));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Parameters\Optional.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(126, 4));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Parameters\Variable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(135, 5));
        }

        [Test]
        public void ParameterTest_RefOutIn()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Parameters\RefOutIn.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.ParameterName.ClassifyAt(130, 4),
                    CSharpNames.ParameterName.ClassifyAt(147, 4),
                    CSharpNames.ParameterName.ClassifyAt(160, 4));
        }

        // TODO: add a scpecial type for lambda|delegate parameter?
        [Test]
        public void ParameterTest_LambdaParameter()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Parameters\LambdaParameter.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(160, 5));
        }

        [Test]
        public void ParameterTest_DelegateParameter()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Parameters\DelegateParameter.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(175, 5));
        }
    }
}