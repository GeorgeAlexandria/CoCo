using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class LocalVariables : CSharpIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Out()
        {
            GetClassifications(@"Access\Locals\OutVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(306, 8));
        }

        [Test]
        public void LocalVariableTest_ValueTuple()
        {
            GetClassifications(@"Access\Locals\ValueTupleVariable.cs").AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(194, 6),
                CSharpNames.LocalVariableName.ClassifyAt(209, 6));
        }

        [Test]
        public void LocalVariableTest_Pattern()
        {
            GetClassifications(@"Access\Locals\PatternVariable.cs").AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(188, 4),
                CSharpNames.LocalVariableName.ClassifyAt(196, 4));
        }

        [Test]
        public void LocalVariableTest_Dynamic()
        {
            GetClassifications(@"Access\Locals\DynamicVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(195, 4));
        }
    }
}