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
            GetContext(@"Access\Parameters\Delegate.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(243, 3));
        }

        [Test]
        public void ParameterTest_LambdaParameter()
        {
            GetContext(@"Access\Parameters\Lambda.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(196, 3),
                CSharpNames.ParameterName.ClassifyAt(341, 3));
        }

        [Test]
        public void ParameterTest_OptionalParameter()
        {
            GetContext(@"Access\Parameters\Optional.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(190, 4));
        }

        [Test]
        public void ParameterTest_RefInOutParameters()
        {
            GetContext(@"Access\Parameters\RefInOut.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(178, 4),
                CSharpNames.ParameterName.ClassifyAt(185, 4),
                CSharpNames.ParameterName.ClassifyAt(204, 4),
                CSharpNames.ParameterName.ClassifyAt(211, 4));
        }

        [Test]
        public void ParameterTest_VariableParameter()
        {
            GetContext(@"Access\Parameters\Variable.cs").GetClassifications().AssertContains(
                CSharpNames.ParameterName.ClassifyAt(195, 5));
        }
    }
}