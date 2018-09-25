using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Access
{
    internal class Members : VisualBasicIdentifierTests
    {
        [Test]
        public void MemberTest_Constant()
        {
            GetClassifications(@"Access\Members\ConstantMember.vb")
                .AssertContains(VisualBasicNames.ConstantFieldName.ClassifyAt(112, 8));
        }

        [Test]
        public void MemberTest_InstanceField()
        {
            GetClassifications(@"Access\Members\InstanceField.vb").AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(125, 5),
                VisualBasicNames.FieldName.ClassifyAt(159, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            GetClassifications(@"Access\Members\InstanceProperty.vb").AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(130, 8),
                VisualBasicNames.PropertyName.ClassifyAt(167, 5));
        }

        [Test]
        public void MemberTest_ModuleField()
        {
            GetClassifications(@"Access\Members\ModuleField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(142, 5));
        }

        [Test]
        public void MemberTest_ModuleProperty()
        {
            GetClassifications(@"Access\Members\ModuleProperty.vb")
                .AssertContains(VisualBasicNames.PropertyName.ClassifyAt(122, 10));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetClassifications(@"Access\Members\TypeField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(122, 13));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetClassifications(@"Access\Members\TypeProperty.vb")
                .AssertContains(VisualBasicNames.PropertyName.ClassifyAt(116, 15));
        }

        [Test]
        public void MemberTest_WithEvents()
        {
            GetClassifications(@"Access\Members\WithEventsProperty.vb")
                .AssertContains(VisualBasicNames.WithEventsPropertyName.ClassifyAt(155, 7));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            GetClassifications(@"Access\Members\InstanceEvent.vb").AssertContains(
                VisualBasicNames.EventName.ClassifyAt(222, 12),
                VisualBasicNames.EventName.ClassifyAt(289, 12),
                VisualBasicNames.EventName.ClassifyAt(355, 12),
                VisualBasicNames.EventName.ClassifyAt(431, 12),
                VisualBasicNames.EventName.ClassifyAt(610, 17));
        }

        [Test]
        public void MemberTest_ModuleEvent()
        {
            GetClassifications(@"Access\Members\ModuleEvent.vb").AssertContains(
                VisualBasicNames.EventName.ClassifyAt(249, 6),
                VisualBasicNames.EventName.ClassifyAt(297, 6),
                VisualBasicNames.EventName.ClassifyAt(344, 6),
                VisualBasicNames.EventName.ClassifyAt(401, 6));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetClassifications(@"Access\Members\TypeEvent.vb").AssertContains(
                VisualBasicNames.EventName.ClassifyAt(256, 6),
                VisualBasicNames.EventName.ClassifyAt(298, 6),
                VisualBasicNames.EventName.ClassifyAt(339, 6),
                VisualBasicNames.EventName.ClassifyAt(390, 6));
        }
    }
}