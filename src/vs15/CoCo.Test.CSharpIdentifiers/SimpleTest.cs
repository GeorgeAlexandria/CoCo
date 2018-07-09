using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers
{
    [TestFixture]
    internal class SimpleTest : CSharpIdentifierTests
    {
        [Test]
        public void CommonTest()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\SimpleExample.cs".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(
                    Names.NamespaceName.ClassifyAt(10, 17),
                    Names.MethodName.ClassifyAt(94, 6),
                    Names.ParameterName.ClassifyAt(108, 6),
                    Names.LocalVariableName.ClassifyAt(144, 5));
        }
    }
}