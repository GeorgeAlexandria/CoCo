using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.XmlDocComment
{
    [TestFixture]
    internal class SimpleExample : VisualBasicIdentifierTests
    {
        [Test]
        public void SimpleTest()
        {
            GetContext(@"XmlDocComment\SimpleExample.vb").GetClassifications().AssertContains(
                VisualBasicNames.SubName.ClassifyAt(64, 6));
        }

        [Test]
        public void SimpleTest_DisableXml()
        {
            GetContext(@"XmlDocComment\SimpleExample.vb")
                .AddInfo(
                    VisualBasicNames.SubName.DisableInXml())
                .GetClassifications().AssertNotContains(
                    VisualBasicNames.SubName.ClassifyAt(64, 6));
        }
    }
}