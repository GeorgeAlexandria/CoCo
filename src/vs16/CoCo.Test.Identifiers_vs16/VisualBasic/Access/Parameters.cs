using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Access
{
    internal class Parameters : VisualBasicIdentifierTests
    {
        [Test]
        public void ParameterTest_ByBalRef()
        {
            GetContext(@"Access\Parameters\ByValRef.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(120, 4),
                VisualBasicNames.ParameterName.ClassifyAt(127, 4));
        }

        [Test]
        public void ParameterTest_Function()
        {
            GetContext(@"Access\Parameters\DelegateParameter.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(147, 3),
                VisualBasicNames.ParameterName.ClassifyAt(201, 3),
                VisualBasicNames.ParameterName.ClassifyAt(207, 3));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            GetContext(@"Access\Parameters\OptionalParameter.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(142, 3));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            GetContext(@"Access\Parameters\Variable.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(131, 3));
        }
    }
}