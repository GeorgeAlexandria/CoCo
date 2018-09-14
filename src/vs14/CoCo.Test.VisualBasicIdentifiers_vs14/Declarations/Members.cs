using CoCo.Analyser.VisualBasic;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.VisualBasicIdentifiers.Declarations
{
    internal class Members : VisualBasicIdentifierTests
    {
        [Test]
        public void MemberTest_InstansField()
        {
            GetClassifications(@"Declarations\Members\InstanceField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(39, 5));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetClassifications(@"Declarations\Members\TypeField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(42, 5));
        }

        [Test]
        public void MemberTest_ModuleField()
        {
            GetClassifications(@"Declarations\Members\ModuleField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(31, 5));
        }

        [Test]
        public void MemberTest_Constant()
        {
            GetClassifications(@"Declarations\Members\ConstantMember.vb")
                .AssertContains(VisualBasicNames.ConstantFieldName.ClassifyAt(46, 8));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            GetClassifications(@"Declarations\Members\InstanceProperty.vb")
                .AssertContains(VisualBasicNames.PropertyName.ClassifyAt(51, 5));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetClassifications(@"Declarations\Members\TypeProperty.vb")
                .AssertContains(VisualBasicNames.PropertyName.ClassifyAt(54, 11));
        }

        [Test]
        public void MemberTest_ModuleProperty()
        {
            GetClassifications(@"Declarations\Members\ModuleProperty.vb")
                .AssertContains(VisualBasicNames.PropertyName.ClassifyAt(50, 5));
        }

        [Test]
        public void MemberTest_WithEvents()
        {
            GetClassifications(@"Declarations\Members\WithEventsProperty.vb")
                .AssertContains(VisualBasicNames.WithEventsPropertyName.ClassifyAt(55, 5));
        }

        [Test]
        public void MemberTest_CustomEvent()
        {
            GetClassifications(@"Declarations\Members\CustomEvent.vb")
                .AssertContains(VisualBasicNames.EventName.ClassifyAt(50, 7));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            GetClassifications(@"Declarations\Members\InstanceEvent.vb")
                .AssertContains(VisualBasicNames.EventName.ClassifyAt(45, 7));
        }

        [Test]
        public void MemberTest_ModuleEvent()
        {
            GetClassifications(@"Declarations\Members\ModuleEvent.vb")
                .AssertContains(VisualBasicNames.EventName.ClassifyAt(37, 7));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            GetClassifications(@"Declarations\Members\TypeEvent.vb")
                .AssertContains(VisualBasicNames.EventName.ClassifyAt(48, 7));
        }
    }
}