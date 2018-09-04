using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers
{
    internal class Constructions : CSharpIdentifierTests
    {
        [Test]
        public void ConstructionTest_Catch_Variable()
        {
            GetClassifications(@"Constructions\CatchVariable.cs")
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(311, 9),
                    CSharpNames.PropertyName.ClassifyAt(321, 7));
        }

        [Test]
        public void ConstructionTest_ForForeach_Variable()
        {
            GetClassifications(@"Constructions\ForForeachControlVariable.cs")
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(242, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(251, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(387, 4),
                    CSharpNames.LocalVariableName.ClassifyAt(399, 4));
        }

        [Test]
        public void ConstructionTest_Using_Variable()
        {
            GetClassifications(@"Constructions\UsingVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(228, 6));
        }

        [Test]
        public void ConstructionTest_Nameof()
        {
            GetClassifications(@"Constructions\Nameof.cs").AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(229, 8),
                CSharpNames.EnumFieldName.ClassifyAt(287, 9),
                CSharpNames.PropertyName.ClassifyAt(342, 12),
                CSharpNames.FieldName.ClassifyAt(389, 5));
        }

        [Test]
        public void ConstructionTest_Return()
        {
            GetClassifications(@"Constructions\Return.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(172, 6));
        }

        [Test]
        public void ConstructionTest_Throw()
        {
            GetClassifications(@"Constructions\Throw.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(204, 9));
        }

        [Test]
        public void ConstructionTest_YieldReturn()
        {
            GetClassifications(@"Constructions\YieldReturn.cs").AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(235, 5),
                CSharpNames.LocalVariableName.ClassifyAt(337, 4));
        }
    }
}