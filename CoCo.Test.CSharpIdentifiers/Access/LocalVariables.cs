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
                .AssertContains(Names.LocalFieldName.ClassifyAt(306, 8));
        }

        [Test]
        public void LocalVariableTest_ValueTuple()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Locals\ValueTupleVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.LocalFieldName.ClassifyAt(194, 6),
                    Names.LocalFieldName.ClassifyAt(209, 6));
        }
    }
}