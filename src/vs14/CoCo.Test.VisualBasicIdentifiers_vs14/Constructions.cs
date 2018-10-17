using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers
{
    internal class Constructions : VisualBasicIdentifierTests
    {
        [Test]
        public void ConstrcutionTest_If()
        {
            GetContext(@"Constructions\If.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(51, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(99, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(145, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(171, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(197, 8));
        }

        [Test]
        public void ConstrcutionTest_Nameof()
        {
            GetContext(@"Constructions\Nameof.vb").GetClassifications().AssertContains(
                VisualBasicNames.SubName.ClassifyAt(65, 6),
                VisualBasicNames.LocalVariableName.ClassifyAt(99, 6));
        }

        [Test]
        public void ConstrcutionTest_Throw()
        {
            GetContext(@"Constructions\Throw.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(101, 8));
        }

        [Test]
        public void ConstrcutionTest_Using()
        {
            GetContext(@"Constructions\Using.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(57, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(96, 6));
        }

        [Test]
        public void ConstrcutionTest_With()
        {
            GetContext(@"Constructions\With.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(121, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(175, 5),
                VisualBasicNames.FieldName.ClassifyAt(254, 5));
        }
    }
}