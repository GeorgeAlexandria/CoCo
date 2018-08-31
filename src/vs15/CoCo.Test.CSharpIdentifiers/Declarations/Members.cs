using CoCo.Analyser;
using CoCo.Test.Common;
using NUnit.Framework;

namespace CoCo.Test.CSharpIdentifiers.Declarations
{
    internal class Members : CSharpIdentifierTests
    {
        [Test]
        public void MemberTest_InstanceField()
        {
            @"Declarations\Members\InstanceField.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.FieldName.ClassifyAt(113, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            @"Declarations\Members\InstanceProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.PropertyName.ClassifyAt(116, 10));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            @"Declarations\Members\InstanceEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.EventName.ClassifyAt(145, 7));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            @"Declarations\Members\ConstantMember.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.ConstantFieldName.ClassifyAt(124, 5));
        }

        // TODO: add a special classifications for Type' members ?
        [Test]
        public void MemberTest_TypeField()
        {
            @"Declarations\Members\TypeField.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.FieldName.ClassifyAt(123, 5));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            @"Declarations\Members\TypeProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.PropertyName.ClassifyAt(126, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            @"Declarations\Members\TypeEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(CSharpNames.EventName.ClassifyAt(155, 7));
        }
    }
}