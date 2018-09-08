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
                .AssertContains(VisualBasicNames.ConstantFieldName.ClassifyAt(111, 8));
        }

        [Test]
        public void MemberTest_InstanceField()
        {
            GetClassifications(@"Access\Members\InstanceField.vb").AssertContains(
                VisualBasicNames.FieldName.ClassifyAt(124, 5),
                VisualBasicNames.FieldName.ClassifyAt(158, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            GetClassifications(@"Access\Members\InstanceProperty.vb").AssertContains(
                VisualBasicNames.PropertyName.ClassifyAt(129, 8),
                VisualBasicNames.PropertyName.ClassifyAt(166, 5));
        }

        [Test]
        public void MemberTest_ModuleField()
        {
            GetClassifications(@"Access\Members\ModuleField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(141, 5));
        }

        [Test]
        public void MemberTest_ModuleProperty()
        {
            GetClassifications(@"Access\Members\ModuleProperty.vb")
                .AssertContains(VisualBasicNames.PropertyName.ClassifyAt(121, 10));
        }

        [Test]
        public void MemberTest_TypeField()
        {
            GetClassifications(@"Access\Members\TypeField.vb")
                .AssertContains(VisualBasicNames.FieldName.ClassifyAt(121, 13));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            GetClassifications(@"Access\Members\TypeProperty.vb")
                .AssertContains(VisualBasicNames.PropertyName.ClassifyAt(115, 15));
        }

        [Test]
        public void MemberTest_WithEvents()
        {
            GetClassifications(@"Access\Members\WithEventsProperty.vb")
                .AssertContains(VisualBasicNames.WithEventsPropertyName.ClassifyAt(154, 7));
        }
    }
}