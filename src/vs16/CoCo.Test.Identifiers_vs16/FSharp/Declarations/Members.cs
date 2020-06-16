using CoCo.Analyser.Classifications.FSharp;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.FSharp.Declarations
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

        [Test]
        public void FieldTest()
        {
            GetContext(@"Declarations\Members\Field.fs").GetClassifications().AssertContains(
                FSharpNames.FieldName.ClassifyAt(37, 5),
                // TODO: classify as mutable field
                FSharpNames.FieldName.ClassifyAt(66, 11));
        }

        [Test]
        public void AnonymousRecordFieldTest()
        {
            GetContext(@"Declarations\Members\AnonymousRecordField.fs").GetClassifications().AssertContains(
                FSharpNames.FieldName.ClassifyAt(45, 5));
        }

        [Test]
        public void PropertiesTest()
        {
            GetContext(@"Declarations\Members\Properties.fs").GetClassifications().AssertContains(
                FSharpNames.PropertyName.ClassifyAt(63, 5),
                FSharpNames.PropertyName.ClassifyAt(194, 5),
                FSharpNames.PropertyName.ClassifyAt(278, 5),
                FSharpNames.PropertyName.ClassifyAt(366, 5),
                FSharpNames.PropertyName.ClassifyAt(445, 5),
                FSharpNames.PropertyName.ClassifyAt(479, 5),
                FSharpNames.PropertyName.ClassifyAt(572, 5));
        }

        [Test]
        public void IndexedPropertiesTest()
        {
            GetContext(@"Declarations\Members\IndexedProperties.fs").GetClassifications().AssertContains(
                FSharpNames.PropertyName.ClassifyAt(70, 5),
                FSharpNames.PropertyName.ClassifyAt(218, 5),
                FSharpNames.PropertyName.ClassifyAt(309, 5));
        }

        [Test]
        public void UnionFieldTest()
        {
            GetContext(@"Declarations\Members\UnionField.fs").GetClassifications().AssertIsEquivalent(
                FSharpNames.ModuleName.ClassifyAt(7, 10),
                FSharpNames.UnionName.ClassifyAt(26, 4),
                FSharpNames.UnionName.ClassifyAt(40, 4),
                FSharpNames.FieldName.ClassifyAt(48, 5),
                FSharpNames.StructureName.ClassifyAt(56, 3));
        }

        [Test]
        public void ModuleBindingValueTest()
        {
            GetContext(@"Declarations\Members\ModuleBindingValue.fs").GetClassifications().AssertContains(
                FSharpNames.ModuleBindingValueName.ClassifyAt(33, 4));
        }
    }
}