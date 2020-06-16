using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Access
{
    internal class Members : VisualBasicIdentifierTests
    {
        [Test]
        public void MemberTest_Constant()
        {
            GetContext(@"Access\Members\ConstantMember.vb").GetClassifications().AssertContains(
                VisualBasicNames.ConstantFieldName.ClassifyAt(112, 8));
        }

        [Test]
        public void MemberTest_InstanceField()
        {
            GetContext(@"Access\Members\InstanceField.vb").GetClassifications().AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(125, 5),
                VisualBasicNames.FieldName.ClassifyAt(159, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            GetContext(@"Access\Members\InstanceProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(130, 8),
                VisualBasicNames.PropertyName.ClassifyAt(167, 5));
        }

        [Test]
        public void MemberTest_ModuleField()
        {
            GetContext(@"Access\Members\ModuleField.vb").GetClassifications().AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(142, 5));
        }

        [Test]
        public void MemberTest_ModuleProperty()
        {
            GetContext(@"Access\Members\ModuleProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(122, 10));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetContext(@"Access\Members\TypeField.vb").GetClassifications().AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(122, 13));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetContext(@"Access\Members\TypeProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(116, 15));
        }

        [Test]
        public void MemberTest_WithEvents()
        {
            GetContext(@"Access\Members\WithEventsProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.WithEventsPropertyName.ClassifyAt(155, 7));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            GetContext(@"Access\Members\InstanceEvent.vb").GetClassifications().AssertContains(
                VisualBasicNames.EventName.ClassifyAt(222, 12),
                VisualBasicNames.EventName.ClassifyAt(289, 12),
                VisualBasicNames.EventName.ClassifyAt(355, 12),
                VisualBasicNames.EventName.ClassifyAt(431, 12),
                VisualBasicNames.EventName.ClassifyAt(610, 17));
        }

        [Test]
        public void MemberTest_ModuleEvent()
        {
            GetContext(@"Access\Members\ModuleEvent.vb").GetClassifications().AssertContains(
                VisualBasicNames.EventName.ClassifyAt(249, 6),
                VisualBasicNames.EventName.ClassifyAt(297, 6),
                VisualBasicNames.EventName.ClassifyAt(344, 6),
                VisualBasicNames.EventName.ClassifyAt(401, 6));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetContext(@"Access\Members\TypeEvent.vb").GetClassifications().AssertContains(
                VisualBasicNames.EventName.ClassifyAt(256, 6),
                VisualBasicNames.EventName.ClassifyAt(298, 6),
                VisualBasicNames.EventName.ClassifyAt(339, 6),
                VisualBasicNames.EventName.ClassifyAt(390, 6));
        }
    }
}