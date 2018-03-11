using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Parameters : CSharpIdentifierTests
    {
        [Test]
        public void ParameterTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Parameters\SimpleParameter.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.ParameterName.ClassifyAt(133, 4));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Parameters\Optional.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.ParameterName.ClassifyAt(126, 4));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Parameters\Variable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.ParameterName.ClassifyAt(135, 5));
        }

        [Test]
        public void ParameterTest_RefOut()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Parameters\RefOut.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.ParameterName.ClassifyAt(128, 4),
                    Names.ParameterName.ClassifyAt(145, 4));
        }
    }
}