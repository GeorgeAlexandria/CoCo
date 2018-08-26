using CoCo.Analyser;
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
                    CSharpNames.EventName.ClassifyAt(195, 7),
                    CSharpNames.EventName.ClassifyAt(234, 7));
        }

        [Test]
        public void MemberTest_Field()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\Field.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.FieldName.ClassifyAt(171, 5),
                    CSharpNames.FieldName.ClassifyAt(215, 5),
                    CSharpNames.FieldName.ClassifyAt(270, 5));
        }

        [Test]
        public void MemberTest_Property()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\Property.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.PropertyName.ClassifyAt(204, 5),
                    CSharpNames.PropertyName.ClassifyAt(249, 5),
                    CSharpNames.PropertyName.ClassifyAt(298, 5));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\ConstantMember.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ConstantFieldName.ClassifyAt(190, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\TypeEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.EventName.ClassifyAt(250, 7));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\TypeField.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.FieldName.ClassifyAt(186, 13));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Access\Members\TypeProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.PropertyName.ClassifyAt(182, 15));
        }
    }
}