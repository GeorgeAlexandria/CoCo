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
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\Event.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.EventName.ClassifyAt(195, 7),
                    CSharpNames.EventName.ClassifyAt(234, 7));
        }

        [Test]
        public void MemberTest_Field()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\Field.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.FieldName.ClassifyAt(171, 5),
                    CSharpNames.FieldName.ClassifyAt(215, 5),
                    CSharpNames.FieldName.ClassifyAt(270, 5));
        }

        [Test]
        public void MemberTest_Property()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\Property.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.PropertyName.ClassifyAt(204, 5),
                    CSharpNames.PropertyName.ClassifyAt(249, 5),
                    CSharpNames.PropertyName.ClassifyAt(298, 5));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\ConstantMember.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ConstantFieldName.ClassifyAt(190, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\TypeEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.EventName.ClassifyAt(250, 7));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\TypeField.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.FieldName.ClassifyAt(186, 13));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\TypeProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.PropertyName.ClassifyAt(182, 15));
        }

        [Test]
        public void MemberTest_ValueTupleField()
        {
            @"Tests\Identifiers\CSharpIdentifiers\Access\Members\ValueTupleFields.cs".GetClassifications(ProjectInfo)
                .AssertContains(
                    CSharpNames.FieldName.ClassifyAt(157, 4),
                    CSharpNames.FieldName.ClassifyAt(166, 4),
                    CSharpNames.FieldName.ClassifyAt(209, 4),
                    CSharpNames.FieldName.ClassifyAt(222, 4));
        }
    }
}