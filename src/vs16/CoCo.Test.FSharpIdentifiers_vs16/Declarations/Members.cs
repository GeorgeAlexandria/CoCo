using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Declarations
{
    internal class Members : FSharpIdentifierTests
    {
        [Test]
        public void AutoPropertyWithWildIdentifierTest()
        {
            GetContext(@"Declarations\Members\AutoPropertyWithWildIdentifier.fs").GetClassifications().AssertContains(
                FSharpNames.PropertyName.ClassifyAt(68, 5));
        }
    }
}