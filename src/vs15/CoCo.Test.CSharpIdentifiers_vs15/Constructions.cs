using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers
{
    internal class Constructions : CSharpIdentifierTests
    {
        [Test]
        public void ConstructionTest_Catch_Variable()
        {
            GetContext(@"Constructions\CatchVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(311, 9),
                CSharpNames.PropertyName.ClassifyAt(321, 7));
        }

        [Test]
        public void ConstructionTest_ForForeach_Variable()
        {
            GetContext(@"Constructions\ForForeachControlVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(242, 5),
                CSharpNames.LocalVariableName.ClassifyAt(251, 5),
                CSharpNames.LocalVariableName.ClassifyAt(387, 4),
                CSharpNames.LocalVariableName.ClassifyAt(399, 4));
        }

        [Test]
        public void ConstructionTest_If_PatternVariable()
        {
            GetContext(@"Constructions\IfPatternVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(254, 5));
        }

        [Test]
        public void ConstructionTest_Switch_PatternVariable()
        {
            GetContext(@"Constructions\SwitchPatternVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(279, 5),
                CSharpNames.LocalVariableName.ClassifyAt(391, 4));
        }

        [Test]
        public void ConstructionTest_Using_Variable()
        {
            GetContext(@"Constructions\UsingVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(228, 6));
        }

        [Test]
        public void ConstructionTest_Nameof()
        {
            GetContext(@"Constructions\Nameof.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(229, 8),
                CSharpNames.EnumFieldName.ClassifyAt(287, 9),
                CSharpNames.PropertyName.ClassifyAt(342, 12),
                CSharpNames.FieldName.ClassifyAt(389, 5),
                CSharpNames.MethodName.ClassifyAt(430, 6));
        }

        [Test]
        public void ConstructionTest_Return()
        {
            GetContext(@"Constructions\Return.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(172, 6));
        }

        [Test]
        public void ConstructionTest_Throw()
        {
            GetContext(@"Constructions\Throw.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(204, 9));
        }

        [Test]
        public void ConstructionTest_YieldReturn()
        {
            GetContext(@"Constructions\YieldReturn.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(235, 5),
                CSharpNames.LocalVariableName.ClassifyAt(337, 4));
        }

        [Test]
        public void ConstructionTest_Inheritance()
        {
            GetContext(@"Constructions\Inheritance.cs")
                .AddInfo(CSharpNames.ClassName.EnableInEditor())
                .AddInfo(CSharpNames.InterfaceName.EnableInEditor())
                .GetClassifications().AssertContains(
                    CSharpNames.ClassName.ClassifyAt(114, 6),
                    CSharpNames.InterfaceName.ClassifyAt(122, 11));
        }
    }
}