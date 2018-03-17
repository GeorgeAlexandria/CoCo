using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Locals : CSharpIdentifierTests
    {
        [Test]
        public void LocalTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\SimpleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalFieldName.ClassifyAt(151, 6),
                    Names.LocalFieldName.ClassifyAt(226, 9),
                    Names.LocalFieldName.ClassifyAt(237, 9));
        }

        [Test]
        public void LocalTest_ForControlVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\ForControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalFieldName.ClassifyAt(160, 5),
                    Names.LocalFieldName.ClassifyAt(171, 5),
                    Names.LocalFieldName.ClassifyAt(183, 5));
        }

        [Test]
        public void LocalTest_ForeachControlVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\ForeachControlVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalFieldName.ClassifyAt(168, 4));
        }

        [Test]
        public void LocalTest_CatchVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\CatchVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalFieldName.ClassifyAt(256, 9));
        }

        [Test]
        public void LocalTest_OutVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\OutVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalFieldName.ClassifyAt(253, 8));
        }

        [Test]
        public void LocalTest_PatternVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\PatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalFieldName.ClassifyAt(204, 5));
        }

        [Test]
        public void LocalTest_UsingVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\UsingVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalFieldName.ClassifyAt(157, 6));
        }

        [Test]
        public void LocalTest_ValueTupleVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\ValueTupleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalFieldName.ClassifyAt(156, 4),
                    Names.LocalFieldName.ClassifyAt(162, 4));
        }

        [Test]
        public void LocalTest_RangeVariable()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Locals\RangeVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.RangeFieldName.ClassifyAt(186, 4),
                    Names.RangeFieldName.ClassifyAt(242, 5),
                    Names.RangeFieldName.ClassifyAt(250, 4),
                    Names.RangeFieldName.ClassifyAt(292, 4));
        }
    }
}