using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class LocalVariables : CSharpIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Dynamic()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Locals\DynamicVariable.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.LocalVariableName.ClassifyAt(195, 4));
        }
    }
}