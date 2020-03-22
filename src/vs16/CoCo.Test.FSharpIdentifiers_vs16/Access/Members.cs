using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.FSharpIdentifiers.Access
{
    internal class Members : FSharpIdentifierTests
    {
        [Test]
        public void RecordFieldInPatternCaseTest()
        {
            GetContext(@"Access\Members\RecordFieldInPatternCase.fs").GetClassifications().AssertContains(
                FSharpNames.FieldName.ClassifyAt(154, 5));
        }
    }
}