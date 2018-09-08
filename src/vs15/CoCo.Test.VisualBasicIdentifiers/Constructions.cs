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
            GetClassifications(@"Constructions\If.vb").AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(51, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(99, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(145, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(171, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(197, 8));
        }

        [Test]
        public void ConstrcutionTest_Nameof()
        {
            GetClassifications(@"Constructions\Nameof.vb").AssertContains(
                VisualBasicNames.SubName.ClassifyAt(65, 6),
                VisualBasicNames.LocalVariableName.ClassifyAt(99, 6));
        }

        [Test]
        public void ConstrcutionTest_Throw()
        {
            GetClassifications(@"Constructions\Throw.vb").
                AssertContains(VisualBasicNames.LocalVariableName.ClassifyAt(101, 8));
        }

        [Test]
        public void ConstrcutionTest_Using()
        {
            GetClassifications(@"Constructions\Using.vb").AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(57, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(96, 6));
        }

        [Test]
        public void ConstrcutionTest_With()
        {
            GetClassifications(@"Constructions\With.vb").AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(121, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(175, 5),
                VisualBasicNames.FieldName.ClassifyAt(254, 5));
        }
    }
}