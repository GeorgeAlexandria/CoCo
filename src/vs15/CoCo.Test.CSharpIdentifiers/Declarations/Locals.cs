using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Locals : CSharpIdentifierTests
    {
        [Test]
        public void LocalTest()
        {
            GetClassifications(@"Declarations\Locals\SimpleVariable.cs")
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(151, 6),
                    CSharpNames.LocalVariableName.ClassifyAt(226, 9),
                    CSharpNames.LocalVariableName.ClassifyAt(237, 9));
        }

        [Test]
        public void LocalTest_ForControlVariable()
        {
            GetClassifications(@"Declarations\Locals\ForControlVariable.cs")
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(160, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(171, 5),
                    CSharpNames.LocalVariableName.ClassifyAt(183, 5));
        }

        [Test]
        public void LocalTest_ForeachControlVariable()
        {
            GetClassifications(@"Declarations\Locals\ForeachControlVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(168, 4));
        }

        [Test]
        public void LocalTest_CatchVariable()
        {
            GetClassifications(@"Declarations\Locals\CatchVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(256, 9));
        }

        [Test]
        public void LocalTest_OutVariable()
        {
            GetClassifications(@"Declarations\Locals\OutVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(253, 8));
        }

        [Test]
        public void LocalTest_PatternVariable()
        {
            GetClassifications(@"Declarations\Locals\PatternVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(204, 5));
        }

        [Test]
        public void LocalTest_UsingVariable()
        {
            GetClassifications(@"Declarations\Locals\UsingVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(157, 6));
        }

        [Test]
        public void LocalTest_ValueTupleVariable()
        {
            GetClassifications(@"Declarations\Locals\ValueTupleVariable.cs").AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(156, 4),
                CSharpNames.LocalVariableName.ClassifyAt(162, 4),
                CSharpNames.LocalVariableName.ClassifyAt(201, 5));
        }

        [Test]
        public void LocalTest_RangeVariable()
        {
            GetClassifications(@"Declarations\Locals\RangeVariable.cs").AssertContains(
                CSharpNames.RangeVariableName.ClassifyAt(186, 4),
                CSharpNames.RangeVariableName.ClassifyAt(242, 5),
                CSharpNames.RangeVariableName.ClassifyAt(250, 4),
                CSharpNames.RangeVariableName.ClassifyAt(292, 4));
        }

        [Test]
        public void LocalTest_DynamicVariable()
        {
            GetClassifications(@"Declarations\Locals\DynamicVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(156, 5));
        }
    }
}