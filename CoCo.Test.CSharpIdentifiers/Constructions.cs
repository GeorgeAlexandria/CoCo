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
                    Names.LocalFieldName.ClassifyAt(311, 9),
                    Names.PropertyName.ClassifyAt(321, 7));
        }

        [Test]
        public void ConstructionTest_ForForeach_Variable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\ForForeachControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalFieldName.ClassifyAt(242, 5),
                    Names.LocalFieldName.ClassifyAt(251, 5),
                    Names.LocalFieldName.ClassifyAt(387, 4),
                    Names.LocalFieldName.ClassifyAt(399, 4));
        }

        [Test]
        public void ConstructionTest_If_PatternVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\IfPatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalFieldName.ClassifyAt(254, 5));
        }

        [Test]
        public void ConstructionTest_Switch_PatternVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\SwitchPatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalFieldName.ClassifyAt(279, 5),
                    Names.LocalFieldName.ClassifyAt(391, 4));
        }

        [Test]
        public void ConstructionTest_Using_Variable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Constructions\UsingVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalFieldName.ClassifyAt(228, 6));
        }
    }
}