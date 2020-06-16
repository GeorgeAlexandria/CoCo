using CoCo.Analyser.Classifications.VisualBasic;
using CoCo.Test.Identifiers.Common;
using NUnit.Framework;

namespace CoCo.Test.Identifiers.VisualBasic.Declarations
{
    internal class Members : VisualBasicIdentifierTests
    {
        [Test]
        public void MemberTest_InstansField()
        {
            GetContext(@"Declarations\Members\InstanceField.vb").GetClassifications().AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(39, 5));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetContext(@"Declarations\Members\TypeField.vb").GetClassifications().AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(42, 5));
        }

        [Test]
        public void MemberTest_ModuleField()
        {
            GetContext(@"Declarations\Members\ModuleField.vb").GetClassifications().AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(31, 5));
        }

        [Test]
        public void MemberTest_Constant()
        {
            GetContext(@"Declarations\Members\ConstantMember.vb").GetClassifications().AssertContains(
                VisualBasicNames.ConstantFieldName.ClassifyAt(46, 8));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            GetContext(@"Declarations\Members\InstanceProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(51, 5));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetContext(@"Declarations\Members\TypeProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(54, 11));
        }

        [Test]
        public void MemberTest_ModuleProperty()
        {
            GetContext(@"Declarations\Members\ModuleProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(50, 5));
        }

        [Test]
        public void MemberTest_WithEvents()
        {
            GetContext(@"Declarations\Members\WithEventsProperty.vb").GetClassifications().AssertContains(
                VisualBasicNames.WithEventsPropertyName.ClassifyAt(55, 5));
        }

        [Test]
        public void MemberTest_CustomEvent()
        {
            GetContext(@"Declarations\Members\CustomEvent.vb").GetClassifications().AssertContains(
                VisualBasicNames.EventName.ClassifyAt(50, 7));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            GetContext(@"Declarations\Members\InstanceEvent.vb").GetClassifications().AssertContains(
                VisualBasicNames.EventName.ClassifyAt(45, 7));
        }

        [Test]
        public void MemberTest_ModuleEvent()
        {
            GetContext(@"Declarations\Members\ModuleEvent.vb").GetClassifications().AssertContains(
                VisualBasicNames.EventName.ClassifyAt(37, 7));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetContext(@"Declarations\Members\TypeEvent.vb").GetClassifications().AssertContains(
                VisualBasicNames.EventName.ClassifyAt(48, 7));
        }
    }
}