using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Declarations
{
    internal class Parameters : VisualBasicIdentifierTests
    {
        [Test]
        public void ParameterTest_Simple()
        {
            GetContext(@"Declarations\Parameters\SimpleParameter.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(52, 3));
        }

        [Test]
        public void ParameterTest_Optional()
        {
            GetContext(@"Declarations\Parameters\OptionalParameter.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(63, 3));
        }

        [Test]
        public void ParameterTest_TypeCharacter()
        {
            GetContext(@"Declarations\Parameters\TypeCharacter.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(50, 4));
        }

        [Test]
        public void ParameterTest_Variable()
        {
            GetContext(@"Declarations\Parameters\Variable.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(56, 3),
                VisualBasicNames.ParameterName.ClassifyAt(121, 3));
        }

        [Test]
        public void ParameterTest_ByValRef()
        {
            GetContext(@"Declarations\Parameters\ByValRef.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(51, 4),
                VisualBasicNames.ParameterName.ClassifyAt(63, 5));
        }

        [Test]
        public void ParameterTest_Delegate()
        {
            GetContext(@"Declarations\Parameters\DelegateParameter.vb").GetClassifications().AssertContains(
                VisualBasicNames.ParameterName.ClassifyAt(79, 3),
                VisualBasicNames.ParameterName.ClassifyAt(109, 3),
                VisualBasicNames.ParameterName.ClassifyAt(172, 3),
                VisualBasicNames.ParameterName.ClassifyAt(188, 3));
        }
    }
}