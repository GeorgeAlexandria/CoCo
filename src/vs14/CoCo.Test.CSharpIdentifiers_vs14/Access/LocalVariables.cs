using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class LocalVariables : CSharpIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Dynamic()
        {
            GetClassifications(@"Access\Locals\DynamicVariable.cs")
                .AssertContains(CSharpNames.LocalVariableName.ClassifyAt(195, 4));
        }
    }
}