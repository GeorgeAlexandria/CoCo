using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class Parameters : VisualBasicIdentifierTests
    {
        [Test]
        public void ParameterTest_ByBalRef()
        {
            GetClassifications(@"Access\Parameters\ByValRef.vb").AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(116, 4),
                VisualBasicNames.ParameterName.ClassifyAt(123, 4));
        }

        [Test]
        public void ParameterTest_Function()
        {
            GetClassifications(@"Access\Parameters\DelegateParameter.vb").AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(143, 3),
                VisualBasicNames.ParameterName.ClassifyAt(197, 3),
                VisualBasicNames.ParameterName.ClassifyAt(203, 3));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            GetClassifications(@"Access\Parameters\OptionalParameter.vb")
                .AssertContains(VisualBasicNames.ParameterName.ClassifyAt(138, 3));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            GetClassifications(@"Access\Parameters\Variable.vb")
                .AssertContains(VisualBasicNames.ParameterName.ClassifyAt(127, 3));
        }
    }
}