using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class LocalVariables : VisualBasicIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Function()
        {
            GetClassifications(@"Access\Locals\FunctionVariable.vb")
                .AssertContains(VisualBasicNames.FunctionVariableName.ClassifyAt(117, 6));
        }

        [Test]
        public void LocalVariableTest()
        {
            GetClassifications(@"Access\Locals\SimpleVariable.vb")
                .AssertContains(VisualBasicNames.LocalVariableName.ClassifyAt(102, 4));
        }

        [Test]
        public void LocalVariableTest_Static()
        {
            GetClassifications(@"Access\Locals\StaticVariable.vb")
                .AssertContains(VisualBasicNames.StaticLocalVariableName.ClassifyAt(159, 8));
        }
    }
}