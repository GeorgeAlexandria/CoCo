using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.XmlDocComment
{
    [TestFixture]
    internal class SimpleExample : CSharpIdentifierTests
    {
        [Test]
        public void SimpleTest()
        {
            GetContext(@"XmlDocComment\SimpleExample.cs").GetClassifications().AssertContains(
                CSharpNames.MethodName.ClassifyAt(130, 6));
        }

        [Test]
        public void SimpleTest_DisableXml()
        {
            GetContext(@"XmlDocComment\SimpleExample.cs")
                .AddInfo(
                    CSharpNames.MethodName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    CSharpNames.MethodName.ClassifyAt(130, 6));
        }
    }
}