using CoCo.Analyser;
using CoCo.Analyser.CSharp;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Access
{
    internal class Members : CSharpIdentifierTests
    {
        [Test]
        public void MemberTest_Event()
        {
            GetClassifications(@"Access\Members\Event.cs").AssertContains(
                CSharpNames.EventName.ClassifyAt(195, 7),
                CSharpNames.EventName.ClassifyAt(234, 7));
        }

        [Test]
        public void MemberTest_Field()
        {
            GetClassifications(@"Access\Members\Field.cs").AssertContains(
                CSharpNames.FieldName.ClassifyAt(171, 5),
                CSharpNames.FieldName.ClassifyAt(215, 5),
                CSharpNames.FieldName.ClassifyAt(270, 5));
        }

        [Test]
        public void MemberTest_Property()
        {
            GetClassifications(@"Access\Members\Property.cs")
                .AssertContains(
                    CSharpNames.PropertyName.ClassifyAt(204, 5),
                    CSharpNames.PropertyName.ClassifyAt(249, 5),
                    CSharpNames.PropertyName.ClassifyAt(298, 5));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            GetClassifications(@"Access\Members\ConstantMember.cs")
                .AssertContains(CSharpNames.ConstantFieldName.ClassifyAt(190, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetClassifications(@"Access\Members\TypeEvent.cs")
                .AssertContains(CSharpNames.EventName.ClassifyAt(250, 7));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetClassifications(@"Access\Members\TypeField.cs")
                .AssertContains(CSharpNames.FieldName.ClassifyAt(186, 13));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetClassifications(@"Access\Members\TypeProperty.cs")
                .AssertContains(CSharpNames.PropertyName.ClassifyAt(182, 15));
        }

        [Test]
        public void MemberTest_ValueTupleField()
        {
            GetClassifications(@"Access\Members\ValueTupleFields.cs").AssertContains(
                CSharpNames.FieldName.ClassifyAt(157, 4),
                CSharpNames.FieldName.ClassifyAt(166, 4),
                CSharpNames.FieldName.ClassifyAt(209, 4),
                CSharpNames.FieldName.ClassifyAt(222, 4),
                CSharpNames.LocalVariableName.ClassifyAt(262, 5),
                CSharpNames.LocalVariableName.ClassifyAt(269, 5),
                CSharpNames.FieldName.ClassifyAt(314, 5),
                CSharpNames.FieldName.ClassifyAt(320, 4),
                CSharpNames.FieldName.ClassifyAt(339, 5));
        }
    }
}