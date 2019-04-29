using CoCo.Analyser.Classifications.CSharp;
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
            GetContext(@"SimpleExample.cs")
                .AddInfo(CSharpNames.ClassName.EnableInEditor())
                .GetClassifications().AssertIsEquivalent(
                    CSharpNames.NamespaceName.ClassifyAt(10, 17),
                    CSharpNames.ClassName.ClassifyAt(51, 13),
                    CSharpNames.MethodName.ClassifyAt(94, 6),
                    CSharpNames.ParameterName.ClassifyAt(108, 6),
                    CSharpNames.LocalVariableName.ClassifyAt(144, 5));
        }
    }
}