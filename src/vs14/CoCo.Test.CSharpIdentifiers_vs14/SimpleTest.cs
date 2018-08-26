using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers
{
    internal class SimpleTest : CSharpIdentifierTests
    {
        [Test]
        public void CommonTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\SimpleExample.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    CSharpNames.NamespaceName.ClassifyAt(10, 17),
                    CSharpNames.MethodName.ClassifyAt(94, 6),
                    CSharpNames.ParameterName.ClassifyAt(108, 6),
                    CSharpNames.LocalVariableName.ClassifyAt(144, 5));
        }
    }
}