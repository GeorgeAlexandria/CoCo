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
            GetContext(@"Declarations\Locals\SimpleVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(62, 3),
                VisualBasicNames.LocalVariableName.ClassifyAt(79, 5),
                VisualBasicNames.LocalVariableName.ClassifyAt(108, 9));
        }

        [Test]
        public void LocalVariableTest_Catch()
        {
            GetContext(@"Declarations\Locals\CatchVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(72, 2));
        }

        [Test]
        public void LocalVariableTest_Foreach()
        {
            GetContext(@"Declarations\Locals\ForeachVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(70, 3));
        }

        [Test]
        public void LocalVariableTest_For()
        {
            GetContext(@"Declarations\Locals\ForVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(61, 5));
        }

        [Test]
        public void LocalVariableTest_Range()
        {
            GetContext(@"Declarations\Locals\RangeVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.RangeVariableName.ClassifyAt(74, 4),
                VisualBasicNames.RangeVariableName.ClassifyAt(108, 5),
                VisualBasicNames.RangeVariableName.ClassifyAt(116, 4),
                VisualBasicNames.RangeVariableName.ClassifyAt(148, 5));
        }

        [Test]
        public void LocalVariableTest_Using()
        {
            GetContext(@"Declarations\Locals\UsingVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(65, 8));
        }

        [Test]
        public void LocalVariableTest_ValueTuple()
        {
            GetContext(@"Declarations\Locals\ValueTupleVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(66, 5));
        }

        [Test]
        public void LocalVariableTest_With()
        {
            GetContext(@"Declarations\Locals\WithVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(61, 9));
        }

        [Test]
        public void LocalVariableTest_Function()
        {
            GetContext(@"Declarations\Locals\FunctionVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.FunctionVariableName.ClassifyAt(76, 6));
        }

        [Test]
        public void LocalVariableTest_Static()
        {
            GetContext(@"Declarations\Locals\StaticVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.StaticLocalVariableName.ClassifyAt(65, 8),
                VisualBasicNames.StaticLocalVariableName.ClassifyAt(75, 8));
        }
    }
}