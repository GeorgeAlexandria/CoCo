using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Members : CSharpIdentifierTests
    {
        [Test]
        public void MemberTest_Event()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\Event.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.EventName.ClassifyAt(195, 7),
                    Names.EventName.ClassifyAt(234, 7));
        }

        [Test]
        public void MemberTest_Field()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\Field.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.FieldName.ClassifyAt(171, 5),
                    Names.FieldName.ClassifyAt(215, 5),
                    Names.FieldName.ClassifyAt(270, 5));
        }

        [Test]
        public void MemberTest_Property()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\Property.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    Names.PropertyName.ClassifyAt(204, 5),
                    Names.PropertyName.ClassifyAt(249, 5),
                    Names.PropertyName.ClassifyAt(298, 5));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\TypeEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.EventName.ClassifyAt(250, 7));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\TypeField.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.FieldName.ClassifyAt(184, 8));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\TypeProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.PropertyName.ClassifyAt(182, 15));
        }
    }
}