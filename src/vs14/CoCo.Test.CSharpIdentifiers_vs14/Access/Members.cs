using CoCo.Analyser.Classifications.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Members : CSharpIdentifierTests
    {
        [Test]
        public void MemberTest_Event()
        {
            GetContext(@"Access\Members\Event.cs").GetClassifications().AssertContains(
                CSharpNames.EventName.ClassifyAt(195, 7),
                CSharpNames.EventName.ClassifyAt(234, 7));
        }

        [Test]
        public void MemberTest_Field()
        {
            GetContext(@"Access\Members\Field.cs").GetClassifications().AssertContains(
                CSharpNames.FieldName.ClassifyAt(171, 5),
                CSharpNames.FieldName.ClassifyAt(215, 5),
                CSharpNames.FieldName.ClassifyAt(270, 5));
        }

        [Test]
        public void MemberTest_Property()
        {
            GetContext(@"Access\Members\Property.cs").GetClassifications().AssertContains(
                CSharpNames.PropertyName.ClassifyAt(204, 5),
                CSharpNames.PropertyName.ClassifyAt(249, 5),
                CSharpNames.PropertyName.ClassifyAt(298, 5));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            GetContext(@"Access\Members\ConstantMember.cs").GetClassifications().AssertContains(
                CSharpNames.ConstantFieldName.ClassifyAt(190, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetContext(@"Access\Members\TypeEvent.cs").GetClassifications().AssertContains(
                CSharpNames.EventName.ClassifyAt(250, 7));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetContext(@"Access\Members\TypeField.cs").GetClassifications().AssertContains(
                CSharpNames.FieldName.ClassifyAt(186, 13));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetContext(@"Access\Members\TypeProperty.cs").GetClassifications().AssertContains(
                CSharpNames.PropertyName.ClassifyAt(182, 15));
        }
    }
}