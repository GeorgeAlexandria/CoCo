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
                VisualBasicNames.ParameterName.ClassifyAt(120, 4),
                VisualBasicNames.ParameterName.ClassifyAt(127, 4));
        }

        [Test]
        public void ParameterTest_Function()
        {
            GetClassifications(@"Access\Parameters\DelegateParameter.vb").AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(147, 3),
                VisualBasicNames.ParameterName.ClassifyAt(201, 3),
                VisualBasicNames.ParameterName.ClassifyAt(207, 3));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            GetClassifications(@"Access\Parameters\OptionalParameter.vb")
                .AssertContains(VisualBasicNames.ParameterName.ClassifyAt(142, 3));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            GetClassifications(@"Access\Parameters\Variable.vb")
                .AssertContains(VisualBasicNames.ParameterName.ClassifyAt(131, 3));
        }
    }
}