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
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\SimpleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(151, 6),
                    CSharpNames.LocalVariableName.ClassifyAt(226, 9),
                    CSharpNames.LocalVariableName.ClassifyAt(237, 9));
        }

        [Test]
        public void LocalTest_ForControlVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\ForControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(160, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(171, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(183, 5));
        }

        [Test]
        public void LocalTest_ForeachControlVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\ForeachControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(168, 4));
        }

        [Test]
        public void LocalTest_CatchVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\CatchVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(256, 9));
        }

        [Test]
        public void LocalTest_OutVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\OutVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(253, 8));
        }

        [Test]
        public void LocalTest_PatternVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\PatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(204, 5));
        }

        [Test]
        public void LocalTest_UsingVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\UsingVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(157, 6));
        }

        [Test]
        public void LocalTest_ValueTupleVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\ValueTupleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(156, 4),
                    CSharpNames.LocalVariableName.ClassifyAt(162, 4),
                    CSharpNames.LocalVariableName.ClassifyAt(201, 5));
        }

        [Test]
        public void LocalTest_RangeVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\RangeVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.RangeVariableName.ClassifyAt(186, 4),
                    CSharpNames.RangeVariableName.ClassifyAt(242, 5),
                    CSharpNames.RangeVariableName.ClassifyAt(250, 4),
                    CSharpNames.RangeVariableName.ClassifyAt(292, 4));
        }

        [Test]
        public void LocalTest_DynamicVariable()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Declarations\Locals\DynamicVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(156, 5));
        }
    }
}