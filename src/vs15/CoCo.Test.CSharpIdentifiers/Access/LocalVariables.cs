using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class LocalVariables : CSharpIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Out()
        {
            @"Access\Locals\OutVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(306, 8));
        }

        [Test]
        public void LocalVariableTest_ValueTuple()
        {
            @"Access\Locals\ValueTupleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(194, 6),
                    CSharpNames.LocalVariableName.ClassifyAt(209, 6));
        }

        [Test]
        public void LocalVariableTest_Pattern()
        {
            @"Access\Locals\PatternVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.LocalVariableName.ClassifyAt(188, 4),
                    CSharpNames.LocalVariableName.ClassifyAt(196, 4));
        }

        [Test]
        public void LocalVariableTest_Dynamic()
        {
            @"Access\Locals\DynamicVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(195, 4));
        }
    }
}