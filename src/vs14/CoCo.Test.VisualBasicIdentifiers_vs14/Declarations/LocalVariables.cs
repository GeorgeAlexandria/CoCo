using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class LocalVariables : VisualBasicIdentifierTests
    {
        [Test]
        public void LocalVariableTest()
        {
            GetClassifications(@"Declarations\Locals\SimpleVariable.vb").AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(62, 3),
                VisualBasicNames.LocalVariableName.ClassifyAt(79, 5),
                VisualBasicNames.LocalVariableName.ClassifyAt(108, 9));
        }

        [Test]
        public void LocalVariableTest_Catch()
        {
            GetClassifications(@"Declarations\Locals\CatchVariable.vb")
                .AssertContains(VisualBasicNames.LocalVariableName.ClassifyAt(72, 2));
        }

        [Test]
        public void LocalVariableTest_Foreach()
        {
            GetClassifications(@"Declarations\Locals\ForeachVariable.vb")
                .AssertContains(VisualBasicNames.LocalVariableName.ClassifyAt(70, 3));
        }

        [Test]
        public void LocalVariableTest_For()
        {
            GetClassifications(@"Declarations\Locals\ForVariable.vb")
                .AssertContains(VisualBasicNames.LocalVariableName.ClassifyAt(61, 5));
        }

        [Test]
        public void LocalVariableTest_Range()
        {
            GetClassifications(@"Declarations\Locals\RangeVariable.vb").AssertContains(
                VisualBasicNames.RangeVariableName.ClassifyAt(74, 4),
                VisualBasicNames.RangeVariableName.ClassifyAt(108, 5),
                VisualBasicNames.RangeVariableName.ClassifyAt(116, 4),
                VisualBasicNames.RangeVariableName.ClassifyAt(148, 5));
        }

        [Test]
        public void LocalVariableTest_Using()
        {
            GetClassifications(@"Declarations\Locals\UsingVariable.vb")
                .AssertContains(VisualBasicNames.LocalVariableName.ClassifyAt(65, 8));
        }

        [Test]
        public void LocalVariableTest_With()
        {
            GetClassifications(@"Declarations\Locals\WithVariable.vb")
                .AssertContains(VisualBasicNames.LocalVariableName.ClassifyAt(61, 9));
        }

        [Test]
        public void LocalVariableTest_Function()
        {
            GetClassifications(@"Declarations\Locals\FunctionVariable.vb")
                .AssertContains(VisualBasicNames.FunctionVariableName.ClassifyAt(76, 6));
        }
    }
}