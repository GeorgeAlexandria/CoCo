using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp.Access
{
    internal class LocalVariables : CSharpIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Out()
        {
            GetContext(@"Access\Locals\OutVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(306, 8));
        }

        [Test]
        public void LocalVariableTest_ValueTuple()
        {
            GetContext(@"Access\Locals\ValueTupleVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(194, 6),
                CSharpNames.LocalVariableName.ClassifyAt(209, 6));
        }

        [Test]
        public void LocalVariableTest_Pattern()
        {
            GetContext(@"Access\Locals\PatternVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(188, 4),
                CSharpNames.LocalVariableName.ClassifyAt(196, 4));
        }

        [Test]
        public void LocalVariableTest_Dynamic()
        {
            GetContext(@"Access\Locals\DynamicVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(195, 4));
        }
    }
}