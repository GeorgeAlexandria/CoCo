using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp.Declarations
{
    internal class Locals : CSharpIdentifierTests
    {
        [Test]
        public void LocalTest()
        {
            GetContext(@"Declarations\Locals\SimpleVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(151, 6),
                CSharpNames.LocalVariableName.ClassifyAt(226, 9),
                CSharpNames.LocalVariableName.ClassifyAt(237, 9));
        }

        [Test]
        public void LocalTest_ForControlVariable()
        {
            GetContext(@"Declarations\Locals\ForControlVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(160, 5),
                CSharpNames.LocalVariableName.ClassifyAt(171, 5),
                CSharpNames.LocalVariableName.ClassifyAt(183, 5));
        }

        [Test]
        public void LocalTest_ForeachControlVariable()
        {
            GetContext(@"Declarations\Locals\ForeachControlVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(168, 4));
        }

        [Test]
        public void LocalTest_CatchVariable()
        {
            GetContext(@"Declarations\Locals\CatchVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(256, 9));
        }

        [Test]
        public void LocalTest_UsingVariable()
        {
            GetContext(@"Declarations\Locals\UsingVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(157, 6));
        }

        [Test]
        public void LocalTest_RangeVariable()
        {
            GetContext(@"Declarations\Locals\RangeVariable.cs").GetClassifications().AssertContains(
                CSharpNames.RangeVariableName.ClassifyAt(186, 4),
                CSharpNames.RangeVariableName.ClassifyAt(242, 5),
                CSharpNames.RangeVariableName.ClassifyAt(250, 4),
                CSharpNames.RangeVariableName.ClassifyAt(292, 4));
        }

        [Test]
        public void LocalTest_DynamicVariable()
        {
            GetContext(@"Declarations\Locals\DynamicVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(156, 5));
        }
    }
}