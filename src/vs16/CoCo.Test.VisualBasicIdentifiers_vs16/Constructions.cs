using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers
{
    internal class Constructions : VisualBasicIdentifierTests
    {
        [Test]
        public void ConstructionTest_If()
        {
            GetContext(@"Constructions\If.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(51, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(99, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(145, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(171, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(197, 8));
        }

        [Test]
        public void ConstructionTest_Nameof()
        {
            GetContext(@"Constructions\Nameof.vb").GetClassifications().AssertContains(
                VisualBasicNames.SubName.ClassifyAt(65, 6),
                VisualBasicNames.LocalVariableName.ClassifyAt(99, 6));
        }

        [Test]
        public void ConstructionTest_Throw()
        {
            GetContext(@"Constructions\Throw.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(101, 8));
        }

        [Test]
        public void ConstructionTest_Using()
        {
            GetContext(@"Constructions\Using.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(57, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(96, 6));
        }

        [Test]
        public void ConstructionTest_With()
        {
            GetContext(@"Constructions\With.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(121, 8),
                VisualBasicNames.LocalVariableName.ClassifyAt(175, 5),
                VisualBasicNames.FieldName.ClassifyAt(254, 5));
        }

        [Test]
        public void ConstructionTest_TypeConstraints()
        {
            GetContext(@"Constructions\TypeConstraints.vb")
                .AddInfo(VisualBasicNames.ClassName.EnableInEditor())
                .AddInfo(VisualBasicNames.InterfaceName.EnableInEditor())
                .GetClassifications().AssertContains(
                    VisualBasicNames.InterfaceName.ClassifyAt(38, 11),
                    VisualBasicNames.InterfaceName.ClassifyAt(53, 11),
                    VisualBasicNames.InterfaceName.ClassifyAt(132, 11),
                    VisualBasicNames.InterfaceName.ClassifyAt(147, 11),
                    VisualBasicNames.InterfaceName.ClassifyAt(173, 11),
                    VisualBasicNames.NamespaceName.ClassifyAt(239, 6),
                    VisualBasicNames.ClassName.ClassifyAt(246, 9),
                    VisualBasicNames.NamespaceName.ClassifyAt(310, 6),
                    VisualBasicNames.ClassName.ClassifyAt(317, 9),
                    VisualBasicNames.ClassName.ClassifyAt(381, 9));
        }
    }
}