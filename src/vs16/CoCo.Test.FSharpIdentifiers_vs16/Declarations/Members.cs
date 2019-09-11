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

        [Test]
        public void AutoPropertyWithSelfIdentifierTest()
        {
            GetContext(@"Declarations\Members\AutoPropertyWithSelfIdentifier.fs").GetClassifications().AssertContains(
                FSharpNames.SelfIdentifierName.ClassifyAt(65, 6),
                FSharpNames.PropertyName.ClassifyAt(72, 5));
        }

        [Test]
        public void AutoPropertyWithPrimaryCtorTest()
        {
            GetContext(@"Declarations\Members\AutoPropertyWithPrimaryCtor.fs").GetClassifications().AssertContains(
                FSharpNames.PropertyName.ClassifyAt(68, 5));
        }

        [Test]
        public void RecordFieldTest()
        {
            GetContext(@"Declarations\Members\RecordField.fs").GetClassifications().AssertContains(
                FSharpNames.FieldName.ClassifyAt(56, 5),
                FSharpNames.FieldName.ClassifyAt(76, 6),
                FSharpNames.PropertyName.ClassifyAt(113, 5));
        }

        [Test]
        public void AbstractPropertyTest()
        {
            GetContext(@"Declarations\Members\AbstractProperty.fs").GetClassifications().AssertContains(
                FSharpNames.PropertyName.ClassifyAt(85, 5));
        }
    }
}