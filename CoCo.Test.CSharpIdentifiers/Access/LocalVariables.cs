using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class LocalVariables : CSharpIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Out()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Locals\OutVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalVariableName.ClassifyAt(306, 8));
        }

        [Test]
        public void LocalVariableTest_ValueTuple()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Locals\ValueTupleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalVariableName.ClassifyAt(194, 6),
                    Names.LocalVariableName.ClassifyAt(209, 6));
        }

        [Test]
        public void LocalVariableTest_Pattern()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Locals\PatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalVariableName.ClassifyAt(188, 4),
                    Names.LocalVariableName.ClassifyAt(196, 4));
        }

        [Test]
        public void LocalVariableTest_Dynamic()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Locals\DynamicVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalVariableName.ClassifyAt(195, 4));
        }
    }
}