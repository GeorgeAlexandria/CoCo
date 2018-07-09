using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers
{
    internal class Constructions : CSharpIdentifierTests
    {
        [Test]
        public void ConstructionTest_Catch_Variable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\CatchVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalVariableName.ClassifyAt(311, 9),
                    Names.PropertyName.ClassifyAt(321, 7));
        }

        [Test]
        public void ConstructionTest_ForForeach_Variable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\ForForeachControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalVariableName.ClassifyAt(242, 5),
                    Names.LocalVariableName.ClassifyAt(251, 5),
                    Names.LocalVariableName.ClassifyAt(387, 4),
                    Names.LocalVariableName.ClassifyAt(399, 4));
        }

        [Test]
        public void ConstructionTest_If_PatternVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\IfPatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalVariableName.ClassifyAt(254, 5));
        }

        [Test]
        public void ConstructionTest_Switch_PatternVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\SwitchPatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalVariableName.ClassifyAt(279, 5),
                    Names.LocalVariableName.ClassifyAt(391, 4));
        }

        [Test]
        public void ConstructionTest_Using_Variable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\UsingVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalVariableName.ClassifyAt(228, 6));
        }

        [Test]
        public void ConstructionTest_Nameof()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\Nameof.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalVariableName.ClassifyAt(229, 8),
                    Names.EnumFieldName.ClassifyAt(287, 9),
                    Names.PropertyName.ClassifyAt(342, 12),
                    Names.FieldName.ClassifyAt(389, 5));
        }

        [Test]
        public void ConstructionTest_Return()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\Return.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalVariableName.ClassifyAt(172, 6));
        }

        [Test]
        public void ConstructionTest_Throw()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\Throw.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalVariableName.ClassifyAt(204, 9));
        }

        [Test]
        public void ConstructionTest_YieldReturn()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\YieldReturn.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalVariableName.ClassifyAt(235, 5),
                    Names.LocalVariableName.ClassifyAt(337, 4));
        }
    }
}