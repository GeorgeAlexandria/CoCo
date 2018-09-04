using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Parameters : CSharpIdentifierTests
    {
        [Test]
        public void ParameterTest_DelegateParameter()
        {
            GetClassifications(@"Access\Parameters\Delegate.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(243, 3));
        }

        [Test]
        public void ParameterTest_LambdaParameter()
        {
            GetClassifications(@"Access\Parameters\Lambda.cs").AssertContains(
                CSharpNames.ParameterName.ClassifyAt(196, 3),
                CSharpNames.ParameterName.ClassifyAt(341, 3));
        }

        [Test]
        public void ParameterTest_OptionalParameter()
        {
            GetClassifications(@"Access\Parameters\Optional.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(190, 4));
        }

        [Test]
        public void ParameterTest_RefInOutParameters()
        {
            GetClassifications(@"Access\Parameters\RefInOut.cs").AssertContains(
                CSharpNames.ParameterName.ClassifyAt(178, 4),
                CSharpNames.ParameterName.ClassifyAt(185, 4),
                CSharpNames.ParameterName.ClassifyAt(204, 4),
                CSharpNames.ParameterName.ClassifyAt(211, 4));
        }

        [Test]
        public void ParameterTest_VariableParameter()
        {
            GetClassifications(@"Access\Parameters\Variable.cs")
                .AssertContains(CSharpNames.ParameterName.ClassifyAt(195, 5));
        }
    }
}