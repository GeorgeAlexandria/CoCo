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
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\InstanceField.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.FieldName.ClassifyAt(113, 5));
        }

        [Test]
        public void MemberTest_InstanceProperty()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\InstanceProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.PropertyName.ClassifyAt(116, 10));
        }

        [Test]
        public void MemberTest_InstanceEvent()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\InstanceEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.EventName.ClassifyAt(145, 7));
        }

        [Test]
        public void MemberTest_ConstantMember()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\ConstantMember.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.ConstantFieldName.ClassifyAt(124, 5));
        }

        // TODO: add a special classifications for Type' members ?
        [Test]
        public void MemberTest_TypeField()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\TypeField.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.FieldName.ClassifyAt(123, 5));
        }

        [Test]
        public void MemberTest_TypeProperty()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\TypeProperty.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.PropertyName.ClassifyAt(126, 8));
        }

        [Test]
        public void MemberTest_TypeEvent()
        {
            @"Tests\CSharpIdentifiers\CSharpIdentifiers\Declarations\Members\TypeEvent.cs".GetClassifications(ProjectInfo)
                .AssertContains(Names.EventName.ClassifyAt(155, 7));
        }
    }
}