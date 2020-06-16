using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.CSharp.Access
{
    internal class LocalVariables : CSharpIdentifierTests
    {
        [Test]
        public void LocalVariableTest_Dynamic()
        {
            GetContext(@"Access\Locals\DynamicVariable.cs").GetClassifications().AssertContains(
                CSharpNames.LocalVariableName.ClassifyAt(195, 4));
        }
    }
}