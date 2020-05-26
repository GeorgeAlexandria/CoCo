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

        [Test]
        public void FieldTest()
        {
            GetContext(@"Access\Members\Field.fs").GetClassifications().AssertContains(
                FSharpNames.FieldName.ClassifyAt(112, 6),
                FSharpNames.FieldName.ClassifyAt(124, 6));
        }

        [Test]
        public void AnonymousRecordFieldTest()
        {
            GetContext(@"Access\Members\AnonymousRecordField.fs").GetClassifications().AssertContains(
                FSharpNames.FieldName.ClassifyAt(72, 5));
        }

        [Test]
        public void AutoPropertyWithImplicitCtorTest()
        {
            GetContext(@"Access\Members\AutoPropertyWithImplicitCtor.fs").GetClassifications().AssertContains(
                FSharpNames.PropertyName.ClassifyAt(111, 5));
        }

        [Test]
        public void PropertyTest()
        {
            GetContext(@"Access\Members\Property.fs").GetClassifications().AssertContains(
                FSharpNames.PropertyName.ClassifyAt(199, 5),
                FSharpNames.PropertyName.ClassifyAt(217, 5));
        }

        [Test]
        public void UnionFieldTest()
        {
            GetContext(@"Access\Members\UnionField.fs").GetClassifications().AssertContains(
                FSharpNames.ParameterName.ClassifyAt(116, 5));
        }
    }
}