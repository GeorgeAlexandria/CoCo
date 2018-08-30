using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers
{
    internal class CommonExample : VisualBasicIdentifierTests
    {
        [Test]
        public void CommonTest()
        {
            @"Tests\Identifiers\VisualBasicIdentifiers\Example.vb".GetClassifications(ProjectInfo)
                .AssertIsEquivalent(VisualBasicNames.SubName.ClassifyAt(37, 6));
        }
    }
}