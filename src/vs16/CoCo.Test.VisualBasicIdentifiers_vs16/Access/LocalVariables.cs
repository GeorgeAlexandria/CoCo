using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class LocalVariables : VisualBasicIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Function()
        {
            GetContext(@"Access\Locals\FunctionVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.FunctionVariableName.ClassifyAt(117, 6));
        }

        [Test]
        public void LocalVariableTest()
        {
            GetContext(@"Access\Locals\SimpleVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(102, 4));
        }

        [Test]
        public void LocalVariableTest_ValutTuple()
        {
            GetContext(@"Access\Locals\ValueTupleVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.LocalVariableName.ClassifyAt(137, 3),
                VisualBasicNames.LocalVariableName.ClassifyAt(149, 3));
        }

        [Test]
        public void LocalVariableTest_Static()
        {
            GetContext(@"Access\Locals\StaticVariable.vb").GetClassifications().AssertContains(
                VisualBasicNames.StaticLocalVariableName.ClassifyAt(159, 8));
        }
    }
}