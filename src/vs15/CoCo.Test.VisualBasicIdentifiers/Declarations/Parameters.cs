using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class Parameters : VisualBasicIdentifierTests
    {
        [Test]
        public void ParameterTest_Simple()
        {
            GetClassifications(@"Declarations\Parameters\SimpleParameter.vb")
                .AssertContains(VisualBasicNames.ParameterName.ClassifyAt(52, 3));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            GetClassifications(@"Declarations\Parameters\OptionalParameter.vb")
                .AssertContains(VisualBasicNames.ParameterName.ClassifyAt(63, 3));
        }

        [Test]
        public void ParameterTest_TypeCharacter()
        {
            GetClassifications(@"Declarations\Parameters\TypeCharacter.vb")
                .AssertContains(VisualBasicNames.ParameterName.ClassifyAt(50, 4));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            GetClassifications(@"Declarations\Parameters\Variable.vb").AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(56, 3),
                VisualBasicNames.ParameterName.ClassifyAt(121, 3));
        }

        [Test]
        public void ParameterTest_ByValRef()
        {
            GetClassifications(@"Declarations\Parameters\ByValRef.vb").AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(51, 4),
                VisualBasicNames.ParameterName.ClassifyAt(63, 5));
        }

        [Test]
        public void ParameterTest_Delegate()
        {
            GetClassifications(@"Declarations\Parameters\DelegateParameter.vb").AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(79, 3),
                VisualBasicNames.ParameterName.ClassifyAt(109, 3),
                VisualBasicNames.ParameterName.ClassifyAt(172, 3),
                VisualBasicNames.ParameterName.ClassifyAt(188, 3));
        }
    }
}