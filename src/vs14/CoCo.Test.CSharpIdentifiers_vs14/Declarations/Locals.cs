using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Locals : CSharpIdentifierTests
    {
        [Test]
        public void LocalTest()
        {
            @"Declarations\Locals\SimpleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(151, 6),
                    CSharpNames.LocalVariableName.ClassifyAt(226, 9),
                    CSharpNames.LocalVariableName.ClassifyAt(237, 9));
        }

        [Test]
        public void LocalTest_ForControlVariable()
        {
            @"Declarations\Locals\ForControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(160, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(171, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(183, 5));
        }

        [Test]
        public void LocalTest_ForeachControlVariable()
        {
            @"Declarations\Locals\ForeachControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(168, 4));
        }

        [Test]
        public void LocalTest_CatchVariable()
        {
            @"Declarations\Locals\CatchVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(256, 9));
        }

        [Test]
        public void LocalTest_UsingVariable()
        {
            @"Declarations\Locals\UsingVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(157, 6));
        }

        [Test]
        public void LocalTest_RangeVariable()
        {
            @"Declarations\Locals\RangeVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.RangeVariableName.ClassifyAt(186, 4),
                    CSharpNames.RangeVariableName.ClassifyAt(242, 5),
                    CSharpNames.RangeVariableName.ClassifyAt(250, 4),
                    CSharpNames.RangeVariableName.ClassifyAt(292, 4));
        }

        [Test]
        public void LocalTest_DynamicVariable()
        {
            @"Declarations\Locals\DynamicVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(156, 5));
        }
    }
}